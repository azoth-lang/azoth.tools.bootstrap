using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class MoveExpressionSyntax : ExpressionSyntax, IMoveExpressionSyntax
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
