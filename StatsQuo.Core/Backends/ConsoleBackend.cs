using System;
using System.Collections.Generic;
using System.Linq;
using ConsoleTables.Core;
using StatsQuo.Core.Metrics;

namespace StatsQuo.Core.Backends
{
	public class ConsoleBackend : IBackend
	{
		public void Flush(List<Metric> metrics)
		{
			var header = Enumerable.Empty<string>()
				.Union(new List<string> { "name" })
				.Union(metrics.SelectMany(x => x.Tags.Keys).Distinct())
				.Union(metrics.SelectMany(x => x.Values.Keys).Distinct())
				.ToArray();

			var table = new ConsoleTable(header);

			foreach (var metric in metrics)
			{
				//var entry = new List<string>(header.Length);
				var entry = new string[header.Length];

				entry[0] = metric.Name;

				foreach (var tag in metric.Tags)
				{
					entry[header.ToList().IndexOf(tag.Key)] = tag.Value;
				}

				foreach (var value in metric.Values)
				{
					entry[header.ToList().IndexOf(value.Key)] = value.Value.ToString();
				}

				table.AddRow(entry.ToArray());
			}

			Console.WriteLine(DateTime.Now.TimeOfDay);
			Console.WriteLine();

			table.Write();
			Console.WriteLine();
		}
	}
}