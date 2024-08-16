using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal abstract class ExpressionSyntax : CodeSyntax, IExpressionSyntax
{
    protected ExpressionSyntax(TextSpan span)
        : base(span)
    {
    }

    public abstract OperatorPrecedence ExpressionPrecedence { get; }
}
