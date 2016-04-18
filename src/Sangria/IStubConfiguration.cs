using System.Net;

namespace Sangria
{
    public interface IStubConfiguration
    {
        bool IsFallback { get; }
        string Resource { get; }
        HttpVerb HttpVerb { get; }
        StubResponse StubbedResponse { get; }
        IStubConfiguration Fallback();
        IStubConfiguration WithHeader(HttpRequestHeader header, string value);
        IStubConfiguration WithQueryStringParameter(string name, string value);
        IServer Returns(StubResponse response);
        bool MatchesRequest(HttpListenerRequest request);
    }
}