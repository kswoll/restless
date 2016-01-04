using System.Windows;
using System.Windows.Controls;
using Restless.ViewModels;
using SexyReact.Views;

namespace Restless.Controls
{
    public class ApiPanel : RxGrid<ApiModel>
    {
        public ApiPanel()
        {
            ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = GridLength.Auto
            });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
            RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            var titleLabel = new Label
            {
                Content = "Title"
            };
            SetColumn(titleLabel, 0);
            SetRow(titleLabel, 0);
            Children.Add(titleLabel);

            var title = new TextBox();
            SetColumn(title, 0);
            SetRow(title, 1);
            Children.Add(title);
        }
    }
}