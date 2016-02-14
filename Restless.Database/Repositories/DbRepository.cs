using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Nito.AsyncEx;
using Restless.Models;
using Restless.Utils;

namespace Restless.Database.Repositories
{
    public class DbRepository
    {
        public IReadOnlyList<ApiItem> Items => items;

        private static readonly Dictionary<Type, Delegate> saveMappers = new Dictionary<Type, Delegate>();

        private readonly RestlessDb db;
        private readonly AsyncLock locker = new AsyncLock();
        private readonly AsyncAutoResetEvent idle = new AsyncAutoResetEvent(false);
        private ImmutableList<ApiItem> items = ImmutableList<ApiItem>.Empty;
        private ImmutableDictionary<IdObject, object> cache = ImmutableDictionary<IdObject, object>.Empty;
        private int isSavePending;

        public DbRepository(RestlessDb db)
        {
            this.db = db;
        }

        static DbRepository()
        {
            CreateMapper<Api, DbApiItem>(ApiSaveMapper);
            CreateMapper<ApiCollection, DbApiItem>(ApiCollectionSaveMapper);
            CreateMapper<ApiHeader, DbApiHeader>(ApiHeaderSaveMapper);
            CreateMapper<ApiInput, DbApiInput>(ApiInputSaveMapper);
            CreateMapper<ApiOutput, DbApiOutput>(ApiOutputSaveMapper);
        }

        private static void CreateMapper<TModel, TDatabase>(Action<TModel, TDatabase> mapper)
        {
            saveMappers[typeof(TModel)] = mapper;
        }

        public async Task Initialize()
        {
            using (await locker.LockAsync())
            {
                await db.Database.MigrateAsync();
            }
        }

        public async Task Load()
        {
            using (await locker.LockAsync())
            {
                var dbApiItems = await db.ApiItems
                    .Include(x => x.Headers)
                    .Include(x => x.Inputs)
                    .Include(x => x.Outputs)
                    .Include(x => x.Outputs)
                    .ToArrayAsync();
                var dbApiItemsByParentId = dbApiItems.ToLookup(x => x.CollectionId ?? 0);

                items = items.AddRange(MapFromDb(dbApiItemsByParentId[0], dbApiItemsByParentId));
            }
        }

        public async Task WaitForIdle()
        {
            using (await locker.LockAsync())
            {
                if (isSavePending == 0)
                    return;
            }
            await idle.WaitAsync();
        }

        public async Task DeleteApiItem(int id)
        {
            using (await locker.LockAsync())
            {
                var item = items.Single(x => x.Id == id);
                var dbItem = (DbApiItem)cache[item];
                cache = cache.Remove(item);
                db.ApiItems.Remove(dbItem);
                await db.SaveChangesAsync();
            }
        }

        public async Task AddItem(ApiItem apiItem)
        {
            var dbApiItem = MapItemToDb(apiItem);
            using (await locker.LockAsync())
            {
                items = items.Add(apiItem);

                db.ApiItems.Add(dbApiItem);
                await db.SaveChangesAsync();
                apiItem.Id = dbApiItem.Id;
            }
        }

        private DbApiItem MapItemToDb(ApiItem apiItem)
        {
            var dbApiItem = new DbApiItem
            {
                Headers = new List<DbApiHeader>(),
                Inputs = new List<DbApiInput>(),
                Outputs = new List<DbApiOutput>(),
                Items = new List<DbApiItem>()
            };
            Bind(apiItem, dbApiItem);

            MapScalarsToDb(apiItem, dbApiItem);
            var api = apiItem as Api;
            if (api != null)
            {
                MapChildrenToDb(api.Inputs, dbApiItem.Inputs);
                MapChildrenToDb(api.Outputs, dbApiItem.Outputs);
                MapChildrenToDb(api.Headers, dbApiItem.Headers);
            }
            else
            {
                var apiCollection = (ApiCollection)apiItem;
                dbApiItem.Type = ApiItemType.Collection;
                if (apiCollection.Items != null)
                {
                    foreach (var childApiItem in apiCollection.Items)
                    {
                        var childDbItem = MapItemToDb(childApiItem);
                        dbApiItem.Items.Add(childDbItem);
                    }                    
                }
            }
            return dbApiItem;
        }

        private void Bind(IdObject modelItem, IIdObject dbItem)
        {
            cache = cache.Add(modelItem, dbItem);
            int? isSavePending = null;
            modelItem.Changed
                .Where(x => x.Property.Name != nameof(IIdObject.Id))
                .Do(_ =>
                {
                    using (locker.Lock())
                    {
                        isSavePending = isSavePending ?? ++this.isSavePending;
                    }
                })
                .Throttle(TimeSpan.FromSeconds(1))
                .Select(async x =>
                {
                    if (typeof(IList).IsAssignableFrom(x.Property.PropertyType))
                    {
                        var oldList = ((IEnumerable)x.OldValue)?.Cast<IdObject>() ?? Enumerable.Empty<IdObject>();
                        var newList = ((IEnumerable)x.NewValue)?.Cast<IdObject>() ?? Enumerable.Empty<IdObject>();
                        var merge = oldList.Merge(newList);
                        var dbListProperty = dbItem.GetType().GetProperty(x.Property.Name);
                        var dbList = (IList)dbListProperty.GetValue(dbItem, null);
                        foreach (var childModelItem in merge.Removed)
                        {
                            var childDbItem = cache[childModelItem];
                            dbList.Remove(childDbItem);
                        }
                        foreach (var childModelItem in merge.Added)
                        {
                            var childDbType = dbListProperty.PropertyType.GetGenericArguments()[0];
                            var childDbItem = (IIdObject)Activator.CreateInstance(childDbType);
                            Bind(childModelItem, childDbItem);
                            MapScalarsToDb(childModelItem, childDbItem);
                            dbList.Add(childDbItem);
                        }
                    }
                    else
                    {
                        MapScalarsToDb(modelItem, dbItem);
                    }
                    using (await locker.LockAsync())
                    {
                        await db.SaveChangesAsync();
                        this.isSavePending--;
                        isSavePending = null;
                        if (this.isSavePending == 0)
                            idle.Set();
                    }
                })
                .Subscribe();
        }

