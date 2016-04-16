using System;
using Spritz.Exceptions;
using Spritz.Proxies;

namespace Spritz
{
    public static class Extensions
    {
        public static IMethodProxy WasCalled(this object obj, Action action)
        {
            var objectProxy = GetObjectProxy(obj);
            return new MethodProxy(objectProxy, action);
        }

        public static object Setup(this object obj, Action action, Action methodBody)
        {
            var objectProxy = GetObjectProxy(obj);
            objectProxy.SetupMethod(action, new object[] { }, methodBody);

            return obj;
        }

        public static object Setup(this object obj, Action action, Action methodBody, params object[] arguments)
        {
            var objectProxy = GetObjectProxy(obj);
            objectProxy.SetupMethod(action, arguments, methodBody);

            return obj;
        }

        public static object Returns(this object obj, Func<object> function, object returnedValue, params object[] arguments)
        {
            var objectProxy = GetObjectProxy(obj);
            objectProxy.SetupReturnedValue(function, arguments, returnedValue);

            return obj;
        }

        private static IObjectProxy GetObjectProxy(object obj)
        {
            var objectProxy = obj as IObjectProxy;
            if (objectProxy == null)
                throw new ProxyNotImplementedException(obj.GetType());

            return objectProxy;
        }
    }
}