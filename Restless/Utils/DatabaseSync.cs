using System;
using System.Linq;
using System.Reactive.Linq;
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
            RxList<TModel> modelList,
            Func<TViewModel, TModel> newModelObject
        )
            where TViewModel : IIdObject, IRxObject
            where TModel : IdObject
        {
            Action<TViewModel, TModel> bind = (viewModel, model) =>
            {
                viewModel.Changed.Where(y => y.Property.Name != nameof(IdObject.Id)).Subscribe(y =>
                {
                    model.GetType().GetProperty(y.Property.Name)?.SetValue(model, y.NewValue);
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
                bind(viewModel, newModel);
                modelList.Add(newModel);
                await repository.WaitForIdle();
            });
            list.ItemRemoved.Subscribe(viewModel =>
            {
                var model = modelList.Single(x => x.Id == viewModel.Id);
                modelList.Remove(model);
            });
        } 
    }
}