using System;
using System.Collections.Generic;
using System.IO;
using Argotic.Extensions.Core;
using Argotic.Syndication;

namespace FeedGenerator
{
    public class FeedBuilder
    {
        public int Port { get; private set; }
        //public string FeedUrl { get; private set; }

        public FeedBuilder(int port)
        {
            Port = port;
        }

        public string Generate(string title, List<string> filePaths, string imagePath = null)
        {
            // todo: add validation library - CuttingEdge.Conditions ?

            var feed = new RssFeed();
            feed.Channel.Link = new Uri(GetUrl(Port, "feed.xml"));
            feed.Channel.Title = title;

            if (imagePath != null)
            {
                var file = new FileInfo(imagePath);
                if (file.Exists)
                {
                    var extension = new YahooMediaSyndicationExtension();
                    extension.Context.Thumbnails.Add(new YahooMediaThumbnail(new Uri(GetUrl(Port, file.Name))));

                    feed.Channel.AddExtension(extension);
                }
            }

            foreach (string filePath in filePaths)
            {
                var item = GetItem(filePath);

                if (item != null)
                    feed.Channel.AddItem(item);
            }

            return feed.CreateNavigator().OuterXml;            
        }

        private RssItem GetItem(string filePath)
        {
            var file = new FileInfo(filePath);

            if (!file.Exists)
                return null;

            string mimeType = GetMimeType(file);

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            var uri = new Uri(GetUrl(Port, file.Name));

            var item = new RssItem();
            item.Title = fileName;
            item.Link = uri;

            item.Enclosures.Add(new RssEnclosure(file.Length, mimeType, item.Link));

            var extension = new YahooMediaSyndicationExtension();
            var mediaContent = new YahooMediaContent(uri);
            mediaContent.FileSize = file.Length;
            mediaContent.ContentType = mimeType;

            extension.Context.AddContent(mediaContent);
            item.AddExtension(extension);

            return item;
        }

        private string GetUrl(int port, string fileName)
        {
            return string.Format(@"http://localhost:{0}/{1}", port, fileName);
        }

        private string GetMimeType(FileInfo file)
        {
            string mimeType = "audio/mp3";

            if (file.Extension.Contains("mp4"))
                mimeType = "video/mp4";

            return mimeType;
        }
    }
}