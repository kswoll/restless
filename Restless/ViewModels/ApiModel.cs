using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.Entity;
using Restless.Database;
using Restless.Models;
using Restless.Utils;
using Restless.Utils.Inputs;
using Restless.Utils.Outputs;
using SexyReact;
using SexyReact.Utils;
using HttpHeaders = Restless.Utils.HttpHeaders;

namespace Restless.ViewModels
{
    public class ApiModel : BaseModel
    {
        public MainWindowModel MainWindow { get; }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public ApiMethod Method { get; set; }
        public RxList<ApiInputModel> Inputs { get; }
        public RxList<ApiOutputModel> Outputs { get; }
        public RxList<ApiHeaderModel> Headers { get; }
        public IRxCommand Send { get; }
        public IRxCommand Reset { get; }
        public ApiResponseModel Response { get; set; }

        public ApiModel(MainWindowModel mainWindow, DbApi dbApi)
        {
            MainWindow = mainWindow;
            Inputs = new RxList<ApiInputModel>();
            Outputs = new RxList<ApiOutputModel>();
            Headers = new RxList<ApiHeaderModel>();

            SubscribeForInputs(this.ObservePropertyChange(x => x.Url), () => Url, ApiInputType.Url);
            SubscribeForInputs(this.ObservePropertyChange(x => x.StringBody), () => StringBody, ApiInputType.Body);
            SubscribeForInputs(this.ObservePropertyChange(x => x.Headers).SelectMany(x => x.ObserveElementProperty(y => y.Name)).Merge(this.ObservePropertyChange(x => x.Headers).SelectMany(x => x.ObserveElementProperty(y => y.Value))), () => string.Join("\n", Headers.Select(x => x.Name + "=" + x.Value)), ApiInputType.Header);

            Id = dbApi.Id;
            Title = dbApi.Title;
            Url = dbApi.Url;
            Method = dbApi.Method;
            Body = dbApi.RequestBody;
            if (dbApi.Inputs != null)
                Inputs.AddRange(dbApi.Inputs.Select(x => new ApiInputModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    DefaultValue = x.DefaultValue,
                    InputType = x.InputType
                }));
            if (dbApi.Outputs != null)
                Outputs.AddRange(dbApi.Outputs.Select(x => new ApiOutputModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Expression = x.Expression,
                    Type = x.Type
                }));
            if (dbApi.RequestHeaders != null)
                Headers.AddRange(dbApi.RequestHeaders.Select(x => new ApiHeaderModel
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
            Outputs.SetUpSync(
                x => new DbApiOutput { ApiId = Id, Name = x.Name, Expression = "" },
                (output, dbOutput) =>
                {
                    dbOutput.Name = output.Name;
                    dbOutput.Expression = output.Expression;
                    dbOutput.Type = output.Type;
                });
            Headers.SetUpSync(
                _ => new DbApiHeader { ApiId = Id, Name = "", Value = "" },
                (header, dbHeader) =>
                {
                    dbHeader.Name = header.Name;
                    dbHeader.Value = header.Value;
                });

            this.ObservePropertiesChange(x => x.Title, x => x.Url, x => x.Method, x => x.Body)
                .Throttle(TimeSpan.FromSeconds(1))
                .SubscribeAsync(async _ =>
                {
                    var db = new RestlessDb();
                    var updatedApi = await db.Apis.SingleAsync(x => x.Id == Id);
                    updatedApi.Title = Title;
                    updatedApi.Url = Url;
                    updatedApi.Method = Method;
                    updatedApi.RequestBody = Body;
                    await db.SaveChangesAsync();
                });
        }

        public byte[] Body
        {
            get { return Get<byte[]>(); }
            set
            {
                Set(value);
                OnChanged(GetType().GetProperty("StringBody"), StringBody);
            }
        }

        public string StringBody
        {
            get { return Body == null ? null : Encoding.UTF8.GetString(Body); }
            set { Body = value == null ? null : Encoding.UTF8.GetBytes(value); }
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
                try
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

                    foreach (var output in Outputs)
                    {
                        await OutputProcessorRegistry.GetProcessor(output.Type).ProcessOutput(responseModel, output);
                    }
                }
                catch (Exception e)
                {
                    responseModel.StatusCode = 0;
                    responseModel.Reason = e.Message;
                    responseModel.Status = "Client Error";
                    responseModel.Headers = new List<ApiHeaderModel>();
                    responseModel.Headers.Add(new ApiHeaderModel
                    {
                        Name = "Content-Type",
                        Value = "text/plain"
                    });
                    responseModel.Response = Encoding.UTF8.GetBytes(e.Message);
                }
            }

            Response = responseModel;
        }

        private void OnReset()
        {
            foreach (var input in Inputs)
            {
                input.Value = null;
            }
            foreach (var output in Outputs)
            {
                output.Value = null;
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
            var contentHeaders = new List<KeyValuePair<string, string>>();
            foreach (var header in Headers)
            {
                var name = Format(header.Name);
                var value = Format(header.Value);
                if (HttpHeaders.IsContentHeader(name))
                    contentHeaders.Add(new KeyValuePair<string, string>(name, value));
                else
                    request.Headers.Add(name, value);
            }

            if (Body != null && Method != ApiMethod.Get)
            {
                var contentType = Headers.SingleOrDefault(x => x.Name == ContentTypes.ContentType)?.Value;
                if (contentType != null && ContentTypes.IsText(contentType))
                {
                    var s = StringBody;
                    s = Format(s);
                    var stringContent = new StringContent(s);
                    request.Content = stringContent;
                }
                else
                {
                    request.Content = new ByteArrayContent(Body);
                }
                foreach (var header in contentHeaders)
                {
                    if (header.Key == ContentTypes.ContentType)
                        request.Content.Headers.ContentType = MediaTypeHeaderValue.Parse(header.Value);
                }
            }

            return request;
        }
    }
}