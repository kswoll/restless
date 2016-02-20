using System;
using System.Drawing;
using System.Reactive.Linq;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using Restless.Controls;
using Restless.Properties;
using Restless.Styles;
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
            SnapsToDevicePixels = true;
            Icon = BitmapFrame.Create(Application.GetResourceStream(new Uri("/Icon.ico", UriKind.Relative)).Stream);

            ConfigureWindowStateAndPosition();

            var apiList = new TreeView();

            grid = new Grid();
            grid.AddColumn(300);
            grid.AddColumn(4);
            grid.AddColumn(1, GridUnitType.Star);
            grid.AddRow(1, GridUnitType.Star);
            grid.Add(apiList, 0, 0);
            grid.AddVerticalSplitter(0, 1);
            var apiListContextMenu = new ContextMenu();

            var addChildApiMenuItem = apiListContextMenu.Add("Add Child _Api");
            var addChildApiCollectionMenuItem = apiListContextMenu.Add("Add Child Api _Collection");
            var apiDeleteMenuItem = apiListContextMenu.Add("_Delete");
            apiList.ContextMenu = apiListContextMenu;

            var menu = new Menu();
            var fileMenu = menu.Add("_File");
            var newApiMenuItem = fileMenu.Add("New _Api");
            var newApiCollectionItem = fileMenu.Add("New Api _Collection");
            var exportAllMenuItem = fileMenu.Add("_Export");
            var importMenuItem = fileMenu.Add("_Import");

            var content = new DockPanel { LastChildFill = true };
            content.Add(menu, Dock.Top);
            content.Add(grid);

            Content = content;

            var apiPanel = new ApiPanel();
            var apiCollectionPanel = new ApiCollectionPanel();

            var selectedItemBinder = new Binding("IsSelected");
            selectedItemBinder.Mode = BindingMode.TwoWay;
            var expandedItemBinder = new Binding("IsExpanded");
            expandedItemBinder.Mode = BindingMode.TwoWay;
            apiList.ItemContainerStyle = new Style(typeof(TreeViewItem));
            apiList.ItemContainerStyle.Setters.Add(new Setter(TreeViewItem.IsSelectedProperty, selectedItemBinder));
            apiList.ItemContainerStyle.Setters.Add(new Setter(TreeViewItem.IsExpandedProperty, expandedItemBinder));

            this.Bind(x => x.Title).To(this, (window, title) => window.Title = title ?? "");
            this.Bind(x => x.Items).To(apiList, x => x.Items, typeof(Item));
            this.Bind(x => x.AddChildApi).To(x => addChildApiMenuItem.Command = x);
            this.Bind(x => x.AddChildApiCollection).To(x => addChildApiCollectionMenuItem.Command = x);
            this.Bind(x => x.DeleteSelectedItem).To(x => apiDeleteMenuItem.Command = x);

            var selectedItemChange = this.Bind(x => x.SelectedItem).ObserveModelPropertyChange();
            var selectedItemNotNullChange = selectedItemChange.Where(x => x != null);
            selectedItemChange.Where(x => x == null).Subscribe(x => HideContent());
            selectedItemNotNullChange.OfType<ApiModel>().Subscribe(x =>
            {
                apiPanel.Model = x;
                ShowContent(apiPanel);
            });
            selectedItemNotNullChange.OfType<ApiCollectionModel>().Subscribe(x =>
            {
                apiCollectionPanel.Model = x;
                if (x != null)
                    ShowContent(apiCollectionPanel);
                else
                    HideContent();
            });

            this.Bind(x => x.SplitterPosition).Mate(grid.ColumnDefinitions[0], ColumnDefinition.WidthProperty);

            var addApi = this.Bind(x => x.AddApi);
            addApi.To(x => newApiMenuItem.Command = x);
            addApi.ObserveModelPropertyChange().SelectMany(x => x).Subscribe(apiModel =>
            {
                Model.SelectedItem = apiModel;
                ((ApiPanel)this.content).InitNew();
            });
            this.Bind(x => x.AddApiCollection).To(x => newApiCollectionItem.Command = x);

            this.Bind(x => x.Export).To(x => exportAllMenuItem.Command = x);
            this.Bind(x => x.Import).To(x => importMenuItem.Command = x);
        }

        private void ConfigureWindowStateAndPosition()
        {
            WindowState = Settings.Default.WindowState;
            var windowPosition = Settings.Default.WindowPosition;
            if (windowPosition != null)
            {
                Left = windowPosition.Value.Left;
                Top = windowPosition.Value.Top;
                Width = windowPosition.Value.Width;
                Height = windowPosition.Value.Height;
            }
            else
            {
                Height = 550;
                Width = 725;
            }
            StateChanged += (sender, args) =>
            {
                Settings.Default.WindowState = WindowState;
                Settings.Default.Save();
            };
            EventHandler positionChanged = (sender, args) =>
            {
                Settings.Default.WindowPosition = new Rectangle((int)Left, (int)Top, (int)Width, (int)Height);
                Settings.Default.Save();
            };
            LocationChanged += positionChanged;
            SizeChanged += (sender, args) => positionChanged(sender, args);
        }

        private void ShowContent(FrameworkElement content)
        {
            HideContent();
            this.content = content;
            grid.Add(content, 0, 2);            
        }

        private void HideContent()
        {
            if (content != null)
                grid.Children.Remove(this.content);            
        }

        public class Item : RxTextBlock<ApiItemModel>
        {
            public Item()
            {
                this.Bind(x => x.ItemModel.Title).To(x => Text = x);
            }

            public override void EndInit()
            {
                base.EndInit();
            }
        }
    }
}