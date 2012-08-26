using System;
using System.Collections.Generic;
using System.Text;
using Suneido.Utility;

namespace Suneido.Language
{
	// immutable
	public class AstNode
	{
		public readonly Token token;
		public readonly string value;
		readonly AstNode[] children;

		public AstNode(Token token, string value)
		{
			this.token = token;
			this.value = value;
			children = new AstNode[0];
		}

		public AstNode(Token token, params AstNode[] children)
		{
			this.token = token;
			this.children = children;
		}

		public AstNode(Token token, List<AstNode> children)
		{
			this.token = token;
			this.children = children.ToArray();
		}

		public object this[int i] 
			{ get { return children[i]; } }

		public override string ToString()
		{
			return toString(0);
		}

		string toString(int indent) 
		{
			bool multi = multiline();
			string sep = multi ? "\n" : " ";
			int childIndent = multi ? indent + 3 : 0;

			StringBuilder sb = new StringBuilder();
			sb.Append(" ".Repeat(indent));
			sb.Append('(').Append(token);
			if (value != null)
				sb.Append("=").Append(value);
			if (children != null)
				foreach (AstNode x in children)
					sb.Append(sep).Append(x == null
							? " ".Repeat(childIndent) + "null"
							: x.toString(childIndent));
			sb.Append(')');
			return sb.ToString();
		}

		bool multiline() 
		{
			const int MAX = 70;
			int total = 0;
			foreach (AstNode x in children) {
				String s = (x == null) ? "null" : x.ToString();
				if (s.Contains("\n"))
					return true;
				int n = s.Length;
				total += n;
				if (n > MAX || total > MAX)
					return true;
			}
			return false;
		}

	}

}

namespace Suneido.Language
{
	using NUnit.Framework;

	[TestFixture]
	public class AstNodeTest
	{
		[Test]
		public void toString()
		{
			test(new AstNode(Token.IDENTIFIER, "fred"), "(IDENTIFIER=fred)");
			test(new AstNode(Token.ADD,
				new AstNode(Token.NUMBER, "123"),
				new AstNode(Token.NUMBER, "456")), 
			     "(+ (NUMBER=123) (NUMBER=456))");
		}

		void test(AstNode node, string expected)
		{
			Assert.That(node.ToString(), Is.EqualTo(expected));
		}
	}

}