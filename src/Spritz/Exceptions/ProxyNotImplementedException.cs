using System;
using Spritz.Resources;

namespace Spritz.Exceptions
{
    public class ProxyNotImplementedException : Exception
    {
        public ProxyNotImplementedException(Type type)
            :base(string.Format(Errors.ProxyNotImplemented, type.FullName))
        {
        }
    }
}