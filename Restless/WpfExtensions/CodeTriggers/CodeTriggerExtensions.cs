using System;
using System.Linq.Expressions;
using System.Windows;

namespace Restless.WpfExtensions.CodeTriggers
{
    public static class CodeTriggerExtensions
    {
        public static CodeTrigger<T> AddTrigger<T>(this T element)
            where T : UIElement
        {
            return new CodeTrigger<T>(element);
        }
    }
}