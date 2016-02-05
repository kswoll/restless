using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Restless.Database;
using Restless.Models;
using Restless.Properties;
using SexyReact;

namespace Restless.ViewModels
{
    public class MainWindowModel : BaseModel
    {
        public string Title { get; set; }
        public IRxFunction<ApiModel> AddApi { get; }
        public IRxCommand ExportAll { get; }
        public RxList<ApiModel> Items { get; }
        public ApiModel SelectedItem { get; set; }
        public List<ApiMethod> Methods { get; }
        public List<ApiOutputType> OutputTypes { get; }
        public int SplitterPosition { get; set; }
        public int ApiSplitterPosition { get; set; }

        public IRxCommand DeleteSelectedItem { get; }

        private Func<string> selectFile;

        private static readonly ApiMethod[] httpMethods = { ApiMethod.Get, ApiMethod.Post, ApiMethod.Put, ApiMethod.Delete };
        private static readonly ApiOutputType[] outputTypes = { ApiOutputType.Default, ApiOutputType.JsonPath };

        public MainWindowModel(Func<string> selectFile)
        {
            this.selectFile = selectFile;

            AddApi = RxFunction.CreateAsync(OnAddApi);
            ExportAll = RxCommand.Create(OnExportAll);
            Title = "Restless";
            Items = new RxList<ApiModel>();
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

            Task.Run(async () =>
            {
                var db = new RestlessDb();
                db.Database.Migrate();
                var apis = await db.Apis
                    .Include(x => x.RequestHeaders)
                    .Include(x => x.Inputs)
                    .Include(x => x.Outputs)
                    .ToArrayAsync();
                foreach (var api in apis)
                {
                    Items.Add(new ApiModel(this, api));
                }
            });

            DeleteSelectedItem = RxCommand.CreateAsync(OnDeleteSelectedItem);
        }

        private async Task<ApiModel> OnAddApi()
        {
            var db = new RestlessDb();
            var dbApi = new DbApi
            {
                Title = "(New Api)",
                Method = ApiMethod.Get
            };
            db.Apis.Add(dbApi);
            await db.SaveChangesAsync();

            var model = new ApiModel(this, dbApi);
            Items.Add(model);
            return model;
        }

        private void OnExportAll()
        {
            var destination = selectFile();
            if (destination == null)
                return;

            var json = JArray.FromObject(Items
                .Select(x => new
                {
                    x.Title,
                    x.Url,
                    Method = x.Method.ToString(),
                    Inputs = x.Inputs.Select(y => new
                    {
                        y.Name,
                        InputType = y.InputType.ToString(),
                        y.DefaultValue
                    }).ToArray(),
                    Outputs = x.Outputs.Select(y => new
                    {
                        y.Name,
                        y.Expression,
                        Type = y.Type.ToString()
                    }).ToArray(),
                    Headers = x.Headers.Select(y => new
                    {
                        y.Name,
                        y.Value
                    }).ToArray(),
                    Body = x.Body == null ? null : Convert.ToBase64String(x.Body)
                }));
            File.WriteAllText(destination, json.ToString());
        }

        private async Task OnDeleteSelectedItem()
        {
            var db = new RestlessDb();
            var dbApi = await db.Apis.SingleAsync(x => x.Id == SelectedItem.Id);
            db.Apis.Remove(dbApi);
            await db.SaveChangesAsync();

            var selectedItemIndex = Items.IndexOf(SelectedItem);
            Items.RemoveAt(selectedItemIndex);
            SelectedItem = null;
        }
    }
}