using System;
using System.Threading.Tasks;
using Restless.ViewModels;

namespace Restless.Controls.ResponseActions
{
    public class PreviousPageResponseAction : IResponseAction
    {
        public string Header => "Previous Page";

        [ResponseActionPredicate]
        public static bool IsActionApplicableToResponse(ApiResponseModel response)
        {
            return false;
        }

        public Task PerformAction(ApiResponseModel response)
        {
            throw new NotImplementedException();
        }
    }
}