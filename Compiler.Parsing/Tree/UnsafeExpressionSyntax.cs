using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class UnsafeExpressionSyntax : ExpressionSyntax, IUnsafeExpressionSyntax
{
    public IExpressionSyntax Expression { [DebuggerStepThrough] get; }

    public UnsafeExpressionSyntax(TextSpan span, IExpressionSyntax expression)
        : base(span)
    {
        Expression = expression;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => $"unsafe ({Expression})";
}
