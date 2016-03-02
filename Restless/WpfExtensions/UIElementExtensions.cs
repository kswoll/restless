using System.Windows;
using System.Windows.Media;

namespace Restless.WpfExtensions
{
    public static class UIElementExtensions
    {
        public static T FindAncestor<T>(this DependencyObject descendent)
            where T : DependencyObject
        {
            var current = descendent;
            while (current != null)
            {
                current = VisualTreeHelper.GetParent(current);
                if (current is T)
                    return (T)current;
            }
            return null;
        }

        public static T GetFirstChild<T>(this DependencyObject parent)
            where T : DependencyObject
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                    return (T)child;
            }
            return null;
        }
    }
}