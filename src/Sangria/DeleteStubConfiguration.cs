using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Sangria.Exceptions;
using Sangria.Resources;

namespace Sangria
{
    public interface IDeleteStubConfiguration : IStubConfiguration
    {
    }

    public class DeleteStubConfiguration : IDeleteStubConfiguration
    {
        public HttpVerb HttpVerb => HttpVerb.Delete;
        public string Resource { get; }
        public bool IsFallback { get; private set; }
        public StubResponse StubbedResponse { get; private set; }

        public DeleteStubConfiguration(IServer server, string resource)
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

        public IStubConfiguration WithQueryStringParameter(string name, string value)
        {
            if (_queryStringParameters.ContainsKey(name))
                throw new InvalidBindingException(string.Format(Errors.DuplicateQueryStringParameter, name));

            _queryStringParameters.Add(name, value);

            return this;
        }

        public IStubConfiguration WithHeader(HttpRequestHeader header, string value)
        {
            if (_headers.ContainsKey(header.ToString()))
                throw new InvalidBindingException(string.Format(Errors.DuplicateHeaderParameter, header));

            _headers.Add(header.ToString(), value);

            return this;
        }

        public bool MatchesRequest(HttpListenerRequest request)
        {
            return _queryStringParameters.All(q => request.QueryString[q.Key] == q.Value) &&
                   _headers.All(h => request.Headers[h.Key] == h.Value);
        }

        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _queryStringParameters = new Dictionary<string, string>();
        private readonly IServer _server;
    }
}