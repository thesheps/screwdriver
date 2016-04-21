using System.Net;
using NUnit.Framework;
using RestSharp;
using Sangria.Exceptions;

namespace Sangria.Tests
{
    public class GivenPost
    {
        [SetUp]
        public void SetUp()
        {
            NetAclChecker.AddAddress($"http://localhost:{Constants.Port}");
        }

        [Test]
        public void WhenIPostAResourceWithNoPredefinedBindings_Then404IsReturned()
        {
            const int port = Constants.Port;

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
            const int port = Constants.Port;

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
            const int port = Constants.Port;
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
            const int port = Constants.Port;
            const string expectedResponse = "<html><body>Great success!</body></html>";

            using (var server = new Server(port)
                .OnPost("Test")
                .WithQueryStringParameter("id", "1")
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
            const int port = Constants.Port;
            const string expectedResponse = "<html><body>Great success!</body></html>";

            using (var server = new Server(port)
                .OnPost("Test")
                   .WithQueryStringParameter("id", "1")
                   .Returns(new StubResponse(HttpStatusCode.OK, expectedResponse))
                .OnPost("Test")
                   .WithQueryStringParameter("id", "3")
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
            const int port = Constants.Port;
            const string expectedResponse = "<html><body>Great success!</body></html>";

            using (var server = new Server(port)
                .OnPost("Test")
                   .WithQueryStringParameter("firstName", "bob")
                   .WithQueryStringParameter("surname", "marley")
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
        public void WhenICreateABindingForASpecificQueryStringAndSpecifyFallback_AndMakeAPostRequestWithIncorrectQueryString_ThenFallbackIsUsed()
        {
            const int port = Constants.Port;
            const string expectedFallback = "<html><body>Fallback!</body></html>";

            using (var server = new Server(port)
                .OnPost("Test")
                    .WithQueryStringParameter("firstName", "bob")
                    .WithQueryStringParameter("surname", "marley")
                    .AsFallback()
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
        public void WhenICreateABindingForSpecificPostBody_AndMakePostRequestWithCorrectPostData_ThenStubIsUsed()
        {
            const int port = Constants.Port;
            const string expectedResponse = "<html><body>Success!</body></html>";
            const string postData = "Hello World.";

            using (var server = new Server(port)
                .OnPost("Test")
                    .WithBody(postData)
                    .Returns(new StubResponse(HttpStatusCode.OK, expectedResponse)))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var restRequest = new RestRequest("test");
                restRequest.AddParameter("text/json", postData, ParameterType.RequestBody);

                var response = client.Post(restRequest);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content, Is.EqualTo(expectedResponse));
            }
        }

        [Test]
        public void WhenICreateABindingForSpecificJsonBody_AndMakePostRequestWithCorrectJsonData_ThenStubIsUsed()
        {
            const int port = Constants.Port;
            const string expectedResponse = "<html><body>Success!</body></html>";

            var json = new { FirstName = "Boby", Surname = "Marley" };

            using (var server = new Server(port)
                .OnPost("Test")
                    .WithJson(json)
                    .Returns(new StubResponse(HttpStatusCode.OK, expectedResponse)))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var restRequest = new RestRequest("test");
                restRequest.AddJsonBody(json);

                var response = client.Post(restRequest);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content, Is.EqualTo(expectedResponse));
            }
        }

        [Test]
        public void WhenICreateABindingForSpecificJsonBodyWithACollection_AndMakePostRequestWithCorrectJsonData_ThenStubIsUsed()
        {
            const int port = Constants.Port;
            const string expectedResponse = "<html><body>Success!</body></html>";

            var json = new { Values = new[] { new { One = 1, Two = 2, Three = 3 }, new { One = 1, Two = 2, Three = 3 } } };

            using (var server = new Server(port)
                .OnPost("Test")
                    .WithJson(json)
                    .Returns(new StubResponse(HttpStatusCode.OK, expectedResponse)))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var restRequest = new RestRequest("test");
                restRequest.AddJsonBody(json);

                var response = client.Post(restRequest);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content, Is.EqualTo(expectedResponse));
            }
        }

        [Test]
        public void WhenICreateABindingWithASpecificHttpHeader_AndMakeARequestWithTheCorrectDetails_ThenTheResponseIsReturned()
        {
            const int port = Constants.Port;
            const string expectedValue = "<html><body>Success!</body></html>";

            using (var server = new Server(port)
                .OnPost("Test")
                    .WithHeader(HttpRequestHeader.ContentType, "Test")
                        .Returns(new StubResponse(HttpStatusCode.OK, expectedValue)))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var restRequest = new RestRequest("test");
                restRequest.AddHeader(HttpRequestHeader.ContentType.ToString(), "Test");

                var response = client.Post(restRequest);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content, Is.EqualTo(expectedValue));
            }
        }

        [Test]
        public void WhenITryToRegisterADuplicateQueryStringAndMakePostRequest_ThenDuplicateBindingExceptionIsThrown()
        {
            const int port = Constants.Port;

            Assert.Throws<InvalidBindingException>(() =>
            {
                new Server(port)
                    .OnPost("Test")
                        .WithQueryStringParameter("id", "1")
                        .WithQueryStringParameter("id", "2");
            });
        }
    }
}