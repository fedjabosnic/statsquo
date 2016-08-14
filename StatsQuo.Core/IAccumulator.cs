using System.Collections.Generic;
using StatsQuo.Core.Metrics;

namespace StatsQuo.Core
{
	public interface IAccumulator
	{
		List<RawMetric> Flush();
	}
}