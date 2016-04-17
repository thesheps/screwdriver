using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Sangria
{
    public interface IServer : IDisposable
    {
        void Start();
        IStubConfiguration OnGet(string resource);
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

        public IStubConfiguration OnGet(string resource)
        {
            var key = resource.ToLower();
            var stubConfiguration = new StubConfiguration(this, key);

            _configurations.Add(stubConfiguration);

            return stubConfiguration;
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
            byte[] buffer;

            var resource = context.Request.Url.LocalPath.Trim('/');
            var configuration = _configurations
                .SingleOrDefault(c => c.Resource.Equals(resource, StringComparison.InvariantCultureIgnoreCase) &&
                                      c.QueryStringParameters.All(q => context.Request.QueryString[q.Key] == q.Value));

            if (configuration != null)
            {
                buffer = Encoding.UTF8.GetBytes(configuration.StubbedResponse.Body);
                context.Response.StatusCode = (int)configuration.StubbedResponse.StatusCode;
            }
            else
            {
                buffer = Encoding.UTF8.GetBytes("<html><body>404!</body></html>");
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }

            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.Close();
        }

        private readonly IList<StubConfiguration> _configurations = new List<StubConfiguration>();
        private readonly HttpListener _listener;
    }
}