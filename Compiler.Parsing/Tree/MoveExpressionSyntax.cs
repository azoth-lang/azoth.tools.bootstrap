using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class MoveExpressionSyntax : DataTypedExpressionSyntax, IMoveExpressionSyntax
{
    public ISimpleNameSyntax Referent { [DebuggerStepThrough] get; }

    public MoveExpressionSyntax(TextSpan span, ISimpleNameSyntax referent)
        : base(span)
    {
        Referent = referent;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Min;

    public override string ToString() => $"move {Referent}";
}
