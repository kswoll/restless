using System;
using Restless.Utils;
using Restless.ViewModels;
using SexyReact;

namespace Restless.Controls.ResponseVisualizers
{
    public interface IResponseVisualizer : ICompareTo<IResponseVisualizer>
    {
        string Header { get; }
        ApiResponseModel Model { get; set; }
    }
}