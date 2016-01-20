using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Restless.ViewModels;

namespace Restless.Controls.ResponseActions
{
    public class PreviousPageResponseAction : PageResponseAction
    {
        public override string Header => "Previous Page";

        public override int CompareTo(IResponseAction other)
        {
            if (other is NextPageResponseAction)
                return -1;
            else
                return 0;
        }

        [ResponseActionPredicate]
        public static ResponseActionState IsActionApplicableToResponse(ApiResponseModel response)
        {
            var totalCount = GetTotalCount(response);
            if (totalCount == null)
                return ResponseActionState.Hidden;

            var offset = GetOffset(response);
            return offset > 0 ? ResponseActionState.Enabled : ResponseActionState.Disabled;
        }

        protected override int AdjustOffset(int offset, int limit)
        {
            return Math.Max(0, offset - limit);
        }
    }
}