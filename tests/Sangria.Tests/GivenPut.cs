using System.Net;
using NUnit.Framework;
using RestSharp;
using Sangria.Exceptions;

namespace Sangria.Tests
{
    public class GivenPut
    {
        [Test]
        public void WhenIPutAResourceWithNoPredefinedBindings_Then404IsReturned()
        {
            const int port = 8080;

            using (var server = new Server(port))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var response = client.Put(new RestRequest("test"));
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            }
        }

        [Test]
        public void WhenIMake2PutRequestsForAResourceWithNoPredefinedBindings_Then404IsReturnedTwice()
        {
            const int port = 8080;

            using (var server = new Server(port))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var response1 = client.Put(new RestRequest("test"));
                var response2 = client.Put(new RestRequest("test"));

                Assert.That(response1.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(response2.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            }
        }

        [Test]
        public void WhenIPutAKnownResource_ThenConfiguredResponseIsReturned()
        {
            const int port = 8080;
            const string expectedResponse = "<html><body>Great success!</body></html>";

            using (var server = new Server(port)
                .OnPut("Test")
                .Returns(new StubResponse(HttpStatusCode.OK, expectedResponse)))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var response = client.Put(new RestRequest("test"));
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content, Is.EqualTo(expectedResponse));
            }
        }

        [Test]
        public void WhenICreateABindingWithASpecificQueryString_AndPutWithNoQueryString_Then404()
        {
            const int port = 8080;
            const string expectedResponse = "<html><body>Great success!</body></html>";

            using (var server = new Server(port)
                .OnPut("Test")
                .WithQueryStringParameter("id", "1")
                .Returns(new StubResponse(HttpStatusCode.OK, expectedResponse)))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var restRequest = new RestRequest("test");
                var response = client.Put(restRequest);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            }
        }

        [TestCase("1", HttpStatusCode.OK)]
        [TestCase("2", HttpStatusCode.NotFound)]
        [TestCase("3", HttpStatusCode.Ambiguous)]
        public void WhenICreateABindingWithASpecificQueryString_AndMakeAPutRequestWithQueryString_ThenConfiguredResponseIsReturned(string id, HttpStatusCode statusCode)
        {
            const int port = 8080;
            const string expectedResponse = "<html><body>Great success!</body></html>";

            using (var server = new Server(port)
                .OnPut("Test")
                   .WithQueryStringParameter("id", "1")
                   .Returns(new StubResponse(HttpStatusCode.OK, expectedResponse))
                .OnPut("Test")
                   .WithQueryStringParameter("id", "3")
                   .Returns(new StubResponse(HttpStatusCode.Ambiguous, expectedResponse)))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var restRequest = new RestRequest("test");
                restRequest.AddQueryParameter("id", id);

                var response = client.Put(restRequest);
                Assert.That(response.StatusCode, Is.EqualTo(statusCode));
            }
        }

        [Test]
        public void WhenICreateABindingWithASpecificQueryString_AndMakeAPutRequestWithIncorrectQueryString_Then404()
        {
            const int port = 8080;
            const string expectedResponse = "<html><body>Great success!</body></html>";

            using (var server = new Server(port)
                .OnPut("Test")
                   .WithQueryStringParameter("firstName", "bob")
                   .WithQueryStringParameter("surname", "marley")
                   .Returns(new StubResponse(HttpStatusCode.OK, expectedResponse)))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var restRequest = new RestRequest("test");
                restRequest.AddQueryParameter("firstName", "bob");

                var response = client.Put(restRequest);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            }
        }

        [Test]
        public void WhenICreateABindingForASpecificQueryStringAndSpecifyFallback_AndMakeAPutRequestWithIncorrectQueryString_ThenFallbackIsUsed()
        {
            const int port = 8080;
            const string expectedFallback = "<html><body>Fallback!</body></html>";

            using (var server = new Server(port)
                .OnPut("Test")
                    .WithQueryStringParameter("firstName", "bob")
                    .WithQueryStringParameter("surname", "marley")
                    .AsFallback()
                        .Returns(new StubResponse(HttpStatusCode.OK, expectedFallback)))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var restRequest = new RestRequest("test");
                restRequest.AddQueryParameter("firstName", "bob");

                var response = client.Put(restRequest);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content, Is.EqualTo(expectedFallback));
            }
        }

        [Test]
        public void WhenICreateABindingForSpecificPutBody_AndMakePutRequestWithCorrectData_ThenStubIsUsed()
        {
            const int port = 8080;
            const string expectedResponse = "<html><body>Success!</body></html>";
            const string putData = "Hello World.";

            using (var server = new Server(port)
                .OnPut("Test")
                    .WithBody(putData)
                    .Returns(new StubResponse(HttpStatusCode.OK, expectedResponse)))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var restRequest = new RestRequest("test");
                restRequest.AddParameter("text/json", putData, ParameterType.RequestBody);

                var response = client.Put(restRequest);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content, Is.EqualTo(expectedResponse));
            }
        }

        [Test]
        public void WhenICreateABindingForSpecificJsonBody_AndMakePutRequestWithCorrectJsonData_ThenStubIsUsed()
        {
            const int port = 8080;
            const string expectedResponse = "<html><body>Success!</body></html>";

            var json = new { FirstName = "Boby", Surname = "Marley" };

            using (var server = new Server(port)
                .OnPut("Test")
                    .WithJson(json)
                    .Returns(new StubResponse(HttpStatusCode.OK, expectedResponse)))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var restRequest = new RestRequest("test");
                restRequest.AddJsonBody(json);

                var response = client.Put(restRequest);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content, Is.EqualTo(expectedResponse));
            }
        }

        [Test]
        public void WhenICreateABindingForSpecificJsonBodyWithACollection_AndMakePutRequestWithCorrectJsonData_ThenStubIsUsed()
        {
            const int port = 8080;
            const string expectedResponse = "<html><body>Success!</body></html>";

            var json = new { Values = new[] { new { One = 1, Two = 2, Three = 3 }, new { One = 1, Two = 2, Three = 3 } } };

            using (var server = new Server(port)
                .OnPut("Test")
                    .WithJson(json)
                    .Returns(new StubResponse(HttpStatusCode.OK, expectedResponse)))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var restRequest = new RestRequest("test");
                restRequest.AddJsonBody(json);

                var response = client.Put(restRequest);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content, Is.EqualTo(expectedResponse));
            }
        }

        [Test]
        public void WhenICreateABindingWithASpecificHttpHeader_AndMakeARequestWithTheCorrectDetails_ThenTheResponseIsReturned()
        {
            const int port = 8080;
            const string expectedValue = "<html><body>Success!</body></html>";

            using (var server = new Server(port)
                .OnPut("Test")
                    .WithHeader(HttpRequestHeader.ContentType, "Test")
                        .Returns(new StubResponse(HttpStatusCode.OK, expectedValue)))
            {
                server.Start();

                var client = new RestClient($"http://localhost:{port}");
                var restRequest = new RestRequest("test");
                restRequest.AddHeader(HttpRequestHeader.ContentType.ToString(), "Test");

                var response = client.Put(restRequest);
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));
                Assert.That(response.Content, Is.EqualTo(expectedValue));
            }
        }

        [Test]
        public void WhenITryToRegisterADuplicateQueryStringAndMakePutRequest_ThenDuplicateBindingExceptionIsThrown()
        {
            const int port = 8080;

            Assert.Throws<InvalidBindingException>(() =>
            {
                new Server(port)
                    .OnPut("Test")
                        .WithQueryStringParameter("id", "1")
                        .WithQueryStringParameter("id", "2");
            });
        }
    }
}