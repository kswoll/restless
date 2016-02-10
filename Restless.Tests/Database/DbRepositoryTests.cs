using System;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using NUnit.Framework;
using Restless.Database;
using Restless.Database.Repositories;
using Restless.Models;

namespace Restless.Tests.Database
{
    [TestFixture]
    public class DbRepositoryTests : DatabaseTestFixture
    {
        [Test]
        public async Task InsertMinimalApi()
        {
            var api = new Api
            {
                Title = "test title",
                Type = ApiItemType.Api,
                Url = "http://test.com",
                Body = "test body",
                Created = new DateTime(2015, 1, 1)
            };
            await repository.InsertApiItem(api);

            Assert.AreNotEqual(0, api.Id);

            var dbApiItem = await db.ApiItems.SingleAsync(x => x.Id == api.Id);
            Assert.AreEqual(api.Title, dbApiItem.Title);
            Assert.AreEqual(api.Type, dbApiItem.Type);
            Assert.AreEqual(api.Url, dbApiItem.Url);
            Assert.AreEqual(api.Body, dbApiItem.RequestBody);
            Assert.AreEqual(api.Created, dbApiItem.Created);
        }
    }
}