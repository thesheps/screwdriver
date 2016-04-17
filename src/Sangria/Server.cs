using System;
using System.Net;
using System.Text;
using System.Threading;

namespace Sangria
{
    public class Server : IDisposable
    {
        public Server(int port)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://*:{port}/");
            _listener.Start();
            _listener.BeginGetContext(ProcessRequest, _listener);
        }

        private static void ProcessRequest(IAsyncResult result)
        {
            var listener = (HttpListener)result.AsyncState;
            listener.BeginGetContext(ProcessRequest, listener);

            var context = listener.EndGetContext(result);
            var buffer = Encoding.UTF8.GetBytes("<html><body>404!</body></html>");

            context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.Close();
        }

        public void Dispose()
        {
            _listener.Stop();
        }

        private readonly HttpListener _listener;
    }
}