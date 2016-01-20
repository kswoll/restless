using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Restless.ViewModels;

namespace Restless.Controls.ResponseActions
{
    public class NextPageResponseAction : IResponseAction
    {
        public string Header => "Next Page";

        [ResponseActionPredicate]
        public static bool IsActionApplicableToResponse(ApiResponseModel response)
        {
            var json = response.JsonResponse as JObject;
            if (json != null)
            {
//                if (json.Property(""))
            }
            return true;
        }

        public Task PerformAction(ApiResponseModel response)
        {
            var limit = int.Parse(response.Api.Inputs.Single(x => x.Name == "limit").Value);
            var offsetInput = response.Api.Inputs.Single(x => x.Name == "offset");
            var offset = int.Parse(offsetInput.Value);
            offset += limit;
            offsetInput.Value = offset.ToString();

            return response.Api.Send.InvokeAsync();
        }
    }
}