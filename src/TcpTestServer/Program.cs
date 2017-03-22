using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace TcpTestServer
{
	class Program
	{
		static readonly StatisticsInfo Statistics = new StatisticsInfo();

		static void Main(string[] args)
		{
			var runTask = RunAsync();

			var exit = false;
			runTask.ContinueWith(it =>
			{
				exit = true;
			});

			while (true)
			{
				if (exit)
					break;

				//Console.Clear();
				Console.WriteLine(Statistics);
				Thread.Sleep(1000);
			}

			Console.WriteLine("Press any key to exit");
			Console.ReadLine();
		}

		private static async Task RunAsync()
		{
			var listener = new TcpListener(IPAddress.Any, 3400);
			listener.Start(655360);

			while (true)
			{
				try
				{
					var client = await listener.AcceptTcpClientAsync();
					var runClientTask = RunClientAsync(client);
					Statistics.IncrementClient();
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Accept Error: {ex}");
					break;
				}
			}
			Console.WriteLine("RunAsync Finish");
		}

		private static async Task RunClientAsync(TcpClient client)
		{
			var buffer = new byte[256];
			var buf = new ArraySegment<byte>(buffer);
			while (true)
			{
				try
				{
					var len = await client.Client.ReceiveAsync(buf, SocketFlags.None);
					if (len == 0)
					{
						client.Dispose();
						Statistics.DecrementClient();
						break;
					}
					Statistics.IncrementReceivedTimes();

					var sendBuffer = new ArraySegment<byte>(buffer, 0, len);
					await client.Client.SendAsync(sendBuffer, SocketFlags.None);
					Statistics.IncrementSendTimes();
				}
				catch (Exception ex)
				{
					//Console.WriteLine($"RunClientAsync Error: {ex}");
					client.Dispose();
					Statistics.DecrementClient();
					break;
				}
			}
		}
	}
}