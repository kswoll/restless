﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Controls.Primitives;
using Restless.Utils;

namespace Restless.WpfExtensions.CodeTriggers
{
    public class CodeTrigger<T>
        where T : UIElement
    {
        public T Element { get; }

        private readonly List<ConditionalCodeTriggerAction<T>> conditionalActions = new List<ConditionalCodeTriggerAction<T>>();
        private ConditionalCodeTriggerAction<T> lastAppliedAction; 

        public CodeTrigger(T element)
        {
            Element = element;
        }

        public void AddProperty<TValue>(Expression<Func<T, TValue>> property)
        {
            var dependencyProperty = property.GetDependencyProperty();
            DependencyPropertyDescriptor
                .FromProperty(dependencyProperty, dependencyProperty.OwnerType)
                .AddValueChanged(Element, (sender, args) => OnTrigger(dependencyProperty));
        }

        public void AddConditionalAction(Func<T, bool> predicate, Action<CodeTriggerActor<T>> setters)
        {
            var actor = new CodeTriggerActor<T>();
            setters(actor);
            conditionalActions.Add(new ConditionalCodeTriggerAction<T>(Element, predicate, actor));
        }

        protected void OnTrigger(DependencyProperty property)
        {
            foreach (var action in conditionalActions)
            {
                if (action.Apply())
                {
                    lastAppliedAction = action;
                    return;
                }
            }
            lastAppliedAction.Unapply();
        }
    }
}