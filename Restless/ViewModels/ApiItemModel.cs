using System;
using System.Threading.Tasks;
using Restless.Models;
using SexyReact;

namespace Restless.ViewModels
{
    public abstract class ApiItemModel : BaseModel, IIdObject
    {
        public MainWindowModel MainWindow { get; }
        public ApiItem ItemModel { get; set; }
        public ApiCollectionModel Parent { get; }
        public RxList<ApiItemModel> Items { get; }
        public bool IsExpanded { get; set; }
        public bool IsSelected { get; set; }

        public abstract ApiItemType Type { get; }
        public abstract ApiItem Export();

        protected ApiItemModel(MainWindowModel mainWindow, ApiCollectionModel parent, ApiItem item)
        {
            MainWindow = mainWindow;
            Parent = parent;
            ItemModel = item;
            Items = new RxList<ApiItemModel>();

            this.ObservePropertyChange(x => x.IsSelected).Subscribe(x =>
            {
                if (x)
                {
                    var current = Parent;
                    while (current != null)
                    {
                        current.IsExpanded = true;
                        current = current.Parent;
                    }
                    MainWindow.SelectedItem = this;
                }
            });
        }

        int IIdObject.Id
        {
            get { return ItemModel.Id; }
            set { ItemModel.Id = value; }
        }

        public async Task Delete()
        {
            if (Parent == null)
            {
                await MainWindow.Repository.DeleteApiItem(ItemModel.Id);
                MainWindow.Items.Remove(this);
            }
            else
            {
                Parent.Items.Remove(this);
            }
        }

        public static ApiItemModel Import(MainWindowModel mainWindow, ApiCollectionModel parent, ApiItem item)
        {
            ApiItemModel result;
            switch (item.Type)
            {
                case ApiItemType.Api:
                    result = ApiModel.Import(mainWindow, parent, (Api)item);
                    break;
                case ApiItemType.Collection:
                    result = ApiCollectionModel.Import(mainWindow, parent, (ApiCollection)item);
                    break;
                default:
                    throw new Exception();
            }
            return result;
        }
    }
}