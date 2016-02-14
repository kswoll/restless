using System.Collections.Immutable;
using System.Linq;
using Restless.Models;
using SexyReact;

namespace Restless.ViewModels
{
    public class ApiCollectionModel : ApiItemModel
    {
        public ApiCollection Model { get; }

        public RxList<ApiItemModel> Items { get; }
        public override ApiItemType Type => ApiItemType.Collection;

        public ApiCollectionModel(MainWindowModel mainWindow, ApiCollectionModel parent, ApiCollection apiCollection) : base(mainWindow, parent, apiCollection)
        {
            Model = apiCollection;
            Items = new RxList<ApiItemModel>();

            if (apiCollection.Items != null)
                Items.AddRange(apiCollection.Items.Select(x => x.Type == ApiItemType.Collection ? 
                    (ApiItemModel)new ApiCollectionModel(mainWindow, this, (ApiCollection)x) :
                    new ApiModel(mainWindow, this, (Api)x)));
        }

        public override ApiItem Export()
        {
            return new ApiCollection
            {
                Type = Type,
                Title = Model.Title,
                Created = Model.Created,
                Items = Items.Select(x => x.Export()).ToImmutableList()
            };
        }

        public static ApiCollectionModel Import(MainWindowModel mainWindow, ApiCollectionModel parent, ApiCollection apiCollection)
        {
            return new ApiCollectionModel(mainWindow, parent, apiCollection);
        }
    }
}