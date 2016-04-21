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
        IGetStubConfiguration OnGet(string resource);
        IPostStubConfiguration OnPost(string resource);
        IPutStubConfiguration OnPut(string resource);
        IDeleteStubConfiguration OnDelete(string resource);
        void AddStubConfiguration(IStubConfiguration stubConfiguration);
        void Start();
        void Stop();
    }

    public class Server : IServer
    {
        public IList<IStubConfiguration> Configurations { get; }

        public Server(int port)
        {
            Configurations = new List<IStubConfiguration>();
            _listener = new HttpListener();
            _listener.Prefixes.Add($"http://+:{port}/");
        }

        public void Start()
        {
            _listener.Start();
            _listener.BeginGetContext(ProcessRequest, _listener);
        }

        public void Stop()
        {
            _listener.Stop();
        }

        public IGetStubConfiguration OnGet(string resource)
        {
            var stubConfiguration = new GetStubConfiguration(this, resource);
            Configurations.Add(stubConfiguration);

            return stubConfiguration;
        }

        public IPostStubConfiguration OnPost(string resource)
        {
            var stubConfiguration = new PostStubConfiguration(this, resource);
            Configurations.Add(stubConfiguration);

            return stubConfiguration;
        }

        public IPutStubConfiguration OnPut(string resource)
        {
            var stubConfiguration = new PutStubConfiguration(this, resource);
            Configurations.Add(stubConfiguration);

            return stubConfiguration;
        }

        public IDeleteStubConfiguration OnDelete(string resource)
        {
            var stubConfiguration = new DeleteStubConfiguration(this, resource);
            Configurations.Add(stubConfiguration);

            return stubConfiguration;
        }

        public void AddStubConfiguration(IStubConfiguration stubConfiguration)
        {
            Configurations.Add(stubConfiguration);
        }

        public void Dispose()
        {
            if (_listener.IsListening)
                _listener.Stop();

            Configurations?.Clear();
            _listener = null;
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
                .Where(c => c.Resource.Equals(resource, StringComparison.InvariantCultureIgnoreCase) &&
                            c.HttpVerb.ToString().Equals(context.Request.HttpMethod, StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (configurations.Any())
            {
                var configuration = configurations.SingleOrDefault(c => c.MatchesRequest(context.Request)) ?? configurations.FirstOrDefault(c => c.IsFallback);

                if (configuration != null)
                {
                    configuration.Execute();
                    buffer = Encoding.UTF8.GetBytes(configuration.StubbedResponse.Body);
                    context.Response.StatusCode = (int)configuration.StubbedResponse.StatusCode;
                }
            }

            context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            context.Response.Close();
        }

        private HttpListener _listener;
    }
}