using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Restless.ViewModels;
using Restless.WpfExtensions;
using SexyReact.Views;

namespace Restless.Controls
{
    public class ApiPanel : RxDockPanel<ApiModel>
    {
        private readonly NameValuePanel<TextBox> title;

        public ApiPanel()
        {
            title = NameValuePanel.Create("Title", new TextBox());
            var url = NameValuePanel.Create("URL", new TextBox());
            var method = new ComboBox
            {
                VerticalAlignment = VerticalAlignment.Bottom,
                Margin = new Thickness(0, 0, 5, 0),
                SelectedIndex = 0
            };

            var urlAndMethod = new Grid();
            urlAndMethod.AddColumn(1, GridUnitType.Star);
            urlAndMethod.AddColumn(100, GridUnitType.Pixel);
            urlAndMethod.AddRow(GridLength.Auto);
            urlAndMethod.Add(url, 0, 0);
            urlAndMethod.Add(method, 0, 1);

            var buttonStyle = new Style
            {
                TargetType = typeof(Button),
                Setters = {  }
            };
            var buttonTemplate = new ControlTemplate
            {

            };
            var buttonGrid = new Grid
            {

            };
            var buttonGridFactory = new FrameworkElementFactory(typeof(Grid));
//            buttonGridFactory.
            buttonTemplate.VisualTree = buttonGridFactory;

            var sendButton = new Button { Content = new Label { Content = "Send" } };

            var buttonsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Thickness(5, 0, 5, 0)
            };
            buttonsPanel.Children.Add(sendButton);

            var apiDetailsPanel = new StackPanel();
            apiDetailsPanel.Children.Add(title);
            apiDetailsPanel.Children.Add(urlAndMethod);
            apiDetailsPanel.Children.Add(new Separator { Margin = new Thickness(0, 10, 0, 10) });
            apiDetailsPanel.Children.Add(buttonsPanel);
            apiDetailsPanel.Children.Add(new Separator { Margin = new Thickness(0, 10, 0, 10) });

            var apiResponsePanel = new Label { Content = "Foo" };

            this.Add(apiDetailsPanel, Dock.Top);
            this.Add(apiResponsePanel);

            this.Bind(x => x.Title).Mate(title.Value);
            this.Bind(x => x.Url).Mate(url.Value);
            this.Bind(x => x.Methods).To(x => method.ItemsSource = x);
            this.Bind(x => x.Method).Mate(method);
        }

        public void InitNew()
        {
            Dispatcher.InvokeAsync(() =>
            {
                title.Value.SelectAll();
                title.Value.Focus();
            });
        }
    }
}