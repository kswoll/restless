using SexyReact;

namespace Restless.ViewModels
{
    public class MainWindowModel : BaseModel
    {
        public string Title { get; set; }
        public IRxCommand AddApi { get; }
        public RxList<ApiModel> Items { get; }

        public MainWindowModel()
        {
            AddApi = RxCommand.Create(AddApiImpl);
            Title = "Restless";
            Items = new RxList<ApiModel>();
        }

        private void AddApiImpl()
        {
            Items.Add(new ApiModel());
        }
    }
}