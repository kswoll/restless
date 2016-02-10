using System.Collections.Generic;

namespace Restless.Models
{
    public class Api : ApiItem
    {
        public string Url { get; set; }
        public ApiMethod Method { get; set; }
        public List<ApiInput> Inputs { get; set; }
        public List<ApiOutput> Outputs { get; set; }
        public List<ApiHeader> Headers { get; set; }
        public string Body { get; set; }
    }
}