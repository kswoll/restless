using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Restless.WpfExtensions;

namespace Restless.Templates
{
    public class TreeViewItemTemplate : Grid
    {
        public override void EndInit()
        {
            base.EndInit();

            var item = (TreeViewItem)TemplatedParent;
            item.Padding = new Thickness(3);
            item.HorizontalContentAlignment = HorizontalAlignment.Left;
            item.VerticalContentAlignment = VerticalAlignment.Top;
            item.Background = Brushes.Transparent;
            item.BorderThickness = new Thickness(1);
            item.Cursor = Cursors.Arrow;
            item.Margin = new Thickness(0, 1, 0, 0);

            Background = null;

            this.AddRow(GridLength.Auto);
            this.AddRow(new GridLength(1, GridUnitType.Star));
            this.AddColumn(15);
            this.AddColumn(GridLength.Auto);
            this.AddColumn(new GridLength(1, GridUnitType.Star));

            var expanderButtonTemplate = new ControlTemplate(typeof(ToggleButton));
            expanderButtonTemplate.VisualTree = new FrameworkElementFactory(typeof(ExpanderButtonTemplate));
            var expanderButton = new ToggleButton
            {
                Name = "ExpanderButton",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                IsTabStop = false,
                Template = expanderButtonTemplate
            };
            Children.Add(expanderButton);

            SolidColorBrush selectionFill;
            SolidColorBrush selectionStroke;
            var selection = new Rectangle
            {
                Opacity = 0,
                StrokeThickness = 1,
                IsHitTestVisible = false,
                RadiusX = 2,
                RadiusY = 2,
                Fill = selectionFill = new SolidColorBrush(Color.FromArgb(0xFF, 0xBA, 0xDD, 0xE9)),
                Stroke = selectionStroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x6D, 0xBD, 0xD1))
            };
            this.Add(selection, 0, 1);

            var headerButtonTemplate = new ControlTemplate(typeof(Button));
            headerButtonTemplate.VisualTree = new FrameworkElementFactory(typeof(HeaderButtonTemplate));
            var header = new Button
            {
                Name = "PART_Header",
                ClickMode = ClickMode.Hover,
                Background = item.Background,
                Foreground = item.Foreground,
                BorderBrush = item.BorderBrush,
                BorderThickness = item.BorderThickness,
                Cursor = item.Cursor,
                HorizontalAlignment = item.HorizontalContentAlignment,
                VerticalAlignment = item.VerticalContentAlignment,
                IsTabStop = false,
                Template = headerButtonTemplate,
                Content = new ContentPresenter
                {
                    Content = item.Header,
                    ContentTemplate = item.HeaderTemplate
                }
            };
            this.Add(header, 0, 1);

/*
            var validation = new Border
            {
                BorderThickness = item.BorderThickness,
                BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xDB, 0x00, 0x0C)),
                CornerRadius = new CornerRadius(2),
                Visibility = Visibility.Collapsed,
                ToolTip = new ToolTip
                {
                    Template = 
                }
            };
            this.Add(validation, 0, 1);
*/
            var itemsHost = new ItemsPresenter
            {
                Name = "ItemsHost",
                Visibility = Visibility.Collapsed,
            };
            SetColumnSpan(itemsHost, 2);
            this.Add(itemsHost, 1, 1);

            var commonStates = new VisualStateGroup { Name = "CommonStates" };
            commonStates.CreateState("Normal");
            commonStates.CreateState("MouseOver");
            commonStates.CreateState("Pressed");
            var disabledState = commonStates.CreateState("Disabled");

            var disabledStoryboard = new Storyboard();
            disabledStoryboard.AddObjectAnimationUsingKeyFrames(header, x => x.Foreground, new SolidColorBrush(Color.FromArgb(0xFF, 0x99, 0x99, 0x99)));
            disabledState.Storyboard = disabledStoryboard;

            var selectionStates = new VisualStateGroup { Name = "SelectionStates" };
            selectionStates.CreateState("Unselected");
            var selectedState = selectionStates.CreateState("Selected");
            var selectedInactiveState = selectionStates.CreateState("SelectedInactive");

