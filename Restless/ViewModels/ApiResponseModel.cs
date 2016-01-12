using System;
using System.Collections.Generic;
using Restless.Models;

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
    }
}