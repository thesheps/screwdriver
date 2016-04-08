using System;

namespace Screwdriver.Builders
{
    public static class Extensions
    {
        public static T With<T>(this T obj, Action<T> function)
        {
            function.Invoke(obj);
            return obj;
        }
    }
}