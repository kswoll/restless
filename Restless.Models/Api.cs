using System.Collections.Immutable;

namespace Restless.Models
{
    public class Api : ApiItem
    {
        public string Url { get; set; }
        public ApiMethod Method { get; set; }
        public ImmutableList<ApiInput> Inputs { get; set; }
        public ImmutableList<ApiOutput> Outputs { get; set; }
        public ImmutableList<ApiHeader> Headers { get; set; }
        public string Body { get; set; }
    }
}