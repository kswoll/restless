using System.Collections;
using System.Windows;

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
    }
}