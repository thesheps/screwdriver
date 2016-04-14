using System;
using Screwdriver.Mocking.Exceptions;
using Screwdriver.Mocking.Proxies;

namespace Screwdriver.Mocking
{
    public static class Extensions
    {
        public static IMethodProxy WasCalled(this object obj, Action action)
        {
            var objectProxy = obj as IObjectProxy;
            if (objectProxy == null)
                throw new ProxyNotImplementedException(obj.GetType());

            return new MethodProxy(objectProxy, action);
        }
    }
}