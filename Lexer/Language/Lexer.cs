using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Suneido.Utility;
using T = Suneido.Language.Token;
using System.Text;

namespace Suneido.Language
{
	public class Source : IEnumerable<Token>
	{
		readonly string src;

		public Source(string src)
		{
			this.src = src;
		}

		public IEnumerator<Token> GetEnumerator()
		{
			return new Lexer(src);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

	}

	class Lexer : IEnumerator<Token>
	{
		readonly string src;
		public int si { get; private set; }
		public int prev { get; private set; }
		Token token;
		public string Value { get; private set; }
		public Token Keyword { get; private set; }

		public Lexer(string src)
		{
			this.src = src;
			si = 0;
		}

		public bool MoveNext()
		{
			token = nextToken();
			return token != T.EOF;
		}

		private Token nextToken()
		{
			prev = si;
			if (si >= src.Length)
				return T.EOF;
			char c = src[si];
			if (Char.IsWhiteSpace(c))
				return whitespace();
			++si;
			switch (c)
			{
			case '#':
				return T.HASH;
			case '(': 
				return T.L_PAREN;
			case ')':
				return T.R_PAREN;
			case ',': 
				return T.COMMA;
			case ';':
				return T.SEMICOLON;
			case '?':
				return T.Q_MARK;
			case '@': 
				return T.AT;
			case '[':
				return T.L_BRACKET;
			case ']': 
				return T.R_BRACKET;
			case '{':
				return T.L_CURLY;
			case '}':
				return T.R_CURLY;
			case '~':
				return T.BITNOT;
			case ':':
				return match(':') ? T.RANGELEN : T.COLON;
			case '=' :
				return match('=') ? T.IS : match('~') ? T.MATCH : T.EQ;
			case '!':
				return match('=') ? T.ISNT : match('~') ? T.MATCHNOT : T.NOT;
			case '<':
				return match('<') ? (match('=') ? T.LSHIFTEQ : T.LSHIFT)
					: match('>') ? T.ISNT : match('=') ? T.LTE : T.LT;
			case '>':
				return match('>') ? (match('=') ? T.RSHIFTEQ : T.RSHIFT)
					: match('=') ? T.GTE : T.GT;
			case '|':
				return match('|') ? T.OR : match('=') ? T.BITOREQ : T.BITOR;
			case '&':
				return match('&') ? T.AND : match('=') ? T.BITANDEQ : T.BITAND;
			case '^':
				return match('=') ? T.BITXOREQ : T.BITXOR;
			case '-':
				return match('-') ? T.DEC : match('=') ? T.SUBEQ : T.SUB;
			case '+':
				return match('+') ? T.INC : match('=') ? T.ADDEQ : T.ADD;
			case '/':
				return match('/') ? lineComment() : match('*') ? spanComment()
					: match('=') ? T.DIVEQ : T.DIV;
			case '*':
				return match('=') ? T.MULEQ : T.MUL;
			case '%':
				return match('=') ? T.MODEQ : T.MOD;
			case '$':
				return match('=') ? T.CATEQ : T.CAT;
			case '`':
				return rawString();
			case '"':
			case '\'':
				return quotedString(c);
			case '.':
				return match('.') ? T.RANGETO
					: Char.IsDigit(src[si]) ? number()
					: T.DOT;
			default:
				return Char.IsDigit(c) ? number()
					: (c == '_' || Char.IsLetter(c)) ? identifier() 
					: T.ERROR;
			}
		}

		private Token whitespace() {
			bool eol = false;
			for (; si < src.Length && Char.IsWhiteSpace(src[si]); ++si)
				if (src[si] == '\n' || src[si] == '\r')
					eol = true;
			return eol ? T.NEWLINE : T.WHITE;
		}

		bool match(char c)
		{
			if (si >= src.Length || src[si] != c)
				return false;
			++si;
			return true;
		}

		bool match(Func<char,bool> pred)
		{
			if (si >= src.Length || ! pred(src[si]))
				return false;
			++si;
			return true;
		}

		bool matchWhile(Func<char,bool> pred)
		{
			int start = si;
			while (si < src.Length && pred(src[si]))
				++si;
			return si > start;
		}

