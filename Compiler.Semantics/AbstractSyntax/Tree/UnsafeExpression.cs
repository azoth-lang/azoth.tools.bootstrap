using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal class UnsafeExpression : Expression, IUnsafeExpression
{
    public IExpression Expression { get; }

    public UnsafeExpression(TextSpan span, DataType dataType, IExpression expression)
        : base(span, dataType)
    {
        Expression = expression;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => $"unsafe ({Expression})";
}
