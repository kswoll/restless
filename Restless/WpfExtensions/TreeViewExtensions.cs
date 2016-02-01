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
                item.SetValue(TreeViewItemFilter.IsFilteredProperty, true);
            else
                item.Visibility = Visibility.Collapsed;

            return descendentAccepted;
        }

        public static void ClearFilter(this TreeView treeView)
        {
            foreach (TreeViewItem item in treeView.Items)
            {
                item.Visibility = Visibility.Visible;
                item.SetValue(TreeViewItemFilter.IsFilteredProperty, false);
            }
        }

        public static RoutedEventHandler AddSelfExpanded(this TreeViewItem item, RoutedEventHandler handler)
        {
            RoutedEventHandler itemOnExpanded = (sender, args) =>
            {
                if (sender == args.Source)
                    handler(sender, args);
            };
            item.Expanded += itemOnExpanded;
            return itemOnExpanded;
        }

        public static RoutedEventHandler AddSelfCollapsed(this TreeViewItem item, RoutedEventHandler handler)
        {
            RoutedEventHandler itemOnCollapsed = (sender, args) =>
            {
                if (sender == args.Source)
                    handler(sender, args);
            };
            item.Collapsed += itemOnCollapsed;
            return itemOnCollapsed;
        }

        public static void PairExpanded(this TreeViewItem item, UIElement pairToObject, RoutedEvent pairToExpandEvent, RoutedEvent pairToCollapseEvent, DependencyProperty pairToProperty)
        {
            item.AddSelfExpanded((sender, args) => pairToObject.SetValue(pairToProperty, true));
            item.AddSelfCollapsed((sender, args) => pairToObject.SetValue(pairToProperty, false));
            pairToObject.SetValue(pairToProperty, item.IsExpanded);
            pairToObject.AddHandler(pairToExpandEvent, new RoutedEventHandler((sender, args) => item.IsExpanded = true));
            pairToObject.AddHandler(pairToCollapseEvent, new RoutedEventHandler((sender, args) => item.IsExpanded = false));
        }
    }
}