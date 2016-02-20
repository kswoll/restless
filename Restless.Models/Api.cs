using System;
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

        public static Api Create()
        {
            return new Api
            {
                Title = "(New Api)",
                Method = ApiMethod.Get,
                Created = DateTime.UtcNow,
                Type = ApiItemType.Api,
                Headers = new RxList<ApiHeader>(),
                Inputs = new RxList<ApiInput>(),
                Outputs = new RxList<ApiOutput>()
            };
        }
    }
}