using System.Threading.Tasks;
using Restless.ViewModels;

namespace Restless.Controls.ResponseActions
{
    public interface IResponseAction
    {
        string Header { get; }
        Task PerformAction(ApiResponseModel response);
    }
}