using System;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Data;
using Restless.Styles;
using Restless.Utils;
using Restless.ViewModels;
using SexyReact;
using SexyReact.Views;

namespace Restless.WpfExtensions
{
    public static class StyleExtensions
    {
        public static void AddSetter<TView, TViewValue, TModel, TModelValue>(this TypedStyle<TView> style, Expression<Func<TView, TViewValue>> property, RxViewObjectBinder<TModel, TModelValue> binder) 
            where TModel : IRxObject
        {
            var dependencyProperty = property.GetDependencyProperty();
            var binding = binder.CreateBinding();
            binding.Mode = BindingMode.TwoWay;
            style.Setters.Add(new Setter(dependencyProperty, binding));
        }
    }
}