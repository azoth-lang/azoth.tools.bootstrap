using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Tokens;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class IdExpressionSyntax : ExpressionSyntax, IIdExpressionSyntax
{
    public IExpressionSyntax Referent { [DebuggerStepThrough] get; }

    public IdExpressionSyntax(TextSpan span, IExpressionSyntax referent)
        : base(span, ExpressionSemantics.IdReference)
    {
        Referent = referent;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

    public override string ToString() => $"id {Referent}";
}
