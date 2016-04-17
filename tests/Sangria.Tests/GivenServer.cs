using System.Net;
using System.Net.Sockets;
using NUnit.Framework;
using RestSharp;
using Sangria.Exceptions;

namespace Sangria.Tests
{
    public class GivenServer
    {
        [Test]
        public void WhenIInitialiseAServerWithAGivenPort_ThenTheSpecifiedPortIsOpened()
        {
            const int port = 1234;
            var portIsOpened = false;

            using (var server = new Server(port))
            {
                server.Start();

                using (var client = new TcpClient())
                {
                    try
                    {
                        client.Connect("127.0.0.1", port);
                        portIsOpened = true;
                    }
                    catch
                    {
                        // ignored
                    }
                }
            }

            Assert.That(portIsOpened);
        }

        [Test]
        public void WhenIRequestAResourceWithNoPredefinedBindings_Then404IsReturned()
        {
            const int port = 8080;

            using (var server = new Server(port))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var response = client.Get(new RestRequest("test"));
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            }
        }

        [Test]
        public void WhenIMake2RequestsForAResourceWithNoPredefinedBindings_Then404IsReturnedTwice()
        {
            const int port = 8080;

            using (var server = new Server(port))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var response1 = client.Get(new RestRequest("test"));
                var response2 = client.Get(new RestRequest("test"));

                Assert.That(response1.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(response2.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            }
        }

        [Test]
        public void WhenIRequestAKnownResource_ThenConfiguredResponseIsReturned()
        {
            const int port = 8080;
            const string expectedResponse = "<html><body>Great success!</body></html>";

            using (var server = new Server(port).OnGet("Test", new StubbedResponse(HttpStatusCode.OK, expectedResponse)))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var response = client.Get(new RestRequest("test"));
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content, Is.EqualTo(expectedResponse));
            }
        }

        [Test]
        public void WhenICreateABindingWithASpecificQueryString_AndMakeARequestWithNoQueryString_Then404()
        {
            const int port = 8080;
            const string expectedResponse = "<html><body>Great success!</body></html>";

            using (var server = new Server(port)
                .OnGet("Test", new StubbedResponse(HttpStatusCode.OK, expectedResponse))
                .WithQueryString("id", "1"))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var restRequest = new RestRequest("test");
                var response = client.Get(restRequest);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            }
        }

        [TestCase("1", HttpStatusCode.OK)]
        [TestCase("2", HttpStatusCode.NotFound)]
        [TestCase("3", HttpStatusCode.Ambiguous)]
        public void WhenICreateABindingWithASpecificQueryString_AndMakeARequestWithQueryString_ThenConfiguredResponseIsReturned(string id, HttpStatusCode statusCode)
        {
            const int port = 8080;
            const string expectedResponse = "<html><body>Great success!</body></html>";

            using (var server = new Server(port)
                .OnGet("Test", new StubbedResponse(HttpStatusCode.OK, expectedResponse))
                   .WithQueryString("id", "1")
                .OnGet("Test", new StubbedResponse(HttpStatusCode.Ambiguous, expectedResponse))
                   .WithQueryString("id", "3"))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var restRequest = new RestRequest("test");
                restRequest.AddQueryParameter("id", id);

                var response = client.Get(restRequest);
                Assert.That(response.StatusCode, Is.EqualTo(statusCode));
            }
        }

        [Test]
        public void WhenITryToRegisterDuplicateQueryString_ThenDuplicateBindingExceptionIsThrown()
        {
            const int port = 8080;
            const string expectedResponse = "<html><body>Great success!</body></html>";

            Assert.Throws<InvalidBindingException>(() =>
            {
                new Server(port)
                    .OnGet("Test", new StubbedResponse(HttpStatusCode.OK, expectedResponse))
                    .WithQueryString("id", "1")
                    .WithQueryString("id", "2");
            });
        }
    }
}