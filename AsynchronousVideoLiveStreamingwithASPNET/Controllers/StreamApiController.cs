using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace AsynchronousVideoLiveStreamingwithASPNET.Controllers
{
    public class StreamApiController : ApiController
    {

        public async void WriteContentToStream(Stream outputStream, HttpContent content, TransportContext transportContext)
        {
            var filePath = HttpContext.Current.Server.MapPath("~/media/1.mp4");
            //var filePath = HttpContext.Current.Server.MapPath("~/media/2.mp3");
            int bufferSize = 1000;
            byte[] buffer = new byte[bufferSize];
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                int totalSize = (int)fileStream.Length;
                while (totalSize > 0)
                {
                    int count = totalSize > bufferSize ? bufferSize : totalSize;
                    int sizeOfReadedBuffer = fileStream.Read(buffer, 0, count);
                    await outputStream.WriteAsync(buffer, 0, sizeOfReadedBuffer);
                    totalSize -= sizeOfReadedBuffer;
                }
            }
        }

        public HttpResponseMessage GetVideoContent()
        {
            var httpResponce = Request.CreateResponse();
            httpResponce.Content = new PushStreamContent((Action<Stream, HttpContent, TransportContext>)WriteContentToStream);
            return httpResponce;
        }
    }
}
