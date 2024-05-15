using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class IntegerLiteralExpressionNode : ExpressionNode, IIntegerLiteralExpressionNode
{
    public override IIntegerLiteralExpressionSyntax Syntax { get; }
    public BigInteger Value => Syntax.Value;
    private ValueAttribute<IntegerConstValueType> type;
    public IntegerConstValueType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, TypeExpressionsAspect.IntegerLiteralExpression_Type);

    public IntegerLiteralExpressionNode(IIntegerLiteralExpressionSyntax syntax)
    {
        Syntax = syntax;
    }
}
