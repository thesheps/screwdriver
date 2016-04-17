using System.Net;

namespace Sangria
{
    public class StubbedResponse
    {
        public HttpStatusCode StatusCode { get; }
        public string Body { get; }

        public StubbedResponse(HttpStatusCode statusCode, string body)
        {
            StatusCode = statusCode;
            Body = body;
        }
    }
}