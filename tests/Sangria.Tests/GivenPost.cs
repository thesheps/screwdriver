using System.Net;
using NUnit.Framework;
using RestSharp;
using Sangria.Exceptions;

namespace Sangria.Tests
{
    public class GivenPost
    {
        [Test]
        public void WhenIPostAResourceWithNoPredefinedBindings_Then404IsReturned()
        {
            const int port = 8080;

            using (var server = new Server(port))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var response = client.Post(new RestRequest("test"));
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            }
        }

        [Test]
        public void WhenIMake2PostRequestsForAResourceWithNoPredefinedBindings_Then404IsReturnedTwice()
        {
            const int port = 8080;

            using (var server = new Server(port))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var response1 = client.Post(new RestRequest("test"));
                var response2 = client.Post(new RestRequest("test"));

                Assert.That(response1.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(response2.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            }
        }

        [Test]
        public void WhenIPostAKnownResource_ThenConfiguredResponseIsReturned()
        {
            const int port = 8080;
            const string expectedResponse = "<html><body>Great success!</body></html>";

            using (var server = new Server(port)
                .OnPost("Test")
                .Returns(new StubResponse(HttpStatusCode.OK, expectedResponse)))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var response = client.Post(new RestRequest("test"));
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content, Is.EqualTo(expectedResponse));
            }
        }

        [Test]
        public void WhenICreateABindingWithASpecificQueryString_AndPostWithNoQueryString_Then404()
        {
            const int port = 8080;
            const string expectedResponse = "<html><body>Great success!</body></html>";

            using (var server = new Server(port)
                .OnPost("Test")
                .WithQueryString("id", "1")
                .Returns(new StubResponse(HttpStatusCode.OK, expectedResponse)))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var restRequest = new RestRequest("test");
                var response = client.Post(restRequest);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            }
        }

        [TestCase("1", HttpStatusCode.OK)]
        [TestCase("2", HttpStatusCode.NotFound)]
        [TestCase("3", HttpStatusCode.Ambiguous)]
        public void WhenICreateABindingWithASpecificQueryString_AndMakeAPostRequestWithQueryString_ThenConfiguredResponseIsReturned(string id, HttpStatusCode statusCode)
        {
            const int port = 8080;
            const string expectedResponse = "<html><body>Great success!</body></html>";

            using (var server = new Server(port)
                .OnPost("Test")
                   .WithQueryString("id", "1")
                   .Returns(new StubResponse(HttpStatusCode.OK, expectedResponse))
                .OnPost("Test")
                   .WithQueryString("id", "3")
                   .Returns(new StubResponse(HttpStatusCode.Ambiguous, expectedResponse)))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var restRequest = new RestRequest("test");
                restRequest.AddQueryParameter("id", id);

                var response = client.Post(restRequest);
                Assert.That(response.StatusCode, Is.EqualTo(statusCode));
            }
        }

        [Test]
        public void WhenICreateABindingWithASpecificQueryString_AndMakeAPostRequestWithIncorrectQueryString_Then404()
        {
            const int port = 8080;
            const string expectedResponse = "<html><body>Great success!</body></html>";

            using (var server = new Server(port)
                .OnPost("Test")
                   .WithQueryString("firstName", "bob")
                   .WithQueryString("surname", "marley")
                   .Returns(new StubResponse(HttpStatusCode.OK, expectedResponse)))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var restRequest = new RestRequest("test");
                restRequest.AddQueryParameter("firstName", "bob");

                var response = client.Post(restRequest);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            }
        }

        [Test]
        public void WhenICreateABindingForASpecificQueryStringAndSpecifyFallback_AndMakeAPostRequestWithIncorrectQueryString_ThenBackupIsUsed()
        {
            const int port = 8080;
            const string expectedFallback = "<html><body>Fallback!</body></html>";

            using (var server = new Server(port)
                .OnPost("Test")
                    .WithQueryString("firstName", "bob")
                    .WithQueryString("surname", "marley")
                    .Fallback()
                        .Returns(new StubResponse(HttpStatusCode.OK, expectedFallback)))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var restRequest = new RestRequest("test");
                restRequest.AddQueryParameter("firstName", "bob");

                var response = client.Post(restRequest);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content, Is.EqualTo(expectedFallback));
            }
        }

        [Test]
        public void WhenITryToRegisterDuplicateQueryStringAndMakePostRequest_ThenDuplicateBindingExceptionIsThrown()
        {
            const int port = 8080;

            Assert.Throws<InvalidBindingException>(() =>
            {
                new Server(port)
                    .OnPost("Test")
                        .WithQueryString("id", "1")
                        .WithQueryString("id", "2");
            });
        }
    }
}