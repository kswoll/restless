using System;
using System.Linq;
using Restless.Models;
using SexyReact;

namespace Restless.ViewModels
{
    public class ApiCollectionModel : ApiItemModel
    {
        public DateTime Created { get; set; }         
        public RxList<ApiItemModel> Items { get; }
        public override ApiItemType Type => ApiItemType.Collection;

        public ApiCollectionModel(MainWindowModel mainWindow, ApiCollectionModel parent, ApiCollection apiCollection) : base(mainWindow, parent)
        {
            Items = new RxList<ApiItemModel>();

            Id = apiCollection.Id;
            Title = apiCollection.Title;
            Created = apiCollection.Created;

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
                Title = Title,
                Created = Created,
                Items = Items.Select(x => x.Export()).ToList()
            };
        }

        public static ApiCollectionModel Import(MainWindowModel mainWindow, ApiCollectionModel parent, ApiCollection apiCollection)
        {
            return new ApiCollectionModel(mainWindow, parent, apiCollection);
        }
    }
}