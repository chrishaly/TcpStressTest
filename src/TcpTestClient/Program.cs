using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common;

namespace TcpTestClient
{
	class Program
	{
		static readonly StatisticsInfo Statistics = new StatisticsInfo();

		static void Main(string[] args)
		{
			Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

			//var remote = new IPEndPoint(IPAddress.Loopback, 3400);
			var host = "localhost";
			var port = 3400;

			if (args.Length == 2)
			{
				host = args[0];
				port = int.Parse(args[1]);
			}

			//StartClientAsync(remote);
			for (int batchIndex = 0; batchIndex < 60; batchIndex++)
			{
				Task.Run(async () =>
				{
					for (int i = 0; i < 1000; i++)
					{
						await StartClientAsync(host, port);
					}
				});
			}

			//for (int i = 0; i < 60000; i++)
			//{
			//	StartClientAsync(host, port).Wait();
			//}

			while (true)
			{
				//Console.Clear();
				Console.WriteLine(Statistics);
				Thread.Sleep(1000);
			}

			Console.WriteLine("Hello World!");
		}

		private static async Task StartClientAsync(string host, int port)
		{
			var client = new TcpClient();
			try
			{
				await client.ConnectAsync(host, port);
				Statistics.IncrementClient();

				var runTask = RunClientAsync(client);
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Connect Error: {ex}");
			}
		}

		private static async Task RunClientAsync(TcpClient client)
		{
			var receiveBufferBytes = new byte[256];

			var sendBytes = Encoding.UTF8.GetBytes("price ever, and a new, shiny red iPhone 7, whose sales will help to …On the 60th day of his presidency came the hardest truth for Donald Trump. He was wrong. James B. Comey — the FBI director whom Trump celebrated on the …Washington PostTrump");
			while (true)
			{
				try
				{
					var sendBuffer = new ArraySegment<byte>(sendBytes, 0, sendBytes.Length);
					await client.Client.SendAsync(sendBuffer, SocketFlags.None);
					Statistics.IncrementSendTimes();

					var receiveBuffer = new ArraySegment<byte>(receiveBufferBytes);
					var len = await client.Client.ReceiveAsync(receiveBuffer, SocketFlags.None);
					if (len == 0)
					{
						client.Dispose();
						Statistics.DecrementClient();
						break;
					}
					Statistics.IncrementReceivedTimes();

					await Task.Delay(30 * 1000);
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