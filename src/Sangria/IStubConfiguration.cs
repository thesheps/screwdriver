using System.Net;

namespace Sangria
{
    public interface IStubConfiguration
    {
        bool IsFallback { get; }
        HttpVerb HttpVerb { get; }
        string Resource { get; }
        StubResponse StubbedResponse { get; }
        IStubConfiguration Fallback();
        IServer Returns(StubResponse response);
        bool MatchesRequest(HttpListenerRequest request);
    }
}