		private Token lineComment()
		{
			matchWhile(c => c != '\r' && c != '\n');
			return T.COMMENT;
		}

		private Token spanComment()
		{
			for (++si; si + 1 < src.Length && (src[si] != '*' || src[si + 1] != '/'); ++si)
				;
			if (si < src.Length)
				si += 2;
			return T.COMMENT;
		}

		Token rawString()
		{
			matchWhile(c => c != '`');
			match('`');
			Value = src.Substring(prev + 1, si - prev - 2);
			return T.STRING;
		}

		#region quoted string
		Token quotedString(char quote)
		{
			var sb = new StringBuilder();
			while (si < src.Length && src[si] != quote)
				sb.Append(escape());
			match(quote);
			Value = sb.ToString();
			return T.STRING;
		}

		char escape()
		{
			if (! match('\\'))
				return src[si++];
			int save = si;
			int d1, d2, d3;
			if (match('n'))
				return '\n';
			else if (match('r'))
				return '\r';
			else if (match('t'))
				return '\t';
			else if (match('\\'))
				return '\\';
			else if (match('"'))
				return '"';
			else if (match('\''))
				return '\'';
			else if (match('x') && digit(16, out d1) && digit(16, out d2))
				return (char) (16 * d1 + d2);
			else if (digit(8, out d1) && digit(8, out d2) && digit(8, out d3))
				return (char) (64 * d1 + 8 * d2 + d3);
			else
			{
				si = save;
				return '\\';
			}
		}

		bool digit(int radix, out int dig)
		{
			char c = src[si];
			dig = Char.IsDigit(c) ? c - '0' 
				: isHexDigit(c) ? 10 + Char.ToLower(c) - 'a'
				: 99;
			return (dig < radix) ? match(c) : false;
		}
		#endregion
		
		#region number
		Token number()
		{
			--si;
			if (hexNumber())
				return T.NUMBER;
			matchWhile(Char.IsDigit);
			if (match('.'))
				matchWhile(Char.IsDigit);
			exponent();
			if (src[si - 1] == '.')
				--si; // don't absorb trailing period
			setValue();
			return T.NUMBER;
		}

		private bool hexNumber() {
			int save = si;
			if (match('0') && (match('x') || match('X')) &&
				matchWhile(isHexDigit))
				return true;
			si = save;
			return false;
		}

		bool isHexDigit(char c)
		{
			return Char.IsDigit(c) ||
				('a' <= c && c <= 'f') || ('A' <= c && c <= 'F');
		}

		private void exponent()
		{
			if (! match('e'))
				return;
			match(c => c == '+' || c == '-');
			matchWhile(Char.IsDigit);
		}
		#endregion

		Token identifier()
		{
			matchWhile(c => Char.IsLetterOrDigit(c) || c == '_');
			match(c => c == '!' || c == '?');
			setValue();

			Keyword = Token.Keywords.GetOrElse(Value, null);
			bool isop = isOperatorKeyword(Keyword);
			if (isop && si < src.Length && src[si] == ':')
				Keyword = null;
			return Keyword != null && isop ? Keyword : T.IDENTIFIER;
		}

		void setValue()
		{
			Value = src.Substring(prev, si - prev);
		}

		bool isOperatorKeyword(Token t)
		{
			return t != null && (t == T.IS || t == T.ISNT ||
				t == T.AND || t == T.OR || t == T.NOT);
		}

		#region IEnumerator
		public Token Current 
			{ get { return token; } }
		object IEnumerator.Current 
			{ get { return Current; } }

		public void Reset()
			{ si = 0; }

		public void Dispose()
			{ }
		#endregion

	}
}

namespace Suneido.Language
{
	using NUnit.Framework;