            var selectedStoryboard = new Storyboard();
            selectedStoryboard.AddDoubleAnimation(selection, x => x.Opacity, .75);
            selectedState.Storyboard = selectedStoryboard;

            var selectedInactiveStoryboard = new Storyboard();
            selectedInactiveStoryboard.AddDoubleAnimation(selection, x => x.Opacity, .2);
            selectedInactiveStoryboard.AddColorAnimation(selectionFill, x => x.Color, Color.FromArgb(0xFF, 0x99, 0x99, 0x99));
            selectedInactiveStoryboard.AddColorAnimation(selectionStroke, x => x.Color, Color.FromArgb(0xFF, 0x33, 0x33, 0x33));
            selectedInactiveState.Storyboard = selectedInactiveStoryboard;

            var hasItemStates = new VisualStateGroup { Name = "HasItemStates" };
            hasItemStates.CreateState("HasItems");
            var noItemsState = hasItemStates.CreateState("NoItems");

            var noItemsStoryboard = new Storyboard();
            noItemsStoryboard.AddObjectAnimationUsingKeyFrames(expanderButton, x => x.Visibility, Visibility.Collapsed);
            noItemsState.Storyboard = noItemsStoryboard;

            var expansionStates = new VisualStateGroup { Name = "ExpansionStates" };
            expansionStates.CreateState("Collapsed");
            var expandedState = expansionStates.CreateState("Expanded");

            var expandedStoryboard = new Storyboard();
            expandedStoryboard.AddObjectAnimationUsingKeyFrames(itemsHost, x => x.Visibility, Visibility.Visible);
            expandedState.Storyboard = expandedStoryboard;

            var stateGroups = this.GetVisualStateGroups();
            stateGroups.Add(commonStates);
            stateGroups.Add(selectionStates);
            stateGroups.Add(hasItemStates);
            stateGroups.Add(expansionStates);
        }

        private class ExpanderButtonTemplate : Grid
        {
            public override void EndInit()
            {
                base.EndInit();

                Background = Brushes.Transparent;

                var commonStates = new VisualStateGroup();
                commonStates.CreateState("Normal");
                var mouseOverState = commonStates.CreateState("MouseOver");
                var disabledState = commonStates.CreateState("Disabled");

                var checkStates = new VisualStateGroup { Name = "CheckStates" };
                var checkedState = checkStates.CreateState("Checked");
                checkStates.CreateState("Unchecked");

                var stateGroups = this.GetVisualStateGroups();
                stateGroups.Add(commonStates);

                var grid = new Grid
                {
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Margin = new Thickness(2, 2, 5, 2)
                };
                var uncheckedVisual = new Path
                {
                    Width = 6,
                    Height = 9,
                    Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF)),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Data = Geometry.Parse("M 0,0 L 0,9 L 5,4.5 Z"),
                    StrokeThickness = 1,
                    StrokeLineJoin = PenLineJoin.Miter,
                    Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x98, 0x98, 0x98))
                };
                grid.Children.Add(uncheckedVisual);
                var checkedVisual = new Path
                {
                    Opacity = 0,
                    Width = 6,
                    Height = 6,
                    Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0x26, 0x26, 0x26)),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Data = Geometry.Parse("M 6,0 L 6,6 L 0,6 Z"),
                    StrokeLineJoin = PenLineJoin.Miter
                };
                grid.Children.Add(checkedVisual);

                Children.Add(grid);

                var mouseOverStoryboard = new Storyboard();
                mouseOverStoryboard.AddColorAnimation(uncheckedVisual, x => ((SolidColorBrush)x.Stroke).Color, Color.FromArgb(0xFF, 0x1B, 0xBB, 0xFA));
                mouseOverState.Storyboard = mouseOverStoryboard;

                var disabledStoryboard = new Storyboard();
                disabledStoryboard.AddDoubleAnimation(this, x => x.Opacity, .7);
                disabledState.Storyboard = disabledStoryboard;

                var checkedStoryboard = new Storyboard();
                checkedStoryboard.AddDoubleAnimation(uncheckedVisual, x => x.Opacity, 0);
                checkedStoryboard.AddDoubleAnimation(checkedVisual, x => x.Opacity, 1);
                checkedState.Storyboard = checkedStoryboard;
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