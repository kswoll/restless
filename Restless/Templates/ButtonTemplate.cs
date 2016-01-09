using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Restless.WpfExtensions;

namespace Restless.Templates
{
    public class ButtonTemplate : Grid
    {
        public override void EndInit()
        {
            var button = (Button)TemplatedParent;

            var stateGroups = this.GetVisualStateGroups();

            var commonStates = new VisualStateGroup();
            commonStates.CreateState("Normal");
            var mouseOverState = commonStates.CreateState("MouseOver");
            var pressedState = commonStates.CreateState("Pressed");
            var disabledState = commonStates.CreateState("Disabled");

            var focusStates = new VisualStateGroup();
            var focusedState = focusStates.CreateState("Focused");
            focusStates.CreateState("Unfocused");

            stateGroups.Add(commonStates);
            stateGroups.Add(focusStates);

            var disabledContent = new Rectangle
            {
                Fill = new SolidColorBrush(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF)),
                RadiusX = 3,
                RadiusY = 3,
                Opacity = 0,
                IsHitTestVisible = false
            };
            var focusContent = new Rectangle
            {
                Stroke = new SolidColorBrush(Color.FromArgb(0xFF, 0x6D, 0xBD, 0xD1)),
                StrokeThickness = 1,
                RadiusX = 2,
                RadiusY = 2,
                Margin = new Thickness(1),
                Opacity = 0,
                IsHitTestVisible = false
            };
            var border = new Border
            {
                CornerRadius = new CornerRadius(3),
                Background = new SolidColorBrush(Colors.White),
                BorderThickness = button.BorderThickness,
                BorderBrush = button.BorderBrush
            };
            var borderGrid = new Grid
            {
                Background = button.Background,
                Margin = new Thickness(1)
            };
            border.Child = borderGrid;
            var borderGridBackground = new Border
            {
                Opacity = 0,
                Background = new SolidColorBrush(Color.FromArgb(0xFF, 0x44, 0x8D, 0xCA))
            };
            borderGrid.Children.Add(borderGridBackground);

            var borderGridGradient = new Rectangle
            {
                Fill = new LinearGradientBrush(
                    new GradientStopCollection(new[]
                    {
                        new GradientStop(Color.FromArgb(0xFF, 0xFF, 0xFF, 0xFF), 0), 
                        new GradientStop(Color.FromArgb(0xF9, 0xFF, 0xFF, 0xFF), .375), 
                        new GradientStop(Color.FromArgb(0xE5, 0xFF, 0xFF, 0xFF), .625), 
                        new GradientStop(Color.FromArgb(0xC6, 0xFF, 0xFF, 0xFF), 1) 
                    }),
                    new Point(.7, 0),
                    new Point(.7, 1))
            };
            borderGrid.Children.Add(borderGridGradient);

            var disabledStoryboard = new Storyboard();
            var disabledAnimation = new DoubleAnimation(0, .55D, new Duration(TimeSpan.FromSeconds(0)));
            disabledStoryboard.AddAnimation(disabledAnimation, disabledContent, x => x.Opacity);
            disabledState.Storyboard = disabledStoryboard;

            var mouseOverStoryboard = new Storyboard();
            mouseOverStoryboard.AddDoubleAnimation(borderGridBackground, x => x.Opacity, 1);

            var mouseOverGradient1Animation = new ColorAnimation(Color.FromArgb(0xF2, 0xFF, 0xFF, 0xFF), new Duration(TimeSpan.FromSeconds(0)));
            Storyboard.SetTarget(mouseOverGradient1Animation, borderGridGradient);
            Storyboard.SetTargetProperty(mouseOverGradient1Animation, new PropertyPath("(0).(1)[1].(2)", Shape.FillProperty, GradientBrush.GradientStopsProperty, GradientStop.ColorProperty));

            var mouseOverGradient2Animation = new ColorAnimation(Color.FromArgb(0xCC, 0xFF, 0xFF, 0xFF), new Duration(TimeSpan.FromSeconds(0)));
            Storyboard.SetTarget(mouseOverGradient2Animation, borderGridGradient);
            Storyboard.SetTargetProperty(mouseOverGradient2Animation, new PropertyPath("Fill.GradientStops[2].Color"));

            var mouseOverGradient3Animation = new ColorAnimation(Color.FromArgb(0x7F, 0xFF, 0xFF, 0xFF), new Duration(TimeSpan.FromSeconds(0)));
            Storyboard.SetTarget(mouseOverGradient3Animation, borderGridGradient);
            Storyboard.SetTargetProperty(mouseOverGradient3Animation, new PropertyPath("Fill.GradientStops[3].Color"));

