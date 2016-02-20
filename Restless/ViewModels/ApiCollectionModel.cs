using System.Linq;
using Restless.Models;
using Restless.Utils;
using SexyReact;

namespace Restless.ViewModels
{
    public class ApiCollectionModel : ApiItemModel
    {
        public ApiCollection Model { get; }

        public override ApiItemType Type => ApiItemType.Collection;

        public ApiCollectionModel(MainWindowModel mainWindow, ApiCollectionModel parent, ApiCollection apiCollection) : base(mainWindow, parent, apiCollection)
        {
            Model = apiCollection;

            if (apiCollection.Items != null)
                Items.AddRange(apiCollection.Items.Select(x => x.Type == ApiItemType.Collection ? 
                    (ApiItemModel)new ApiCollectionModel(mainWindow, this, (ApiCollection)x) :
                    new ApiModel(mainWindow, this, (Api)x)));

            Items.SetUpSync(
                MainWindow.Repository,
                Model.Items,
                x => x.ItemModel);
        }

        public override ApiItem Export()
        {
            return new ApiCollection
            {
                Type = Type,
                Title = Model.Title,
                Created = Model.Created,
                Items = Items.Select(x => x.Export()).ToRxList()
            };
        }

        public static ApiCollectionModel Import(MainWindowModel mainWindow, ApiCollectionModel parent, ApiCollection apiCollection)
        {
            return new ApiCollectionModel(mainWindow, parent, apiCollection);
        }
    }
}