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

        public static object Setup(this object obj, Action action, Action methodBody)
        {
            var objectProxy = obj as IObjectProxy;
            if (objectProxy == null)
                throw new ProxyNotImplementedException(obj.GetType());

            objectProxy.SetupMethod(action, new object[] { }, methodBody);

            return obj;
        }

        public static object Setup(this object obj, Action action, Action methodBody, params object[] arguments)
        {
            var objectProxy = obj as IObjectProxy;
            if (objectProxy == null)
                throw new ProxyNotImplementedException(obj.GetType());

            objectProxy.SetupMethod(action, arguments, methodBody);

            return obj;
        }

        public static object Returns(this object obj, Func<object> function, object returnedValue, params object[] arguments)
        {
            var objectProxy = obj as IObjectProxy;
            if (objectProxy == null)
                throw new ProxyNotImplementedException(obj.GetType());

            objectProxy.SetupReturn(function, arguments, returnedValue);

            return obj;
        }
    }
}