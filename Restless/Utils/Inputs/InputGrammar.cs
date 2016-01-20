using PEG;
using PEG.Builder;
using PEG.SyntaxTree;

namespace Restless.Utils.Inputs
{
    public class InputGrammar : Grammar<InputGrammar>
    {
        public static readonly InputGrammar Instance = Create();
        public static readonly PegParser<InputString> Parser = new PegParser<InputString>(Instance);

        public static InputString Parse(string s)
        {
            return Parser.Parse(s);
        }

        public virtual Expression Root() => -Token();

        public virtual Expression Token()
        {
            return VariableToken() | StringToken();
        }

        [Ast(typeof(StringInputToken))]
        public virtual Expression StringToken()
        {
            return +(!VariableToken() + Any);
        }

        [Ast(typeof(VariableInputToken))]
        public virtual Expression VariableToken()
        {
            return "%{" + VariableName() + '}';
        }

        public virtual Expression VariableName()
        {
            return +(!("%{"._() | '}') + Any);
        }
    }
}