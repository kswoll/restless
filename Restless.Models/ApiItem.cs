using System;

namespace Restless.Models
{
    public abstract class ApiItem : IdObject
    {
        public DateTime Created { get; set; }
        public string Title { get; set; }
        public ApiItemType Type { get; set; }
    }
}