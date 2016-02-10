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
        public ApiItemModel SelectedItem { get; set; }
        public List<ApiMethod> Methods { get; }
        public List<ApiOutputType> OutputTypes { get; }
        public int SplitterPosition { get; set; }
        public int ApiSplitterPosition { get; set; }

        public IRxCommand DeleteSelectedItem { get; }
        public DbRepository Repository { get; }

        private readonly Func<SelectFileType, string, string, string> selectFile;

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
            Import = RxCommand.Create(OnImport);
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

            Repository = new DbRepository(() => new RestlessDb());

            Task.Run(async () =>
            {
                await Repository.Initialize();
                var apiItems = await Repository.GetApiItems();
                foreach (var apiItem in apiItems)
                {
                    if (apiItem is Api)
                        Items.Add(new ApiModel(this, null, (Api)apiItem));
                    else
                        Items.Add(new ApiCollectionModel(this, null, (ApiCollection)apiItem));
                }                
            });

            DeleteSelectedItem = RxCommand.CreateAsync(OnDeleteSelectedItem);
        }

        private async Task<ApiModel> OnAddApi(ApiCollectionModel parent)
        {
            var api = new Api
            {
                Title = "(New Api)",
                Method = ApiMethod.Get,
                Created = DateTime.UtcNow,
                Type = ApiItemType.Api
            };
            await Repository.InsertApiItem(api);

            var model = new ApiModel(this, parent, api);
            Items.Add(model);
            return model;
        }

        private async Task<ApiCollectionModel> OnAddApiCollection(ApiCollectionModel parent)
        {
            var apiCollection = new ApiCollection
            {
                Title = "(New Api Collection)",
                Created = DateTime.UtcNow,
                Type = ApiItemType.Collection
            };
            await Repository.InsertApiItem(apiCollection);

            var model = new ApiCollectionModel(this, parent, apiCollection);

            Items.Add(model);
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

        private void OnImport()
        {
            var file = selectFile(SelectFileType.Open, JsonExtension, "Choose a file to import");
            if (file == null)
                return;

            var json = File.ReadAllText(file);
            var data = JsonConvert.DeserializeObject<ApiItem[]>(json, importExportJsonSettings);
            Items.AddRange(data.Select(x => ApiItemModel.Import(this, null, x)));
        }

        private async Task OnDeleteSelectedItem()
        {
            await SelectedItem.Delete();
            SelectedItem = null;
        }
    }
}