using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Sangria.Exceptions;

namespace Sangria
{
    public interface IServer : IDisposable
    {
        void Start();
        IServer OnGet(string resource, string response);
    }

    public class Server : IServer
    {
        public Server(int port)
        {
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://*:{port}/");
        }

        public void Start()
        {
            _listener.Start();
            _listener.BeginGetContext(ProcessRequest, _listener);
        }

        public IServer OnGet(string resource, string response)
        {
            var key = resource.ToLower();
            if (_responses.ContainsKey(key))
                throw new DuplicateBindingException(key);

            _responses.Add(key, response);

            return this;
        }

        public void Dispose()
        {
            _listener.Stop();
        }

        private void ProcessRequest(IAsyncResult result)
        {
            var listener = (HttpListener)result.AsyncState;
            listener.BeginGetContext(ProcessRequest, listener);

            var context = listener.EndGetContext(result);
            string response;
            byte[] buffer;

            if (_responses.TryGetValue(context.Request.Url.LocalPath.Trim('/').ToLower(), out response))
            {
                buffer = Encoding.UTF8.GetBytes(response);
                context.Response.StatusCode = (int)HttpStatusCode.OK;
            }
            else
            {
                buffer = Encoding.UTF8.GetBytes("<html><body>404!</body></html>");
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }

            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.Close();
        }

        private readonly Dictionary<string, string> _responses = new Dictionary<string, string>();
        private readonly HttpListener _listener;
    }
}