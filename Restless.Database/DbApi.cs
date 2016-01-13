using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Restless.Models;

namespace Restless.Database
{
    [Table("Api")]
    public class DbApi
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public ApiMethod Method { get; set; }

        public string Url { get; set; }

        public byte[] RequestBody { get; set; }

        public List<DbApiHeader> RequestHeaders { get; set; }
        public List<DbApiResponseComplication> ResponseComplications { get; set; }
    }
}