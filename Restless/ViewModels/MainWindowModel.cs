using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
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
        public RxList<ApiModel> Items { get; }
        public ApiModel SelectedItem { get; set; }
        public List<ApiMethod> Methods { get; }
        public List<ApiOutputType> OutputTypes { get; }
        public int SplitterPosition { get; set; }

        public IRxCommand DeleteSelectedItem { get; }

        private static readonly ApiMethod[] httpMethods = { ApiMethod.Get, ApiMethod.Post, ApiMethod.Put, ApiMethod.Delete };
        private static readonly ApiOutputType[] outputTypes = { ApiOutputType.Default, ApiOutputType.JsonPath };

        public MainWindowModel()
        {
            AddApi = RxFunction.CreateAsync(OnAddApi);
            Title = "Restless";
            Items = new RxList<ApiModel>();
            Methods = httpMethods.ToList();
            OutputTypes = outputTypes.ToList();

            SplitterPosition = Settings.Default.MainWindowSplitterPosition;
            this.ObservePropertyChange(x => x.SplitterPosition).Throttle(TimeSpan.FromSeconds(1)).Subscribe(x =>
            {
                Settings.Default.MainWindowSplitterPosition = x;
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