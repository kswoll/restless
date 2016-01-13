using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Restless.ViewModels;

namespace Restless.Controls.ResponseVisualizers
{
    public static class ResponseVisualizerRegistry
    {
        private static readonly Dictionary<string, List<Type>> visualizersByContentType = new Dictionary<string, List<Type>>();

        static ResponseVisualizerRegistry()
        {
            foreach (var type in typeof(ResponseVisualizerRegistry).Assembly.GetTypes().Where(x => x.IsDefined(typeof(ResponseVisualizerAttribute), true)))
            {
                RegisterVisualizer(type);
            }
        }

        public static void RegisterVisualizer(Type type)
        {
            var attribute = type.GetCustomAttribute<ResponseVisualizerAttribute>();
            foreach (var contentType in attribute.ContentTypes)
            {
                List<Type> list;
                if (!visualizersByContentType.TryGetValue(contentType, out list))
                {
                    list = new List<Type>();
                    visualizersByContentType[contentType] = list;
                }
                list.Add(type);
            }
        }

        public static IEnumerable<IResponseVisualizer> GetVisualizers(ApiResponseModel model)
        {
            var contentType = model.ContentType;
            if (contentType != null)
            {
                contentType = contentType.Split(';').First();

                List<Type> list;
                if (visualizersByContentType.TryGetValue(contentType, out list))
                {
                    foreach (var type in list)
                    {
                        var visualizer = (IResponseVisualizer)Activator.CreateInstance(type);
                        visualizer.Model = model;
                        yield return visualizer;
                    }
                }
            }
        }
    }
}