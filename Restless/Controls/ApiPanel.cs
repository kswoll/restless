using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Restless.Controls.RequestVisualizers;
using Restless.Properties;
using Restless.Utils;
using Restless.ViewModels;
using Restless.WpfExtensions;
using SexyReact.Views;

namespace Restless.Controls
{
    public class ApiPanel : RxGrid<ApiModel>
    {
        private readonly List<RequestVisualizer> requestVisualizers = new List<RequestVisualizer>
        {
            new GeneralRequestVisualizer(),
            new HeadersRequestVisualizer(),
            new BodyRequestVisualizer(),
            new InputsRequestVisualizer(),
            new OutputsRequestVisualizer()
        };

        private ApiResponsePanel currentApiResponsePanel;

        public ApiPanel()
        {
            var sendButton = new Button { Content = Icons.Get(IconResources.Send, 22, 18), Focusable = false, ToolTip = "Send the request to the server" };
            var resetButton = new Button { Content = Icons.Get(IconResources.Reset, 22, 14), Focusable = false, ToolTip = "Reset transient data back to their default state", Padding = new Thickness(3) };

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

            var apiDetailsPanel = new TabControl();
//            SetIsSharedSizeScope(apiDetailsPanel, true);
            foreach (var requestVisualizer in requestVisualizers)
            {
                var item = new TabItem { Header = requestVisualizer.Title, Content = requestVisualizer };
                requestVisualizer.InitializeTab(item);
                apiDetailsPanel.Items.Add(item);
            }

            var topPanel = new DockPanel();
            topPanel.Add(buttonsAndStatusPanel, Dock.Bottom);
            topPanel.Add(apiDetailsPanel);

            topPanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            var preferredHeight = topPanel.DesiredSize.Height;
            var topRow = this.AddRow(preferredHeight);
            this.AddRow(4);
            this.AddRow(new GridLength(1, GridUnitType.Star));
            topRow.MinHeight = preferredHeight;
            this.Add(topPanel, 0, 0);
            this.AddHorizontalSplitter(1, 0);

            this.Bind(x => x.Send).To(x => sendButton.Command = x);
            this.Bind(x => x.Reset).To(x => resetButton.Command = x);
            this.Bind(x => x.Response).To(x =>
            {
                if (currentApiResponsePanel == null)
                {
                    currentApiResponsePanel = new ApiResponsePanel();
                    this.Add(currentApiResponsePanel, 2, 0);
                }
                currentApiResponsePanel.Model = x;
            });
            this.Bind(x => x.Response).To(x => statusPanel.Visibility = x == null ? Visibility.Hidden : Visibility.Visible);
            this.Bind(x => x.Response.StatusCode).To(x => statusCodeLabel.Content = x);
            this.Bind(x => x.Response.Status).To(x => statusLabel.Content = x);
            this.Bind(x => x.MainWindow.ApiSplitterPosition).Mate(topRow, RowDefinition.HeightProperty);

            foreach (var requestVisualizer in requestVisualizers)
            {
                this.Bind(x => x).To(x => requestVisualizer.Model = x);
            }
        }

        public void InitNew()
        {
            foreach (var requestVisualizer in requestVisualizers)
            {
                requestVisualizer.InitializeNew();
            }
        }
    }
}