using System.Collections.Generic;
using StatsQuo.Core.Metrics;

namespace StatsQuo.Core
{
	public interface IBackend
	{
		void Flush(List<Metric> metrics);
	}
}