	[TestFixture]
	public class LexerTest
	{
		[Test]
		public void Basic()
		{
			Assert.False(new Lexer("").MoveNext());

			var lexer = new Lexer("#");
			Assert.True(lexer.MoveNext());
			Assert.That(lexer.Current, Is.EqualTo(T.HASH));

			test(" \t", T.WHITE);
			test(" \n \t", T.NEWLINE);
			test("#(),;?@[]{}~", 
			    T.HASH, T.L_PAREN, T.R_PAREN, T.COMMA, T.SEMICOLON, T.Q_MARK,
			    T.AT, T.L_BRACKET, T.R_BRACKET, T.L_CURLY, T.R_CURLY, T.BITNOT);
			test(":", T.COLON);
			test("::", T.RANGELEN);
			test("==", T.IS); 
			test("=~", T.MATCH);
			test("=", T.EQ);
			test("\n", T.NEWLINE);
			test("::", T.RANGELEN);
			test(":", T.COLON);
			test("==", T.IS);
			test("=~", T.MATCH);
			test("=", T.EQ);
			test("!=", T.ISNT);
			test("!~", T.MATCHNOT);
			test("!", T.NOT);
			test("<<=", T.LSHIFTEQ);
			test("<<", T.LSHIFT);
			test("<>", T.ISNT);
			test("<=", T.LTE);
			test("<", T.LT);
			test(">>=", T.RSHIFTEQ);
			test(">=", T.GTE);
			test(">", T.GT);
			test("||", T.OR);
			test("|=", T.BITOREQ);
			test("|", T.BITOR);
			test("&&", T.AND);
			test("&=", T.BITANDEQ);
			test("&", T.BITAND);
			test("^=", T.BITXOREQ);
			test("^", T.BITXOR);
			test("--", T.DEC);
			test("-=", T.SUBEQ);
			test("-", T.SUB);
			test("++", T.INC);
			test("+=", T.ADDEQ);
			test("+", T.ADD);
			test("// blah blah\n", T.COMMENT, T.NEWLINE);
			test("/* blah blah */+", T.COMMENT, T.ADD);
			test("/=", T.DIVEQ);
			test("/", T.DIV);
			test("*=", T.MULEQ);
			test("*", T.MUL);
			test("%=", T.MODEQ);
			test("%", T.MOD);
			test("$=", T.CATEQ);
			test("$", T.CAT);

			testVal("``", T.STRING, "");
			testVal("`hello`", T.STRING, "hello");
			testVal("'hello'", T.STRING, "hello");
			testVal("\"hello\"", T.STRING, "hello");
			testVal(@"'\t\r\n\'\""\\'", T.STRING, "\t\r\n\'\"\\");
			testVal(@"'\040'", T.STRING, " ");
			testVal(@"'\x20'", T.STRING, " ");

			testVal("fred", T.IDENTIFIER);
			testVal("where", T.IDENTIFIER, keyword: T.WHERE);
			testVal("and", T.AND, keyword: T.AND);
			test("is:", T.IDENTIFIER, T.COLON);
		
			testVal("0", T.NUMBER);
			test("0xx", T.NUMBER, T.IDENTIFIER);
			testVal("123", T.NUMBER);
			testVal("0123", T.NUMBER);
			test("123.", T.NUMBER, T.DOT);
			testVal(".123", T.NUMBER);
			testVal("123.456", T.NUMBER);
			test("3..6", T.NUMBER, T.RANGETO, T.NUMBER);
			testVal("1e6", T.NUMBER);
			testVal("1e+6", T.NUMBER);
			testVal("1e-6", T.NUMBER);
			testVal(".1e6", T.NUMBER);
			testVal("1.e6", T.NUMBER);
			testVal("1.1e6", T.NUMBER);
			testVal("0x7f", T.NUMBER);
		}

		void test(string s, params Token[] tokens)
		{
			var pairs = tokens.Concat(new[] { T.SEMICOLON })
				.Zip(new Source(s + ";"), (a, b) => new[] { a,b });
			foreach (var x in pairs)
				Assert.That(x[1], Is.EqualTo(x[0]));
		}

		void testVal(string s, Token token, string value = null, Token keyword = null)
		{
			var lexer = new Lexer(s + ";");
			lexer.MoveNext();
			Assert.That(lexer.Current, Is.EqualTo(token));
			if (value != null)
				Assert.That(lexer.Value, Is.EqualTo(value));
			Assert.That(lexer.Keyword, Is.EqualTo(keyword));

			lexer.MoveNext();
			Assert.That(lexer.Current, Is.EqualTo(T.SEMICOLON));
			Assert.IsFalse(lexer.MoveNext());
		}
	}
}
