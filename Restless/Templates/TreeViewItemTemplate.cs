using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using Restless.WpfExtensions;
using Restless.WpfExtensions.CodeTriggers;

namespace Restless.Templates
{
    public class TreeViewItemTemplate : Grid, IAddChild
    {
        public ItemsPresenter ItemsHost { get; private set; }
        public ContentPresenter Header { get; private set; }

        void IAddChild.AddChild(object value)
        {
            if (value is ItemsPresenter)
                ItemsHost = (ItemsPresenter)value;
            else if (value is ContentPresenter)
                Header = (ContentPresenter)value;
        }

        public override void EndInit()
        {
            var item = (TreeViewItem)TemplatedParent;
            var treeView = (IRestlessTreeView)item.FindAncestor<TreeView>();
            treeView.NotifyItemCreated(item);

            this.AddRow(GridLength.Auto);
            RowDefinitions.Add(new RowDefinition());
            ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto, MinWidth = 19 });
            this.AddColumn(GridLength.Auto);
            this.AddColumn(new GridLength(1, GridUnitType.Star));

            var expanderButtonTemplate = new ControlTemplate(typeof(ToggleButton));
            expanderButtonTemplate.VisualTree = new FrameworkElementFactory(typeof(ExpanderButtonTemplate));

            var expander = new ToggleButton
            {
                Name = "Expander",
                IsChecked = false,
                ClickMode = ClickMode.Press,
                Template = expanderButtonTemplate
            };
            Children.Add(expander);

            Header.Content = item.Header;
            Header.ContentTemplate = item.HeaderTemplate;
            Header.ContentStringFormat = item.HeaderStringFormat;
            Header.HorizontalAlignment = item.HorizontalContentAlignment;
            Header.SnapsToDevicePixels = item.SnapsToDevicePixels;
            var border = new Border
            {
                BorderThickness = item.BorderThickness,
                Padding = new Thickness(2, 1, 2, 1),
                BorderBrush = item.BorderBrush,
                SnapsToDevicePixels = true
            };
            SetColumn(border, 1);
            border.Child = Header;
            Children.Add(border);

            SetColumnSpan(ItemsHost, 2);
            this.Add(ItemsHost, 1, 1);

            var colorTrigger = item.AddTrigger();
            colorTrigger.AddProperty(x => x.IsSelected);
            colorTrigger.AddProperty(x => x.IsSelectionActive);
            colorTrigger.AddProperty(x => x.IsEnabled);
            colorTrigger.AddProperty(TreeViewItemFilter.IsFilteredProperty);
            colorTrigger.AddConditionalAction(x => !x.IsEnabled, setters =>
            {
                setters.Set(item, x => x.Foreground, SystemColors.GrayTextBrush);
            });
            colorTrigger.AddConditionalAction(x => TreeViewItemFilter.GetIsFiltered(x) && x.IsSelected, setters =>
            {
                setters.Set(item, x => x.Foreground, SystemColors.GrayTextBrush);
                setters.Set(border, x => x.Background, SystemColors.InactiveSelectionHighlightBrush);
            });
            colorTrigger.AddConditionalAction(x => TreeViewItemFilter.GetIsFiltered(x), setters =>
            {
                setters.Set(item, x => x.Foreground, SystemColors.GrayTextBrush);
            });
            colorTrigger.AddConditionalAction(x => x.IsSelected && !x.IsSelectionActive, setters =>
            {
                setters.Set(item, x => x.Foreground, SystemColors.InactiveSelectionHighlightTextBrush);
                setters.Set(border, x => x.Background, SystemColors.InactiveSelectionHighlightBrush);
            });
            colorTrigger.AddConditionalAction(x => x.IsSelected, setters =>
            {
                setters.Set(item, x => x.Foreground, SystemColors.HighlightTextBrush);
                setters.Set(border, x => x.Background, SystemColors.HighlightBrush);
            });

