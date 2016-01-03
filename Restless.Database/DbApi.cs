using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Restless.Database
{
    [Table("Api")]
    public class DbApi
    {
        public int Id { get; set; }

        [Required]
        public string HttpMethod { get; set; }

        [Required]
        public string Url { get; set; }

        public byte[] Body { get; set; }
        public string ResponseVisualizer { get; set; }

        public List<DbApiHeader> Headers { get; set; }
    }
}