        private IEnumerable<TModel> MapChildrenFromDb<TDatabase, TModel>(IEnumerable<TDatabase> source, Func<TDatabase, TModel> factory)
            where TModel : IdObject
            where TDatabase : IIdObject, new()
        {
            foreach (var item in source)
            {
                var model = factory(item);
                Bind(model, item);
                yield return model;
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
                        Inputs = MapChildrenFromDb(dbApiItem.Inputs, y => new ApiInput
                        {
                            Id = y.Id,
                            Name = y.Name,
                            DefaultValue = y.DefaultValue,
                            InputType = y.InputType
                        }).ToImmutableList(),
                        Outputs = MapChildrenFromDb(dbApiItem.Outputs, y => new ApiOutput
                        {
                            Id = y.Id,
                            Name = y.Name,
                            Type = y.Type,
                            Expression = y.Expression
                        }).ToImmutableList(),
                        Headers = MapChildrenFromDb(dbApiItem.Headers, y => new ApiHeader
                        {
                            Id = y.Id,
                            Name = y.Name,
                            Value = y.Value
                        }).ToImmutableList(),
                        Body = dbApiItem.RequestBody
                    };
                }
                else
                {
                    apiItem = new ApiCollection
                    {
                        Type = ApiItemType.Collection,
                        Items = MapFromDb(dbApiItemsByParentId[dbApiItem.Id], dbApiItemsByParentId).ToImmutableList()
                    };
                }
                apiItem.Id = dbApiItem.Id;
                apiItem.Created = dbApiItem.Created;
                apiItem.Title = dbApiItem.Title;

                Bind(apiItem, dbApiItem);
                yield return apiItem;
            }
        }

        private static void ApiItemSaveMapper(ApiItem apiItem, DbApiItem dbApiItem)
        {
            dbApiItem.Created = apiItem.Created;
            dbApiItem.Title = apiItem.Title;            
        }

        private static void ApiSaveMapper(Api api, DbApiItem dbApi)
        {
            ApiItemSaveMapper(api, dbApi);
            dbApi.Type = ApiItemType.Api;
            dbApi.Url = api.Url;
            dbApi.Method = api.Method;
            dbApi.RequestBody = api.Body;            
        }

        private static void ApiCollectionSaveMapper(ApiCollection apiCollection, DbApiItem dbApiItem)
        {
            ApiItemSaveMapper(apiCollection, dbApiItem);
            dbApiItem.Type = ApiItemType.Collection;
        }

        private static void ApiInputSaveMapper(ApiInput apiInput, DbApiInput dbApiInput)
        {
            dbApiInput.Name = apiInput.Name;
            dbApiInput.DefaultValue = apiInput.DefaultValue;
            dbApiInput.InputType = apiInput.InputType;            
        }

        private static void ApiOutputSaveMapper(ApiOutput apiOutput, DbApiOutput dbApiOutput)
        {
            dbApiOutput.Name = apiOutput.Name;
            dbApiOutput.Type = apiOutput.Type;
            dbApiOutput.Expression = apiOutput.Expression;
        }

        private static void ApiHeaderSaveMapper(ApiHeader apiHeader, DbApiHeader dbApiHeader)
        {
            dbApiHeader.Name = apiHeader.Name;
            dbApiHeader.Value = apiHeader.Value;            
        }

        private static void MapScalarsToDb<TModel, TDatabase>(TModel modelItem, TDatabase dbItem)
        {
            saveMappers[modelItem.GetType()].DynamicInvoke(modelItem, dbItem);
        }

        private void MapChildrenToDb<TModel, TDatabase>(ImmutableList<TModel> modelObjects, List<TDatabase> dbObjects)
            where TModel : IdObject
            where TDatabase : IIdObject, new()
        {
            if (modelObjects == null)
                return;

            foreach (var modelObject in modelObjects)
            {
                object dbObjectFromCache;
                TDatabase dbObject;
                if (cache.TryGetValue(modelObject, out dbObjectFromCache))
                {
                    dbObject = (TDatabase)dbObjectFromCache;
                }
                else
                {
                    dbObject = new TDatabase();
                    Bind(modelObject, dbObject);
                    dbObjects.Add(dbObject);
                }
                MapScalarsToDb(modelObject, dbObject);
            }
        }
    }
}