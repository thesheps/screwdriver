using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using Newtonsoft.Json;
using Sangria.Exceptions;
using Sangria.Resources;

namespace Sangria
{
    public interface IPostStubConfiguration : IStubConfiguration
    {
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
                   && _headers.All(h => request.Headers[h.Key] == h.Value)
                   && (_body == null || _body.Equals(body))
                   && (_jsonBody == null || MatchesJsonBody(body));
        }

        private bool MatchesJsonBody(string json)
        {
            var obj = JsonConvert.DeserializeObject(json, _jsonBody.GetType());
            return ObjectsAreEqual(obj, _jsonBody);
        }

        private static bool ObjectsAreEqual(object o1, object o2)
        {
            var properties1 = o1.GetType().GetProperties();
            var properties2 = o2.GetType().GetProperties();

            return properties1.All(p1 => properties2.SingleOrDefault(p2 => PropertiesAreEqual(new Property(o1, p1), new Property(o2, p2))) != null);
        }

        private static bool PropertiesAreEqual(Property property1, Property property2)
        {
            if (property1.Name != property2.Name || property1.Type != property2.Type)
                return false;

            var value1 = property1.GetValue();
            var value2 = property2.GetValue();

            if (!property1.IsCollection())
                return value1.Equals(value2);

            var collection1 = (ICollection<object>)value1;
            var collection2 = (ICollection<object>)value2;

            return collection1.All(p1 => collection2.Any(p2 => ObjectsAreEqual(p1, p2)));
        }

        private readonly Dictionary<string, string> _headers = new Dictionary<string, string>();
        private readonly Dictionary<string, string> _queryStringParameters = new Dictionary<string, string>();
        private readonly IServer _server;
        private string _body;
        private object _jsonBody;

        private class Property
        {
            public string Name => _propertyInfo.Name;
            public Type Type => _propertyInfo.PropertyType;

            public Property(object sourceObject, PropertyInfo propertyInfo)
            {
                _sourceObject = sourceObject;
                _propertyInfo = propertyInfo;
            }

            public object GetValue()
            {
                return _propertyInfo.GetValue(_sourceObject);
            }

            public bool IsCollection()
            {
                return Type.GetInterface("ICollection") != null;
            }

            private readonly object _sourceObject;
            private readonly PropertyInfo _propertyInfo;
        }
    }
}