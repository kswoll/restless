using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Expression = System.Linq.Expressions.Expression;

namespace Restless.WpfExtensions
{
    public static class StoryboardExtensions
    {
        public static void AddDoubleAnimation<T, TValue>(this Storyboard storyboard, T target, Expression<Func<T, TValue>> targetProperty,
            double toValue, Duration? duration = null)
            where T : DependencyObject
        {
            storyboard.AddAnimation(new DoubleAnimation(toValue, duration ?? new Duration(TimeSpan.FromSeconds(0))), target, targetProperty);
        }

        public static void AddDoubleAnimation<T, TValue>(this Storyboard storyboard, T target, Expression<Func<T, TValue>> targetProperty,
            double fromValue, double toValue, Duration? duration = null)
            where T : DependencyObject
        {
            storyboard.AddAnimation(new DoubleAnimation(fromValue, toValue, duration ?? new Duration(TimeSpan.FromSeconds(0))), target, targetProperty);
        }

        public static void AddColorAnimation<T, TValue>(this Storyboard storyboard, T target, Expression<Func<T, TValue>> targetProperty,
            Color toValue, Duration? duration = null)
            where T : DependencyObject
        {
            storyboard.AddAnimation(new ColorAnimation(toValue, duration ?? new Duration(TimeSpan.FromSeconds(0))), target, targetProperty);
        }

        public static void AddColorAnimation<T, TValue>(this Storyboard storyboard, T target, Expression<Func<T, TValue>> targetProperty,
            Color fromValue, Color toValue, Duration? duration = null
        )
            where T : DependencyObject
        {
            storyboard.AddAnimation(new ColorAnimation(fromValue, toValue, duration ?? new Duration(TimeSpan.FromSeconds(0))), target, targetProperty);
        }

        public static void AddObjectAnimationUsingKeyFrames<T, TValue>(this Storyboard storyboard, T target, Expression<Func<T, TValue>> targetProperty,
            Duration? duration, params ObjectKeyFrame[] keyFrames
        )
            where T : DependencyObject
        {
            var animation = new ObjectAnimationUsingKeyFrames();
            animation.Duration = duration ?? new Duration(TimeSpan.FromSeconds(0));
            foreach (var keyFrame in keyFrames)
            {
                animation.KeyFrames.Add(keyFrame);
            }
            storyboard.AddAnimation(animation, target, targetProperty);
        }

        public static void AddObjectAnimationUsingKeyFrames<T, TValue>(this Storyboard storyboard, T target, Expression<Func<T, TValue>> targetProperty,
            params ObjectKeyFrame[] keyFrames
        )
            where T : DependencyObject
        {
            storyboard.AddObjectAnimationUsingKeyFrames(target, targetProperty, null, keyFrames);
        }

        public static void AddObjectAnimationUsingKeyFrames<T, TValue>(this Storyboard storyboard, T target, Expression<Func<T, TValue>> targetProperty,
            TValue keyFrame
        )
            where T : DependencyObject
        {
            storyboard.AddObjectAnimationUsingKeyFrames(target, targetProperty, (Duration?)null, new DiscreteObjectKeyFrame(keyFrame, KeyTime.FromPercent(0)));
        }

        public static void AddAnimation<T, TValue>(this Storyboard storyboard, Timeline animation, T target, Expression<Func<T, TValue>> targetProperty)
            where T : DependencyObject
        {
            Storyboard.SetTarget(animation, target);

            var propertyPath = PropertyPathGenerator.CreatePropertyPath(targetProperty);
            Storyboard.SetTargetProperty(animation, propertyPath);

            storyboard.Children.Add(animation);
        }

        private class PropertyPathGenerator : ExpressionVisitor
        {
            private readonly List<IToken> tokens = new List<IToken>();

            public static PropertyPath CreatePropertyPath<T, TValue>(Expression<Func<T, TValue>> expression)
                where T : DependencyObject
            {
                var generator = new PropertyPathGenerator();
                generator.Visit(expression);

                var builder = new StringBuilder();
                foreach (var token in ((IEnumerable<IToken>)generator.tokens).Reverse())
                {
                    if (token is MemberToken && builder.Length > 0)
                    {
                        builder.Append('.');
                    }
                    builder.Append(token.ToString());
                }
                return new PropertyPath(builder.ToString());
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                var token = node.Member.Name;
                tokens.Add(new MemberToken(token));

                return base.VisitMember(node);
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Method.Name == "get_Item")
                {
                    var index = node.Arguments.Cast<ConstantExpression>().Single();
                    var indexValue = (int)index.Value;
                    tokens.Add(new IndexToken(indexValue));
                }
                else
                {
                    throw new InvalidOperationException("Cannot invoke methods in a property expression");
                }
                return base.VisitMethodCall(node);
            }

            protected override Expression VisitIndex(IndexExpression node)
            {
                var index = node.Arguments.Cast<ConstantExpression>().Single();
                var indexValue = (int)index.Value;
                tokens.Add(new IndexToken(indexValue));

                return base.VisitIndex(node);
            }

            private interface IToken
            {
                string ToString();
            }

            private class MemberToken : IToken
            {
                public string Name { get; }

                public MemberToken(string name)
                {
                    Name = name;
                }

                public override string ToString()
                {
                    return Name;
                }
            }

            private class IndexToken : IToken
            {
                public int Index { get; }

                public IndexToken(int index)
                {
                    Index = index;
                }

                public override string ToString()
                {
                    return $"[{Index}]";
                }
            }
        }
    }
}