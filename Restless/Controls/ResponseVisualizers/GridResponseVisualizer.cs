using System.Linq;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;
using Restless.ViewModels;
using Restless.WpfExtensions;
using SexyReact.Views;

namespace Restless.Controls.ResponseVisualizers
{
    public class GridResponseVisualizer : RxDockPanel<ApiResponseModel>, IResponseVisualizer
    {
        public string Header => "Grid";
        public bool IsThisPrimary(IResponseVisualizer other) => true;

        public GridResponseVisualizer()
        {
            var grid = new DataGrid
            {
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                IsReadOnly = true,
                SelectionUnit = DataGridSelectionUnit.Cell
            };
            this.Add(grid);

            this.Bind(x => x.JsonResponse).To(_ =>
            {
                var rows = (JArray)Model.Api.Outputs.Single(x => x.Name == "rows").Value;
                grid.ItemsSource = rows;
            });
        }

        [ResponseVisualizerPredicate]
        public static bool IsVisualizerEnabled(ApiResponseModel model)
        {
            return model.JsonResponse != null && model.Api.Outputs.SingleOrDefault(x => x.Name == "rows")?.Value != null;
        }

        public int CompareTo(IResponseVisualizer other)
        {
            if (other is JsonResponseVisualizer)
                return 1;
            else
                return 0;
        }
    }
}