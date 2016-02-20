using System;
using System.Collections.Immutable;
using SexyReact;

namespace Restless.Models
{
    public class ApiCollection : ApiItem
    {
        public RxList<ApiItem> Items { get; set; }

        public ApiCollection()
        {
        }

        public static ApiCollection Create()
        {
            return new ApiCollection
            {
                Title = "(New Api Collection)",
                Created = DateTime.UtcNow,
                Type = ApiItemType.Collection,
                Items = new RxList<ApiItem>()
            };
        }
    }
}