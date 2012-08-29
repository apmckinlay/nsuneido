using System;
using T = Suneido.Language.Token;

namespace Suneido.Language
{
	internal class AstOutput : ParseOutput<AstNode>
	{
		public AstNode conditional(AstNode cond, AstNode iftrue, AstNode iffalse)
		{
			return new AstNode(T.Q_MARK, iftrue, iffalse);
		}

		public AstNode and(AstNode left, AstNode right)
		{
			return new AstNode(T.AND, left, right);
		}

		public AstNode or(AstNode left, AstNode right)
		{
			return new AstNode(T.OR, left, right);
		}

		public AstNode binary(Token op, AstNode left, AstNode right)
		{
			return new AstNode(op, left, right);
		}

		public AstNode constant(Token type, string value)
		{
			return new AstNode(type, value);
		}

		public AstNode identifier(string name)
		{
			return new AstNode(T.IDENTIFIER, name);
		}
	}
}

