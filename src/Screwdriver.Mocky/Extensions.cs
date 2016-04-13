using System;
using Screwdriver.Mocking.Exceptions;

namespace Screwdriver.Mocking
{
    public static class Extensions
    {
        public static bool WasCalled(this object obj, Action action, params object[] parameters)
        {
            var proxy = obj as IProxy;
            if (proxy == null)
                throw new ProxyNotImplementedException(obj.GetType());

            return proxy.HasMethodBeenCalled(action, parameters);
        }
    }
}