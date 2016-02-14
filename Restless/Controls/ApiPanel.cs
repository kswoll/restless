﻿using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using Restless.Controls.RequestVisualizers;
using Restless.Models;
using Restless.Properties;
using Restless.Utils;
using Restless.ViewModels;
using Restless.WpfExtensions;
using SexyReact;
using SexyReact.Views;

namespace Restless.Controls
{
    public class ApiPanel : RxGrid<ApiModel>
    {
        private ApiResponsePanel currentApiResponsePanel;
        private List<RequestVisualizer> requestVisualizers = new List<RequestVisualizer>();

        public ApiPanel()
        {
            requestVisualizers.Add(new GeneralRequestVisualizer());

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

            var apiOutputsGrid = new RxDataGrid<ApiOutputModel>
            {
                AutoGenerateColumns = false,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                CanUserAddRows = true,
                CanUserDeleteRows = true
            };
            apiOutputsGrid.AddTextColumn("Name", x => x.Name, new DataGridLength(1, DataGridLengthUnitType.Star));
            apiOutputsGrid.AddTextColumn("Expression", x => x.Expression, new DataGridLength(2, DataGridLengthUnitType.Star));
            apiOutputsGrid.AddTextColumn("Value", x => x.Value, new DataGridLength(2, DataGridLengthUnitType.Star), true);
            var outputTypeColumn = apiOutputsGrid.AddComboBoxColumn("Type", x => x.Type, new DataGridLength(1, DataGridLengthUnitType.Star));
            var apiOutputsPanel = new Grid();
            apiOutputsPanel.RowDefinitions.Add(new RowDefinition { SharedSizeGroup = "apiTabs" });
            apiOutputsPanel.Children.Add(apiOutputsGrid);

            var apiDetailsPanel = new TabControl();
            var bodyTab = new TabItem { Header = "Body", Content = apiBodyPanel };
            SetIsSharedSizeScope(apiDetailsPanel, true);
            foreach (var requestVisualizer in requestVisualizers)
            {
                apiDetailsPanel.Items.Add(new TabItem { Header = requestVisualizer.Title, Content = requestVisualizer });
            }
            apiDetailsPanel.Items.Add(new TabItem { Header = "Headers", Content = apiHeadersPanel });
            apiDetailsPanel.Items.Add(bodyTab);
            apiDetailsPanel.Items.Add(new TabItem { Header = "Inputs", Content = apiInputsPanel });
            apiDetailsPanel.Items.Add(new TabItem { Header = "Outputs", Content = apiOutputsPanel });

            var topPanel = new DockPanel();
            topPanel.Add(buttonsAndStatusPanel, Dock.Bottom);
            topPanel.Add(apiDetailsPanel);

            topPanel.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

            var topRow = this.AddRow((int)topPanel.DesiredSize.Height);
            this.AddRow(4);
            this.AddRow(new GridLength(1, GridUnitType.Star));
            topRow.MinHeight = topPanel.DesiredSize.Height;
            this.Add(topPanel, 0, 0);
            this.AddHorizontalSplitter(1, 0);

            this.Bind(x => x.Inputs).To(x => apiInputsGrid.ItemsSource = x?.ToObservableCollection());
            this.Bind(x => x.Outputs).To(x => apiOutputsGrid.ItemsSource = x?.ToObservableCollection());
            this.Bind(x => x.Headers).To(x => apiHeadersGrid.ItemsSource = x?.ToObservableCollection());
            this.Bind(x => x.Model.Body).Mate(apiBodyTextBox);
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
            this.Bind(x => x.MainWindow.OutputTypes).To(outputTypeColumn);
            this.Bind(x => x.MainWindow.ApiSplitterPosition).Mate(topRow, RowDefinition.HeightProperty);
            this.Bind(x => x.Model.Method).To(x => bodyTab.Visibility = x.IsBodyAllowed() ? Visibility.Visible : Visibility.Collapsed);

            foreach (var requestVisualizer in requestVisualizers)
            {
                this.Bind(x => x).To(x => requestVisualizer.Model = x);
            }
        }

        public void InitNew()
        {
            foreach (var requestVisualizer in requestVisualizers)
            {
                requestVisualizer.InitNew();
            }
        }
    }
}