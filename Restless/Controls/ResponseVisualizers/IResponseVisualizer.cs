using Restless.ViewModels;
using SexyReact;

namespace Restless.Controls.ResponseVisualizers
{
    public interface IResponseVisualizer
    {
        string Header { get; }
        ApiResponseModel Model { get; set; }
    }
}