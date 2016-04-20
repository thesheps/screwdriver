using System.Linq;
using System.Net;

namespace Sangria
{
    public interface IDeleteStubConfiguration : IStubConfiguration
    {
    }

    public class DeleteStubConfiguration : StubConfiguration, IDeleteStubConfiguration
    {
        public override HttpVerb HttpVerb => HttpVerb.Delete;

        public DeleteStubConfiguration(IServer server, string resource)
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