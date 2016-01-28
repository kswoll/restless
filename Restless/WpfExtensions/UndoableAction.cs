using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Windows;

namespace Restless.WpfExtensions
{
    public class UndoableAction
    {
        private readonly List<UndoableAssignment> assignments = new List<UndoableAssignment>();

        public void Set<T, TValue>(T element, Expression<Func<T, TValue>> property, TValue newValue)
            where T : UIElement
        {
            assignments.Add(new UndoableAssignment<T, TValue>(element, property, newValue));
        }

        public void Do()
        {
            foreach (var assignment in assignments) 
                assignment.Do();
        }

        public void Undo()
        {
            foreach (var assignment in assignments)
                assignment.Undo();
        } 
    }
}