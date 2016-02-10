using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Restless.Database;
using Restless.Database.Repositories;

namespace Restless.Tests.Database
{
    public class DatabaseTestFixture
    {
        protected readonly RestlessDb db;
        protected readonly DbRepository repository;

        private readonly string databaseFile;

        public DatabaseTestFixture()
        {
            databaseFile = Path.GetTempFileName();
            db = new RestlessDb(databaseFile);
            repository = new DbRepository(() => new RestlessDb(databaseFile));
        }

        [SetUp]
        public async Task SetUp()
        {
            await repository.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(databaseFile);
        }
    }
}