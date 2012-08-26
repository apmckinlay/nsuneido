using System;
using System.Collections.Generic;
using T = Suneido.Language.Token;

namespace Suneido.Language
{
	public class ParseExpression : Parse
	{
		public ParseExpression(Lexer lexer) : base(lexer)
		{
		}

		public AstNode parse()
		{
			return matchReturn(T.EOF, expression());
		}

		AstNode expression()
		{
			return conditionalExpression();
		}

		private AstNode conditionalExpression() {
			AstNode expr = orExpression();
			if (token == T.Q_MARK) 
			{
				++statementNest;
				match(T.Q_MARK);
				AstNode t = expression();
				match(T.COLON);
				--statementNest;
				AstNode f = expression();
				return new AstNode(T.Q_MARK, expr, t, f);
			} 
			else
				return expr;
		}

		private AstNode orExpression()
		{
			AstNode expr = andExpression();
			if (token != T.OR)
				return expr;
			var list = new List<AstNode>() { expr };
			while (matchIf(T.OR))
				list.Add(andExpression());
			return new AstNode(T.OR, list);
		}

		private AstNode andExpression()
		{
			AstNode expr = inExpression();
			if (token != T.AND)
				return expr;
			var list = new List<AstNode>() { expr };
			while (matchIf(T.AND))
				list.Add(inExpression());
			return new AstNode(T.AND, list);
		}

		AstNode inExpression()
		{
			return addExpression(); // TODO
		}

	 	private AstNode addExpression()
		{
			AstNode result = mulExpression();
			while (token == T.ADD || token == T.SUB || token == T.CAT) {
				Token op = token;
				match();
				result = new AstNode(op, result, mulExpression());
			}
			return result;
		}

		private AstNode mulExpression()
		{
			AstNode result = unaryExpression();
			while (token == T.MUL || token == T.DIV || token == T.MOD) {
				Token op = token;
				match();
				result = new AstNode(op, result, unaryExpression());
			}
			return result;
		}

		AstNode unaryExpression()
		{
			if (token == T.NUMBER || token == T.STRING || token == T.IDENTIFIER)
				return matchReturn(new AstNode(token, value));
			throw syntaxError();
		}

	}
}

namespace Suneido.Language
{
	using NUnit.Framework;

	[TestFixture]
	public class ParseExpressionTest
	{
		[Test]
		public void Basic()
		{
			test("123", "(NUMBER=123)");
			test("x + 2", "(+ (IDENTIFIER=x) (NUMBER=2))");
		}

		void test(string src, string expected)
		{
			var parse = new ParseExpression(new Lexer(src));
			AstNode result = parse.parse();
			Assert.That(result.ToString(), Is.EqualTo(expected));
		}
	}

}