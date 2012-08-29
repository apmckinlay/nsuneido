using System;
using T = Suneido.Language.Token;

namespace Suneido.Language
{
	/// <summary>
	/// Base class for parsing
	/// </summary>
	internal class Parse<TAst>
	{
		readonly Lexer lexer;
		readonly Lexer ahead; // used for look ahead
		protected readonly ParseOutput<TAst> builder;
		protected int statementNest = 0;

		internal Parse(Lexer lexer, ParseOutput<TAst> builder)
		{
			this.lexer = lexer;
			ahead = new Lexer(lexer);
			lexer.MoveNext();
			this.builder = builder;
		}

		protected Token token
			{ get { return lexer.Current; }}
		protected string value
			{ get { return lexer.Value; }}

		internal TAst matchReturn(Token expected, TAst result)
		{
			match(expected);
			return result;
		}

		internal TAst matchReturn(TAst result)
		{
			match();
			return result;
		}

		protected bool matchIf(Token expected)
		{
			if (matches(expected)) {
				match();
				return true;
			} else
				return false;
		}

		protected void match(Token expected)
		{
			verify(expected);
			match();
		}

		protected void match()
		{
			moveNext();
			if (statementNest != 0 || token.infix() || lookAhead().infix())
				while (token == T.NEWLINE)
					moveNext();
		}

		private void moveNext()
		{
			if (token == T.L_CURLY || token == T.L_PAREN || token == T.L_BRACKET)
				++statementNest;
			else if (token == T.R_CURLY || token == T.R_PAREN || token == T.R_BRACKET)
				--statementNest;
			do
				lexer.MoveNext();
			while (token == T.WHITE);
		}

		private void verify(Token expected)
		{
			if (! matches(expected))
				throw syntaxError("expected: " + expected + " got: " + token);
		}

		private bool matches(Token expected)
		{
			return token == expected || lexer.Keyword == expected;
		}

		protected Exception syntaxError()
		{
			String value = lexer.Value;
			return syntaxError("unexpected " + token + (value == null ? "" : " " + value));
		}

		protected Exception syntaxError(string message)
		{
			return new Exception("syntax error: " + message);
		}

		Token lookAhead()
		{
			ahead.CopyPosition(lexer);
			ahead.MoveNext();
			return ahead.Current;
		}

	}
}

