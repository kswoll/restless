using System.Linq;
using NUnit.Framework;
using Restless.Utils.Inputs;

namespace Restless.Tests.Utils.Inputs
{
    [TestFixture]
    public class InputGrammarTests
    {
        [Test]
        public void OneStringToken()
        {
            var s = "one";
            var inputs = InputGrammar.Parser.Parse(s);
            Assert.AreEqual("one", ((StringInputToken)inputs.Tokens.Single()).Value);
        }

        [Test]
        public void OneVariableToken()
        {
            var s = "%{one}";
            var inputs = InputGrammar.Parser.Parse(s);
            Assert.AreEqual("one", ((VariableInputToken)inputs.Tokens.Single()).Variable);
        }

        [Test]
        public void MixedTokens()
        {
            var s = "%{one}plus%{two}equals%{three}";
            var inputs = InputGrammar.Parser.Parse(s);
            Assert.AreEqual("one", ((VariableInputToken)inputs.Tokens[0]).Variable);
            Assert.AreEqual("plus", ((StringInputToken)inputs.Tokens[1]).Value);
            Assert.AreEqual("two", ((VariableInputToken)inputs.Tokens[2]).Variable);
            Assert.AreEqual("equals", ((StringInputToken)inputs.Tokens[3]).Value);
            Assert.AreEqual("three", ((VariableInputToken)inputs.Tokens[4]).Variable);
        }
    }
}