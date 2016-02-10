using Newtonsoft.Json;
using SexyReact;

namespace Restless.Models
{
    [Rx]
    public class IdObject : RxObject, IIdObject
    {
        [JsonIgnore]
        public int Id { get; set; }
    }
}