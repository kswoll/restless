using System;
using System.Windows;
using System.Windows.Controls;
using Restless.Controls.ResponseVisualizers;
using Restless.ViewModels;
using Restless.WpfExtensions;
using SexyReact;
using SexyReact.Views;

namespace Restless.Controls
{
    public class ApiPanel : RxDockPanel<ApiModel>
    {
        private readonly NameValuePanel<TextBox> title;
        private UIElement currentApiResponsePanel;

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

            var sendButton = new Button { Content = new Label { Content = "Send" }, Focusable = false };

            var buttonsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            buttonsPanel.Children.Add(sendButton);

            var statusLabel = new Label();
            var statusCodeLabel = new Label();
            var statusPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right
            };
            statusPanel.Children.Add(statusCodeLabel);
            statusPanel.Children.Add(statusLabel);

            var buttonsAndStatusPanel = new DockPanel();
            buttonsAndStatusPanel.Add(buttonsPanel, Dock.Left);
            buttonsAndStatusPanel.Add(statusPanel);

            var apiGeneralStackPanel = new StackPanel
            {
                Margin = new Thickness(0, 0, 0, 10)
            };
            apiGeneralStackPanel.Children.Add(title);
            apiGeneralStackPanel.Children.Add(urlAndMethod);
            var apiGeneralPanel = new Grid();
            apiGeneralPanel.RowDefinitions.Add(new RowDefinition { SharedSizeGroup = "apiTabs" });
            apiGeneralPanel.Children.Add(apiGeneralStackPanel);

            var apiHeadersGrid = new RxDataGrid<ApiHeaderModel>
            {
                AutoGenerateColumns = false
            };
            apiHeadersGrid.AddTextColumn("Name", x => x.Name);
            apiHeadersGrid.AddTextColumn("Value", x => x.Value);
            var apiHeadersPanel = new Grid();
            apiHeadersPanel.RowDefinitions.Add(new RowDefinition { SharedSizeGroup = "apiTabs" });
            apiHeadersPanel.Children.Add(apiHeadersGrid);

            var apiDetailsPanel = new TabControl();
            Grid.SetIsSharedSizeScope(apiDetailsPanel, true);
            apiDetailsPanel.Items.Add(new TabItem { Header = "General", Content = apiGeneralPanel });
            apiDetailsPanel.Items.Add(new TabItem { Header = "Headers", Content = apiHeadersPanel });

            var topPanel = new StackPanel();
            topPanel.Children.Add(apiDetailsPanel);
            topPanel.Children.Add(buttonsAndStatusPanel);
            topPanel.Children.Add(new Separator { Margin = new Thickness(0, 0, 0, 0) });

            this.Add(topPanel, Dock.Top);
//            this.Add(apiResponsePanel);

            this.Bind(x => x.Title).Mate(title.Value);
            this.Bind(x => x.Url).Mate(url.Value);
            this.Bind(x => x.Methods).To(x => method.ItemsSource = x);
            this.Bind(x => x.Method).Mate(method);
            this.Bind(x => x.Headers).To(x => apiHeadersGrid.ItemsSource = x == null ? null : x.ToObservableCollection());
            this.Bind(x => x.Send).To(x => sendButton.Command = x);
            this.Bind(x => x.Response).To(x =>
            {
                if (currentApiResponsePanel != null)
                {
                    Children.Remove(currentApiResponsePanel);
                }
                var responseVisualizer = new DefaultResponseVisualizer();
                currentApiResponsePanel = responseVisualizer;
                responseVisualizer.Model = x;
                this.Add(responseVisualizer);
            });
            this.Bind(x => x.Response).To(x => statusPanel.Visibility = x == null ? Visibility.Hidden : Visibility.Visible);
            this.Bind(x => x.Response.StatusCode).To(x => statusCodeLabel.Content = x);
            this.Bind(x => x.Response.Status).To(x => statusLabel.Content = x);
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