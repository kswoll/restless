﻿using System;
using Restless.Properties;
using Restless.Utils;
using Restless.ViewModels;

namespace Restless.Controls.ResponseActions
{
    public class PreviousPageResponseAction : PageResponseAction
    {
        public override object Header => Icons.Get(IconResources.PageLeft);
        public override string ToolTip => "Navigate to the previous page of results";

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
            var offset = GetOffset(response);
            var limit = GetLimit(response);
            if (totalCount == null || offset == null || limit == null)
                return ResponseActionState.Hidden;

            return offset > 0 ? ResponseActionState.Enabled : ResponseActionState.Disabled;
        }

        protected override int AdjustOffset(int offset, int limit)
        {
            return Math.Max(0, offset - limit);
        }
    }
}