using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Screwdriver.Mocking
{
    public interface IProxy
    {
        void CallMethod([CallerMemberName] string methodName = null);
        bool HasMethodBeenCalled(Action action);
    }

    public abstract class Proxy : IProxy
    {
        public void CallMethod([CallerMemberName] string methodName = null)
        {
            if (!_methodCalls.ContainsKey(methodName))
                _methodCalls.Add(methodName, 0);

            _methodCalls[methodName]++;
        }

        public bool HasMethodBeenCalled(Action action)
        {
            return _methodCalls.ContainsKey(action.Method.Name.Split('.').Last());
        }

        private readonly IDictionary<string, int> _methodCalls = new Dictionary<string, int>();
    }
}