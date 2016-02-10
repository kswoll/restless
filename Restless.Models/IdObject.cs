using Newtonsoft.Json;

namespace Restless.Models
{
    public class IdObject : IIdObject
    {
        [JsonIgnore]
        public int Id { get; set; }
    }
}