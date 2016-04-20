using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sangria
{
    public interface IPutStubConfiguration : IStubConfiguration
    {
        IPutStubConfiguration WithBody(string body);
        IPutStubConfiguration WithJson(object json);
    }

    public class PutStubConfiguration : StubConfiguration, IPutStubConfiguration
    {
        public override HttpVerb HttpVerb => HttpVerb.Put;

        public PutStubConfiguration(IServer server, string resource)
            : base(server, resource)
        {
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

        public override bool MatchesRequest(HttpListenerRequest request)
        {
            var reader = new StreamReader(request.InputStream);
            var body = reader.ReadToEnd();

            return QueryStringParameters.All(q => request.QueryString[q.Key] == q.Value) &&
                   Headers.All(h => request.Headers[h.Key] == h.Value) &&
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

        private string _body;
        private object _jsonBody;
    }
}