using System;
using System.Threading.Tasks;
using Restless.Models;

namespace Restless.ViewModels
{
    public abstract class ApiItemModel : BaseModel
    {
        public MainWindowModel MainWindow { get; }
        public ApiItem ItemModel { get; set; }
        public ApiCollectionModel Parent { get; }

        public abstract ApiItemType Type { get; }
        public abstract ApiItem Export();

        protected ApiItemModel(MainWindowModel mainWindow, ApiCollectionModel parent, ApiItem item)
        {
            MainWindow = mainWindow;
            Parent = parent;
            ItemModel = item;
        }

        public async Task Delete()
        {
            await MainWindow.Repository.DeleteApiItem(ItemModel.Id);

            var items = (Parent?.Items ?? MainWindow.Items);
            var selectedItemIndex = items.IndexOf(this);
            items.RemoveAt(selectedItemIndex);
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