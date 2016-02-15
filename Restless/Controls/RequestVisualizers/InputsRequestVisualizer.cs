using System.Windows.Controls;
using Restless.ViewModels;
using SexyReact;
using SexyReact.Views;

namespace Restless.Controls.RequestVisualizers
{
    public class InputsRequestVisualizer : RequestVisualizer
    {
        public override string Title => "Inputs";

        public InputsRequestVisualizer()
        {
            var apiInputsGrid = new RxDataGrid<ApiInputModel>
            {
                AutoGenerateColumns = false,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                SelectionUnit = DataGridSelectionUnit.Cell
            };
            apiInputsGrid.AddTextColumn("Name", x => x.Name, new DataGridLength(1, DataGridLengthUnitType.Star), true);
            apiInputsGrid.AddTextColumn("Default Value", x => x.DefaultValue, new DataGridLength(2, DataGridLengthUnitType.Star));
            apiInputsGrid.AddTextColumn("Value", x => x.Value, new DataGridLength(2, DataGridLengthUnitType.Star));
            apiInputsGrid.AddTextColumn("Type", x => x.InputType, new DataGridLength(1, DataGridLengthUnitType.Star), true);
            Children.Add(apiInputsGrid);

            this.Bind(x => x.Inputs).To(x => apiInputsGrid.ItemsSource = x?.ToObservableCollection());
        }
    }
}