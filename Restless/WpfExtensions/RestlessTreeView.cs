using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Restless.Templates;

namespace Restless.WpfExtensions
{
    public class RestlessTreeView : TreeView
    {
        public static readonly DependencyProperty SelectedTreeViewItemProperty = DependencyProperty.Register(nameof(SelectedTreeViewItem), typeof(TreeViewItem), typeof(TreeView));

        public event Action<TreeViewItem> ItemCreated;
        public event Action<TreeViewItem> ItemSelected;

        public TreeViewItem SelectedTreeViewItem
        {
            get { return (TreeViewItem)GetValue(SelectedTreeViewItemProperty); }
            set { SetValue(SelectedTreeViewItemProperty, value); }
        }

        protected override void OnKeyUp(KeyEventArgs args)
        {
            base.OnKeyUp(args);

            if (args.Key == Key.Apps && SelectedTreeViewItem != null && ContextMenu != null)
            {
                args.Handled = true;
                ContextMenu.Placement = PlacementMode.Bottom;
                var template = SelectedTreeViewItem.GetFirstChild<TreeViewItemTemplate>();
                ContextMenu.PlacementTarget = template.Header;
                ContextMenu.IsOpen = true;
            }
        }

        protected virtual void OnItemCreated(TreeViewItem item)
        {
            ItemCreated?.Invoke(item);

            item.ContextMenu = ContextMenu;
        }

        protected override void OnSelectedItemChanged(RoutedPropertyChangedEventArgs<object> e)
        {
            base.OnSelectedItemChanged(e);
            if (SelectedItem == null)
                OnItemSelected(null);
        }

        protected virtual void OnItemSelected(TreeViewItem item)
        {
            ItemSelected?.Invoke(item);
        }

        internal void NotifySelected(TreeViewItem item)
        {
            SelectedTreeViewItem = item;
            OnItemSelected(item);
        }

        internal void NotifyItemCreated(TreeViewItem item)
        {
            OnItemCreated(item);
        }
    }
}