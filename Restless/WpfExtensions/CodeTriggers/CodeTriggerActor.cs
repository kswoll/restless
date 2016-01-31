using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Windows;

namespace Restless.WpfExtensions.CodeTriggers
{
    public class CodeTriggerActor<T> : IEnumerable<ICodeTriggerSetter>
        where T : UIElement
    {
        private readonly List<ICodeTriggerSetter> setters = new List<ICodeTriggerSetter>();

        public void Set<TTarget, TValue>(TTarget target, Expression<Func<TTarget, TValue>> property, TValue value)
        {
            setters.Add(new CodeTriggerSetter<TTarget, TValue>(target, property, value));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<ICodeTriggerSetter> GetEnumerator()
        {
            return setters.GetEnumerator();
        }
    }
}