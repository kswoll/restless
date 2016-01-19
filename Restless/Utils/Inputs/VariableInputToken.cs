using System;
using PEG.Builder;

namespace Restless.Utils.Inputs
{
    public class VariableInputToken : InputToken
    {
        [Consume("VariableName")]
        public string Variable { get; set; }

        public override string Format(Func<string, string> valueProvider)
        {
            return valueProvider(Variable);
        }
    }
}