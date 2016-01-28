using System;
using System.Linq.Expressions;
using System.Windows;
using Restless.Utils;

namespace Restless.WpfExtensions
{
    public abstract class UndoableAssignment
    {
        public abstract void Do();
        public abstract void Undo();
    }

    public class UndoableAssignment<T, TValue> : UndoableAssignment
        where T : UIElement
    {
        private readonly T element;
        private readonly Expression<Func<T, TValue>> property;
        private readonly TValue originalValue;
        private readonly TValue newValue;

        public UndoableAssignment(T element, Expression<Func<T, TValue>> property, TValue newValue)
        {
            this.element = element;
            this.property = property;
            this.newValue = newValue;

            originalValue = property.GetValue(element);
        }

        public override void Do()
        {
            property.SetValue(element, newValue);
        }

        public override void Undo()
        {
            property.SetValue(element, originalValue);
        }
    }
}