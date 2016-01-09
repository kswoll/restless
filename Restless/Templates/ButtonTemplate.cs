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
            disabledStoryboard.AddDoubleAnimation(disabledContent, x => x.Opacity, .55D);
            disabledState.Storyboard = disabledStoryboard;

            var mouseOverStoryboard = new Storyboard();
            mouseOverStoryboard.AddDoubleAnimation(borderGridBackground, x => x.Opacity, 1);
            mouseOverStoryboard.AddColorAnimation(borderGridGradient, x => ((LinearGradientBrush)x.Fill).GradientStops[1].Color, 
                Color.FromArgb(0xF2, 0xFF, 0xFF, 0xFF));
            mouseOverStoryboard.AddColorAnimation(borderGridGradient, x => ((LinearGradientBrush)x.Fill).GradientStops[2].Color,
                Color.FromArgb(0xCC, 0xFF, 0xFF, 0xFF));
            mouseOverStoryboard.AddColorAnimation(borderGridGradient, x => ((LinearGradientBrush)x.Fill).GradientStops[3].Color,
                Color.FromArgb(0x7F, 0xFF, 0xFF, 0xFF));
            mouseOverState.Storyboard = mouseOverStoryboard;

            var pressedStoryboard = new Storyboard();
            pressedStoryboard.AddColorAnimation(border, x => ((SolidColorBrush)x.Background).Color, 
                Color.FromArgb(0xFF, 0x6D, 0xBD, 0xD1));
            pressedStoryboard.AddDoubleAnimation(borderGridBackground, x => x.Opacity, 1);
            pressedStoryboard.AddColorAnimation(borderGridGradient, x => ((LinearGradientBrush)x.Fill).GradientStops[0].Color, 
                Color.FromArgb(0xD8, 0xFF, 0xFF, 0xFF));
            pressedStoryboard.AddColorAnimation(borderGridGradient, x => ((LinearGradientBrush)x.Fill).GradientStops[1].Color, 
                Color.FromArgb(0xC6, 0xFF, 0xFF, 0xFF));
            pressedStoryboard.AddColorAnimation(borderGridGradient, x => ((LinearGradientBrush)x.Fill).GradientStops[2].Color, 
                Color.FromArgb(0x8C, 0xFF, 0xFF, 0xFF));
            pressedStoryboard.AddColorAnimation(borderGridGradient, x => ((LinearGradientBrush)x.Fill).GradientStops[3].Color, 
                Color.FromArgb(0x3F, 0xFF, 0xFF, 0xFF));
            pressedState.Storyboard = pressedStoryboard;

            var focusedStoryboard = new Storyboard();
            focusedStoryboard.AddDoubleAnimation(focusContent, x => x.Opacity, 1);
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