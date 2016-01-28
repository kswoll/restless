using System;
using System.Linq;
using System.Linq.Expressions;

namespace Restless.Utils
{
    public static class ExpressionExtensions
    {
        public static TValue GetValue<T, TValue>(this Expression<Func<T, TValue>> property, T obj)
        {
            var properties = property.GetPropertyAccessList();
            object current = obj;
            foreach (var currentProperty in properties)
            {
                current = currentProperty.GetValue(current, null);
            }
            return current == null ? default(TValue) : (TValue)current;
        }

        public static void SetValue<T, TValue>(this Expression<Func<T, TValue>> property, T obj, TValue value)
        {
            var properties = property.GetPropertyAccessList();
            object current = obj;
            foreach (var currentProperty in properties.Take(properties.Count - 1))
            {
                current = currentProperty.GetValue(current, null);
            }
            properties.Last().SetValue(current, value, null);
        }
    }
}