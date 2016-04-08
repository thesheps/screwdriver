using System;
using Screwdriver.Mocking.Resources;

namespace Screwdriver.Mocking.Exceptions
{
    public class ProxyNotImplementedException : Exception
    {
        public ProxyNotImplementedException(Type type)
            :base(string.Format(Errors.ProxyNotImplemented, type.FullName))
        {
        }
    }
}