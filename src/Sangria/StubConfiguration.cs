using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Sangria.Exceptions;
using Sangria.Resources;

namespace Sangria
{
    public interface IStubConfiguration
    {
        bool IsFallback { get; }
        string Resource { get; }
        StubResponse StubbedResponse { get; }
        HttpVerb HttpVerb { get; }
        IStubConfiguration AsFallback();
        IStubConfiguration WithHeader(HttpRequestHeader header, string value);
        IStubConfiguration WithQueryStringParameter(string name, string value);
        IServer Returns(StubResponse response);
        bool MatchesRequest(HttpListenerRequest request);
        bool ReceivedRequests(int i);
        void Execute();
    }

    public abstract class StubConfiguration : IStubConfiguration
    {
        public abstract HttpVerb HttpVerb { get; }

        public string Resource { get; }
        public bool IsFallback { get; protected set; }
        public StubResponse StubbedResponse { get; protected set; }

        protected Dictionary<string, string> Headers = new Dictionary<string, string>();
        protected Dictionary<string, string> QueryStringParameters = new Dictionary<string, string>();

        protected StubConfiguration(IServer server, string resource)
        {
            _server = server;
            Resource = resource;
        }

        public IStubConfiguration AsFallback()
        {
            if (_server.Configurations.Any(c => c.Resource.Equals(Resource, StringComparison.InvariantCultureIgnoreCase) && c.IsFallback))
                throw new DuplicateFallbackException(Resource);

            IsFallback = true;

            return this;
        }

        public IServer Returns(StubResponse response)
        {
            StubbedResponse = response;
            return _server;
        }

        public IStubConfiguration WithQueryStringParameter(string name, string value)
        {
            if (QueryStringParameters.ContainsKey(name))
                throw new InvalidBindingException(string.Format(Errors.DuplicateQueryStringParameter, name));

            QueryStringParameters.Add(name, value);

            return this;
        }

        public IStubConfiguration WithHeader(HttpRequestHeader header, string value)
        {
            if (Headers.ContainsKey(header.ToString()))
                throw new InvalidBindingException(string.Format(Errors.DuplicateHeaderParameter, header));

            Headers.Add(header.ToString(), value);

            return this;
        }

        public bool ReceivedRequests(int i)
        {
            return _calls == i;
        }

        public void Execute()
        {
            _calls++;
            OnExecute();
        }

        protected virtual void OnExecute()
        {
        }

        public abstract bool MatchesRequest(HttpListenerRequest request);

        private readonly IServer _server;
        private int _calls;
    }
}