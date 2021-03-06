using System;
using System.Collections.Generic;
using Suneido.Utility;

namespace Suneido.Language
{
	public class Token
	{
		public readonly string Name;
		public static readonly Dictionary<string,Token> Keywords = 
			new Dictionary<string, Token>();
		readonly Feature feature;

		class Feature { };
		static Feature INFIX = new Feature();


		Token(string name, Feature feature = null)
		{
			Name = name;
			if (name.IsLower())
				Keywords[name] = this;
			this.feature = feature;
		}

		public override int GetHashCode()
		{
			return Name.GetHashCode();
		}

		public bool infix()
		{
			return feature == INFIX;
		}

		public override string ToString()
		{
			return Name;
		}

		public static Token 
			NIL 		= new Token("NIL"),
			EOF 		= new Token("EOF"),
			ERROR 		= new Token("ERROR"),
			IDENTIFIER 	= new Token("IDENTIFIER"),
			NUMBER 		= new Token("NUMBER"),
			STRING 		= new Token("STRING"),
			WHITE 		= new Token("WHITE"),
			COMMENT 	= new Token("COMMENT"),
			NEWLINE 	= new Token("NEWLINE"),

			// operators and punctuation
			HASH 		= new Token("#"),
			COMMA 		= new Token(","),
			COLON 		= new Token(":"),
			SEMICOLON 	= new Token(";"),
			Q_MARK 		= new Token("?", INFIX),
			AT 			= new Token("@"),
			DOT 		= new Token("."),
			L_PAREN 	= new Token("("),
			R_PAREN 	= new Token(")"),
			L_BRACKET 	= new Token("["),
			R_BRACKET 	= new Token("]"),
			L_CURLY 	= new Token("{"),
			R_CURLY 	= new Token("}"),
			IS 			= new Token("==", INFIX),
			ISNT 		= new Token("!=", INFIX),
			MATCH 		= new Token("=~", INFIX),
			MATCHNOT 	= new Token("!~", INFIX),
			LT 			= new Token("<", INFIX),
			LTE 		= new Token("<=", INFIX),
			GT 			= new Token(">", INFIX),
			GTE 		= new Token(">=", INFIX),
			NOT 		= new Token("not"),
			INC 		= new Token("++"),
			DEC 		= new Token("--"),
			BITNOT		= new Token("~", INFIX),
			ADD 		= new Token("+", INFIX),
			SUB 		= new Token("-", INFIX),
			CAT 		= new Token("$", INFIX),
			MUL 		= new Token("*", INFIX),
			DIV 		= new Token("/", INFIX),
			MOD 		= new Token("%", INFIX),
			LSHIFT 		= new Token("<<", INFIX),
			RSHIFT 		= new Token(">>", INFIX),
			BITOR 		= new Token("|", INFIX),
			BITAND 		= new Token("&", INFIX),
			BITXOR 		= new Token("^", INFIX),
			EQ 			= new Token("=", INFIX),
			ADDEQ 		= new Token("+=", INFIX),
			SUBEQ 		= new Token("-=", INFIX),
			CATEQ 		= new Token("$=", INFIX),
			MULEQ 		= new Token("*=", INFIX),
			DIVEQ 		= new Token("/=", INFIX),
			MODEQ 		= new Token("%=", INFIX),
			LSHIFTEQ 	= new Token("<<=", INFIX),
			RSHIFTEQ 	= new Token(">>=", INFIX),
			BITOREQ 	= new Token("|=", INFIX),
			BITANDEQ 	= new Token("&=", INFIX),
			BITXOREQ 	= new Token("^=", INFIX),
			RANGETO 	= new Token(".."),
			RANGELEN 	= new Token("::"),

