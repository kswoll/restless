using System;
using System.Windows;

namespace Restless.WpfExtensions.CodeTriggers
{
    public class ConditionalCodeTriggerAction<T>
        where T : UIElement
    {
        private T element;
        private Func<T, bool> predicate;
        private CodeTriggerActor<T> actor;

        public ConditionalCodeTriggerAction(T element, Func<T, bool> predicate, CodeTriggerActor<T> actor)
        {
            this.element = element;
            this.predicate = predicate;
            this.actor = actor;
        }

        public bool Apply()
        {
            if (predicate(element))
            {
                foreach (var setter in actor)
                {
                    setter.Set();
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Unapply()
        {
            foreach (var setter in actor)
            {
                setter.Unset();
            }
        }
    }
}