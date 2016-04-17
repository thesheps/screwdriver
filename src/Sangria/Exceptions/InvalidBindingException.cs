using System;

namespace Sangria.Exceptions
{
    public class InvalidBindingException : Exception
    {
        public InvalidBindingException(string message)
            :base(message)
        {
        }
    }
}