			// langauge keywords
			AND 		= new Token("and", INFIX),
			BREAK 		= new Token("break"),
			CALLBACK 	= new Token("callback"),
			CASE 		= new Token("case"),
			CATCH 		= new Token("catch"),
			CLASS 		= new Token("class"),
			CONTINUE 	= new Token("continue"),
			CREATE 		= new Token("create"),
			DEFAULT 	= new Token("default"),
			DLL 		= new Token("dll"),
			DO 			= new Token("do"),
			ELSE 		= new Token("else"),
			FALSE 		= new Token("false"),
			FOR 		= new Token("for"),
			FOREVER 	= new Token("forever"),
			FUNCTION 	= new Token("function"),
			IF 			= new Token("if"),
			IN 			= new Token("in"),
			NEW 		= new Token("new"),
			OR 			= new Token("or", INFIX),
			RETURN 		= new Token("return"),
			STRUCT 		= new Token("struct"),
			SWITCH 		= new Token("switch"),
			SUPER 		= new Token("super"),
			THIS 		= new Token("this"),
			THROW 		= new Token("throw"),
			TRUE 		= new Token("true"),
			TRY 		= new Token("try"),
			WHILE 		= new Token("while"),

			// query keywords
			ALTER 		= new Token("alter"),
			AVERAGE 	= new Token("average"),
			CASCADE 	= new Token("cascade"),
			COUNT 		= new Token("count"),
			DELETE 		= new Token("delete"),
			DROP 		= new Token("drop"),
			ENSURE 		= new Token("ensure"),
			EXTEND 		= new Token("extend"),
			HISTORY 	= new Token("history"),
			INDEX 		= new Token("index"),
			INSERT 		= new Token("insert"),
			INTERSECT 	= new Token("intersect"),
			INTO 		= new Token("into"),
			JOIN 		= new Token("join"),
			KEY 		= new Token("key"),
			LEFTJOIN 	= new Token("leftjoin"),
			LIST 		= new Token("list"),
			MAX 		= new Token("max"),
			MIN 		= new Token("min"),
			MINUS 		= new Token("minus"),
			PROJECT 	= new Token("project"),
			REMOVE 		= new Token("remove"),
			RENAME 		= new Token("rename"),
			REVERSE 	= new Token("reverse"),
			SET 		= new Token("set"),
			SORT 		= new Token("sort"),
			SUMMARIZE 	= new Token("summarize"),
			SVIEW 		= new Token("sview"),
			TIMES 		= new Token("times"),
			TO 			= new Token("to"),
			TOTAL 		= new Token("total"),
			UNION 		= new Token("union"),
			UNIQUE 		= new Token("unique"),
			UPDATE 		= new Token("update"),
			UPDATES 	= new Token("updates"),
			VIEW 		= new Token("view"),
			WHERE 		= new Token("where"),

			// for AST
			ARG 		= new Token("ARG"),
			ASSIGNOP 	= new Token("ASSIGNOP"),
			BINARYOP 	= new Token("BINARYOP"),
			BLOCK 		= new Token("BLOCK"),
			CALL 		= new Token("CALL"),
			DATE 		= new Token("DATE"),
			FOR_IN 		= new Token("FOR_IN"),
			MEMBER 		= new Token("MEMBER"),
			METHOD 		= new Token("METHOD"),
			OBJECT 		= new Token("OBJECT"),
			POSTINCDEC 	= new Token("POSTINCDEC"),
			PREINCDEC 	= new Token("PREINCDEC"),
			RECORD 		= new Token("RECORD"),
			RVALUE 		= new Token("RVALUE"),
			SELFREF 	= new Token("SELFREF"),
			SUBSCRIPT 	= new Token("SUBSCRIPT"),
			SYMBOL 		= new Token("SYMBOL");
	}

}

namespace Suneido.Language
{
	using NUnit.Framework;

	[TestFixture]
	public class TokensTest
	{
		[Test]
		public void Basic()
		{
			Assert.That(Token.NIL.ToString(), Is.EqualTo("NIL"));
			Assert.That(Token.Keywords["where"], Is.EqualTo(Token.WHERE));
		}
	}
}
