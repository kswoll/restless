using System.Threading.Tasks;
using Nito.AsyncEx;
using Restless.Utils.Outputs.Json;
using Restless.ViewModels;

namespace Restless.Utils.Outputs
{
    public class JsonPathOutputProcessor : IOutputProcessor
    {
        public Task ProcessOutput(ApiResponseModel response, ApiOutputModel output)
        {
            var expression = JsonPathGrammar.Parse(output.Expression);
            var value = expression.Evaluate(response.JsonResponse);
            output.Value = value.ToString();

            return TaskConstants.Completed;
        }
    }
}