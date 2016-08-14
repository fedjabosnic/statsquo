using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StatsQuo.Core.Metrics;
using StatsQuo.Core.Utilities;

namespace StatsQuo.Core
{
	public class Processor
	{
		private readonly TimeSpan _interval;
		private readonly List<IAccumulator> _accumulators;
		private readonly List<IBackend> _backends;

		public Processor(TimeSpan interval, List<IAccumulator> accumulators, List<IBackend> backends)
		{
			_interval = interval;
			_accumulators = accumulators;
			_backends = backends;

			Task.Factory.StartNew(() =>
			{
				// NOTE: Custom refresh timer implementation
				// Timers in the CLR, TPL and Rx are all exposed to timer drift, causing inaccurate ticking over time
				// This custom implementation should be more accurate (but not perfect) as it recalculates the next delay after each tick
				// See the following for more information:
				// http://stackoverflow.com/questions/13838113/observable-timer-how-to-avoid-timer-drift
				// http://stackoverflow.com/questions/6259120/system-threading-timer-call-drifts-a-couple-seconds-every-day
				// http://stackoverflow.com/questions/8431995/c-sharp-net-2-threading-timer-time-drifting

				// TODO: There is an edge case here when processing takes longer than the interval (next interval will be missed)
				// NOTE: This should be extracted and tested so that edge cases are cleared up

				var target = DateTime.MinValue;

				while (true)
				{
					var now = DateTime.UtcNow;
					target = target >= now ? target : now.RoundUp(interval);

					Thread.Sleep(target - now);
					
					var messages = _accumulators.SelectMany(x => x.Flush()).ToList();
					Task.Factory.StartNew(() => Process(target, messages));

					target = target.Add(interval);
				}
			}, TaskCreationOptions.LongRunning);

		}

		public void Process(DateTime time, List<RawMetric> messages)
		{
			Console.Clear();

			var buckets = new Map<string, Map<string, Map<string, List<double>>>>();

			var first = DateTime.MaxValue;
			var last = DateTime.MinValue;

			var item = 0;

			// Parse messages and bucket into unique series by name -> tags -> type
			foreach (var message in messages)
			{
				item++;
				first = message.Time < first ? message.Time : first;
				last = message.Time > last ? message.Time : last;

				buckets[message.Name][message.Tags][message.Type].Add(message.Value);
			}

			// Simple debugging info to show stats are in line with the refresh period (will be removed later)
			Console.WriteLine($"At: {time.TimeOfDay}");
			Console.WriteLine($"First: {first.TimeOfDay}");
			Console.WriteLine($"Last: {last.TimeOfDay}");

			var metrics = new List<Metric>();

			// Process all unique buckets and calculate metrics
			foreach (var named in buckets)
			{
				foreach (var tagged in named.Value)
				{
					foreach (var typed in tagged.Value)
					{
						var name = named.Key;
						var tags = tagged.Key.Split(',').Select(x => x.Split(':')).ToDictionary(x => x[0], x => x[1]);
						var values = new Dictionary<string, double>();

						var type = typed.Key;
						var components = typed.Value;

						// NOTE: Using linq is probably inefficient - this can be refactored later if performance becomes an issue

						switch (type)
						{
							case "g":
								values["gauge"] = components.Last();
								break;
							case "c":
								values["count"] = components.Sum();
								values["rate"] = values["count"] / _interval.TotalSeconds;
								break;
							case "ms":
								values["count"] = components.Count();
								values["sum"] = components.Sum();
								values["mean"] = components.Average();
								values["min"] = components.Min();
								values["max"] = components.Max();
								// TODO: Calculate percentiles
								//values["P50"] = ?
								//values["P90"] = ?
								//values["P99"] = ?
								break;
						}

						metrics.Add(new Metric { Name = name, Time = time, Tags = tags, Values = values });
					}
				}
			}

			Parallel.ForEach(_backends, b => b.Flush(metrics));
		}
	}
}
