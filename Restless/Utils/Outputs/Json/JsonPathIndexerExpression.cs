using Newtonsoft.Json.Linq;
using PEG.Builder;

namespace Restless.Utils.Outputs.Json
{
    public class JsonPathIndexerExpression : JsonPathExpression
    {
        [Consume("Integer")]
        public int Index { get; set; }

        [Consume("PathExpression")]
        public JsonPathExpression Target { get; set; }
        
        public override JToken Evaluate(JToken token)
        {
            return ((JArray)token)[Index];
        }
    }
}