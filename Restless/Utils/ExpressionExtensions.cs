using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;
using Expression = System.Linq.Expressions.Expression;

namespace Restless.Utils
{
    public static class ExpressionExtensions
    {
        public static DependencyProperty GetDependencyProperty<T, TValue>(this Expression<Func<T, TValue>> property)
        {
            var propertyInfo = property.GetPropertyInfo();
            var dependencyPropertyInfo = propertyInfo.DeclaringType.GetField(propertyInfo.Name + "Property");
            if (dependencyPropertyInfo == null)
                throw new Exception("DependencyProperty not found for " + property);
            var dependencyProperty = (DependencyProperty)dependencyPropertyInfo.GetValue(null);
            return dependencyProperty;
        }

        public static TValue GetValue<T, TValue>(this Expression<Func<T, TValue>> property, T obj)
        {
            var properties = property.GetPropertyPath();
            object current = obj;
            foreach (var currentProperty in properties)
            {
                current = currentProperty.GetValue(current, null);
            }
            return current == null ? default(TValue) : (TValue)current;
        }

        public static void SetValue<T, TValue>(this Expression<Func<T, TValue>> property, T obj, TValue value)
        {
            var properties = property.GetPropertyPath();
            object current = obj;
            foreach (var currentProperty in properties.Take(properties.Length - 1))
            {
                current = currentProperty.GetValue(current, null);
            }
            properties.Last().SetValue(current, value, null);
        }

        private static Expression Unwrap(Expression expression)
        {
            if (expression == null)
                return null;
            var unaryExpression = expression as UnaryExpression;
            if (unaryExpression != null && unaryExpression.NodeType == ExpressionType.Convert)
                return unaryExpression.Operand;
            return expression;
        }

        public static PropertyInfo GetPropertyInfo(this LambdaExpression expression)
        {
            return expression.GetPropertyPath().LastOrDefault();
        }

        public static PropertyInfo[] GetPropertyPath(this LambdaExpression expression)
        {
            var member = Unwrap(expression.Body) as MemberExpression;
            if (member == null)
            {
                return new PropertyInfo[0];
            }
            var firstProperty = member.Member as PropertyInfo;
            if (firstProperty == null)
            {
                throw new Exception(member.Member + " must be a property.");
            }
            var previousMember = Unwrap(member.Expression) as MemberExpression;
            if (previousMember == null)
            {
                return new[] { firstProperty };
            }

            var previousProperty = previousMember.Member as PropertyInfo;
            if (previousProperty == null)
            {
                throw new Exception(previousMember.Member + " must be a property.");
            }
            var secondPreviousMember = Unwrap(previousMember.Expression) as MemberExpression;
            if (secondPreviousMember == null)
            {
                return new[] { previousProperty, firstProperty };
            }

            var secondPreviousProperty = secondPreviousMember.Member as PropertyInfo;
            if (secondPreviousProperty == null)
            {
                throw new Exception(secondPreviousMember.Member + " must be a property.");                
            }
            var thirdPreviousMember = Unwrap(secondPreviousMember.Expression) as MemberExpression;
            if (thirdPreviousMember == null)
            {
                return new[] { secondPreviousProperty, previousProperty, firstProperty };
            }
            
            var stack = new Stack<PropertyInfo>();
            stack.Push(firstProperty);
            stack.Push(previousProperty);
            stack.Push(secondPreviousProperty);
            
            var current = thirdPreviousMember;
            while (current != null)
            {
                var property = current.Member as PropertyInfo;
                if (property == null)
                {
                    throw new Exception(current.Member + " must be a property.");                                    
                }
                stack.Push(property);
                current = Unwrap(current.Expression) as MemberExpression;
            }

            return stack.ToArray();
        }
    }
}