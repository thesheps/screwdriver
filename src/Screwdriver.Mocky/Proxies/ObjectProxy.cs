using System;
using System.Collections.Generic;
using System.Linq;

namespace Screwdriver.Mocking.Proxies
{
    public interface IObjectProxy
    {
        void CallMethod(string methodName, object[] parameters);
        bool HasMethodBeenCalled(Action action, object[] parameters);
    }

    public abstract class ObjectProxy : IObjectProxy
    {
        public void CallMethod(string methodName, object[] parameters)
        {
            if (!_methodCalls.ContainsKey(methodName))
                _methodCalls.Add(methodName, new MethodCall(parameters));

            _methodCalls[methodName].Calls++;
        }

        public bool HasMethodBeenCalled(Action action, object[] parameters)
        {
            MethodCall methodCall;
            var key = action.Method.Name.Split('.').Last();

            return _methodCalls.TryGetValue(key, out methodCall) && methodCall.Parameters.All(parameters.Contains);
        }

        private class MethodCall
        {
            public IList<object> Parameters { get; }
            public int Calls { get; set; }

            public MethodCall(IList<object> parameters)
            {
                Parameters = parameters;
                Calls = 1;
            }
        }

        private readonly IDictionary<string, MethodCall> _methodCalls = new Dictionary<string, MethodCall>();
    }
}