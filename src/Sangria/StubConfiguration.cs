using System;
using System.Linq;
using System.Net;
using Sangria.Exceptions;

namespace Sangria
{
    public abstract class StubConfiguration : IStubConfiguration
    {
        public bool IsFallback { get; private set; }
        public string Resource { get; }
        public StubResponse StubbedResponse { get; private set; }

        protected StubConfiguration(IServer server, string resource)
        {
            _server = server;
            Resource = resource;
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

        public abstract bool MatchesRequest(HttpListenerRequest request);
        public abstract HttpVerb HttpVerb { get; }

        private readonly IServer _server;
    }
}