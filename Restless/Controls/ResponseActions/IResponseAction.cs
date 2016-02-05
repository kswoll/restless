using System;
using System.Threading.Tasks;
using Restless.Utils;
using Restless.ViewModels;

namespace Restless.Controls.ResponseActions
{
    public interface IResponseAction : ICompareTo<IResponseAction>
    {
        object Header { get; }
        string ToolTip { get; }
        Task PerformAction(ApiResponseModel response);
    }
}