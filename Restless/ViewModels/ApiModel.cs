using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading;
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
        public RxList<ApiHeaderModel> Headers { get; }
        public IRxCommand Send { get; }
        public ApiResponseModel Response { get; set; }

        private static readonly ApiMethod[] httpMethods = { ApiMethod.Get, ApiMethod.Post, ApiMethod.Put, ApiMethod.Delete };

        public ApiModel(DbApi dbApi)
        {
            Id = dbApi.Id;
            Title = dbApi.Title;
            Url = dbApi.Url;
            Methods = httpMethods.ToList();
            Method = dbApi.Method;
            Headers = dbApi.Headers == null ? new RxList<ApiHeaderModel>() : new RxList<ApiHeaderModel>(dbApi.Headers.Select(x => new ApiHeaderModel
            {
                Id = x.Id,
                Name = x.Name,
                Value = x.Value
            }));
            Send = RxCommand.CreateAsync(SendImpl);

            var semaphore = new Semaphore(1, 1);
            Headers.ItemAdded.SubscribeAsync(async x =>
            {
                semaphore.WaitOne();
                var db = new RestlessDb();
                var dbApiHeader = new DbApiHeader
                {
                    ApiId = Id,
                    Name = "",
                    Value = ""
                };
                db.ApiHeaders.Add(dbApiHeader);
                await db.SaveChangesAsync();
                x.Id = dbApiHeader.Id;
                semaphore.Release();
            });
            Headers.ItemRemoved.SubscribeAsync(async x =>
            {
                semaphore.WaitOne();
                var db = new RestlessDb();
                var dbApiHeader = await db.ApiHeaders.SingleAsync(y => y.Id == x.Id);
                db.ApiHeaders.Remove(dbApiHeader);
                await db.SaveChangesAsync();
                semaphore.Release();
            });
            Headers.ObserveElementChange(x => x.Name, x => x.Value).SubscribeAsync(async x =>
            {
                semaphore.WaitOne();
                var db = new RestlessDb();
                var dbApiHeader = await db.ApiHeaders.SingleAsync(y => y.Id == x.Element.Id);
                dbApiHeader.Name = x.Element.Name;
                dbApiHeader.Value = x.Element.Value;
                await db.SaveChangesAsync();
                semaphore.Release();
            });

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

        private async Task SendImpl()
        {
            var request = CreateRequest();
            var responseModel = new ApiResponseModel { Api = this };

            using (var client = CreateClient())
            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsByteArrayAsync();
                stopwatch.Stop();

                responseModel.ContentLength = responseBody.Length;
                responseModel.Elapsed = stopwatch.Elapsed;
                responseModel.Response = responseBody;
                responseModel.Status = response.StatusCode.ToString();
                responseModel.StatusCode = (int)response.StatusCode;
                responseModel.Reason = response.ReasonPhrase;
                responseModel.Headers = response.Headers
                    .Concat(response.Content == null ? Enumerable.Empty<KeyValuePair<string, IEnumerable<string>>>() : response.Content.Headers)
                    .Select(x => new ApiHeaderModel
                    {
                        Name = x.Key,
                        Value = string.Join(", ", x.Value)
                    })
                    .OrderBy(x => x.Name)
                    .ToList();
            }

            Response = responseModel;
        }

        private static HttpClient CreateClient()
        {
            return new HttpClient(new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip,
                AllowAutoRedirect = true
            });
        }

        private HttpMethod MapApiMethod()
        {
            switch (Method)
            {
                case ApiMethod.Delete:
                    return HttpMethod.Delete;
                case ApiMethod.Get:
                    return HttpMethod.Get;
                case ApiMethod.Post:
                    return HttpMethod.Post;
                case ApiMethod.Put:
                    return HttpMethod.Put;
                default:
                    throw new Exception($"Unexpected ApiMethod: {Method}");
            }
        }

        private HttpRequestMessage CreateRequest()
        {
            var request = new HttpRequestMessage(MapApiMethod(), Url);
            foreach (var header in Headers)
            {
                request.Headers.Add(header.Name, header.Value);
            }
            return request;
        }
    }
}