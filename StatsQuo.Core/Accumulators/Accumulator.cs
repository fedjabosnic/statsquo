using System.Collections.Generic;
using System.Threading;
using StatsQuo.Core.Metrics;

namespace StatsQuo.Core.Accumulators
{
	public abstract class Accumulator : IAccumulator
	{
		private List<RawMetric> _metrics = new List<RawMetric>();

		protected void Add(RawMetric metric)
		{
			_metrics.Add(metric);
		}

		public List<RawMetric> Flush()
		{
			return Interlocked.Exchange(ref _metrics, new List<RawMetric>());
		}
	}
}