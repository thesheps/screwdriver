using System.Collections.Generic;

namespace Sangria
{
    public interface IStubConfiguration : IServer
    {
        IStubConfiguration WithQueryString(string name, string value);
    }

    public class StubConfiguration : IStubConfiguration
    {
        public string Resource { get; }
        public StubbedResponse StubbedResponse { get; }
        public Dictionary<string, string> QueryStringParameters = new Dictionary<string, string>(); 

        public StubConfiguration(IServer server, string resource, StubbedResponse stubbedResponse)
        {
            _server = server;
            Resource = resource;
            StubbedResponse = stubbedResponse;
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