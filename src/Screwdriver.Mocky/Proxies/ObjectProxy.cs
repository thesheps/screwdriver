﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Screwdriver.Mocking.Proxies
{
    public interface IObjectProxy
    {
        void CallMethod(string methodName, object[] parameters);
        void SetupMethod(Action action, object[] parameters, Action methodBody);
        int GetMethodCallCount(Action action, object[] parameters);
    }

    public abstract class ObjectProxy : IObjectProxy
    {
        public void CallMethod(string methodName, object[] parameters)
        {
            if (!_methodCalls.ContainsKey(methodName))
                _methodCalls.Add(methodName, new MethodCall(parameters));

            _methodCalls[methodName].Execute();
        }

        public void SetupMethod(Action action, object[] parameters, Action methodBody)
        {
            var key = action.Method.Name.Split('.').Last();
            if (!_methodCalls.ContainsKey(key))
                _methodCalls.Add(key, new MethodCall(parameters, methodBody));
        }

        public int GetMethodCallCount(Action action, object[] parameters)
        {
            MethodCall methodCall;
            var key = action.Method.Name.Split('.').Last();
            var found = _methodCalls.TryGetValue(key, out methodCall) && methodCall.Parameters.All(parameters.Contains);

            return found ? methodCall.Calls : 0;
        }

        private class MethodCall
        {
            public IList<object> Parameters { get; }
            public int Calls { get; private set; }

            public MethodCall(IList<object> parameters)
            {
                Parameters = parameters;
            }

            public MethodCall(IList<object> parameters, Action methodBody)
            {
                Parameters = parameters;
                _methodBody = methodBody;
            }

            public void Execute()
            {
                Calls++;
                _methodBody?.Invoke();
            }

            private readonly Action _methodBody;
        }

        private readonly IDictionary<string, MethodCall> _methodCalls = new Dictionary<string, MethodCall>();
    }
}