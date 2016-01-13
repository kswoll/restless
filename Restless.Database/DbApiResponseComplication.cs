using System.ComponentModel.DataAnnotations.Schema;

namespace Restless.Database
{
    [Table("ApiResponseVisualizer")]
    public class DbApiResponseComplication
    {
        public int Id { get; set; }
        public int ApiId { get; set; }
        public int Priority { get; set; }
        public string ComplicationClass { get; set; }
        public string ComplicationData { get; set; }

        public DbApi Api { get; set; }
    }
}