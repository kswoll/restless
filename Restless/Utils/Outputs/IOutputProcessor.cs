using System.Threading.Tasks;
using Restless.ViewModels;

namespace Restless.Utils.Outputs
{
    public interface IOutputProcessor
    {
        Task ProcessOutput(ApiResponseModel response, ApiOutputModel output);
    }
}