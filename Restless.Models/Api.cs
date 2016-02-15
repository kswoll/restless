using System.Collections.Immutable;
using SexyReact;

namespace Restless.Models
{
    public class Api : ApiItem
    {
        public string Url { get; set; }
        public ApiMethod Method { get; set; }
        public RxList<ApiInput> Inputs { get; set; }
        public RxList<ApiOutput> Outputs { get; set; }
        public RxList<ApiHeader> Headers { get; set; }
        public string Body { get; set; }
    }
}