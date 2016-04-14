using System;

namespace Screwdriver.Mocking.Proxies
{
    public interface IMethodProxy
    {
        bool AtLeastOnce();
        bool Exactly(int times);
        IMethodProxy WithParameters(params object[] parameters);
    }

    public class MethodProxy : IMethodProxy
    {
        public MethodProxy(IObjectProxy objectProxy, Action action)
        {
            _objectProxy = objectProxy;
            _action = action;
        }

        public bool AtLeastOnce()
        {
            return _objectProxy.GetMethodCallCount(_action, _parameters) >= 1;
        }

        public bool Exactly(int times)
        {
            return _objectProxy.GetMethodCallCount(_action, _parameters) == times;
        }

        public IMethodProxy WithParameters(params object[] parameters)
        {
            _parameters = parameters;
            return this;
        }

        private readonly IObjectProxy _objectProxy;
        private readonly Action _action;
        private object[] _parameters = { };
    }
}