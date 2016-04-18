using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Sangria.Exceptions;
using Sangria.Resources;

namespace Sangria
{
    public interface IPostStubConfiguration : IStubConfiguration
    {
        IPostStubConfiguration WithQueryString(string name, string value);
        IPostStubConfiguration WithBody(string body);
        IPostStubConfiguration WithJson(object json);
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

        public IPostStubConfiguration WithJson(object json)
        {
            _jsonBody = json;
            return this;
        }

        public bool MatchesRequest(HttpListenerRequest request)
        {
            var reader = new StreamReader(request.InputStream);
            var body = reader.ReadToEnd();

            return _queryStringParameters.All(q => request.QueryString[q.Key] == q.Value)
                   && (_body == null || _body.Equals(body))
                   && (_jsonBody == null || MatchesJsonBody(body));
        }

        private bool MatchesJsonBody(string json)
        {
            var obj = JsonConvert.DeserializeObject(json, _jsonBody.GetType());
            var properties1 = _jsonBody.GetType().GetProperties();
            var properties2 = obj.GetType().GetProperties();

            return properties1.All(p1 => properties2.SingleOrDefault(p2 => p2.Name == p1.Name && p2.GetValue(obj).Equals(p1.GetValue(_jsonBody))) != null);
        }

        private readonly Dictionary<string, string> _queryStringParameters = new Dictionary<string, string>();
        private readonly IServer _server;
        private string _body;
        private object _jsonBody;
    }
}