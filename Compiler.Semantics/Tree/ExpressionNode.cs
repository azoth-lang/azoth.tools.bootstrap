using System;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class ExpressionNode : AmbiguousExpressionNode, IExpressionNode
{
    public abstract override ITypedExpressionSyntax Syntax { get; }
    private ValueAttribute<ValueId> valueId;
    public ValueId ValueId
        => valueId.TryGetValue(out var value) ? value
            : valueId.GetValue((IExpressionNode)this, ExpressionTypesAspect.Expression_ValueId);
    // TODO make this abstract once all expressions have type implemented
    public virtual DataType Type => throw new NotImplementedException();

    private protected ExpressionNode() { }

    public IExpressionNode? Predecessor() => (IExpressionNode?)InheritedPredecessor();
}
