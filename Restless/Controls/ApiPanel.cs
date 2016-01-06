using System.Windows;
using System.Windows.Controls;
using Restless.ViewModels;
using Restless.WpfExtensions;
using SexyReact.Views;

namespace Restless.Controls
{
    public class ApiPanel : RxStackPanel<ApiModel>
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

            Children.Add(title);
            Children.Add(urlAndMethod);

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