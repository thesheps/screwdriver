using System.Collections.Generic;
using System.Linq;
using System.Net;
using Sangria.Exceptions;
using Sangria.Resources;

namespace Sangria
{
    public interface IGetStubConfiguration : IStubConfiguration
    {
        IGetStubConfiguration WithQueryString(string name, string value);
    }

    public class GetStubConfiguration : StubConfiguration, IGetStubConfiguration
    {
        public override HttpVerb HttpVerb => HttpVerb.Get;

        public GetStubConfiguration(IServer server, string resource)
            : base(server, resource)
        {
        }

        public IGetStubConfiguration WithQueryString(string name, string value)
        {
            if (_queryStringParameters.ContainsKey(name))
                throw new InvalidBindingException(string.Format(Errors.DuplicateQueryStringParameter, name));

            _queryStringParameters.Add(name, value);

            return this;
        }

        public override bool MatchesRequest(HttpListenerRequest request)
        {
            return _queryStringParameters.All(q => request.QueryString[q.Key] == q.Value);
        }

        private readonly Dictionary<string, string> _queryStringParameters = new Dictionary<string, string>();
    }
}