using System.Windows.Controls;
using Restless.ViewModels;
using Restless.WpfExtensions;
using SexyReact.Views;

namespace Restless.Controls.ResponseVisualizers
{
    public class HeadersResponseVisualizer : RxDockPanel<ApiResponseModel>, IResponseVisualizer
    {
        public bool IsThisPrimary(IResponseVisualizer other) => false;
        public int CompareTo(IResponseVisualizer other) => other is SummaryResponseVisualizer ? -1 : 1;
        public string Header => "Headers";

        [ResponseVisualizerPredicate]
        public static bool IsVisualizerVisible(ApiResponseModel response) => true;

        public HeadersResponseVisualizer()
        {
            var headersGrid = new RxDataGrid<ApiHeaderModel>()
            {
                AutoGenerateColumns = false,
                IsReadOnly = true,
                HeadersVisibility = DataGridHeadersVisibility.None,
                SelectionUnit = DataGridSelectionUnit.Cell
            };
            headersGrid.AddTextColumn("Name", x => x.Name).Width = new DataGridLength(1, DataGridLengthUnitType.Star);
            headersGrid.AddTextColumn("Value", x => x.Value).Width = new DataGridLength(2, DataGridLengthUnitType.Star);

            this.Add(headersGrid);

            this.Bind(x => x.Headers).To(x => headersGrid.ItemsSource = x);
        }
    }
}