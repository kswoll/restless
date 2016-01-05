using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Restless.Controls;
using Restless.Database;
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
            this.Bind(x => x.Title).To(this, (window, title) => window.Title = title ?? "");
            Height = 550;
            Width = 725;

            var apiListItemTemplate = new FrameworkElementFactory(typeof(TextBlock));
            this.Bind(x => x.Title).To(apiListItemTemplate, TextBlock.TextProperty);

            var apiList = new ListView();
            apiList.ItemTemplate = new DataTemplate { VisualTree = apiListItemTemplate };
            this.Bind(x => x.Items).To(apiList, (view, list) => view.ItemsSource = list);

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

            var addApi = this.Bind(x => x.AddApi);
            addApi.To(x => newApiMenuItem.Command = x);
            addApi.ObserveModelPropertyChange().SelectMany(x => x).Subscribe(apiModel =>
            {
                apiList.SelectedValue = apiModel;
                ((ApiPanel)this.content).InitNew();
            });
            
            var content = new DockPanel { LastChildFill = true };
            content.Add(menu, Dock.Top);
            content.Add(grid);

            Content = content;

            apiList.SelectionChanged += (sender, args) =>
            {
                var item = (ApiModel)args.AddedItems[0];
                var itemPanel = new ApiPanel { Model = item };

                if (this.content != null)
                    grid.Children.Remove(this.content);
                this.content = itemPanel;
                grid.Add(itemPanel, 0, 2);
            };
        }
    }
}