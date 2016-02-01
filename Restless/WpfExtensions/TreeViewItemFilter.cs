using System.Windows;
using System.Windows.Controls;

namespace Restless.WpfExtensions
{
    public class TreeViewItemFilter : DependencyObject
    {
        public static readonly DependencyProperty IsFilteredProperty = DependencyProperty.RegisterAttached("IsFiltered", typeof(bool), typeof(TreeViewItemFilter), new UIPropertyMetadata(false));

        public static bool GetIsFiltered(TreeViewItem item)
        {
            return (bool)item.GetValue(IsFilteredProperty);
        }

        public static void SetIsFiltered(TreeViewItem item, bool value)
        {
            item.SetValue(IsFilteredProperty, value);
        }
    }
}