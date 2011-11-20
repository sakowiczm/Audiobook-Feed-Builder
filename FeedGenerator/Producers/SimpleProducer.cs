using System;
using System.Text;
using Kayak;

namespace FeedGenerator
{
	public class SimpleProducer : IDataProducer
	{
		ArraySegment<byte> data;

		public SimpleProducer(string data) : this(data, Encoding.UTF8) { }
		public SimpleProducer(string data, Encoding encoding) : this(encoding.GetBytes(data)) { }
		public SimpleProducer(byte[] data) : this(new ArraySegment<byte>(data)) { }
		public SimpleProducer(ArraySegment<byte> data)
		{
			this.data = data;
		}

		public IDisposable Connect(IDataConsumer channel)
		{
			channel.OnData(data, null);
			channel.OnEnd();
			return null;
		}
	}
}