using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal class UnsafeExpression : Expression, IUnsafeExpression
{
    public IExpression Expression { get; }

    public UnsafeExpression(
        TextSpan span,
        DataType dataType,
        ExpressionSemantics semantics,
        IExpression expression)
        : base(span, dataType, semantics)
    {
        Expression = expression;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => $"unsafe ({Expression})";
}
