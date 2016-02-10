using System.Collections.Generic;

namespace Restless.Models
{
    public class ApiCollection : ApiItem
    {
        public List<ApiItem> Items { get; set; }
    }
}