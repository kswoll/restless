using SexyReact;

namespace Restless.ViewModels
{
    public class ApiModel : BaseModel
    {
        public string Title { get; set; }
        public string Url { get; set; }

        public ApiModel()
        {
            Title = "(New Api)";
        }
    }
}