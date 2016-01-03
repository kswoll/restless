using SexyReact;

namespace Restless.ViewModels
{
    public class MainWindowModel : RxObject
    {
        public string Title { get; set; }
        public IRxCommand AddApi { get; }
        public RxList<ApiItemModel> Items { get; }

        public MainWindowModel()
        {
            AddApi = RxCommand.Create(AddApiImpl);
            Title = "Restless";
            Items = new RxList<ApiItemModel>();
        }

        private void AddApiImpl()
        {
            Items.Add(new ApiItemModel());
        }
    }
}