using System;
using System.IO;
using Kayak;

namespace FeedGenerator
{
	public class FileProducer : IDataProducer
	{
		private string _fileName;
		private int _bufferSize = 2048;
			
		public FileProducer(string fileName, int bufferSize)
		{ 
			_fileName = fileName;
			_bufferSize = bufferSize;
		}

		public FileProducer(string fileName) : this(fileName, 2048) { }

		public IDisposable Connect(IDataConsumer channel)
		{
			using(Stream source = File.OpenRead(_fileName)) 
			{
				var buffer = new byte[_bufferSize];
				while(source.Read(buffer, 0, buffer.Length) > 0) 
				{
					channel.OnData(new ArraySegment<byte>(buffer), null);
				}
			}
			
			channel.OnEnd();
			return null;
		}
	}
}