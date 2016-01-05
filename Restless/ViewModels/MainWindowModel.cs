using System.Threading.Tasks;
using System.Windows.Threading;
using Microsoft.Data.Entity;
using Restless.Database;
using SexyReact;

namespace Restless.ViewModels
{
    public class MainWindowModel : BaseModel
    {
        public string Title { get; set; }
        public IRxFunction<ApiModel> AddApi { get; }
        public RxList<ApiModel> Items { get; }

        public MainWindowModel()
        {
            AddApi = RxFunction.CreateAsync(AddApiImpl);
            Title = "Restless";
            Items = new RxList<ApiModel>();

            Task.Run(async () =>
            {
                var db = new RestlessDb();
                var apis = await db.Apis.ToArrayAsync();
                foreach (var api in apis)
                {
                    Items.Add(new ApiModel(api));
                }
            });
        }

        private async Task<ApiModel> AddApiImpl()
        {
            var db = new RestlessDb();
            var dbApi = new DbApi
            {
                Title = "(New Api)",
                HttpMethod = "GET"
            };
            db.Apis.Add(dbApi);
            await db.SaveChangesAsync();

            var model = new ApiModel(dbApi);
            Items.Add(model);
            return model;
        }
    }
}