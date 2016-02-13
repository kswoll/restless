using System.Collections.Immutable;

namespace Restless.Models
{
    public class ApiCollection : ApiItem
    {
        public ImmutableList<ApiItem> Items { get; set; }
    }
}