using System;
using System.Threading.Tasks;
using Restless.Models;
using Restless.ViewModels;

namespace Restless.Utils.Outputs
{
    public class DefaultOutputProcessor : IOutputProcessor
    {
        public Task ProcessOutput(ApiResponseModel response, ApiOutputModel output)
        {
            switch (response.ContentType)
            {
                case ContentTypes.ApplicationJson:
                    return OutputProcessorRegistry.GetProcessor(ApiOutputType.JsonPath).ProcessOutput(response, output);
                default:
                    throw new Exception($"No output processor defined for Content-Type: {response.ContentType}");
            }
        }
    }
}