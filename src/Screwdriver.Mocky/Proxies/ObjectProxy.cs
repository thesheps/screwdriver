using System;
using System.Collections.Generic;
using System.Linq;

namespace Screwdriver.Mocking.Proxies
{
    public interface IObjectProxy
    {
        void CallVoidMethod(string methodName, object[] parameters);
        object CallReturningMethod(string methodName, object[] parameters);
        void SetupMethod(Action action, object[] parameters, Action methodBody);
        void SetupReturn(Func<object> function, object[] parameters, object returnedValue);
        int GetMethodCallCount(Action action, object[] parameters);
    }

    public abstract class ObjectProxy : IObjectProxy
    {
        public void CallVoidMethod(string methodName, object[] parameters)
        {
            CallReturningMethod(methodName, parameters);
        }

        public object CallReturningMethod(string methodName, object[] parameters)
        {
            MethodCall methodCall;
            var key = CreateMethodCallKey(methodName, parameters);
            var found = _methodCalls.TryGetValue(key, out methodCall);

            if (!found)
            {
                methodCall = new MethodCall(parameters);
                _methodCalls.Add(key, methodCall);
            }

            if (methodCall.Parameters.All(parameters.Contains))
                return methodCall.Execute();

            return null;
        }

        public void SetupMethod(Action action, object[] parameters, Action methodBody)
        {
            MethodCall methodCall;
            var key = CreateMethodCallKey(action, parameters);
            var found = _methodCalls.TryGetValue(key, out methodCall) && methodCall.Parameters.All(parameters.Contains);

            if (!found)
            {
                methodCall = new MethodCall(parameters);
                methodCall.SetMethodBody(methodBody);
                _methodCalls.Add(key, methodCall);
            }
            else
                methodCall.SetMethodBody(methodBody);
        }

        public void SetupReturn(Func<object> function, object[] parameters, object returnedValue)
        {
            MethodCall methodCall;
            var key = CreateMethodCallKey(function, parameters);
            var found = _methodCalls.TryGetValue(key, out methodCall) && methodCall.Parameters.All(parameters.Contains);

            if (!found)
            {
                methodCall = new MethodCall(parameters);
                methodCall.SetReturnedValue(returnedValue);
                _methodCalls.Add(key, methodCall);
            }
            else
                methodCall.SetReturnedValue(returnedValue);
        }

        public int GetMethodCallCount(Action action, object[] parameters)
        {
            MethodCall methodCall;
            var key = CreateMethodCallKey(action, parameters);
            var found = _methodCalls.TryGetValue(key, out methodCall) && methodCall.Parameters.All(parameters.Contains);

            return found ? methodCall.Calls : 0;
        }

        private static string CreateMethodCallKey(Func<object> function, IEnumerable<object> parameters)
        {
            return CreateMethodCallKey(function.Method.Name.Split('.').Last(), parameters);
        }

        private static string CreateMethodCallKey(Action action, IEnumerable<object> parameters)
        {
            return CreateMethodCallKey(action.Method.Name.Split('.').Last(), parameters);
        }

        private static string CreateMethodCallKey(string methodName, IEnumerable<object> parameters)
        {
            return $"{methodName}_{string.Join("_", parameters.Select(p => p.GetType().Name))}";
        }

        private class MethodCall
        {
            public IList<object> Parameters { get; }
            public int Calls { get; private set; }

            public MethodCall(IList<object> parameters)
            {
                Parameters = parameters;
            }

            public void SetMethodBody(Action methodBody)
            {
                _methodBody = methodBody;
            }

            public void SetReturnedValue(object returnedValue)
            {
                _returnedValue = returnedValue;
            }

            public object Execute()
            {
                Calls++;
                _methodBody?.Invoke();
                return _returnedValue;
            }

            private Action _methodBody;
            private object _returnedValue;
        }

        private readonly IDictionary<string, MethodCall> _methodCalls = new Dictionary<string, MethodCall>();
    }
}