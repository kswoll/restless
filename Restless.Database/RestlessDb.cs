using System.Configuration;
using System.IO;
using System.Linq;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using SexyReact;

namespace Restless.Database
{
    public class RestlessDb : DbContext
    {
        public DbSet<DbApiItem> ApiItems { get; set; }
        public DbSet<DbApiHeader> ApiHeaders { get; set; }
        public DbSet<DbApiCall> ApiCalls { get; set; }

        private readonly string configuredDatabaseFile = ConfigurationManager.AppSettings["Database"];
        private readonly string databaseFile;

        public RestlessDb()
        {
            databaseFile = configuredDatabaseFile;
        }

        public RestlessDb(string databaseFile)
        {
            this.databaseFile = databaseFile;
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

            foreach (var type in typeof(DbObject).Assembly.GetTypes().Where(x => typeof(DbObject).IsAssignableFrom(x) && x != typeof(DbObject)))
            {
                modelBuilder.Entity(type).Ignore(nameof(RxObject.Changed));
                modelBuilder.Entity(type).Ignore(nameof(RxObject.Changing));
            }

//            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
//            {
//                relationship.DeleteBehavior = DeleteBehavior.Restrict;
//            }
        }
    }
}