using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Restless.Models;

namespace Restless.Database.Repositories
{
    public class DbRepository
    {
        private readonly Func<RestlessDb> db;

        public DbRepository(Func<RestlessDb> db)
        {
            this.db = db;
        }

        public async Task Initialize()
        {
            using (var db = this.db())
            {
                await db.Database.MigrateAsync();
            }
        }

        public async Task<ApiItem[]> GetApiItems()
        {
            return await GetApiItems(db => db.ApiItems);
        }

        public async Task DeleteApiItem(int id)
        {
            using (var db = this.db())
            {
                var dbApi = await db.ApiItems.SingleAsync(x => x.Id == id);
                db.ApiItems.Remove(dbApi);
            }
        }

        public async Task InsertApiItem(ApiItem item)
        {
            var dbApiItem = new DbApiItem();
            MapToDb(item, dbApiItem, null);

            using (var db = this.db())
            {
                db.ApiItems.Add(dbApiItem);
                await db.SaveChangesAsync();
                item.Id = dbApiItem.Id;
            }
        }

        public async Task UpdateApiItem(ApiItem item)
        {
            
        }

        private async Task<ApiItem[]> GetApiItems(Func<RestlessDb, IQueryable<DbApiItem>> query)
        {
            using (var db = this.db())
            {
                var dbApiItems = await query(db)
                    .Include(x => x.RequestHeaders)
                    .Include(x => x.Inputs)
                    .Include(x => x.Outputs)
                    .Include(x => x.Outputs)
                    .ToArrayAsync();
                var dbApisItemsByParentId = dbApiItems.ToLookup(x => x.CollectionId ?? 0);

                var roots = new List<ApiItem>();
                roots.AddRange(MapFromDb(dbApisItemsByParentId[0], dbApisItemsByParentId));
                return roots.ToArray();
            }
        }

        private IEnumerable<ApiItem> MapFromDb(IEnumerable<DbApiItem> dbApis, ILookup<int, DbApiItem> dbApiItemsByParentId)
        {
            foreach (var dbApiItem in dbApis)
            {
                ApiItem apiItem;
                if (dbApiItem.Type == ApiItemType.Api)
                {
                    apiItem = new Api
                    {
                        Type = ApiItemType.Api,
                        Url = dbApiItem.Url,
                        Method = dbApiItem.Method,
                        Inputs = dbApiItem.Inputs.Select(y => new ApiInput
                        {
                            Id = y.Id,
                            Name = y.Name,
                            DefaultValue = y.DefaultValue,
                            InputType = y.InputType
                        }).ToList(),
                        Outputs = dbApiItem.Outputs.Select(y => new ApiOutput
                        {
                            Id = y.Id,
                            Name = y.Name,
                            Type = y.Type,
                            Expression = y.Expression
                        }).ToList(),
                        Headers = dbApiItem.RequestHeaders.Select(y => new ApiHeader
                        {
                            Id = y.Id,
                            Name = y.Name,
                            Value = y.Value
                        }).ToList(),
                        Body = dbApiItem.RequestBody
                    };
                }
                else
                {
                    apiItem = new ApiCollection
                    {
                        Type = ApiItemType.Collection,
                        Items = MapFromDb(dbApiItemsByParentId[dbApiItem.Id], dbApiItemsByParentId).ToList()
                    };
                }
                apiItem.Id = dbApiItem.Id;
                apiItem.Created = dbApiItem.Created;
                apiItem.Title = dbApiItem.Title;

                yield return apiItem;
            }
        }

        private void MapToDb(ApiItem apiItem, DbApiItem dbApiItem, ILookup<int, DbApiItem> apiItemsByParentId)
        {
            var api = apiItem as Api;
            if (api != null)
            {
                dbApiItem.Type = ApiItemType.Api;
                dbApiItem.Url = api.Url;
                dbApiItem.Method = api.Method;
                dbApiItem.Inputs = api.Inputs?.Select(y => new DbApiInput
                {
                    Id = y.Id,
                    Name = y.Name,
                    DefaultValue = y.DefaultValue,
                    InputType = y.InputType
                }).ToList();
                dbApiItem.Outputs = api.Outputs?.Select(y => new DbApiOutput
                {
                    Id = y.Id,
                    Name = y.Name,
                    Type = y.Type,
                    Expression = y.Expression
                }).ToList();
                dbApiItem.RequestHeaders = api.Headers?.Select(y => new DbApiHeader
                {
                    Id = y.Id,
                    Name = y.Name,
                    Value = y.Value
                }).ToList();
                dbApiItem.RequestBody = api.Body;
            }
            else
            {
                var apiCollection = (ApiCollection)apiItem;
                dbApiItem.Type = ApiItemType.Collection;
//                dbApiItem.Items = apiCollection.Items.Select(x => );
            }
            dbApiItem.Id = apiItem.Id;
            dbApiItem.Created = apiItem.Created;
            dbApiItem.Title = apiItem.Title;
        }

//        private async Task MapChildren()
    }
}