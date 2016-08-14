using System;
using System.Collections.Generic;
using System.Reflection;
using StatsQuo.Core;
using StatsQuo.Core.Accumulators;
using StatsQuo.Core.Backends;

namespace StatsQuo
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine($"StatsQuo {Assembly.GetExecutingAssembly().GetName().Version}");
			Console.WriteLine();

			// TODO: Configure through command line arguments

			var interval = TimeSpan.Parse("00:00:10");
			var accumulators = new List<IAccumulator> { new UdpAccumulator("127.0.0.1", 8125) };
			var backends = new List<IBackend> { new ConsoleBackend() };

			var processor = new Processor(interval, accumulators, backends);

			Console.Read();
		}
	}
}
