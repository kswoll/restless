using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Restless.Database;
using Restless.Models;
using Restless.Utils;

namespace Restless.ViewModels
{
    public abstract class ApiItemModel : BaseModel
    {
        public MainWindowModel MainWindow { get; }
        public int Id { get; set; }
        public string Title { get; set; }
        public ApiCollectionModel Parent { get; }

        public abstract ApiItemType Type { get; }
        public abstract ApiItem Export();
        protected abstract Task Delete(RestlessDb db);

        protected ApiItemModel(MainWindowModel mainWindow, ApiCollectionModel parent)
        {
            MainWindow = mainWindow;
            Parent = parent;
        }

        public async Task Delete()
        {
            var db = new RestlessDb();

            await Delete(db);
            await db.SaveChangesAsync();            

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