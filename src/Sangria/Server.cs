using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Sangria
{
    public interface IServer : IDisposable
    {
        IList<IStubConfiguration> Configurations { get; }
        IStubConfiguration OnGet(string resource);
        void Start();
    }

    public class Server : IServer
    {
        public IList<IStubConfiguration> Configurations { get; }

        public Server(int port)
        {
            Configurations = new List<IStubConfiguration>();
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
            var stubConfiguration = new StubConfiguration(this, resource);
            Configurations.Add(stubConfiguration);

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

            var buffer = Encoding.UTF8.GetBytes(Resources.Html.MissingStub);
            var context = listener.EndGetContext(result);
            context.Response.StatusCode = (int)HttpStatusCode.NotFound;

            var resource = context.Request.Url.LocalPath.Trim('/');
            var configurations = Configurations
                .Where(c => c.Resource.Equals(resource, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

            if (configurations.Any())
            {
                var configuration = configurations.SingleOrDefault(c => c.QueryStringParameters.All(q => context.Request.QueryString[q.Key] == q.Value)) ??
                                    configurations.FirstOrDefault(c => c.IsFallback);

                if (configuration != null)
                {
                    buffer = Encoding.UTF8.GetBytes(configuration.StubbedResponse.Body);
                    context.Response.StatusCode = (int)configuration.StubbedResponse.StatusCode;
                }
            }

            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.Close();
        }

        private readonly HttpListener _listener;
    }
}