using SexyReact;

namespace Restless.ViewModels
{
    public class ApiItemModel : RxObject
    {
        public string Title { get; set; }

        public ApiItemModel()
        {
            Title = "(New Api)";
        }
    }
}