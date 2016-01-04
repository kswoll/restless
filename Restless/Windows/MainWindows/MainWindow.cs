using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Restless.Controls;
using Restless.ViewModels;
using SexyReact.Views;

namespace Restless.Windows.MainWindows
{
    public class MainWindow : RxWindow<MainWindowModel>
    {
        private UIElement content;

        public MainWindow()
        {
            this.SetBinding(TitleProperty, new Binding("Title"));
            this.Bind(x => x.Title).To(this, (window, title) => Title = title ?? "");
            Height = 550;
            Width = 725;

            var apiListItemTemplate = new FrameworkElementFactory(typeof(TextBlock));

            apiListItemTemplate.SetValue(TextBlock.TextProperty, new Binding(nameof(ApiItemModel.Title)));

            var apiList = new ListView();
            apiList.ItemTemplate = new DataTemplate { VisualTree = apiListItemTemplate };
            this.Bind(x => x.Items).To(this, (window, items) => apiList.ItemsSource = items);

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

            apiList.SelectionChanged += (sender, args) =>
            {
                var item = (ApiItemModel)apiList.SelectedValue;
                var itemPanel = new ApiPanel
                {
                    Model = new ApiModel
                    {
                        Title = item.Title
                    }
                };
                Grid.SetColumn(itemPanel, 2);
                Grid.SetRow(itemPanel, 0);

                if (this.content != null)
                    grid.Children.Remove(this.content);
                this.content = itemPanel;
                grid.Children.Add(itemPanel);
            };
        }
    }
}