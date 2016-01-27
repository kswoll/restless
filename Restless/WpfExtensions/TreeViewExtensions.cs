using System;
using System.Windows;
using System.Windows.Controls;

namespace Restless.WpfExtensions
{
    public static class TreeViewExtensions
    {
        public static void ExpandAll(this TreeView treeView)
        {
            foreach (TreeViewItem item in treeView.Items)
            {
                item.ExpandSubtree();
            }
        }

        public static void CollapseAll(this TreeView treeView)
        {
            foreach (TreeViewItem item in treeView.Items)
            {
                item.CollapseSubtree();
            }
        }

        public static void CollapseSubtree(this TreeViewItem item)
        {
            item.IsExpanded = false;
            foreach (TreeViewItem child in item.Items)
            {
                child.CollapseSubtree();
            }
        }

        public static void Filter(this TreeView treeView, Func<TreeViewItem, bool> predicate)
        {
            foreach (TreeViewItem item in treeView.Items)
            {
                item.Filter(predicate);
            }
        }

        public static bool Filter(this TreeViewItem item, Func<TreeViewItem, bool> predicate)
        {
            var itemAccepted = predicate(item);
            var descendentAccepted = false;
            foreach (TreeViewItem child in item.Items)
            {
                descendentAccepted = descendentAccepted | child.Filter(predicate);
            }
            if (itemAccepted)
                return true;
            else if (descendentAccepted)
                VisualStateManager.GoToElementState(item, "Disabled", true);
            else
                item.Visibility = Visibility.Collapsed;

            return descendentAccepted;
        }

        public static void ClearFilter(this TreeView treeView)
        {
            foreach (TreeViewItem item in treeView.Items)
            {
                item.Visibility = Visibility.Visible;
                VisualStateManager.GoToElementState(item, "Normal", true);
            }
        }
    }
}