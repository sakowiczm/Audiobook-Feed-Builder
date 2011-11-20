using System.Collections.Generic;
using System.IO;
using System.Linq;
using Kayak;
using Kayak.Http;

namespace FeedGenerator
{
    public class RequestDelegate : IHttpRequestDelegate
    {
        public int Port { get; private set; }
        public string FeedTitle { get; private set; }
        public List<string> FilePaths { get; private set; }
        public string ImagePath { get; private set; }

        public RequestDelegate(int port, string feedTitle, List<string> filePaths, string imagePath)
        {
            Port = port;
            FeedTitle = feedTitle;
            FilePaths = filePaths;
            ImagePath = imagePath;

            if (ImagePath != null && FilePaths != null)
                FilePaths.Add(ImagePath);
        }

        public void OnRequest(HttpRequestHead request, IDataProducer requestBody, IHttpResponseDelegate response)
        {
            if (request.Uri.StartsWith("/feed.xml"))
            {
                var body = new FeedBuilder(Port).Generate(FeedTitle, FilePaths, ImagePath);

                var headers = new HttpResponseHead()
                                  {
                                      Status = "200 OK",
                                      Headers = new Dictionary<string, string>() 
                                                    {
                                                        { "Content-Type", "text/plain" },
                                                        { "Content-Length", body.Length.ToString() },
                                                    }
                                  };

                response.OnResponse(headers, new BufferedProducer(body));
                return;
            }

            // deal with request for file content
            string uri = request.Uri.Replace("%20", " ").Replace("/", "\\");
            string filePath = FilePaths.Where(d => d.Contains(uri)).FirstOrDefault();

            if (filePath != null)
            {
                FileStream fileStream = File.OpenRead(filePath);

                // todo: deal with oom excepiton on large files

                byte[] buffer = new byte[fileStream.Length];
                fileStream.Read(buffer, 0, (int)fileStream.Length);
                fileStream.Close();

                string mimeType = GetMimeType(filePath);

                var headers = new HttpResponseHead()
                                  {
                                      Status = "200 OK",
                                      Headers = new Dictionary<string, string>() 
                                                    {
                                                        { "Content-Type", mimeType },
                                                        { "Content-Length", buffer.Length.ToString() },
                                                    }
                                  };

                response.OnResponse(headers, new BufferedProducer(buffer));
                return;                
            }
            else
            {
                var responseBody = "The resource you requested ('" + request.Uri + "') could not be found.";
                var headers = new HttpResponseHead()
                                  {
                                      Status = "404 Not Found",
                                      Headers = new Dictionary<string, string>()
                                                    {
                                                        { "Content-Type", "text/plain" },
                                                        { "Content-Length", responseBody.Length.ToString() }
                                                    }
                                  };
                var body = new BufferedProducer(responseBody);

                response.OnResponse(headers, body);
                return;
            }
        }

        // todo: fix and unify with other GetMimeType
        private string GetMimeType(string filePath)
        {
            string mimeType = "audio/mp3";

            if (Path.GetExtension(filePath).Contains("mp4"))
                mimeType = "video/mp4";

            return mimeType;
        }
    }
}