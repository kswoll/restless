using System.Collections.Immutable;
using SexyReact;

namespace Restless.Models
{
    public class ApiCollection : ApiItem
    {
        public RxList<ApiItem> Items { get; set; }
    }
}