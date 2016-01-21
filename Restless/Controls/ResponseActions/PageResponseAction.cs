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
            var offsetInput = response.Api.Inputs.SingleOrDefault(x => x.Name == "offset");
            return offsetInput;
        }

        protected static int? GetOffset(ApiResponseModel response)
        {
            var input = GetOffsetInput(response);
            if (input == null)
                return null;
            return int.Parse(input.Value);
        }

        protected static int? GetLimit(ApiResponseModel response)
        {
            var input = response.Api.Inputs.SingleOrDefault(x => x.Name == "limit");
            if (input == null)
                return null;
            return int.Parse(input.Value);
        }

        protected static int? GetTotalCount(ApiResponseModel response)
        {
            var output = response.Api.Outputs.SingleOrDefault(x => x.Name == "total");
            if (output == null)
                return null;
            return int.Parse(output.Value);
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