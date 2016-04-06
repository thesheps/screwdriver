using System;
using System.Dynamic;
using Screwdriver.Builders.Exceptions;

namespace Screwdriver.Builders
{
    public class Builder<T>
    {
        public dynamic With { get; }

        public Builder()
        {
            With = new DynamicBuilder<T>();
        }

        public T Build()
        {
            return ((DynamicBuilder<T>)With).Result;
        }

        private class DynamicBuilder<T> : DynamicObject
        {
            public T Result { get; }

            public DynamicBuilder()
            {
                Result = (T)Activator.CreateInstance(typeof(T));
            }

            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                var propertyInfo = typeof(T).GetProperty(binder.Name);
                result = this;

                if (propertyInfo == null)
                    throw new UnknownPropertyException();

                if (propertyInfo.PropertyType != args[0].GetType())
                    throw new UnknownPropertyException();

                propertyInfo.SetValue(Result, args[0]);

                return true;
            }
        }
    }
}