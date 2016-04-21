using System.Net;
using System.Net.Sockets;
using NUnit.Framework;
using RestSharp;
using Sangria.Exceptions;

namespace Sangria.Tests
{
    public class GivenGet
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
        public void WhenIGetAResourceWithNoPredefinedBindings_Then404IsReturned()
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

            using (var server = new Server(port)
                .OnGet("Test")
                .Returns(new StubResponse(HttpStatusCode.OK, expectedResponse)))
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
                .OnGet("Test")
                .WithQueryStringParameter("id", "1")
                .Returns(new StubResponse(HttpStatusCode.OK, expectedResponse)))
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
                .OnGet("Test")
                   .WithQueryStringParameter("id", "1")
                   .Returns(new StubResponse(HttpStatusCode.OK, expectedResponse))
                .OnGet("Test")
                   .WithQueryStringParameter("id", "3")
                   .Returns(new StubResponse(HttpStatusCode.Ambiguous, expectedResponse)))
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
        public void WhenICreateABindingWithASpecificQueryString_AndMakeARequestWithIncorrectQueryString_Then404()
        {
            const int port = 8080;
            const string expectedResponse = "<html><body>Great success!</body></html>";

            using (var server = new Server(port)
                .OnGet("Test")
                   .WithQueryStringParameter("firstName", "bob")
                   .WithQueryStringParameter("surname", "marley")
                   .Returns(new StubResponse(HttpStatusCode.OK, expectedResponse)))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var restRequest = new RestRequest("test");
                restRequest.AddQueryParameter("firstName", "bob");

                var response = client.Get(restRequest);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            }
        }

        [Test]
        public void WhenICreateABindingForASpecificQueryStringAndSpecifyFallback_AndMakeARequestWithIncorrectQueryString_ThenBackupIsUsed()
        {
            const int port = 8080;
            const string expectedFallback = "<html><body>Fallback!</body></html>";

            using (var server = new Server(port)
                .OnGet("Test")
                    .WithQueryStringParameter("firstName", "bob")
                    .WithQueryStringParameter("surname", "marley")
                    .AsFallback()
                        .Returns(new StubResponse(HttpStatusCode.OK, expectedFallback)))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var restRequest = new RestRequest("test");
                restRequest.AddQueryParameter("firstName", "bob");

                var response = client.Get(restRequest);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content, Is.EqualTo(expectedFallback));
            }
        }

        [Test]
        public void WhenICreateABindingWithASpecificHttpHeader_AndMakeARequestWithTheCorrectDetails_ThenTheResponseIsReturned()
        {
            const int port = 8080;
            const string expectedValue = "<html><body>Success!</body></html>";

            using (var server = new Server(port)
                .OnGet("Test")
                    .WithHeader(HttpRequestHeader.ContentType, "Test")
                        .Returns(new StubResponse(HttpStatusCode.OK, expectedValue)))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var restRequest = new RestRequest("test");
                restRequest.AddHeader(HttpRequestHeader.ContentType.ToString(), "Test");

                var response = client.Get(restRequest);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content, Is.EqualTo(expectedValue));
            }
        }

        [Test]
        public void WhenITryToRegisterDuplicateQueryString_ThenDuplicateBindingExceptionIsThrown()
        {
            const int port = 8080;

            Assert.Throws<InvalidBindingException>(() =>
            {
                new Server(port)
                    .OnGet("Test")
                        .WithQueryStringParameter("id", "1")
                        .WithQueryStringParameter("id", "2");
            });
        }
    }
}