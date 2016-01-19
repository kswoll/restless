using System;
using System.Threading;
using Microsoft.Data.Entity;
using Restless.Database;
using Restless.Models;
using SexyReact;
using SexyReact.Utils;

namespace Restless.Utils
{
    public static class DatabaseSync
    {
        public static void SetUpSync<TModelObject, TDbObject>(
            this RxList<TModelObject> list, 
            Func<TModelObject, TDbObject> newDbObject,
            Action<TModelObject, TDbObject> updateDbObject
        )
            where TModelObject : IIdObject, IRxObject
            where TDbObject : class, IIdObject
        {
            var semaphore = new Semaphore(1, 1);
            list.ItemAdded.SubscribeAsync(async x =>
            {
                semaphore.WaitOne();
                var db = new RestlessDb();
                var dbObject = newDbObject(x);
                db.Set<TDbObject>().Add(dbObject);
                await db.SaveChangesAsync();
                x.Id = dbObject.Id;
                semaphore.Release();
            });
            list.ItemRemoved.SubscribeAsync(async x =>
            {
                semaphore.WaitOne();
                var db = new RestlessDb();
                var dbApiHeader = await db.Set<TDbObject>().SingleAsync(y => y.Id == x.Id);
                db.Set<TDbObject>().Remove(dbApiHeader);
                await db.SaveChangesAsync();
                semaphore.Release();
            });
            list.ObserveElementChange().SubscribeAsync(async x =>
            {
                semaphore.WaitOne();
                var db = new RestlessDb();
                var dbObject = await db.Set<TDbObject>().SingleAsync(y => y.Id == x.Element.Id);
                updateDbObject(x.Element, dbObject);
                await db.SaveChangesAsync();
                semaphore.Release();
            });            
        } 
    }
}