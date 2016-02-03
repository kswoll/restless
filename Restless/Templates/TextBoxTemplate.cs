using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using Restless.WpfExtensions;
using Restless.WpfExtensions.CodeTriggers;

namespace Restless.Templates
{
    public class TextBoxTemplate : Border, IAddChild
    {
        private ScrollViewer contentHost;

        void IAddChild.AddChild(object value)
        {
            if (value is ScrollViewer)
                contentHost = (ScrollViewer)value;
        }

        public override void EndInit()
        {
            var textBox = (TextBox)TemplatedParent;
            BorderBrush = new SolidColorBrush(Color.FromArgb(0xFF, 0xAB, 0xAD, 0xB3));
            BorderThickness = textBox.BorderThickness;
            Background = textBox.Background;
            SnapsToDevicePixels = true;

            var container = new Grid();

            var placeholder = new Label
            {
                Foreground = new SolidColorBrush(Colors.LightGray),
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Center
            };
            container.Children.Add(contentHost);
            container.Children.Add(placeholder);

            contentHost.Focusable = false;
            contentHost.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
            contentHost.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;
            Child = container;

            var enabledTrigger = textBox.AddTrigger();
            enabledTrigger.AddProperty(x => x.IsEnabled);
            enabledTrigger.AddConditionalAction(x => !x.IsEnabled, setters => setters.Set(this, x => x.Opacity, .56));

            var brushTrigger = textBox.AddTrigger();
            brushTrigger.AddProperty(x => x.IsMouseOver);
            brushTrigger.AddProperty(x => x.IsKeyboardFocused);
            brushTrigger.AddConditionalAction(x => x.IsMouseOver, setters => setters.Set(this, x => ((SolidColorBrush)x.BorderBrush).Color, Color.FromArgb(0xFF, 0x7E, 0xB4, 0xEA)));
            brushTrigger.AddConditionalAction(x => x.IsKeyboardFocused, setters => setters.Set(this, x => ((SolidColorBrush)x.BorderBrush).Color, Color.FromArgb(0xFF, 0x56, 0x9D, 0xE5)));

            var placeholderTextTrigger = textBox.AddTrigger();
            placeholderTextTrigger.AddProperty(Placeholder.PlaceholderProperty);
            placeholderTextTrigger.AddConditionalAction(x => true, setters => setters.Set(placeholder, x => x.Content, Placeholder.GetPlaceholder(textBox)));

            var placeholderTrigger = textBox.AddTrigger();
            placeholderTrigger.AddProperty(x => x.Text);
            placeholderTrigger.AddProperty(x => x.IsKeyboardFocused);
            placeholderTrigger.AddProperty(Placeholder.PlaceholderProperty);
            placeholderTrigger.AddConditionalAction(x => string.IsNullOrEmpty(Placeholder.GetPlaceholder(x)) || !string.IsNullOrEmpty(x.Text) || x.IsKeyboardFocused, setters =>
            {
                setters.Set(placeholder, x => x.Visibility, Visibility.Hidden);
            });

            base.EndInit();
        }
    }
}