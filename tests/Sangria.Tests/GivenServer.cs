using System.Net;
using NUnit.Framework;
using RestSharp;
using Sangria.Tests.TestClasses;

namespace Sangria.Tests
{
    public class GivenServer
    {
        [Test]
        public void WhenICallAGivenKnownStub_ThenICanAssertThatItHasBeenCalled()
        {
            const int port = Constants.Port;
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

        [Test]
        public void WhenIHaveAStubImplementation_ThenICanAssertThatItHasBeenCalled()
        {
            const int port = Constants.Port;
            const string expectedResponse = "<html><body>Great success!</body></html>";

            using (var server = new Server(port))
            {
                server.AddStubConfiguration(new TestGetStubConfiguration(server));
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var restRequest = new RestRequest("test");
                restRequest.AddHeader(HttpRequestHeader.ContentType.ToString(), "Test");

                var response = client.Get(restRequest);
                Assert.That(server.Configurations[0].ReceivedRequests(1));
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content, Is.EqualTo(expectedResponse));
            }
        }
    }
}