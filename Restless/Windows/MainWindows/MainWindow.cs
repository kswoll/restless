using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using Restless.Controls;
using Restless.ViewModels;
using Restless.WpfExtensions;
using SexyReact.Views;

namespace Restless.Windows.MainWindows
{
    public class MainWindow : RxWindow<MainWindowModel>
    {
        private readonly Grid grid;
        private UIElement content;

        public MainWindow()
        {
            Height = 550;
            Width = 725;

            var apiListItemTemplate = new FrameworkElementFactory(typeof(TextBlock));

            var apiList = new ListView();
            apiList.ItemTemplate = new DataTemplate { VisualTree = apiListItemTemplate };

            grid = new Grid();
            grid.AddColumn(300);
            grid.AddColumn(4);
            grid.AddColumn(1, GridUnitType.Star);
            grid.AddRow(1, GridUnitType.Star);

            grid.Add(apiList, 0, 0);
            grid.AddVerticalSplitter(0, 1);

            var apiListContextMenu = new ContextMenu();
            var apiDeleteMenuItem = apiListContextMenu.Add("_Delete");
            apiList.ContextMenu = apiListContextMenu;

            var menu = new Menu();
            var fileMenu = menu.Add("_File");
            var newApiMenuItem = fileMenu.Add("_New Api");

            var content = new DockPanel { LastChildFill = true };
            content.Add(menu, Dock.Top);
            content.Add(grid);

            Content = content;

            var itemPanel = new ApiPanel();

            this.Bind(x => x.Title).To(this, (window, title) => window.Title = title ?? "");
            this.Bind(x => x.Title).To(apiListItemTemplate, TextBlock.TextProperty);
            this.Bind(x => x.Items).To(apiList, x => x.SelectedItem);
            this.Bind(x => x.DeleteSelectedItem).To(x => apiDeleteMenuItem.Command = x);
            this.Bind(x => x.SelectedItem).ObserveModelPropertyChange().Subscribe(x =>
            {
                itemPanel.Model = x;
                Show(itemPanel);
            });

            var addApi = this.Bind(x => x.AddApi);
            addApi.To(x => newApiMenuItem.Command = x);
            addApi.ObserveModelPropertyChange().SelectMany(x => x).Subscribe(apiModel =>
            {
                apiList.SelectedValue = apiModel;
                ((ApiPanel)this.content).InitNew();
            });            
        }

        private void Show(FrameworkElement content)
        {
            if (this.content != null)
                grid.Children.Remove(this.content);
            this.content = content;
            grid.Add(content, 0, 2);            
        }
    }
}