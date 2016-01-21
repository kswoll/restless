using Restless.ViewModels;

namespace Restless.Controls.ResponseActions
{
    public class NextPageResponseAction : PageResponseAction
    {
        public override string Header => "Next Page";

        public override int CompareTo(IResponseAction other)
        {
            if (other is PreviousPageResponseAction)
                return 1;
            else
                return 0;
        }

        [ResponseActionPredicate]
        public static ResponseActionState IsActionApplicableToResponse(ApiResponseModel response)
        {
            var totalCount = GetTotalCount(response);
            var offset = GetOffset(response);
            var limit = GetLimit(response);
            if (totalCount == null || offset == null || limit == null)
                return ResponseActionState.Hidden;

            return offset + limit < totalCount ? ResponseActionState.Enabled : ResponseActionState.Disabled;
        }

        protected override int AdjustOffset(int offset, int limit)
        {
            return offset + limit;
        }
    }
}