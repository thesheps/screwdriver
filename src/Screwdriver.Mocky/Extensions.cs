using System;
using Screwdriver.Mocking.Exceptions;
using Screwdriver.Mocking.Proxies;

namespace Screwdriver.Mocking
{
    public static class Extensions
    {
        public static IMethodProxy WasCalled(this object obj, Action action)
        {
            var objectProxy = GetMethodProxy(obj);
            return new MethodProxy(objectProxy, action);
        }

        public static object Setup(this object obj, Action action, Action methodBody)
        {
            var objectProxy = GetMethodProxy(obj);
            objectProxy.SetupMethod(action, new object[] { }, methodBody);

            return obj;
        }

        public static object Setup(this object obj, Action action, Action methodBody, params object[] arguments)
        {
            var objectProxy = GetMethodProxy(obj);
            objectProxy.SetupMethod(action, arguments, methodBody);

            return obj;
        }

        public static object Returns(this object obj, Func<object> function, object returnedValue, params object[] arguments)
        {
            var objectProxy = GetMethodProxy(obj);
            objectProxy.SetupReturnedValue(function, arguments, returnedValue);

            return obj;
        }

        private static IObjectProxy GetMethodProxy(object obj)
        {
            var objectProxy = obj as IObjectProxy;
            if (objectProxy == null)
                throw new ProxyNotImplementedException(obj.GetType());
            return objectProxy;
        }
    }
}