using System.Net;

namespace Sangria
{
    public interface IStubConfiguration
    {
        HttpVerb HttpVerb { get; }
        string Resource { get; }
        bool IsFallback { get; }
        bool MatchesRequest(HttpListenerRequest request);
        StubResponse StubbedResponse { get; }
        IStubConfiguration Fallback();
        IServer Returns(StubResponse response);
    }
}