using System.Collections.Generic;
using Sangria.Exceptions;
using Sangria.Resources;

namespace Sangria
{
    public interface IStubConfiguration : IServer
    {
        string Resource { get; }
        StubbedResponse StubbedResponse { get; }
        Dictionary<string, string> QueryStringParameters { get; }
        IStubConfiguration WithQueryString(string name, string value);
    }

    public class StubConfiguration : IStubConfiguration
    {
        public string Resource { get; }
        public StubbedResponse StubbedResponse { get; }
        public Dictionary<string, string> QueryStringParameters { get; }

        public StubConfiguration(IServer server, string resource, StubbedResponse stubbedResponse)
        {
            _server = server;
            Resource = resource;
            StubbedResponse = stubbedResponse;
            QueryStringParameters = new Dictionary<string, string>();
        }

        public void Start()
        {
            _server.Start();
        }

        public IStubConfiguration OnGet(string resource, StubbedResponse stubbedResponse)
        {
            return _server.OnGet(resource, stubbedResponse);
        }

        public IStubConfiguration WithQueryString(string name, string value)
        {
            if (QueryStringParameters.ContainsKey(name))
                throw new InvalidBindingException(string.Format(Errors.DuplicateQueryStringParameter, name));

            QueryStringParameters.Add(name, value);
            return this;
        }

        public void Dispose()
        {
            _server.Dispose();
        }

        private readonly IServer _server;
    }
}