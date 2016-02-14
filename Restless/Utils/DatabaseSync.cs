using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.Data.Entity;
using Restless.Database;
using Restless.Database.Repositories;
using Restless.Models;
using SexyReact;
using SexyReact.Utils;

namespace Restless.Utils
{
    public static class DatabaseSync
    {
        public static void SetUpSync<TViewModel, TModel>(
            this RxList<TViewModel> list, 
            DbRepository repository,
            ImmutableList<TModel> modelList,
            Func<TViewModel, TModel> newModelObject
        )
            where TViewModel : IIdObject, IRxObject
            where TModel : IdObject
        {
            Action<TViewModel, TModel> bind = (viewModel, model) =>
            {
                viewModel.Changed.Where(y => y.Property.Name != nameof(IdObject.Id)).Subscribe(y =>
                {
                    model.GetType().GetProperty(y.Property.Name).SetValue(model, y.NewValue);
                });
            };
            var modelsById = modelList.ToDictionary(x => x.Id);
            foreach (var item in list)
            {
                bind(item, modelsById[item.Id]);
            }
            list.ItemAdded.Subscribe(async viewModel =>
            {
                viewModel.ObservePropertyChange(y => y.Id).SubscribeOnce(y => viewModel.Id = y);
                var newModel = newModelObject(viewModel);
                modelList.Add(newModel);
                await repository.WaitForIdle();
                bind(viewModel, newModel);
            });
            list.ItemRemoved.Subscribe(async viewModel =>
            {
                var db = new RestlessDb();
                var dbApiHeader = await db.Set<TModel>().SingleAsync(y => y.Id == viewModel.Id);
                db.Set<TModel>().Remove(dbApiHeader);
                await db.SaveChangesAsync();
            });
        } 
    }
}