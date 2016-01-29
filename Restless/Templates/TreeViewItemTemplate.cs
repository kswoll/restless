using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Restless.WpfExtensions;

namespace Restless.Templates
{
    public class TreeViewItemTemplate : Grid, IAddChild
    {
        private ItemsPresenter itemsHost;
        private ContentPresenter header;

        void IAddChild.AddChild(object value)
        {
            if (value is ItemsPresenter)
                itemsHost = (ItemsPresenter)value;
            else if (value is ContentPresenter)
                header = (ContentPresenter)value;
        }

        public override void EndInit()
        {
            var item = (TreeViewItem)TemplatedParent;

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

            header.Content = item.Header;
            header.ContentTemplate = item.HeaderTemplate;
            header.ContentStringFormat = item.HeaderStringFormat;
            header.ContentSource = "Header";
            header.HorizontalAlignment = item.HorizontalContentAlignment;
            header.SnapsToDevicePixels = item.SnapsToDevicePixels;
            var border = new Border
            {
                BorderThickness = item.BorderThickness,
                Padding = item.Padding,
                BorderBrush = item.BorderBrush,
                Name = "Bd",
                SnapsToDevicePixels = true
            };
            SetColumn(border, 1);
            border.Child = header;
            Children.Add(border);

            SetColumnSpan(itemsHost, 2);
            this.Add(itemsHost, 1, 1);

            var stateGroups = this.GetVisualStateGroups();

            var commonStates = new VisualStateGroup();
            commonStates.CreateState("Normal");
            var disabledState = commonStates.CreateState("Disabled");
            stateGroups.Add(commonStates);

            var hasItemsStates = new VisualStateGroup();
            hasItemsStates.CreateState("HasItems");
            var noItemsState = hasItemsStates.CreateState("NoItems");
            stateGroups.Add(hasItemsStates);

            var expandedStates = new VisualStateGroup();
            expandedStates.CreateState("Expanded");
            var collapsedState = expandedStates.CreateState("Collapsed");
            stateGroups.Add(expandedStates);

            var selectedStates = new VisualStateGroup();
            selectedStates.CreateState("Unselected");
            var selectedState = selectedStates.CreateState("Selected");
            var selectedInactiveState = selectedStates.CreateState("SelectedInactive");
            stateGroups.Add(selectedStates);

            disabledState.AddObjectAnimationUsingKeyFrames(item, x => x.Foreground, SystemColors.GrayTextBrush);
            noItemsState.AddObjectAnimationUsingKeyFrames(expander, x => x.Visibility, Visibility.Hidden);
            collapsedState.AddObjectAnimationUsingKeyFrames(itemsHost, x => x.Visibility, Visibility.Collapsed);
            selectedState.AddObjectAnimationUsingKeyFrames(border, x => x.Background, SystemColors.HighlightBrush);
            selectedState.AddObjectAnimationUsingKeyFrames(item, x => x.Foreground, SystemColors.HighlightTextBrush);
            selectedInactiveState.AddObjectAnimationUsingKeyFrames(border, x => x.Background, SystemColors.InactiveSelectionHighlightBrush);
            selectedInactiveState.AddObjectAnimationUsingKeyFrames(item, x => x.Foreground, SystemColors.InactiveSelectionHighlightTextBrush);

            item.PairExpanded(expander, ToggleButton.CheckedEvent, ToggleButton.UncheckedEvent, ToggleButton.IsCheckedProperty);

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
                    Name = "ExpandPath",
                    Data = Geometry.Parse("M0,0L0,6L6,0z"),
                    RenderTransform = new RotateTransform(135, 3, 3)
                };
                Padding = new Thickness(5);
                Background = new SolidColorBrush(Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF));
                Width = 16;
                Height = 16;
                Child = expandPath;

                var stateGroups = this.GetVisualStateGroups();

                var commonStates = new VisualStateGroup();
                commonStates.CreateState("Normal");
                var mouseOverState = commonStates.CreateState("MouseOver");
                stateGroups.Add(commonStates);

                var checkedStates = new VisualStateGroup();
                var checkedState = checkedStates.CreateState("Checked");
                checkedStates.CreateState("Unchecked");
                stateGroups.Add(checkedStates);

                mouseOverState.AddColorAnimation(expandPath, x => ((SolidColorBrush)x.Fill).Color, Color.FromArgb(0xFF, 0x27, 0xC7, 0xF7));
                mouseOverState.AddColorAnimation(expandPath, x => ((SolidColorBrush)x.Stroke).Color, Color.FromArgb(0xFF, 0xCC, 0xEE, 0xFB));

                checkedState.AddObjectAnimationUsingKeyFrames(expandPath, x => x.RenderTransform, new RotateTransform(180, 3, 3));
                checkedState.AddColorAnimation(expandPath, x => ((SolidColorBrush)x.Fill).Color, Color.FromArgb(0xFF, 0x59, 0x59, 0x59));
                checkedState.AddColorAnimation(expandPath, x => ((SolidColorBrush)x.Stroke).Color, Color.FromArgb(0xFF, 0x26, 0x26, 0x26));
            }
        }

        private class HeaderButtonTemplate : Grid
        {
            public override void EndInit()
            {
                base.EndInit();

                var button = (Button)TemplatedParent;

                Background = button.Background;

                var hover = new Rectangle
                {
                    Opacity = 0,
                    Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xBA, 0xDD, 0xE9)),
                    Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x6D, 0xBD, 0xD1)),
                    StrokeThickness = 1,
                    IsHitTestVisible = false,
                    RadiusX = 2,
                    RadiusY = 2
                };
                Children.Add(hover);
                var content = new ContentPresenter
                {
                    Cursor = button.Cursor,
                    Content = button.Content,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = button.Padding
                };
                Children.Add(content);

                var commonStates = new VisualStateGroup();
                commonStates.CreateState("Normal");
                var pressedState = commonStates.CreateState("Pressed");
                var disabledState = commonStates.CreateState("Disabled");

                var stateGroups = this.GetVisualStateGroups();
                stateGroups.Add(commonStates);

                pressedState.AddDoubleAnimation(hover, x => x.Opacity, .5);
                disabledState.AddDoubleAnimation(this, x => x.Opacity, .55);
            }
        }
    }
}