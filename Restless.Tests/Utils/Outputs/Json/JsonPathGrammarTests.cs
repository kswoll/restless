using NUnit.Framework;
using Restless.Utils.Outputs.Json;

namespace Restless.Tests.Utils.Outputs.Json
{
    [TestFixture]
    public class JsonPathGrammarTests
    {
        [Test]
        public void RootReference()
        {
            var expression = (JsonPathPropertyExpression)JsonPathGrammar.Parse("foo");
            Assert.IsNull(expression.Target);
            Assert.AreEqual("foo", expression.Name);
        }

        [Test]
        public void SubReference()
        {
            var expression = (JsonPathPropertyExpression)JsonPathGrammar.Parse("foo.bar");
            var target = (JsonPathPropertyExpression)expression.Target;
            Assert.IsNull(target.Target);
            Assert.AreEqual("foo", target.Name);
            Assert.AreEqual("bar", expression.Name);
        }

        [Test]
        public void Indexer()
        {
            var expression = (JsonPathIndexerExpression)JsonPathGrammar.Parse("foo[10]");
            var target = (JsonPathPropertyExpression)expression.Target;
            Assert.IsNull(target.Target);
            Assert.AreEqual("foo", target.Name);
            Assert.AreEqual(10, expression.Index);
        }
    }
}