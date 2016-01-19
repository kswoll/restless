using System;

namespace Restless.Utils.Inputs
{
    public abstract class InputToken
    {
        public abstract string Format(Func<string, string> valueProvider);
    }
}