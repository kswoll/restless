using Restless.Database;

namespace Restless.ViewModels
{
    public class ApiModel : BaseModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }

        public ApiModel(DbApi dbApi)
        {
            Id = dbApi.Id;
            Title = dbApi.Title;
        }
    }
}