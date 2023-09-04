using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree;

internal abstract class RecoverExpression : Expression, IRecoverExpression
{
    public IExpression Value { get; }

    protected RecoverExpression(
        TextSpan span,
        DataType dataType,
        ExpressionSemantics semantics,
        IExpression value)
        : base(span, dataType, semantics)
    {
        Value = value;
    }
}
