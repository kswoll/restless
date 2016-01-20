using PEG;
using PEG.Builder;
using PEG.SyntaxTree;

// ReSharper disable FunctionRecursiveOnAllPaths

namespace Restless.Utils.Outputs.Json
{
    public class JsonPathGrammar : Grammar<JsonPathGrammar>
    {
        public static readonly JsonPathGrammar Instance = Create();
        public static readonly PegParser<JsonPathExpression> Parser = new PegParser<JsonPathExpression>(Instance);

        public static JsonPathExpression Parse(string s)
        {
            return Parser.Parse(s);
        }

        public virtual Expression Root() => PathExpression();

        public virtual Expression PathExpression()
        {
            return IndexerExpression() | PropertyExpression();
        }

        [Ast(typeof(JsonPathPropertyExpression))]
        public virtual Expression PropertyExpression()
        {
            return ~(PathExpression() + '.') + Identifier();
        }

        [Ast(typeof(JsonPathIndexerExpression))]
        public virtual Expression IndexerExpression()
        {
            return PathExpression() + '[' + Integer() + ']';
        }

        public virtual Expression Identifier()
        {
            return 'a'.To('Z') + -('a'.To('Z') | '0'.To('9'));
        }

        public virtual Expression Integer()
        {
            return +'0'.To('9');
        }
    }
}