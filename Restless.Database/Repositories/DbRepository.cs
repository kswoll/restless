using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Restless.Models;

namespace Restless.Database.Repositories
{
    public static class DbRepository
    {
        public static async Task Initialize()
        {
            using (var db = new RestlessDb())
            {
                await db.Database.MigrateAsync();
            }            
        }

        public static async Task<ApiItem[]> GetApiItems()
        {
            return await GetApiItems(db => db.ApiItems);
        }

        public static async Task<ApiItem> GetApiItem(int id)
        {
            return (await GetApiItems(db => db.ApiItems.Where(x => x.Id == id))).Single();
        }

        public static async Task Delete(int id)
        {
            using (var db = new RestlessDb())
            {
                var dbApi = await db.ApiItems.SingleAsync(x => x.Id == id);
                db.ApiItems.Remove(dbApi);                            
            }
        }

        private static async Task<ApiItem[]> GetApiItems(Func<RestlessDb, IQueryable<DbApiItem>> query)
        {
            using (var db = new RestlessDb())
            {
                var dbApiItems = await query(db)
                    .Include(x => x.RequestHeaders)
                    .Include(x => x.Inputs)
                    .Include(x => x.Outputs)
                    .ToArrayAsync();
                var dbApisItemsByParentId = dbApiItems.ToLookup(x => x.CollectionId ?? 0);

                var roots = new List<ApiItem>();
                roots.AddRange(Map(dbApisItemsByParentId[0], dbApisItemsByParentId));
                return roots.ToArray();
            }
        }

        private static IEnumerable<ApiItem> Map(IEnumerable<DbApiItem> dbApis, ILookup<int, DbApiItem> dbApiItemsByParentId)
        {
            foreach (var dbApi in dbApis)
            {
                ApiItem apiItem;
                if (dbApi.Type == ApiItemType.Api)
                {
                    Api api;
                    apiItem = api = new Api
                    {
                        Type = ApiItemType.Api,
                        Url = dbApi.Url,
                        Method = dbApi.Method,
                        Inputs = dbApi.Inputs.Select(y => new ApiInput
                        {
                            Id = y.Id,
                            Name = y.Name,
                            DefaultValue = y.DefaultValue,
                            InputType = y.InputType
                        }).ToList(),
                        Outputs = dbApi.Outputs.Select(y => new ApiOutput
                        {
                            Id = y.Id,
                            Name = y.Name,
                            Type = y.Type,
                            Expression = y.Expression
                        }).ToList(),
                        Headers = dbApi.RequestHeaders.Select(y => new ApiHeader
                        {
                            Id = y.Id,
                            Name = y.Name,
                            Value = y.Value
                        }).ToList()
                    };
                    if (dbApi.RequestBody != null)
                    {
                        if (ContentTypes.IsText(api.Headers.GetContentType()))
                        {
                            api.Body = Encoding.UTF8.GetString(dbApi.RequestBody);
                        }
                        else
                        {
                            api.Body = Convert.ToBase64String(dbApi.RequestBody);
                        }
                    }
                }
                else
                {
                    apiItem = new ApiCollection
                    {
                        Type = ApiItemType.Collection,
                        Items = Map(dbApiItemsByParentId[dbApi.Id], dbApiItemsByParentId).ToList()
                    };
                }
                apiItem.Id = dbApi.Id;
                apiItem.Created = dbApi.Created;
                apiItem.Title = dbApi.Title;

                yield return apiItem;
            }
        }
    }
}