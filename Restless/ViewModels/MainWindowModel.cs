using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Restless.Database;
using Restless.Database.Repositories;
using Restless.Models;
using Restless.Properties;
using Restless.Utils;
using SexyReact;

namespace Restless.ViewModels
{
    public class MainWindowModel : BaseModel
    {
        public string Title { get; set; }
        public IRxFunction<ApiModel> AddApi { get; }
        public IRxFunction<ApiCollectionModel> AddApiCollection { get; }
        public IRxFunction<ApiModel> AddChildApi { get; }
        public IRxFunction<ApiCollectionModel> AddChildApiCollection { get; }
        public IRxCommand Export { get; }
        public IRxCommand Import { get; }
        public RxList<ApiItemModel> Items { get; }
        public List<ApiMethod> Methods { get; }
        public List<ApiOutputType> OutputTypes { get; }
        public int SplitterPosition { get; set; }
        public int ApiSplitterPosition { get; set; }

        public IRxCommand DeleteSelectedItem { get; }
        public DbRepository Repository { get; }

        private readonly Func<SelectFileType, string, string, string> selectFile;
        private ApiItemModel selectedItem;

        private static readonly ApiMethod[] httpMethods = { ApiMethod.Get, ApiMethod.Post, ApiMethod.Put, ApiMethod.Delete };
        private static readonly ApiOutputType[] outputTypes = { ApiOutputType.Default, ApiOutputType.JsonPath };

        public MainWindowModel(Func<SelectFileType, string, string, string> selectFile)
        {
            this.selectFile = selectFile;

            var canAddChild = this.ObserveProperty(x => x.SelectedItem).Select(x => x is ApiCollectionModel);

            AddApi = RxFunction.CreateAsync(async () => await OnAddApi(null));
            AddApiCollection = RxFunction.CreateAsync(async () => await OnAddApiCollection(null));
            AddChildApi = RxFunction.CreateAsync(async () => await OnAddApi((ApiCollectionModel)SelectedItem), canAddChild);
            AddChildApiCollection = RxFunction.CreateAsync(async () => await OnAddApiCollection((ApiCollectionModel)SelectedItem), canAddChild);
            Export = RxCommand.Create(OnExport);
            Import = RxCommand.CreateAsync(OnImport);
            Title = "Restless";
            Items = new RxList<ApiItemModel>();
            Methods = httpMethods.ToList();
            OutputTypes = outputTypes.ToList();

            SplitterPosition = Settings.Default.MainWindowSplitterPosition;
            ApiSplitterPosition = Settings.Default.ApiPanelSplitterPosition;
            this.ObservePropertyChange(x => x.SplitterPosition).Throttle(TimeSpan.FromSeconds(1)).Subscribe(x =>
            {
                Settings.Default.MainWindowSplitterPosition = x;
                Settings.Default.Save();
            });
            this.ObservePropertyChange(x => x.ApiSplitterPosition).Throttle(TimeSpan.FromSeconds(1)).Subscribe(x =>
            {
                Settings.Default.ApiPanelSplitterPosition = x;
                Settings.Default.Save();
            });

            Repository = new DbRepository(new RestlessDb());

            Task.Run(async () =>
            {
                await Repository.Initialize();
                await Repository.Load();
                foreach (var apiItem in Repository.Items)
                {
                    if (apiItem is Api)
                        Items.Add(new ApiModel(this, null, (Api)apiItem));
                    else
                        Items.Add(new ApiCollectionModel(this, null, (ApiCollection)apiItem));
                }                
            });

            DeleteSelectedItem = RxCommand.CreateAsync(OnDeleteSelectedItem);
        }

        public ApiItemModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                selectedItem?.NotifySelectedChanged(false);
                selectedItem = value;
                OnChanged(GetType().GetProperty("SelectedItem"), value);
                selectedItem?.NotifySelectedChanged(true);
            }
        }

        private async Task<ApiModel> OnAddApi(ApiCollectionModel parent)
        {
            var api = Api.Create();
            if (parent == null)
                await Repository.AddItem(api);

            var model = new ApiModel(this, parent, api);
            if (parent == null)
                Items.Add(model);
            else
                parent.Items.Add(model);

            SelectedItem = model;

            return model;
        }

        private async Task<ApiCollectionModel> OnAddApiCollection(ApiCollectionModel parent)
        {
            var apiCollection = ApiCollection.Create();
            if (parent == null)
                await Repository.AddItem(apiCollection);

            var model = new ApiCollectionModel(this, parent, apiCollection);

            if (parent == null)
                Items.Add(model);
            else
                parent.Items.Add(model);

            SelectedItem = model;

            return model;
        }

        private static readonly JsonSerializerSettings importExportJsonSettings = new JsonSerializerSettings
        {
            Converters = { new ApiItemJsonConverter(), new StringEnumConverter() },
            Formatting = Formatting.Indented
        };

        private const string JsonExtension = "Json Files (*.json)|*.json";

        private void OnExport()
        {
            var destination = selectFile(SelectFileType.Save, JsonExtension, "Provide a file to which your apis will be exported");
            if (destination == null)
                return;

            var json = JsonConvert.SerializeObject(Items.Select(x => x.Export()), importExportJsonSettings);
            File.WriteAllText(destination, json);
        }

        private async Task OnImport()
        {
            var file = selectFile(SelectFileType.Open, JsonExtension, "Choose a file to import");
            if (file == null)
                return;

            var json = File.ReadAllText(file);
            var data = JsonConvert.DeserializeObject<ApiItem[]>(json, importExportJsonSettings);
            foreach (var item in data.Select(x => ApiItemModel.Import(this, null, x)))
            {
                await Repository.AddItem(item.ItemModel);
            }
        }

        private async Task OnDeleteSelectedItem()
        {
            await SelectedItem.Delete();
            SelectedItem = null;
        }
    }
}