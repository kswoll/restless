using System.Windows.Controls;
using Restless.ViewModels;
using SexyReact;
using SexyReact.Views;

namespace Restless.Controls.RequestVisualizers
{
    public class OutputsRequestVisualizer : RequestVisualizer
    {
        public override string Title => "Outputs";

        public OutputsRequestVisualizer()
        {
            var apiOutputsGrid = new RxDataGrid<ApiOutputModel>
            {
                AutoGenerateColumns = false,
                HeadersVisibility = DataGridHeadersVisibility.Column | DataGridHeadersVisibility.Row,
                CanUserAddRows = true,
                CanUserDeleteRows = true,
                SelectionUnit = DataGridSelectionUnit.CellOrRowHeader
            };
            apiOutputsGrid.AddTextColumn("Name", x => x.Name, new DataGridLength(1, DataGridLengthUnitType.Star));
            apiOutputsGrid.AddTextColumn("Expression", x => x.Expression, new DataGridLength(2, DataGridLengthUnitType.Star));
            apiOutputsGrid.AddTextColumn("Value", x => x.ValueAsString, new DataGridLength(2, DataGridLengthUnitType.Star), true);
            var outputTypeColumn = apiOutputsGrid.AddComboBoxColumn("Type", x => x.Type, new DataGridLength(1, DataGridLengthUnitType.Star));

            Children.Add(apiOutputsGrid);

            this.Bind(x => x.Outputs).To(x => apiOutputsGrid.ItemsSource = x?.ToObservableCollection());
            this.Bind(x => x.MainWindow.OutputTypes).To(outputTypeColumn);
        }
    }
}