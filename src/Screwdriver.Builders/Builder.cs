using System;
using System.Dynamic;
using Screwdriver.Builders.Exceptions;

namespace Screwdriver.Builders
{
    public interface IBuilder<out T>
    {
        T Build();
    }

    public class Builder
    {
        public static T Create<T>(IBuilder<T> builder)
        {
            return builder.Build();
        }

        public static Builder<T> Create<T>()
        {
            return new Builder<T>();
        }
    }

    public class Builder<T>
    {
        public dynamic With { get; }

        internal Builder()
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