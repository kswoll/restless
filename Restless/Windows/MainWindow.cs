using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Restless.Windows
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            Title = "Restless";
            Height = 350;
            Width = 525;

            var button = new Button
            {
                Content = "Scan Drive"
            };
            button.Click += (sender, args) =>
            {
//                var analyzer = new DiskAnalyzer();
//                analyzer.AnalyzeDrive(new DriveInfo("c:\\"));
            };

            var apiList = new ListView();

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
            menu.Items.Add(new MenuItem
            {
                Header = "_File"
            });
            
            var content = new DockPanel { LastChildFill = true };
            DockPanel.SetDock(menu, Dock.Top);
            content.Children.Add(menu);
            content.Children.Add(grid);

//            grid.Children.Add(menu);

            Content = content;
        }
    }
}