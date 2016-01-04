using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Restless.ViewModels;
using Restless.WpfExtensions;
using SexyReact.Views;

namespace Restless.Controls
{
    public class ApiPanel : RxGrid<ApiModel>
    {
        public ApiPanel()
        {
            this.AddColumn(1, GridUnitType.Star);
            this.AddRow(GridLength.Auto);
            this.AddRow(GridLength.Auto);

            var titleLabel = new Label
            {
                Content = "Title"
            };
            this.Add(titleLabel, 0, 0);

            var title = new TextBox();
            title.Margin = new Thickness(5, 0, 5, 0);
            this.Add(title, 1, 0);

            this.Bind(x => x.Title).Mate(title);
        }
    }
}