using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax.Tree;

internal class LoopExpression : Expression, ILoopExpression
{
    public IBlockExpression Block { get; }

    public LoopExpression(TextSpan span, DataType dataType, IBlockExpression block)
        : base(span, dataType)
    {
        Block = block;
    }

    protected override OperatorPrecedence ExpressionPrecedence => OperatorPrecedence.Primary;

    public override string ToString() => $"loop {Block}";
}
