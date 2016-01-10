using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Restless.Models;
using Restless.ViewModels;
using Restless.WpfExtensions;
using SexyReact;
using SexyReact.Views;

namespace Restless.Controls.ResponseVisualizers
{
    public class DefaultResponseVisualizer : RxTabControl<ApiResponseModel>
    {
        public DefaultResponseVisualizer()
        {
            Visibility = Visibility.Hidden;

            var bodyTab = new TabItem
            {
                Header = "Body"
            };

            var bodyText = new TextBox();
            bodyText.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
            bodyText.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
            bodyText.IsReadOnly = true;
            bodyTab.Content = bodyText;
//            bodyTab.Content = new ScrollViewer { Content = bodyText };

            var headersTab = new TabItem
            {
                Header = "Headers"
            };
            var headersGrid = new RxDataGrid<ApiHeaderModel>()
            {
                AutoGenerateColumns = false,
                IsReadOnly = true,
                HeadersVisibility = DataGridHeadersVisibility.None,
                SelectionUnit = DataGridSelectionUnit.Cell
            };
            headersGrid.AddTextColumn("Name", x => x.Name);
            headersGrid.AddTextColumn("Value", x => x.Value);
            headersTab.Content = headersGrid;

            var summaryTab = new TabItem
            {
                Header = "Summary"
            };
            var summaryPanel = new StackPanel();

            var elapsedPanel = NameValuePanel.Create("Elapsed", new Label());
            summaryPanel.Children.Add(elapsedPanel);

            var contentLengthPanel = NameValuePanel.Create("Content Length", new Label());
            summaryPanel.Children.Add(contentLengthPanel);
            summaryTab.Content = summaryPanel;

            Items.Add(bodyTab);
            Items.Add(headersTab);
            Items.Add(summaryTab);

            this.Bind(x => x.Headers).To(x => headersGrid.ItemsSource = x);
            this.Bind(x => x.Response).To(x =>
            {
                Visibility = x == null ? Visibility.Hidden : Visibility.Visible;
                bodyText.Text = x == null ? "" : Encoding.UTF8.GetString(x);
            });
            this.Bind(x => x.ContentLength).To(x => contentLengthPanel.Value.Content = x);
            this.Bind(x => x.Elapsed).To(x => elapsedPanel.Value.Content = x);
        }
    }
}