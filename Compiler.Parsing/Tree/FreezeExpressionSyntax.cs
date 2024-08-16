using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class FreezeExpressionSyntax : ExpressionSyntax, IFreezeExpressionSyntax
{
    public ISimpleNameSyntax Referent { [DebuggerStepThrough] get; }

    public FreezeExpressionSyntax(TextSpan span, ISimpleNameSyntax referent)
        : base(span)
    {
        Referent = referent;
    }

    public override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

    public override string ToString() => $"freeze {Referent}";
}
