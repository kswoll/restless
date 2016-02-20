using System.ComponentModel.DataAnnotations.Schema;

namespace Restless.Database
{
    [Table("ApiResponseVisualizer")]
    public class DbApiResponseComplication : DbObject
    {
        public int ApiId { get; set; }
        public int Priority { get; set; }
        public string ComplicationClass { get; set; }
        public string ComplicationData { get; set; }

        public DbApiItem Api { get; set; }
    }
}