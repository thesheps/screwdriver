using System;

namespace Sidecar.Exceptions
{
    public class UnknownPropertyException : Exception
    {
        public UnknownPropertyException(string propertyName)
            : base(string.Format(Resources.Errors.UnknownProperty, propertyName))
        {
        }
    }
}