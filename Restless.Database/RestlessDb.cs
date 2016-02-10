using System.Configuration;
using System.IO;
using System.Linq;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;

namespace Restless.Database
{
    public class RestlessDb : DbContext
    {
        public DbSet<DbApiItem> ApiItems { get; set; }
        public DbSet<DbApiHeader> ApiHeaders { get; set; }
        public DbSet<DbApiCall> ApiCalls { get; set; }

        private readonly string configuredDatabaseFile = ConfigurationManager.AppSettings["Database"];
        private readonly string databaseFile;

        public RestlessDb(string databaseFile = null)
        {
            this.databaseFile = databaseFile ??configuredDatabaseFile;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlite($"Data Source={databaseFile};");
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

//            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
//            {
//                relationship.DeleteBehavior = DeleteBehavior.Restrict;
//            }
        }
    }
}