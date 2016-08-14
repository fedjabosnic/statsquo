using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using StatsQuo.Core.Metrics;

namespace StatsQuo.Core.Accumulators
{
	public class UdpAccumulator : Accumulator
	{
		private Task task;

		public UdpAccumulator(string host, int port)
		{
			task = Task.Factory.StartNew(o =>
			{
				try
				{
					var udp = new UdpClient(port);

					while (true)
					{
						IPEndPoint caller = null;
						var bytes = udp.Receive(ref caller);
						var message = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
						
						// TODO: Add tighter regex (forbidden characters, correct tag separators etc)
						var match = Regex.Match(message, @"^(?<name>[^\|:]*):(?<value>[^\|:]*)\|(?<type>[^\|:]*)(\|(?<tags>[^\|]*))?$");

						if (match.Groups["name"].Success && match.Groups["value"].Success && match.Groups["type"].Success)
						{
							var name = match.Groups["name"].Value;
							var type = match.Groups["type"].Value;
							var value = double.Parse(match.Groups["value"].Value);

							var tags = match.Groups["tags"].Success
								? string.Join(",", match.Groups["tags"].Value.Split(',').OrderBy(x => x))
								: string.Empty;

							var metric = new RawMetric { Name = name, Type = type, Tags = tags, Value = value, Time = DateTime.Now };
							Add(metric);
						}
					}
				}
				catch (Exception ex)
				{
					throw;
				}

			}, TaskCreationOptions.LongRunning);
		}
	}
}