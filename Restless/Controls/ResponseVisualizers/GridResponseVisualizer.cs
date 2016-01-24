using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
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
//                AutoGenerateColumns = false,
                CanUserAddRows = false,
                CanUserDeleteRows = false,
                CanUserSortColumns = true,
                IsReadOnly = true,
                SelectionUnit = DataGridSelectionUnit.Cell,
                HeadersVisibility = DataGridHeadersVisibility.Column
            };
            this.Add(grid);

            this.Bind(x => x.JsonResponse).To(_ =>
            {
                var rows = (JArray)Model.Api.Outputs.Single(x => x.Name == "rows").Value;
                string currentSortedColumn = null;
                grid.AutoGeneratingColumn += (sender, args) =>
                {
                    args.Column.CanUserSort = true;
                    args.Column.SortMemberPath = args.PropertyName;
                };
                grid.Sorting += (sender, args) =>
                {
                    Func<JToken, JToken> keySelector = x => x[args.Column.SortMemberPath];
                    var newRows = currentSortedColumn == null || currentSortedColumn != args.Column.SortMemberPath ?
                        rows.OrderBy(keySelector) :
                        rows.OrderByDescending(keySelector);
                    currentSortedColumn = currentSortedColumn == args.Column.SortMemberPath ? null : args.Column.SortMemberPath;
                    grid.ItemsSource = new JArray(newRows);
                    args.Handled = true;
                };
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