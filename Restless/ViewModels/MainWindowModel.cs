using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Restless.Database;
using Restless.Models;
using SexyReact;

namespace Restless.ViewModels
{
    public class MainWindowModel : BaseModel
    {
        public string Title { get; set; }
        public IRxFunction<ApiModel> AddApi { get; }
        public RxList<ApiModel> Items { get; }
        public ApiModel SelectedItem { get; set; }

        public IRxCommand DeleteSelectedItem { get; }

        public MainWindowModel()
        {
            AddApi = RxFunction.CreateAsync(OnAddApi);
            Title = "Restless";
            Items = new RxList<ApiModel>();

            Task.Run(async () =>
            {
                var db = new RestlessDb();
                var apis = await db.Apis
                    .Include(x => x.RequestHeaders)
                    .Include(x => x.Inputs)
                    .ToArrayAsync();
                foreach (var api in apis)
                {
                    Items.Add(new ApiModel(api));
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

            var model = new ApiModel(dbApi);
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