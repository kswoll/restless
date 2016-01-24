using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
            var button = (ButtonBase)TemplatedParent;

            Background = button.Background;

            var stateGroups = this.GetVisualStateGroups();

            var commonStates = new VisualStateGroup();
            commonStates.CreateState("Normal");
            var mouseOverState = commonStates.CreateState("MouseOver");
            var pressedState = commonStates.CreateState("Pressed");
            var disabledState = commonStates.CreateState("Disabled");
            var checkedState = commonStates.CreateState("Checked");

            var focusStates = new VisualStateGroup();
            var focusedState = focusStates.CreateState("Focused");
            focusStates.CreateState("Unfocused");

            stateGroups.Add(commonStates);
            stateGroups.Add(focusStates);

            var disabledContent = new Rectangle
            {
                Fill = new SolidColorBrush(((SolidColorBrush)button.Background).Color),
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

            var disabledStoryboard = new Storyboard();
            disabledStoryboard.AddDoubleAnimation(disabledContent, x => x.Opacity, .55D);
            disabledState.Storyboard = disabledStoryboard;

            var mouseOverStoryboard = new Storyboard();
            mouseOverStoryboard.AddColorAnimation(this, x => ((SolidColorBrush)x.Background).Color, 
                Color.FromArgb(0xFF, 0xBF, 0xBF, 0xBF));
            mouseOverState.Storyboard = mouseOverStoryboard;

            var pressedStoryboard = new Storyboard();
            pressedStoryboard.AddColorAnimation(this, x => ((SolidColorBrush)x.Background).Color, 
                Color.FromArgb(0xFF, 0x6D, 0xBD, 0xD1));
            pressedState.Storyboard = pressedStoryboard;

            var focusedStoryboard = new Storyboard();
            focusedStoryboard.AddDoubleAnimation(focusContent, x => x.Opacity, 1);
            focusedState.Storyboard = focusedStoryboard;

            var checkedStoryboard = new Storyboard();
            checkedStoryboard.AddColorAnimation(this, x => ((SolidColorBrush)x.Background).Color, 
                Color.FromArgb(0xFF, 0x6D, 0xBD, 0xD1));
            checkedState.Storyboard = checkedStoryboard;

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
            Children.Add(contentPresenter);
            Children.Add(disabledContent);
            Children.Add(focusContent);
        }
    }
}