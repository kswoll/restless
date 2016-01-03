using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restless.Database
{
    [Table("ApiCallRequestHeader")]
    public class DbApiCallRequestHeader
    {
        public int Id { get; set; }
        public int ApiCallId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Value { get; set; }

        public DbApi Api { get; set; }
    }
}