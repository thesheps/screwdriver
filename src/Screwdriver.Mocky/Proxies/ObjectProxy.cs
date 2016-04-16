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
        void SetupReturnedValue(Func<object> function, object[] parameters, object returnedValue);
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

            if (found)
                return methodCall.Parameters.All(parameters.Contains) ? methodCall.Execute() : null;

            methodCall = new MethodCall(parameters);
            _methodCalls.Add(key, methodCall);

            return methodCall.Parameters.All(parameters.Contains) ? methodCall.Execute() : null;
        }

        public void SetupMethod(Action action, object[] parameters, Action methodBody)
        {
            var key = CreateMethodCallKey(action, parameters);
            SetupMethod(key, parameters, call => call.SetMethodBody(methodBody));
        }

        public void SetupReturnedValue(Func<object> function, object[] parameters, object returnedValue)
        {
            var key = CreateMethodCallKey(function, parameters);
            SetupMethod(key, parameters, call => call.SetReturnedValue(returnedValue));
        }

        private void SetupMethod(string key, IList<object> parameters, Action<MethodCall> setupAction)
        {
            MethodCall methodCall;
            var found = _methodCalls.TryGetValue(key, out methodCall) && methodCall.Parameters.All(parameters.Contains);

            if (!found)
            {
                methodCall = new MethodCall(parameters);
                setupAction(methodCall);
                _methodCalls.Add(key, methodCall);
            }
            else
                setupAction(methodCall);
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