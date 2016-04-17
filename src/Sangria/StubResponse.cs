using System.Net;

namespace Sangria
{
    public class StubResponse
    {
        public HttpStatusCode StatusCode { get; }
        public string Body { get; }

        public StubResponse(HttpStatusCode statusCode, string body)
        {
            StatusCode = statusCode;
            Body = body;
        }
    }
}