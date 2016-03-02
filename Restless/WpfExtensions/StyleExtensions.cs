using System;
using System.Linq.Expressions;
using System.Windows;
using Restless.Styles;

namespace Restless.WpfExtensions
{
    public static class StyleExtensions
    {
        public static void AddSetter<T, TValue>(this TypedStyle<T> style, Expression<Func<T, TValue>> property)
        {
            
        }
    }
}