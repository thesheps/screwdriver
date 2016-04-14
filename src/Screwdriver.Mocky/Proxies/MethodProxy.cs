using System;

namespace Screwdriver.Mocking.Proxies
{
    public interface IMethodProxy
    {
        bool AtLeastOnce();
        bool WithParameters(params object[] parameters);
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
            return _objectProxy.HasMethodBeenCalled(_action, new object[] { });
        }

        public bool WithParameters(params object[] parameters)
        {
            return _objectProxy.HasMethodBeenCalled(_action, parameters);
        }

        private readonly IObjectProxy _objectProxy;
        private readonly Action _action;
    }
}