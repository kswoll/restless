using System.Collections.Generic;
using Restless.Models;

namespace Restless.Utils.Outputs
{
    public static class OutputProcessorRegistry
    {
        private static readonly Dictionary<ApiOutputType, IOutputProcessor> processors = new Dictionary<ApiOutputType, IOutputProcessor>();

        static OutputProcessorRegistry()
        {
            processors[ApiOutputType.Default] = new DefaultOutputProcessor();
            processors[ApiOutputType.JsonPath] = new JsonPathOutputProcessor();
        }

        public static IOutputProcessor GetProcessor(ApiOutputType outputType)
        {
            return processors[outputType];
        }
    }
}