using System;
using Screwdriver.Exceptions;

namespace Screwdriver.Assertions
{
    public abstract class When
    {
        protected void Then(Func<bool> func)
        {
            if (!func()) throw new FalseException();
            throw new TrueException();
        }

        protected void Throws<TException>(Action action) where TException : Exception
        {
            try
            {
                action.Invoke();
            }
            catch (TException)
            {
                throw new TrueException();
            }
            catch (Exception)
            {
                throw new FalseException();
            }

            throw new FalseException();
        }

        protected virtual void SetUp()
        {
        }

        protected virtual void TearDown()
        {
        }
    }
}