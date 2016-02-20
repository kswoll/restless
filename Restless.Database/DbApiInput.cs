using System.ComponentModel.DataAnnotations.Schema;
using Restless.Models;

namespace Restless.Database
{
    [Table("ApiInput")]
    public class DbApiInput : DbObject
    {
        public int ApiId { get; set; }
        public ApiInputType InputType { get; set; }
        public string Name { get; set; }
        public string DefaultValue { get; set; }
        
        public DbApiItem Api { get; set; }
    }
}