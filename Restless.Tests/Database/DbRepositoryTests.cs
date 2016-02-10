using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using NUnit.Framework;
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

        [Test]
        public async Task InsertMinimalApiCollection()
        {
            var collection = new ApiCollection
            {
                Title = "test title",
                Type = ApiItemType.Collection,
                Created = new DateTime(2015, 1, 1)
            };
            await repository.InsertApiItem(collection);

            Assert.AreNotEqual(0, collection.Id);

            var dbApiItem = await db.ApiItems.SingleAsync(x => x.Id == collection.Id);
            Assert.AreEqual(collection.Title, dbApiItem.Title);
            Assert.AreEqual(collection.Type, dbApiItem.Type);
            Assert.AreEqual(collection.Created, dbApiItem.Created);
        }

        [Test]
        public async Task InsertApiWithHeader()
        {
            var api = new Api
            {
                Headers = new List<ApiHeader> { new ApiHeader { Name = "test name", Value = "test value" } }
            };

            await repository.InsertApiItem(api);

            var dbApiItem = await db.ApiItems.Include(x => x.RequestHeaders).SingleAsync(x => x.Id == api.Id);
            Assert.AreNotEqual(0, dbApiItem.RequestHeaders[0].Id);
        }

        [Test]
        public async Task UpdateApiWithHeader()
        {
            var api = new Api
            {
                Headers = new List<ApiHeader> { new ApiHeader { Name = "test name", Value = "test value" } }
            };

            await repository.InsertApiItem(api);

            api.Headers[0].Name = "test name2";

            await repository.UpdateApiItem(api);

            var dbApiItem = await db.ApiItems.Include(x => x.RequestHeaders).SingleAsync(x => x.Id == api.Id);
            Assert.AreNotEqual(0, dbApiItem.RequestHeaders[0].Id);
            Assert.AreEqual(1, dbApiItem.RequestHeaders.Count);
            Assert.AreEqual(api.Headers[0].Name, dbApiItem.RequestHeaders[0].Name);
        }
    }
}