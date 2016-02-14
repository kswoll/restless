using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Restless.WpfExtensions;
using SexyReact.Views;

namespace Restless.Controls.RequestVisualizers
{
    public class GeneralRequestVisualizer : RequestVisualizer
    {
        public override string Title => "General";

        private readonly NameValuePanel<TextBox> title;

        public GeneralRequestVisualizer()
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

            var apiGeneralStackPanel = new StackPanel
            {
                Margin = new Thickness(0, 0, 0, 10)
            };
            apiGeneralStackPanel.Children.Add(title);
            apiGeneralStackPanel.Children.Add(urlAndMethod);
            Children.Add(apiGeneralStackPanel);

            this.Bind(x => x.Model.Title).Mate(title.Value);
            this.Bind(x => x.Model.Url).Mate(url.Value);
            this.Bind(x => x.Model.Method).Mate(method, x => x.MainWindow.Methods);
        }

        public override void InitNew()
        {
            Dispatcher.InvokeAsync(() =>
            {
                title.Value.SelectAll();
                title.Value.Focus();
            }, DispatcherPriority.ApplicationIdle);
        }
    }
}