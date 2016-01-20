using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Restless.ViewModels;

namespace Restless.Controls.ResponseActions
{
    public static class ResponseActionRegistry
    {
        private static readonly Dictionary<string, List<Type>> actionsByContentType = new Dictionary<string, List<Type>>();
        private static readonly List<Tuple<Func<ApiResponseModel, ResponseActionState>, Type>> actionsWithPredicate = new List<Tuple<Func<ApiResponseModel, ResponseActionState>, Type>>();

        static ResponseActionRegistry()
        {
            foreach (var type in typeof(ResponseActionRegistry).Assembly.GetTypes().Where(x => typeof(IResponseAction).IsAssignableFrom(x)))
            {
                RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                var attribute = type.GetCustomAttribute<ResponseActionAttribute>();
                if (attribute != null)
                {
                    foreach (var contentType in attribute.ContentTypes)
                    {
                        List<Type> list;
                        if (!actionsByContentType.TryGetValue(contentType, out list))
                        {
                            list = new List<Type>();
                            actionsByContentType[contentType] = list;
                        }                        
                    }
                }

                var predicateMethod = type.GetMethods().Where(x => x.IsDefined(typeof(ResponseActionPredicateAttribute))).SingleOrDefault();
                if (predicateMethod != null)
                {
                    if (!predicateMethod.IsStatic)
                        throw new Exception("ResponseActionPredicate must be a static method");
                    if (predicateMethod.ReturnType != typeof(ResponseActionState))
                        throw new Exception("Return type of a ResponseActionPredicate must be ResponseActionState");
                    if (predicateMethod.GetParameters().Length != 1 || predicateMethod.GetParameters()[0].ParameterType != typeof(ApiResponseModel))
                        throw new Exception("ResponseActionPredicate must declare one parameter of type ApiResponseModel");
                    RegisterAction(type, x => (ResponseActionState)predicateMethod.Invoke(null, new[] { x }));
                }
            }            
        }

        public static void RegisterAction(Type type, Func<ApiResponseModel, ResponseActionState> predicate)
        {
            actionsWithPredicate.Add(Tuple.Create(predicate, type));
        }

        public static IEnumerable<Tuple<IResponseAction, ResponseActionState>> GetActions(ApiResponseModel model)
        {
            var contentType = model.ContentType;
            if (contentType != null)
            {
                contentType = contentType.Split(';').First();

                List<Type> list;
                if (actionsByContentType.TryGetValue(contentType, out list))
                {
                    foreach (var type in list)
                    {
                        var action = (IResponseAction)Activator.CreateInstance(type);
                        yield return Tuple.Create(action, ResponseActionState.Enabled);
                    }
                }
            }
            foreach (var item in actionsWithPredicate)
            {
                var responseActionState = item.Item1(model);
                var action = (IResponseAction)Activator.CreateInstance(item.Item2);                    
                yield return Tuple.Create(action, responseActionState);
            }
        }
    }
}