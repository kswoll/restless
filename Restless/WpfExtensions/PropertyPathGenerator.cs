using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Windows;
using Expression = System.Linq.Expressions.Expression;

namespace Restless.WpfExtensions
{
    public class PropertyPathGenerator : ExpressionVisitor
    {
        private readonly List<IToken> tokens = new List<IToken>();

        public static PropertyPath CreatePropertyPath<T, TValue>(Expression<Func<T, TValue>> expression)
            where T : DependencyObject
        {
            var generator = new PropertyPathGenerator();
            generator.Visit(expression);

            var builder = new StringBuilder();
            foreach (var token in ((IEnumerable<IToken>)generator.tokens).Reverse())
            {
                if (token is MemberToken && builder.Length > 0)
                {
                    builder.Append('.');
                }
                builder.Append(token.ToString());
            }
            return new PropertyPath(builder.ToString());
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            var token = node.Member.Name;
            tokens.Add(new MemberToken(token));

            return base.VisitMember(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.Name == "get_Item")
            {
                var index = node.Arguments.Cast<ConstantExpression>().Single();
                var indexValue = (int)index.Value;
                tokens.Add(new IndexToken(indexValue));
            }
            else
            {
                throw new InvalidOperationException("Cannot invoke methods in a property expression");
            }
            return base.VisitMethodCall(node);
        }

        protected override Expression VisitIndex(IndexExpression node)
        {
            var index = node.Arguments.Cast<ConstantExpression>().Single();
            var indexValue = (int)index.Value;
            tokens.Add(new IndexToken(indexValue));

            return base.VisitIndex(node);
        }

        private interface IToken
        {
            string ToString();
        }

        private class MemberToken : IToken
        {
            public string Name { get; }

            public MemberToken(string name)
            {
                Name = name;
            }

            public override string ToString()
            {
                return Name;
            }
        }

        private class IndexToken : IToken
        {
            public int Index { get; }

            public IndexToken(int index)
            {
                Index = index;
            }

            public override string ToString()
            {
                return $"[{Index}]";
            }
        }
    }
}