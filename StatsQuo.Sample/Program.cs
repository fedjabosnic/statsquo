using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StatsQuo.Sample
{
	class Program
	{
		static void Main(string[] args)
		{
			var service = Guid.NewGuid().ToString().Substring(0,8);
			var client = new UdpClient();

			var random = new Random();
			var sent = 0;

			Task.Factory.StartNew(() =>
			{
				while (true)
				{
					Console.WriteLine(Interlocked.Exchange(ref sent, 0));
					Thread.Sleep(1000);
				}
			}, TaskCreationOptions.LongRunning);

			while (true)
			{
				for (int i = 0; i < 100; i++)
				{
					var message = $"iterations:{random.Next(0, 50000)}|ms|service:{service},host:xps";
					//var message = $"iterations:1|c|service:{service},host:xps";

					var bytes = Encoding.UTF8.GetBytes(message);

					client.Send(bytes, bytes.Length, "127.0.0.1", 8125);

					Interlocked.Increment(ref sent);
				}

				Thread.Sleep(1);
			}

		}
	}
}
