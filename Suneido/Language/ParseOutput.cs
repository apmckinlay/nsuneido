using System;
using T = Suneido.Language.Token;

namespace Suneido.Language
{

	internal interface ParseOutput<TExpr>
	{
		TExpr conditional(TExpr cond, TExpr iftrue, TExpr iffalse);
		TExpr and(TExpr left, TExpr right);
		TExpr or(TExpr left, TExpr right);

		TExpr binary(Token op, TExpr left, TExpr right);

		TExpr constant(Token type, string value);

		TExpr identifier(string value);
	}

}

