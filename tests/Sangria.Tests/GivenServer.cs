using System.Net;
using NUnit.Framework;
using RestSharp;

namespace Sangria.Tests
{
    public class GivenServer
    {
        [Test]
        public void WhenICallAGivenKnownStub_ThenICanAssertThatItHasBeenCalled()
        {
            const int port = 8080;
            const string expectedResponse = "<html><body>Great success!</body></html>";

            using (var server = new Server(port)
                .OnGet("Test")
                .Returns(new StubResponse(HttpStatusCode.OK, expectedResponse)))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                client.Get(new RestRequest("test"));

                Assert.That(server.Configurations[0].ReceivedRequests(1));
            }
        }
    }
}