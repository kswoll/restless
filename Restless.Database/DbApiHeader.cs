using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restless.Database
{
    [Table("ApiHeader")]
    public class DbApiHeader
    {
        public int Id { get; set; } 
        public int ApiId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }

        public DbApi Api { get; set; }
    }
}