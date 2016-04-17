using System;

namespace Sangria.Exceptions
{
    public class DuplicateBindingException : Exception
    {
        public DuplicateBindingException(string resource)
            :base(string.Format(Resources.Errors.DuplicateBinding, resource))
        {
        }
    }
}