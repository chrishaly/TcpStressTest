using System.Threading;

namespace Common
{
	public class StatisticsInfo
	{
		public int ClientCount;
		public int ReceivedTimes;
		public int SendTimes;

		public void IncrementClient() => Interlocked.Increment(ref ClientCount);
		public void DecrementClient() => Interlocked.Decrement(ref ClientCount);

		public void IncrementReceivedTimes() => Interlocked.Increment(ref ReceivedTimes);
		//public void DecrementReceivedTimes() => Interlocked.Decrement(ref ReceivedTimes);

		public void IncrementSendTimes() => Interlocked.Increment(ref SendTimes);
		//public void DecrementSendTimes() => Interlocked.Decrement(ref SendTimes);

		public override string ToString()
		{
			return $"ClientCount: {ClientCount}, ReceivedTimes: {ReceivedTimes}, SendTimes: {SendTimes}";
		}
	}
}