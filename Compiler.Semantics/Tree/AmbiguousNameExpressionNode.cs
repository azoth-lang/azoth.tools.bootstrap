using System;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class AmbiguousNameExpressionNode : AmbiguousExpressionNode, IAmbiguousNameExpressionNode
{
    public abstract override INameExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    private ValueAttribute<ValueId> valueId;
    public ValueId ValueId
        => valueId.TryGetValue(out var value) ? value
            : valueId.GetValue((IExpressionNode)this, ExpressionTypesAspect.Expression_ValueId);
    // TODO make this abstract once all expressions have type implemented (also, not all names should have types)
    public virtual IExpressionAntetype Antetype
        => throw new NotImplementedException($"{GetType().GetFriendlyName()}.{nameof(Antetype)} not implemented.");
    // TODO make this abstract once all expressions have type implemented (also, not all names should have types)
    public virtual DataType Type
        => throw new NotImplementedException($"{GetType().GetFriendlyName()}.{nameof(Type)} not implemented.");
    // TODO make this abstract once all expressions have flow state implemented (also, not all names should have flow state)
    public virtual FlowState FlowStateAfter
        => throw new NotImplementedException($"{GetType().GetFriendlyName()}.{nameof(FlowStateAfter)} not implemented.");

    private protected AmbiguousNameExpressionNode() { }

    public new IPreviousValueId PreviousValueId() => base.PreviousValueId();

    internal override IPreviousValueId PreviousValueId(IChildNode before)
        => ValueId;
}
