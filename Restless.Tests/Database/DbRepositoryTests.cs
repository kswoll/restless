using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
            await repository.AddItem(api);

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
            await repository.AddItem(collection);

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
                Headers = ImmutableList.Create(new ApiHeader { Name = "test name", Value = "test value" })
            };

            await repository.AddItem(api);

            var dbApiItem = await db.ApiItems.Include(x => x.Headers).SingleAsync(x => x.Id == api.Id);
            Assert.AreNotEqual(0, dbApiItem.Headers[0].Id);
        }

        [Test]
        public async Task UpdateApiWithHeader()
        {
            var api = new Api
            {
                Headers = ImmutableList.Create(new ApiHeader { Name = "test name", Value = "test value" })
            };

            await repository.AddItem(api);

            api.Headers[0].Name = "test name2";

            await repository.WaitForIdle();

            var dbApiItem = await db.ApiItems.Include(x => x.Headers).SingleAsync(x => x.Id == api.Id);
            Assert.AreNotEqual(0, dbApiItem.Headers[0].Id);
            Assert.AreEqual(1, dbApiItem.Headers.Count);
            Assert.AreEqual(api.Headers[0].Name, dbApiItem.Headers[0].Name);
        }

        [Test]
        public async Task UpdateApiAddHeader()
        {
            var api = new Api();
            await repository.AddItem(api);

            api.Headers = ImmutableList.Create(new ApiHeader { Name = "test name", Value = "test value" });

            await repository.WaitForIdle();

            var dbApiItem = await db.ApiItems.Include(x => x.Headers).SingleAsync(x => x.Id == api.Id);
            Assert.AreNotEqual(0, dbApiItem.Headers[0].Id);
            Assert.AreEqual(1, dbApiItem.Headers.Count);
            Assert.AreEqual(api.Headers[0].Name, dbApiItem.Headers[0].Name);            
        }

        [Test]
        public async Task InsertApiChild()
        {
            var collection = new ApiCollection
            {
                Items = ImmutableList<ApiItem>.Empty
            };
            var child = new Api();
            collection.Items.Add(child);
            await repository.AddItem(collection);

            var dbApiItem = await db.ApiItems.Include(x => x.Items).SingleAsync(x => x.Id == collection.Id);
//            Assert
        }
    }
}