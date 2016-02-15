using System.Windows.Controls;
using Restless.ViewModels;
using SexyReact;
using SexyReact.Views;

namespace Restless.Controls.RequestVisualizers
{
    public class HeadersRequestVisualizer : RequestVisualizer
    {
        public override string Title => "Headers";

        public HeadersRequestVisualizer()
        {
            var apiHeadersGrid = new RxDataGrid<ApiHeaderModel>
            {
                AutoGenerateColumns = false,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                CanUserAddRows = true,
                SelectionUnit = DataGridSelectionUnit.Cell
            };
            apiHeadersGrid.AddTextColumn("Name", x => x.Name).Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            apiHeadersGrid.AddTextColumn("Value", x => x.Value).Width = new DataGridLength(2, DataGridLengthUnitType.Star);
            Children.Add(apiHeadersGrid);

            this.Bind(x => x.Headers).To(x => apiHeadersGrid.ItemsSource = x?.ToObservableCollection());
        }
    }
}