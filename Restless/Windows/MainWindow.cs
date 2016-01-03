using System.Windows;
using System.Windows.Controls;
using Restless.ViewModels;
using SexyReact.Views;

namespace Restless.Windows
{
    public class MainWindow : RxWindow<MainWindowModel>
    {
        public MainWindow()
        {
            this.Bind(x => x.Title).To(this, (window, title) => Title = title ?? "");
            Height = 550;
            Width = 725;

            var apiList = new ListView();
            this.Bind(x => x.Items).To(this, (window, items) => apiList.ItemsSource = items);
//            apiList.ItemsSource = 
//            apiList.SetBinding(ItemsControl.ItemsSourceProperty, nameof(MainWindowModel.Items));

            var grid = new Grid();
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(300)
            });
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(4)
            });
            grid.ColumnDefinitions.Add(new ColumnDefinition
            {
                Width = new GridLength(1, GridUnitType.Star)
            });
            grid.RowDefinitions.Add(new RowDefinition
            {
                Height = new GridLength(1, GridUnitType.Star)
            });
            Grid.SetColumn(apiList, 0);
            Grid.SetRow(apiList, 0);
            grid.Children.Add(apiList);

            var splitter = new GridSplitter
            {
                Width = 4,
                ResizeDirection = GridResizeDirection.Columns,
                ResizeBehavior = GridResizeBehavior.BasedOnAlignment,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch
            };
            Grid.SetColumn(splitter, 1);
            Grid.SetRow(splitter, 0);
            grid.Children.Add(splitter);

            var menu = new Menu();
            var fileMenu = new MenuItem
            {
                Header = "_File"
            };
            var newApiMenuItem = new MenuItem
            {
                Header = "_New Api"
            };
            fileMenu.Items.Add(newApiMenuItem);
            this.Bind(x => x.AddApi).To(x => newApiMenuItem.Command = x);
            menu.Items.Add(fileMenu);
            
            var content = new DockPanel { LastChildFill = true };
            DockPanel.SetDock(menu, Dock.Top);
            content.Children.Add(menu);
            content.Children.Add(grid);

            Content = content;
        }
    }
}