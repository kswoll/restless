using Newtonsoft.Json.Linq;

namespace Restless.Utils.Outputs.Json
{
    public abstract class JsonPathExpression
    {
        public abstract JToken Evaluate(JToken token);
    }
}