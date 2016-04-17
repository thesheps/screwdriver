using System;
using System.Collections.Generic;
using System.Linq;
using Sangria.Exceptions;
using Sangria.Resources;

namespace Sangria
{
    public interface IStubConfiguration : IServer
    {
        bool IsFallback { get; }
        string Resource { get; }
        StubbedResponse StubbedResponse { get; }
        Dictionary<string, string> QueryStringParameters { get; }
        IStubConfiguration WithQueryString(string name, string value);
        IStubConfiguration Fallback();
        IServer Returns(StubbedResponse response);
    }

    public class StubConfiguration : IStubConfiguration
    {
        public bool IsFallback { get; private set; }
        public string Resource { get; }
        public StubbedResponse StubbedResponse { get; private set; }
        public Dictionary<string, string> QueryStringParameters { get; }
        public IList<IStubConfiguration> Configurations => _server.Configurations;

        public StubConfiguration(IServer server, string resource)
        {
            _server = server;
            Resource = resource;
            QueryStringParameters = new Dictionary<string, string>();
        }

        public void Start()
        {
            _server.Start();
        }

        public IStubConfiguration OnGet(string resource)
        {
            return _server.OnGet(resource);
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

        public IServer Returns(StubbedResponse response)
        {
            StubbedResponse = response;
            return _server;
        }

        public void Dispose()
        {
            _server.Dispose();
        }

        private readonly IServer _server;
    }
}