using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Sangria.Exceptions;
using Sangria.Resources;

namespace Sangria
{
    public interface IPostStubConfiguration : IStubConfiguration
    {
        IPostStubConfiguration WithQueryString(string name, string value);
        IPostStubConfiguration WithBody(string body);
    }

    public class PostStubConfiguration : IPostStubConfiguration
    {
        public HttpVerb HttpVerb => HttpVerb.Post;
        public string Resource { get; }
        public bool IsFallback { get; private set; }
        public StubResponse StubbedResponse { get; private set; }

        public PostStubConfiguration(IServer server, string resource)
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

        public IPostStubConfiguration WithQueryString(string name, string value)
        {
            if (_queryStringParameters.ContainsKey(name))
                throw new InvalidBindingException(string.Format(Errors.DuplicateQueryStringParameter, name));

            _queryStringParameters.Add(name, value);

            return this;
        }

        public IPostStubConfiguration WithBody(string body)
        {
            _body = body;
            return this;
        }

        public bool MatchesRequest(HttpListenerRequest request)
        {
            var reader = new StreamReader(request.InputStream);
            var body = reader.ReadToEnd();

            return _queryStringParameters.All(q => request.QueryString[q.Key] == q.Value) && (_body == null || _body.Equals(body));
        }

        private readonly Dictionary<string, string> _queryStringParameters = new Dictionary<string, string>();
        private readonly IServer _server;
        private string _body;
    }
}