using System;
using System.Collections.Generic;
using T = Suneido.Language.Token;

namespace Suneido.Language
{
	internal class ParseExpression<TAst> : Parse<TAst>
	{
		internal ParseExpression(Lexer lexer, ParseOutput<TAst> builder) 
			: base(lexer, builder)
		{
		}

		internal TAst parse()
		{
			return matchReturn(T.EOF, expression());
		}

		TAst expression()
		{
			return conditionalExpression();
		}

		private TAst conditionalExpression()
		{
			TAst expr = orExpression();
			if (matchIf(T.Q_MARK)) {
				++statementNest;
				TAst t = expression();
				match(T.COLON);
				--statementNest;
				TAst f = expression();
				return builder.conditional(expr, t, f);
			} else
				return expr;
		}

		private TAst orExpression()
		{
			TAst expr = andExpression();
			while (matchIf(T.OR))
				expr = builder.or(expr, andExpression());
			return expr;
		}

		private TAst andExpression()
		{
			TAst expr = inExpression();
			while (matchIf(T.AND))
				expr = builder.and(expr, inExpression());
			return expr;
		}

		TAst inExpression()
		{
			return addExpression(); // TODO
		}

		private TAst addExpression()
		{
			TAst expr = mulExpression();
			while (token == T.ADD || token == T.SUB || token == T.CAT) {
				Token op = token;
				match();
				expr = builder.binary(op, expr, mulExpression());
			}
			return expr;
		}

		private TAst mulExpression()
		{
			TAst result = unaryExpression();
			while (token == T.MUL || token == T.DIV || token == T.MOD) {
				Token op = token;
				match();
				result = builder.binary(op, result, unaryExpression());
			}
			return result;
		}

		TAst unaryExpression()
		{
			if (token == T.NUMBER || token == T.STRING)
				return matchReturn(builder.constant(token, value));
			else if (token == T.IDENTIFIER)
				return matchReturn(builder.identifier(value));
			else
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
			var parse = new ParseExpression<AstNode>(new Lexer(src), new AstOutput());
			AstNode result = parse.parse();
			Assert.That(result.ToString(), Is.EqualTo(expected));
		}
	}

}