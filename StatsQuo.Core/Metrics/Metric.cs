using System;
using System.Collections.Generic;

namespace StatsQuo.Core.Metrics
{
	public class Metric
	{
		public string Name { get; set; }
		public DateTime Time { get; set; }
		public Dictionary<string, string> Tags { get; set; }
		public Dictionary<string, double> Values { get; set; }
	}
}