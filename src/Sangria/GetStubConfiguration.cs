using System.Linq;
using System.Net;

namespace Sangria
{
    public interface IGetStubConfiguration : IStubConfiguration
    {
    }

    public class GetStubConfiguration : StubConfiguration, IGetStubConfiguration
    {
        public override HttpVerb HttpVerb => HttpVerb.Get;

        public GetStubConfiguration(IServer server, string resource)
            : base(server, resource)
        {
        }

        public override bool MatchesRequest(HttpListenerRequest request)
        {
            return QueryStringParameters.All(q => request.QueryString[q.Key] == q.Value) &&
                   Headers.All(h => request.Headers[h.Key] == h.Value);
        }
    }
}