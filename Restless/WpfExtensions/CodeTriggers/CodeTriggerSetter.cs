using System;
using System.Linq.Expressions;
using Restless.Utils;

namespace Restless.WpfExtensions.CodeTriggers
{
    public class CodeTriggerSetter<TTarget, TValue> : ICodeTriggerSetter
    {
        private readonly TTarget target;
        private readonly Expression<Func<TTarget, TValue>> property;
        private readonly TValue originalValue;
        private readonly TValue value;

        public CodeTriggerSetter(TTarget target, Expression<Func<TTarget, TValue>> property, TValue value)
        {
            this.target = target;
            this.property = property;
            this.value = value;

            originalValue = property.GetValue(target);
        }

        public void Set()
        {
            property.SetValue(target, value);
        }

        public void Unset()
        {
            property.SetValue(target, originalValue);
        }
    }
}