using System;
using T = Suneido.Language.Token;
using System.Linq.Expressions;

namespace Suneido.Language
{
	internal class DyExprOutput : ParseOutput<Expression>
	{
		public Expression conditional(Expression cond, Expression iftrue, Expression iffalse)
		{
			return Expression.Condition(cond, iftrue, iffalse);
		}

		public Expression and(Expression left, Expression right)
		{
			return Expression.AndAlso(left, right);
		}

		public Expression or(Expression left, Expression right)
		{
			return Expression.OrElse(left, right);
		}

		public Expression binary(Token op, Expression left, Expression right)
		{
			if (op == T.ADD)
				return Expression.Add(left, right);
			else if (op == T.SUB)
				return Expression.Subtract(left, right);
//			else if (op == T.CAT)
//				// TODO call string.Concat
			else if (op == T.MUL)
				return Expression.Multiply(left, right);
			else if (op == T.DIV)
				return Expression.Divide(left, right);
			else if (op == T.MOD)
				return Expression.Modulo(left, right);
			else
				throw new ArgumentException("DyExprOutput binary invalid op " + op);
		}

		public Expression constant(Token type, string value)
		{
			if (type == T.STRING)
				return Expression.Constant(value);
			else if (type == T.NUMBER)
				return Expression.Constant(Convert.ToInt32(value)); // TODO float
			else
				throw new ArgumentException("DyExprOutput constant invalid type " + type);
		}

		public Expression identifier(string name)
		{
			throw new NotImplementedException();
//			return Expression.Parameter(T.IDENTIFIER, value);
		}
	}
}

namespace Suneido.Language
{
	using NUnit.Framework;

	[TestFixture]
	public class DyExprTest
	{
		[Test]
		public void Parse()
		{
			parse("123 + 456", "(123 + 456)");
		}

		void parse(string src, string expected)
		{
			var expr = parse(src);
			Assert.That(expr.ToString(), Is.EqualTo(expected));
		}

		[Test]
		public void Execute()
		{
			execute("123 + 456", "579");
			execute("2 * 3 + 4", "10");
		}

		void execute(string src, string expected)
		{
			var expr = parse(src);
			var action = Expression.Lambda<Func<Int32>>(expr).Compile();
			object result = action();
			Assert.That(result.ToString(), Is.EqualTo(expected));
		}

		static Expression parse(string src)
		{
			var parse = new ParseExpression<Expression>(new Lexer(src), new DyExprOutput());
			return parse.parse();
		}
	}
}
