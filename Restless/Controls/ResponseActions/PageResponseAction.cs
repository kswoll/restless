using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Restless.ViewModels;

namespace Restless.Controls.ResponseActions
{
    public abstract class PageResponseAction : IResponseAction
    {
        public abstract int CompareTo(IResponseAction other);
        public abstract string Header { get; }

        protected abstract int AdjustOffset(int offset, int limit);

        protected static ApiInputModel GetOffsetInput(ApiResponseModel response)
        {
            var offsetInput = response.Api.Inputs.Single(x => x.Name == "offset");
            return offsetInput;
        }

        protected static int GetOffset(ApiResponseModel response)
        {
            return int.Parse(GetOffsetInput(response).Value);
        }

        protected static int GetLimit(ApiResponseModel response)
        {
            return int.Parse(response.Api.Inputs.Single(x => x.Name == "limit").Value);
        }

        protected static int? GetTotalCount(ApiResponseModel response)
        {
            var json = response.JsonResponse as JObject;
            if (json != null)
                return int.Parse((string)json["TotalCount"]);
            else
                return null;
        }

        public Task PerformAction(ApiResponseModel response)
        {
            var limit = int.Parse(response.Api.Inputs.Single(x => x.Name == "limit").Value);
            var offsetInput = response.Api.Inputs.Single(x => x.Name == "offset");
            var offset = int.Parse(offsetInput.Value);
            offset = AdjustOffset(offset, limit);
            offsetInput.Value = offset.ToString();

            return response.Api.Send.InvokeAsync();            
        }         
    }
}