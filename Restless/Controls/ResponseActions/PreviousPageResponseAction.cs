using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Restless.ViewModels;

namespace Restless.Controls.ResponseActions
{
    public class PreviousPageResponseAction : IResponseAction
    {
        public string Header => "Previous Page";

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
            throw new NotImplementedException();
        }
    }
}