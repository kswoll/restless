using System;

namespace Restless.Database
{
    public class DbApiCollection
    {
        public int Id { get; set; } 

        public int? CollectionId { get; set; }
        public string Name { get; set; }
        public DateTime Created { get; set; }

        public DbApiCollection Collection { get; set; }
    }
}