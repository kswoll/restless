using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Restless.ViewModels;
using Restless.WpfExtensions;
using SexyReact;
using SexyReact.Views;

namespace Restless.Controls
{
    public class ApiPanel : RxDockPanel<ApiModel>
    {
        private readonly NameValuePanel<TextBox> title;
        private ApiResponsePanel currentApiResponsePanel;

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
            var resetButton = new Button { Content = new Label { Content = "Reset" }, Focusable = false };

            var buttonsPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal
            };
            buttonsPanel.Children.Add(sendButton);
            buttonsPanel.Children.Add(resetButton);

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
                AutoGenerateColumns = false,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                CanUserAddRows = true
            };
            apiHeadersGrid.AddTextColumn("Name", x => x.Name).Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            apiHeadersGrid.AddTextColumn("Value", x => x.Value).Width = new DataGridLength(2, DataGridLengthUnitType.Star);
            var apiHeadersPanel = new Grid();
            apiHeadersPanel.RowDefinitions.Add(new RowDefinition { SharedSizeGroup = "apiTabs" });
            apiHeadersPanel.Children.Add(apiHeadersGrid);

            var apiBodyTextBox = new TextBox();
            var apiBodyPanel = new Grid();
            apiBodyPanel.RowDefinitions.Add(new RowDefinition { SharedSizeGroup = "apiTabs" });
            apiBodyPanel.Children.Add(apiBodyTextBox);

            var apiInputsGrid = new RxDataGrid<ApiInputModel>
            {
                AutoGenerateColumns = false,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                CanUserAddRows = false,
                CanUserDeleteRows = false
            };

            apiInputsGrid.AddTextColumn("Name", x => x.Name, new DataGridLength(1, DataGridLengthUnitType.Star), true);
            apiInputsGrid.AddTextColumn("Default Value", x => x.DefaultValue, new DataGridLength(2, DataGridLengthUnitType.Star));
            apiInputsGrid.AddTextColumn("Value", x => x.Value, new DataGridLength(2, DataGridLengthUnitType.Star));
            apiInputsGrid.AddTextColumn("Type", x => x.InputType, new DataGridLength(1, DataGridLengthUnitType.Star), true);
            var apiInputsPanel = new Grid();
            apiInputsPanel.RowDefinitions.Add(new RowDefinition { SharedSizeGroup = "apiTabs" });
            apiInputsPanel.Children.Add(apiInputsGrid);

            var apiDetailsPanel = new TabControl();
            Grid.SetIsSharedSizeScope(apiDetailsPanel, true);
            apiDetailsPanel.Items.Add(new TabItem { Header = "General", Content = apiGeneralPanel });
            apiDetailsPanel.Items.Add(new TabItem { Header = "Headers", Content = apiHeadersPanel });
            apiDetailsPanel.Items.Add(new TabItem { Header = "Body", Content = apiBodyPanel });
            apiDetailsPanel.Items.Add(new TabItem { Header = "Inputs", Content = apiInputsPanel });

            var topPanel = new StackPanel();
            topPanel.Children.Add(apiDetailsPanel);
            topPanel.Children.Add(buttonsAndStatusPanel);
            topPanel.Children.Add(new Separator { Margin = new Thickness(0, 0, 0, 0) });

            this.Add(topPanel, Dock.Top);

            this.Bind(x => x.Title).Mate(title.Value);
            this.Bind(x => x.Url).Mate(url.Value);
            this.Bind(x => x.Method).Mate(method, x => x.Methods);
            this.Bind(x => x.Inputs).To(x => apiInputsGrid.ItemsSource = x?.ToObservableCollection());
            this.Bind(x => x.Headers).To(x => apiHeadersGrid.ItemsSource = x?.ToObservableCollection());
            this.Bind(x => x.StringBody).Mate(apiBodyTextBox);
            this.Bind(x => x.Send).To(x => sendButton.Command = x);
            this.Bind(x => x.Reset).To(x => resetButton.Command = x);
            this.Bind(x => x.Response).To(x =>
            {
                if (currentApiResponsePanel != null)
                {
                    Children.Remove(currentApiResponsePanel);
                }
                currentApiResponsePanel = new ApiResponsePanel();
                currentApiResponsePanel.Model = x;
                this.Add(currentApiResponsePanel);
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
            }, DispatcherPriority.ApplicationIdle);
        }
    }
}