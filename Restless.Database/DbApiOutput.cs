using System.ComponentModel.DataAnnotations.Schema;
using Restless.Models;

namespace Restless.Database
{
    [Table("ApiOutput")]
    public class DbApiOutput : IIdObject
    {
        public int Id { get; set; }
        public int ApiId { get; set; }
        public string Name { get; set; }
        public string Expression { get; set; }

        public DbApi Api { get; set; }
    }
}