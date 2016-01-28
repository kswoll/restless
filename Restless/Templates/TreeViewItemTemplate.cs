using System;
using System.Runtime.CompilerServices;
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

        public TreeViewItemTemplate()
        {
            Children.Add(new Label { Name = "Foo" });
        }

        void IAddChild.AddChild(object value)
        {
            if (value is ItemsPresenter)
                itemsHost = (ItemsPresenter)value;
        }

        public override void EndInit()
        {
            var item = (TreeViewItem)TemplatedParent;
            var template = item.Template;
//            template.
//            item.Padding = new Thickness(3);
//            item.HorizontalContentAlignment = HorizontalAlignment.Left;
//            item.VerticalContentAlignment = VerticalAlignment.Top;
//            item.Background = Brushes.Transparent;
//            item.BorderThickness = new Thickness(1);
//            item.Cursor = Cursors.Arrow;
//            item.Margin = new Thickness(0, 1, 0, 0);

//            Background = null;

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

            var borderContent = new ContentPresenter
            {
                Content = item.Header,
                ContentTemplate = item.HeaderTemplate,
                ContentStringFormat = item.HeaderStringFormat,
                ContentSource = "Header",
                Name = "PART_Header",
                HorizontalAlignment = item.HorizontalContentAlignment,
                SnapsToDevicePixels = item.SnapsToDevicePixels
            };
            var border = new Border
            {
                BorderThickness = item.BorderThickness,
                Padding = item.Padding,
                BorderBrush = item.BorderBrush,
                Name = "Bd",
                SnapsToDevicePixels = true
            };
            SetColumn(border, 1);
            border.Child = borderContent;
            Children.Add(border);

            SetColumnSpan(itemsHost, 2);
            this.Add(itemsHost, 1, 1);

            var collapsedAction = new UndoableAction();
            collapsedAction.Set(itemsHost, x => x.Visibility, Visibility.Collapsed);

            var hasItemsAction = new UndoableAction();
            hasItemsAction.Set(expander, x => x.Visibility, Visibility.Hidden);

            var selectedAction = new UndoableAction();
            selectedAction.Set(border, x => x.Background, SystemColors.HighlightBrush);
            selectedAction.Set(item, x => x.Foreground, SystemColors.HighlightTextBrush);

            var unfocusedSelectedAction = new UndoableAction();
            unfocusedSelectedAction.Set(border, x => x.Background, SystemColors.InactiveSelectionHighlightBrush);
            unfocusedSelectedAction.Set(item, x => x.Foreground, SystemColors.InactiveSelectionHighlightTextBrush);

            var disabledAction = new UndoableAction();
            disabledAction.Set(item, x => x.Foreground, SystemColors.GrayTextBrush);

            item.Collapsed += (sender, args) => collapsedAction.Do();
            item.Expanded += (sender, args) => collapsedAction.Do();

            item.IsEnabledChanged += (sender, args) =>
            {
                if ((bool)args.NewValue)
                    disabledAction.Undo();
                else
                    disabledAction.Do();
            };

            base.EndInit();
        }

        private class ExpanderButtonTemplate : Grid
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
                var border = new Border
                {
                    Padding = new Thickness(5),
                    Background = new SolidColorBrush(Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF)),
                    Width = 16,
                    Height = 16
                };
                border.Child = expandPath;

                var checkedAction = new UndoableAction();
                checkedAction.Set(expandPath, x => x.RenderTransform, new RotateTransform(180, 3, 3));
                checkedAction.Set(expandPath, x => x.Fill, new SolidColorBrush(Color.FromArgb(0xFF, 0x59, 0x59, 0x59)));
                checkedAction.Set(expandPath, x => x.Stroke, new SolidColorBrush(Color.FromArgb(0xFF, 0x26, 0x26, 0x26)));

                var mouseOverAction = new UndoableAction();
                mouseOverAction.Set(expandPath, x => x.Stroke, new SolidColorBrush(Color.FromArgb(0xFF, 0x27, 0xC7, 0xF7)));
                mouseOverAction.Set(expandPath, x => x.Fill, new SolidColorBrush(Color.FromArgb(0xFF, 0xCC, 0xEE, 0xFB)));

                var mouseOverAndCheckedAction = new UndoableAction();
                mouseOverAndCheckedAction.Set(expandPath, x => x.Stroke, new SolidColorBrush(Color.FromArgb(0xFF, 0x1C, 0xC4, 0xF7)));
                mouseOverAndCheckedAction.Set(expandPath, x => x.Fill, new SolidColorBrush(Color.FromArgb(0xFF, 0x82, 0xDF, 0xFB)));

                button.Checked += (sender, args) =>
                {
                    if (button.IsMouseDirectlyOver)
                        mouseOverAndCheckedAction.Do();
                    else
                        checkedAction.Do();
                };
                button.Unchecked += (sender, args) =>
                {
                    if (button.IsMouseDirectlyOver)
                        mouseOverAndCheckedAction.Undo();
                    else 
                        checkedAction.Undo();
                };
                button.IsMouseDirectlyOverChanged += (sender, args) =>
                {
                    if (button.IsMouseDirectlyOver)
                    {
                        if (button.IsChecked ?? false)
                            mouseOverAndCheckedAction.Do();
                        else
                            mouseOverAction.Do();
                    }
                    else
                    {
                        if (button.IsChecked ?? false)
                            mouseOverAndCheckedAction.Undo();
                        else
                            mouseOverAction.Undo();
                    }
                };
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

                var pressedStoryboard = new Storyboard();
                pressedStoryboard.AddDoubleAnimation(hover, x => x.Opacity, .5);
                pressedState.Storyboard = pressedStoryboard;

                var disabledStoryboard = new Storyboard();
                disabledStoryboard.AddDoubleAnimation(this, x => x.Opacity, .55);
                disabledState.Storyboard = disabledStoryboard;
            }
        }
    }
}