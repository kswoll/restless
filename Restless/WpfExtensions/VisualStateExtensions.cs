using System;
using System.Collections;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Restless.WpfExtensions
{
    public static class VisualStateExtensions
    {
        public static IList GetVisualStateGroups(this FrameworkElement element)
        {
            return VisualStateManager.GetVisualStateGroups(element);
        }

        public static VisualState CreateState(this VisualStateGroup group, string name)
        {
            var state = new VisualState { Name = name };
            group.States.Add(state);
            return state;
        }

        private static Storyboard GetStoryboard(this VisualState state)
        {
            var storyboard = state.Storyboard;
            if (storyboard == null)
                state.Storyboard = storyboard = new Storyboard();
            return storyboard;
        }

        public static void AddDoubleAnimation<T, TValue>(this VisualState state, T target, Expression<Func<T, TValue>> targetProperty,
            double toValue, Duration? duration = null)
            where T : DependencyObject
        {
            state.GetStoryboard().AddAnimation(new DoubleAnimation(toValue, duration ?? new Duration(TimeSpan.FromSeconds(0))), target, targetProperty);
        }

        public static void AddDoubleAnimation<T, TValue>(this VisualState state, T target, Expression<Func<T, TValue>> targetProperty,
            double fromValue, double toValue, Duration? duration = null)
            where T : DependencyObject
        {
            state.GetStoryboard().AddAnimation(new DoubleAnimation(fromValue, toValue, duration ?? new Duration(TimeSpan.FromSeconds(0))), target, targetProperty);
        }

        public static void AddColorAnimation<T, TValue>(this VisualState state, T target, Expression<Func<T, TValue>> targetProperty,
            Color toValue, Duration? duration = null)
            where T : DependencyObject
        {
            state.GetStoryboard().AddAnimation(new ColorAnimation(toValue, duration ?? new Duration(TimeSpan.FromSeconds(0))), target, targetProperty);
        }

        public static void AddColorAnimation<T, TValue>(this VisualState state, T target, Expression<Func<T, TValue>> targetProperty,
            Color fromValue, Color toValue, Duration? duration = null
        )
            where T : DependencyObject
        {
            state.GetStoryboard().AddAnimation(new ColorAnimation(fromValue, toValue, duration ?? new Duration(TimeSpan.FromSeconds(0))), target, targetProperty);
        }

        public static void AddObjectAnimationUsingKeyFrames<T, TValue>(this VisualState state, T target, Expression<Func<T, TValue>> targetProperty,
            Duration? duration, params ObjectKeyFrame[] keyFrames
        )
            where T : DependencyObject
        {
            state.GetStoryboard().AddObjectAnimationUsingKeyFrames(target, targetProperty, duration, keyFrames);
        }

        public static void AddObjectAnimationUsingKeyFrames<T, TValue>(this VisualState state, T target, Expression<Func<T, TValue>> targetProperty,
            params ObjectKeyFrame[] keyFrames
        )
            where T : DependencyObject
        {
            state.GetStoryboard().AddObjectAnimationUsingKeyFrames(target, targetProperty, null, keyFrames);
        }

        public static void AddObjectAnimationUsingKeyFrames<T, TValue>(this VisualState state, T target, Expression<Func<T, TValue>> targetProperty,
            TValue keyFrame
        )
            where T : DependencyObject
        {
            state.GetStoryboard().AddObjectAnimationUsingKeyFrames(target, targetProperty, (Duration?)null, new DiscreteObjectKeyFrame(keyFrame, KeyTime.FromPercent(0)));
        }

        public static void AddAnimation<T, TValue>(this VisualState state, Timeline animation, T target, Expression<Func<T, TValue>> targetProperty)
            where T : DependencyObject
        {
            state.GetStoryboard().AddAnimation(animation, target, targetProperty);
        }
    }
}