using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Restless.Database;
using Restless.Models;
using SexyReact;
using SexyReact.Utils;

namespace Restless.ViewModels
{
    public class ApiModel : BaseModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public ApiMethod Method { get; set; }
        public List<ApiMethod> Methods { get; }

        private static readonly ApiMethod[] httpMethods = { ApiMethod.Get, ApiMethod.Post, ApiMethod.Put, ApiMethod.Delete };

        public ApiModel(DbApi dbApi)
        {
            Id = dbApi.Id;
            Title = dbApi.Title;
            Url = dbApi.Url;
            Methods = httpMethods.ToList();
            Method = dbApi.Method;

            this.ObservePropertiesChange(x => x.Title, x => x.Url, x => x.Method)
                .Throttle(TimeSpan.FromSeconds(1))
                .SubscribeAsync(async _ =>
                {
                    var db = new RestlessDb();
                    var updatedApi = await db.Apis.SingleAsync(x => x.Id == Id);
                    updatedApi.Title = Title;
                    updatedApi.Url = Url;
                    updatedApi.Method = Method;
                    await db.SaveChangesAsync();
                });
        }
    }
}