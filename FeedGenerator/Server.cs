using System;
using System.Collections.Generic;
using Kayak;
using Kayak.Http;
using System.Net;

namespace FeedGenerator
{
    public class Server : IDisposable
    {
        public int Port { get; private set; }
        public string Title { get; private set; }
        public List<string> FilePaths { get; private set; }
        public string ImagePath { get; private set; }

        private IScheduler _scheduler;
        private IServer _server;

        public Server(int port, string title, List<string> filePaths, string imagePath)
        {
            Port = port;
            Title = title;
            FilePaths = filePaths;
            ImagePath = imagePath;
        }

        public void Start()
        {
            _scheduler = KayakScheduler.Factory.Create(new SchedulerDelegate());
            _server = KayakServer.Factory.CreateHttp(new RequestDelegate(Port, Title, FilePaths, ImagePath), _scheduler);

            _server.Listen(new IPEndPoint(IPAddress.Any, Port));
            _scheduler.Start();
        }

        public void Stop()
        {
            _scheduler.Stop();
        }

        public void Dispose()
        {
            if (_server != null)
            {
                _server.Dispose();
            }
        }
    }
}
