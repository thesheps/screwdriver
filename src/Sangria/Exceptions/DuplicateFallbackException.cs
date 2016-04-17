using System;

namespace Sangria.Exceptions
{
    public class DuplicateFallbackException : Exception
    {
        public DuplicateFallbackException(string resource)
            :base(string.Format(Resources.Errors.DuplicateFallback, resource))
        {
        }
    }
}