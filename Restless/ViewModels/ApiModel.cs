using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Restless.Database;
using Restless.Models;
using Restless.Utils;
using Restless.Utils.Inputs;
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
        public RxList<ApiInputModel> Inputs { get; }
        public RxList<ApiHeaderModel> Headers { get; }
        public IRxCommand Send { get; }
        public IRxCommand Reset { get; }
        public ApiResponseModel Response { get; set; }

        private static readonly ApiMethod[] httpMethods = { ApiMethod.Get, ApiMethod.Post, ApiMethod.Put, ApiMethod.Delete };

        public ApiModel(DbApi dbApi)
        {
            SubscribeForInputs(this.ObservePropertyChange(x => x.Url), () => Url, ApiInputType.Url);
            SubscribeForInputs(this.ObservePropertyChange(x => x.Headers).SelectMany(x => x.ObserveElementProperty(y => y.Name)).Merge(this.ObservePropertyChange(x => x.Headers).SelectMany(x => x.ObserveElementProperty(y => y.Value))), () => string.Join("\n", Headers.Select(x => x.Name + "=" + x.Value)), ApiInputType.Header);

            Id = dbApi.Id;
            Title = dbApi.Title;
            Url = dbApi.Url;
            Methods = httpMethods.ToList();
            Method = dbApi.Method;
            Inputs = dbApi.Inputs == null ? new RxList<ApiInputModel>() : new RxList<ApiInputModel>(dbApi.Inputs.Select(x => new ApiInputModel
            {
                Id = x.Id,
                Name = x.Name,
                DefaultValue = x.DefaultValue,
                InputType = x.InputType
            }));
            Headers = dbApi.RequestHeaders == null ? new RxList<ApiHeaderModel>() : new RxList<ApiHeaderModel>(dbApi.RequestHeaders.Select(x => new ApiHeaderModel
            {
                Id = x.Id,
                Name = x.Name,
                Value = x.Value
            }));
            Send = RxCommand.CreateAsync(OnSend);
            Reset = RxCommand.Create(OnReset);

            Inputs.SetUpSync(
                x => new DbApiInput { ApiId = Id, Name = x.Name, DefaultValue = "", InputType = x.InputType },
                (input, dbInput) =>
                {
                    dbInput.DefaultValue = input.DefaultValue;
                });
            Headers.SetUpSync(
                _ => new DbApiHeader { ApiId = Id, Name = "", Value = "" },
                (header, dbHeader) =>
                {
                    dbHeader.Name = header.Name;
                    dbHeader.Value = header.Value;
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

        private void SubscribeForInputs<T>(IObservable<T> observable, Func<string> value, ApiInputType inputType)
        {
            observable.Throttle(TimeSpan.FromSeconds(1)).Subscribe(_ => CheckForInputs(inputType, value()));
        }

        private void CheckForInputs(ApiInputType inputType, string s)
        {
            var inputString = InputGrammar.Parse(s);
            var inputVariables = inputString.Tokens.OfType<VariableInputToken>().ToArray();
            var existingInputs = Inputs.Where(x => x.InputType == inputType).ToArray();
            var obsoleteInputs = existingInputs.Where(x => !inputVariables.Any(y => x.Name == y.Variable)).ToArray();

            foreach (var input in obsoleteInputs)
            {
                Inputs.Remove(input);
            }

            foreach (var variable in inputVariables)
            {
                if (!Inputs.Any(x => x.Name == variable.Variable))
                {
                    var insertionPoint = Inputs.BinarySearch(x => x.Name, variable.Variable);
                    if (insertionPoint < 0)
                    {
                        insertionPoint = ~insertionPoint;
                        Inputs.Insert(insertionPoint, new ApiInputModel { Name = variable.Variable, InputType = inputType });
                    }
                }
            }
        }

        private async Task OnSend()
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

        private void OnReset()
        {
            foreach (var input in Inputs)
            {
                input.Value = null;
            }
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

        private string Format(string s)
        {
            return InputGrammar.Parse(s).Format(name => Inputs.Single(x => x.Name == name).Value);
        }

        private HttpRequestMessage CreateRequest()
        {
            foreach (var input in Inputs)
            {
                if (input.Value == null)
                    input.Value = input.DefaultValue;
            }

            var request = new HttpRequestMessage(MapApiMethod(), Format(Url));
            foreach (var header in Headers)
            {
                request.Headers.Add(Format(header.Name), Format(header.Value));
            }
            return request;
        }
    }
}