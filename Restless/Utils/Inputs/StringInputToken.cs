using System;
using PEG.Builder;

namespace Restless.Utils.Inputs
{
    public class StringInputToken : InputToken
    {
        [Consume]
        public string Value { get; set; }

        public override string Format(Func<string, string> valueProvider)
        {
            return Value;
        }
    }
}