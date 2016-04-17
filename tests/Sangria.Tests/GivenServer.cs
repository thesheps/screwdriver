using System.Net;
using System.Net.Sockets;
using NUnit.Framework;
using RestSharp;

namespace Sangria.Tests
{
    public class GivenServer
    {
        [Test]
        public void WhenIInitialiseAServerWithAGivenPort_ThenTheSpecifiedPortIsOpened()
        {
            const int port = 1234;
            var portIsOpened = false;

            using (new Server(port))
            {
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

            using (new Server(port))
            {
                var client = new RestClient($"http://localhost:{port}");
                var response = client.Get(new RestRequest("test"));
                Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            }
        }

        [Test]
        public void WhenIMake2RequestsForAResourceWithNoPredefinedBindings_Then404IsReturnedTwice()
        {
            const int port = 8080;

            using (new Server(port))
            {
                var client = new RestClient($"http://localhost:{port}");
                var response1 = client.Get(new RestRequest("test"));
                var response2 = client.Get(new RestRequest("test"));

                Assert.That(response1.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
                Assert.That(response2.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
            }
        }
    }
}