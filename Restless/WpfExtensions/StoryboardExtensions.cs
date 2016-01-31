using System;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

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
    }
}