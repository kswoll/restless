using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PEG.Builder;

namespace Restless.Utils.Inputs
{
    public class InputString
    {
        [Consume]
        public List<InputToken> Tokens { get; set; }

        public string Format(Func<string, string> valueProvider)
        {
            var builder = new StringBuilder();
            foreach (var token in Tokens)
            {
                builder.Append(token.Format(valueProvider));
            }
            return builder.ToString();
        }
    }
}