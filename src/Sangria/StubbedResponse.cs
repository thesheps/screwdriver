using System.Net;

namespace Sangria
{
    public class StubbedResponse
    {
        public HttpStatusCode StatusCode { get; }
        public string Response { get; }

        public StubbedResponse(HttpStatusCode statusCode, string response)
        {
            StatusCode = statusCode;
            Response = response;
        }
    }
}