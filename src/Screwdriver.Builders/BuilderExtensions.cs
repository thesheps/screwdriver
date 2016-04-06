using System;

namespace Screwdriver.Builders
{
    public static class BuilderExtensions
    {
        public static T With<T>(this T obj, Action<T> function)
        {
            function.Invoke(obj);
            return obj;
        }
    }
}