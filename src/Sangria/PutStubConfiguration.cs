using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sangria.Exceptions;
using Sangria.Resources;

namespace Sangria
{
    public interface IPutStubConfiguration : IStubConfiguration
    {
        IPutStubConfiguration WithBody(string body);
        IPutStubConfiguration WithJson(object json);
    }

    public class PutStubConfiguration : IPutStubConfiguration
    {
        public HttpVerb HttpVerb => HttpVerb.Put;
        public string Resource { get; }
        public bool IsFallback { get; private set; }
        public StubResponse StubbedResponse { get; private set; }

        public PutStubConfiguration(IServer server, string resource)
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

        public IPutStubConfiguration WithBody(string body)
        {
            _body = body;
            return this;
        }

        public IPutStubConfiguration WithJson(object json)
        {
            _jsonBody = json;
            return this;
        }

        public bool MatchesRequest(HttpListenerRequest request)
        {
            var reader = new StreamReader(request.InputStream);
            var body = reader.ReadToEnd();

            return _queryStringParameters.All(q => request.QueryString[q.Key] == q.Value) &&
                   _headers.All(h => request.Headers[h.Key] == h.Value) && 
                   (_body == null || _body.Equals(body)) && 
                   (_jsonBody == null || MatchesJsonBody(body));
        }

        private bool MatchesJsonBody(string jsonString)
        {
            var obj = JsonConvert.DeserializeAnonymousType(jsonString, _jsonBody);
            var j1 = JToken.FromObject(obj);
            var j2 = JToken.FromObject(_jsonBody);

            return JToken.DeepEquals(j1, j2);
        }

        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _queryStringParameters = new Dictionary<string, string>();
        private readonly IServer _server;
        private string _body;
        private object _jsonBody;
    }
}