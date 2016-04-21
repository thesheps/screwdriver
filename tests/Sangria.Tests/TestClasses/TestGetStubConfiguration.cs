using System.Net;

namespace Sangria.Tests.TestClasses
{
    public class TestGetStubConfiguration : GetStubConfiguration
    {
        public TestGetStubConfiguration(IServer server) : base(server, "Test")
        {
            StubbedResponse = new StubResponse(HttpStatusCode.OK, "<html><body>Great success!</body></html>");
        }
    }
}