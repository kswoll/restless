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
        private static readonly List<Tuple<Func<ApiResponseModel, bool>, Type>> visualizersWithPredicate = new List<Tuple<Func<ApiResponseModel, bool>, Type>>();

        static ResponseVisualizerRegistry()
        {
            foreach (var type in typeof(ResponseVisualizerRegistry).Assembly.GetTypes().Where(x => typeof(IResponseVisualizer).IsAssignableFrom(x)))
            {
                RegisterVisualizer(type);
            }
        }

        public static void RegisterVisualizer(Type type)
        {
            var attribute = type.GetCustomAttribute<ResponseVisualizerAttribute>();
            if (attribute != null)
            {
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

            var predicateMethod = type.GetMethods().Where(x => x.IsDefined(typeof(ResponseVisualizerPredicateAttribute))).SingleOrDefault();
            if (predicateMethod != null)
            {
                if (!predicateMethod.IsStatic)
                    throw new Exception("ResponseActionPredicate must be a static method");
                if (predicateMethod.ReturnType != typeof(bool))
                    throw new Exception("Return type of a ResponseActionPredicate must be bool");
                if (predicateMethod.GetParameters().Length != 1 || predicateMethod.GetParameters()[0].ParameterType != typeof(ApiResponseModel))
                    throw new Exception("ResponseActionPredicate must declare one parameter of type ApiResponseModel");
                RegisterVisualizer(type, x => (bool)predicateMethod.Invoke(null, new[] { x }));
            }
        }

        public static void RegisterVisualizer(Type type, Func<ApiResponseModel, bool> predicate)
        {
            visualizersWithPredicate.Add(Tuple.Create(predicate, type));
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

            foreach (var item in visualizersWithPredicate)
            {
                var isVisible = item.Item1(model);
                if (isVisible)
                {
                    var visualizer = (IResponseVisualizer)Activator.CreateInstance(item.Item2);                    
                    visualizer.Model = model;
                    yield return visualizer;                    
                }
            }
        }
    }
}