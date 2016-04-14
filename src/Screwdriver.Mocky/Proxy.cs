using System;
using System.Collections.Generic;
using System.Linq;

namespace Screwdriver.Mocking
{
    public interface IProxy
    {
        void CallMethod(string methodName, object[] parameters);
        bool HasMethodBeenCalled(Action action, params object[] parameters);
    }

    public abstract class Proxy : IProxy
    {
        public void CallMethod(string methodName, object[] parameters)
        {
            if (!_methodCalls.ContainsKey(methodName))
                _methodCalls.Add(methodName, new MethodCall(parameters));

            _methodCalls[methodName].Calls++;
        }

        public bool HasMethodBeenCalled(Action action, params object[] parameters)
        {
            MethodCall methodCall;
            var key = action.Method.Name.Split('.').Last();

            return _methodCalls.TryGetValue(key, out methodCall) && methodCall.Parameters.All(parameters.Contains);
        }

        private readonly IDictionary<string, MethodCall> _methodCalls = new Dictionary<string, MethodCall>();

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
    }
}