            var expanderTrigger = item.AddTrigger();
            expanderTrigger.AddProperty(x => x.HasItems);
            expanderTrigger.AddConditionalAction(x => !x.HasItems, setters => setters.Set(expander, x => x.Visibility, Visibility.Hidden));

            var noItemsTrigger = item.AddTrigger();
            noItemsTrigger.AddProperty(x => x.IsExpanded);
            noItemsTrigger.AddConditionalAction(x => !x.IsExpanded, setters => setters.Set(ItemsHost, x => x.Visibility, Visibility.Collapsed));
            noItemsTrigger.AddConditionalAction(x => x.IsExpanded, setters => setters.Set(ItemsHost, x => x.Visibility, Visibility.Visible));

            item.PairExpanded(expander, ToggleButton.CheckedEvent, ToggleButton.UncheckedEvent, ToggleButton.IsCheckedProperty);

            item.Selected += (sender, args) =>
            {
                if (item.IsSelected)
                    ((IRestlessTreeView)item.FindAncestor<TreeView>()).NotifySelected(item);
            };

            base.EndInit();
        }

        private class ExpanderButtonTemplate : Border
        {
            public override void EndInit()
            {
                base.EndInit();

                var button = (ToggleButton)TemplatedParent;

                button.Focusable = false;
                button.Width = 16;
                button.Height = 16;

                var expandPath = new Path
                {
                    Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF)),
                    Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x81, 0x81, 0x81)),
                    Data = Geometry.Parse("M0,0L0,6L6,0z"),
                    RenderTransform = new RotateTransform(135, 3, 3)
                };
                Padding = new Thickness(5);
                Background = new SolidColorBrush(Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF));
                Width = 16;
                Height = 16;
                Child = expandPath;

                var strokeAndFillTrigger = button.AddTrigger();
                strokeAndFillTrigger.AddProperty(x => x.IsChecked);
                strokeAndFillTrigger.AddProperty(x => x.IsMouseOver);
                strokeAndFillTrigger.AddConditionalAction(x => x.IsMouseOver && (x.IsChecked ?? false), setters =>
                {
                    setters.Set(expandPath, x => ((SolidColorBrush)x.Stroke).Color, Color.FromArgb(0xFF, 0x1C, 0xC4, 0xF7));
                    setters.Set(expandPath, x => ((SolidColorBrush)x.Fill).Color, Color.FromArgb(0xFF, 0x82, 0xDF, 0xFB));
                });
                strokeAndFillTrigger.AddConditionalAction(x => x.IsChecked ?? false, setters =>
                {
                    setters.Set(expandPath, x => ((SolidColorBrush)x.Fill).Color, Color.FromArgb(0xFF, 0x59, 0x59, 0x59));
                    setters.Set(expandPath, x => ((SolidColorBrush)x.Stroke).Color, Color.FromArgb(0xFF, 0x26, 0x26, 0x26));
                });
                strokeAndFillTrigger.AddConditionalAction(x => x.IsMouseOver, setters =>
                {
                    setters.Set(expandPath, x => ((SolidColorBrush)x.Fill).Color, Color.FromArgb(0xFF, 0x27, 0xC7, 0xF7));
                    setters.Set(expandPath, x => ((SolidColorBrush)x.Stroke).Color, Color.FromArgb(0xFF, 0xCC, 0xEE, 0xFB));
                });

                var rotationTrigger = button.AddTrigger();
                rotationTrigger.AddProperty(x => x.IsChecked);
                rotationTrigger.AddConditionalAction(x => x.IsChecked ?? false, setters =>
                {
                    setters.Set(expandPath, x => x.RenderTransform, new RotateTransform(180, 3, 3));
                });
                rotationTrigger.AddConditionalAction(x => !x.IsChecked ?? false, setters =>
                {
                    setters.Set(expandPath, x => x.RenderTransform, new RotateTransform(135, 3, 3));
                });
            }
        }
    }
}