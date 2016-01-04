using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Restless.Controls;
using Restless.ViewModels;
using Restless.WpfExtensions;
using SexyReact.Views;

namespace Restless.Windows.MainWindows
{
    public class MainWindow : RxWindow<MainWindowModel>
    {
        private UIElement content;

        public MainWindow()
        {
            this.Bind(x => x.Title).To(this, (window, title) => Title = title ?? "");
            Height = 550;
            Width = 725;

            var apiListItemTemplate = new FrameworkElementFactory(typeof(TextBlock));

            apiListItemTemplate.SetValue(TextBlock.TextProperty, new Binding(nameof(ApiModel.Title)));

            var apiList = new ListView();
            apiList.ItemTemplate = new DataTemplate { VisualTree = apiListItemTemplate };
            this.Bind(x => x.Items).To(this, (window, items) => apiList.ItemsSource = items);

            var grid = new Grid();
            grid.AddColumn(300);
            grid.AddColumn(4);
            grid.AddColumn(1, GridUnitType.Star);
            grid.AddRow(1, GridUnitType.Star);

            grid.Add(apiList, 0, 0);
            grid.AddVerticalSplitter(0, 1);

            var menu = new Menu();
            var fileMenu = menu.Add("_File");
            var newApiMenuItem = fileMenu.Add("_New Api");

            this.Bind(x => x.AddApi).To(x => newApiMenuItem.Command = x);
            
            var content = new DockPanel { LastChildFill = true };
            content.Add(menu, Dock.Top);
            content.Add(grid);

            Content = content;

            apiList.SelectionChanged += (sender, args) =>
            {
                var item = (ApiModel)apiList.SelectedValue;
                var itemPanel = new ApiPanel { Model = item };

                if (this.content != null)
                    grid.Children.Remove(this.content);
                this.content = itemPanel;
                grid.Add(itemPanel, 0, 2);
            };
        }
    }
}