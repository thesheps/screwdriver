using System;
using System.Collections.Generic;
using System.Linq;
using Sangria.Exceptions;
using Sangria.Resources;

namespace Sangria
{
    public interface IStubConfiguration
    {
        bool IsFallback { get; }
        string Resource { get; }
        StubResponse StubbedResponse { get; }
        Dictionary<string, string> QueryStringParameters { get; }
        HttpVerb HttpVerb { get; }
        IStubConfiguration WithQueryString(string name, string value);
        IStubConfiguration Fallback();
        IServer Returns(StubResponse response);
    }

    public class StubConfiguration : IStubConfiguration
    {
        public HttpVerb HttpVerb { get; }
        public bool IsFallback { get; private set; }
        public string Resource { get; }
        public StubResponse StubbedResponse { get; private set; }
        public Dictionary<string, string> QueryStringParameters { get; }

        public StubConfiguration(IServer server, HttpVerb httpVerb, string resource)
        {
            _server = server;
            HttpVerb = httpVerb;
            Resource = resource;
            QueryStringParameters = new Dictionary<string, string>();
        }

        public IStubConfiguration WithQueryString(string name, string value)
        {
            if (QueryStringParameters.ContainsKey(name))
                throw new InvalidBindingException(string.Format(Errors.DuplicateQueryStringParameter, name));

            QueryStringParameters.Add(name, value);

            return this;
        }

        public IStubConfiguration Fallback()
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

        private readonly IServer _server;
    }
}