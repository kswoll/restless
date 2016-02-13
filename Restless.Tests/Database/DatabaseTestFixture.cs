using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Restless.Database;
using Restless.Database.Repositories;

namespace Restless.Tests.Database
{
    public class DatabaseTestFixture
    {
        protected RestlessDb db;
        protected DbRepository repository;

        private string databaseFile;

        [SetUp]
        public async Task SetUp()
        {
            databaseFile = Path.GetTempFileName();
            db = new RestlessDb(databaseFile);
            repository = new DbRepository(db);
            await repository.Initialize();
        }

        [TearDown]
        public void TearDown()
        {
            File.Delete(databaseFile);
        }
    }
}