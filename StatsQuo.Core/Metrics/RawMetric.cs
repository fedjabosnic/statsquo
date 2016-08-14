using System;

namespace StatsQuo.Core.Metrics
{
	public class RawMetric
	{
		public DateTime Time { get; set; }

		public string Name { get; set; }
		public string Type { get; set; }
		public string Tags { get; set; }
		public double Value { get; set; }
	}
}