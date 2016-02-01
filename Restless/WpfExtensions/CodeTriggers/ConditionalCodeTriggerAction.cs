using System;
using System.Collections.Generic;
using System.Windows;

namespace Restless.WpfExtensions.CodeTriggers
{
    public class ConditionalCodeTriggerAction<T>
        where T : UIElement
    {
        private readonly T element;
        private readonly Func<T, bool> predicate;
        private readonly CodeTriggerActor<T> actor;

        public ConditionalCodeTriggerAction(T element, Func<T, bool> predicate, CodeTriggerActor<T> actor)
        {
            this.element = element;
            this.predicate = predicate;
            this.actor = actor;
        }

        public bool Apply(HashSet<object> appliedSetters)
        {
            if (predicate(element))
            {
                foreach (var setter in actor)
                {
                    setter.Set();
                    appliedSetters?.Add(setter.Key);
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Unapply(HashSet<object> appliedSetters)
        {
            foreach (var setter in actor)
            {
                if (!appliedSetters?.Contains(setter.Key) ?? true)
                    setter.Unset();
            }
        }
    }
}