            mouseOverStoryboard.Children.Add(mouseOverGradient1Animation);
            mouseOverStoryboard.Children.Add(mouseOverGradient2Animation);
            mouseOverStoryboard.Children.Add(mouseOverGradient3Animation);
            mouseOverState.Storyboard = mouseOverStoryboard;

            var pressedBackgroundColorAnimation = new ColorAnimation(Color.FromArgb(0xFF, 0x6D, 0xBD, 0xD1), new Duration(TimeSpan.FromSeconds(0)));
            Storyboard.SetTarget(pressedBackgroundColorAnimation, border);
            Storyboard.SetTargetProperty(pressedBackgroundColorAnimation, new PropertyPath("Background.Color"));

            var pressedBackgroundOpacityAnimation = new DoubleAnimation(1, new Duration(TimeSpan.FromSeconds(0)));
            Storyboard.SetTarget(pressedBackgroundOpacityAnimation, borderGridBackground);
            Storyboard.SetTargetProperty(pressedBackgroundOpacityAnimation, new PropertyPath(OpacityProperty));

            var pressedGradient1Animation = new ColorAnimation(Color.FromArgb(0xD8, 0xFF, 0xFF, 0xFF), new Duration(TimeSpan.FromSeconds(0)));
            Storyboard.SetTarget(pressedGradient1Animation, borderGridGradient);
            Storyboard.SetTargetProperty(pressedGradient1Animation, new PropertyPath("(0).(1)[0].(2)", Shape.FillProperty, GradientBrush.GradientStopsProperty, GradientStop.ColorProperty));

            var pressedGradient2Animation = new ColorAnimation(Color.FromArgb(0xD8, 0xFF, 0xFF, 0xFF), new Duration(TimeSpan.FromSeconds(0)));
            Storyboard.SetTarget(pressedGradient2Animation, borderGridGradient);
            Storyboard.SetTargetProperty(pressedGradient2Animation, new PropertyPath("(0).(1)[1].(2)", Shape.FillProperty, GradientBrush.GradientStopsProperty, GradientStop.ColorProperty));

            var pressedGradient3Animation = new ColorAnimation(Color.FromArgb(0xD8, 0xFF, 0xFF, 0xFF), new Duration(TimeSpan.FromSeconds(0)));
            Storyboard.SetTarget(pressedGradient3Animation, borderGridGradient);
            Storyboard.SetTargetProperty(pressedGradient3Animation, new PropertyPath("(0).(1)[2].(2)", Shape.FillProperty, GradientBrush.GradientStopsProperty, GradientStop.ColorProperty));

            var pressedGradient4Animation = new ColorAnimation(Color.FromArgb(0xD8, 0xFF, 0xFF, 0xFF), new Duration(TimeSpan.FromSeconds(0)));
            Storyboard.SetTarget(pressedGradient4Animation, borderGridGradient);
            Storyboard.SetTargetProperty(pressedGradient4Animation, new PropertyPath("(0).(1)[3].(2)", Shape.FillProperty, GradientBrush.GradientStopsProperty, GradientStop.ColorProperty));

            var pressedStoryboard = new Storyboard();
            pressedStoryboard.Children.Add(pressedBackgroundColorAnimation);
            pressedStoryboard.Children.Add(pressedBackgroundOpacityAnimation);
            pressedStoryboard.Children.Add(pressedGradient1Animation);
            pressedStoryboard.Children.Add(pressedGradient2Animation);
            pressedStoryboard.Children.Add(pressedGradient3Animation);
            pressedStoryboard.Children.Add(pressedGradient4Animation);
            pressedState.Storyboard = pressedStoryboard;

            var focusedOpacityAnimation = new DoubleAnimation(1, new Duration(TimeSpan.FromSeconds(0)));
            Storyboard.SetTarget(focusedOpacityAnimation, focusContent);
            Storyboard.SetTargetProperty(focusedOpacityAnimation, new PropertyPath(OpacityProperty));

            var focusedStoryboard = new Storyboard();
            focusedStoryboard.Children.Add(focusedOpacityAnimation);
            focusedState.Storyboard = focusedStoryboard;

            var contentPresenter = new ContentPresenter
            {
                Content = button.Content,
                ContentTemplate = button.ContentTemplate,
                VerticalAlignment = button.VerticalContentAlignment,
                HorizontalAlignment = button.HorizontalContentAlignment,
                Margin = new Thickness(2, 2, 2, 2)
            };
            SetColumn(contentPresenter, 0);
            SetRow(contentPresenter, 0);
            Children.Add(border);
            Children.Add(contentPresenter);
            Children.Add(disabledContent);
            Children.Add(focusContent);
        }
    }
}