using Newtonsoft.Json.Linq;
using PEG.Builder;

namespace Restless.Utils.Outputs.Json
{
    public class JsonPathPropertyExpression : JsonPathExpression
    {
        [Consume("Identifier")]
        public string Name { get; set; }

        [Consume("PathExpression")]
        public JsonPathExpression Target { get; set; }

        public override JToken Evaluate(JToken token)
        {
            if (Target != null)
            {
                var target = (JObject)Target.Evaluate(token);
                return target[Name];
            }
            else
            {
                return ((JObject)token)[Name];
            }
        }
    }
}