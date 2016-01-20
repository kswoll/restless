using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Restless.Utils;

namespace Restless.ViewModels
{
    public class ApiResponseModel : BaseModel
    {
        public ApiModel Api { get; set; }
        public List<ApiHeaderModel> Headers { get; set; }
        public byte[] Response { get; set; }
        public long ContentLength { get; set; }
        public TimeSpan Elapsed { get; set; }
        public int StatusCode { get; set; }
        public string Status { get; set; }
        public string Reason { get; set; }

        public string ContentType => Headers.SingleOrDefault(x => x.Name == ContentTypes.ContentType)?.Value.Split(';').First();
        public string StringResponse => stringResponse.Value;
        public JToken JsonResponse => jsonResponse.Value;

        private readonly Lazy<string> stringResponse;
        private readonly Lazy<JToken> jsonResponse;

        public ApiResponseModel()
        {
            stringResponse = new Lazy<string>(() => Response == null || !ContentTypes.IsText(ContentType) ? null : Encoding.UTF8.GetString(Response));
            jsonResponse = new Lazy<JToken>(() => Response == null || ContentType != ContentTypes.ApplicationJson ? null : JToken.Parse(StringResponse));
        }
    }
}