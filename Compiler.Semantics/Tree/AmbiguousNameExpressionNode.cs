using System;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class AmbiguousNameExpressionNode : AmbiguousExpressionNode, IAmbiguousNameExpressionNode
{
    public abstract override INameExpressionSyntax Syntax { get; }
    IExpressionSyntax IAmbiguousExpressionNode.Syntax => Syntax;
    private IMaybeExpressionAntetype? expectedAntetype;
    private bool expectedAntetypeCached;
    public IMaybeExpressionAntetype? ExpectedAntetype
        => GrammarAttribute.IsCached(in expectedAntetypeCached) ? expectedAntetype
            : this.Inherited(ref expectedAntetypeCached, ref expectedAntetype, Inherited_ExpectedAntetype);
    // TODO make this abstract once all expressions have type implemented (also, not all names should have types)
    public virtual IMaybeExpressionAntetype Antetype
        => throw new NotImplementedException($"{GetType().GetFriendlyName()}.{nameof(Antetype)} not implemented.");
    private DataType? expectedType;
    private bool expectedTypeCached;
    public DataType? ExpectedType
        => GrammarAttribute.IsCached(in expectedTypeCached) ? expectedType
            : this.Inherited(ref expectedTypeCached, ref expectedType, Inherited_ExpectedType);
    // TODO make this abstract once all expressions have type implemented (also, not all names should have types)
    public virtual DataType Type
        => throw new NotImplementedException($"{GetType().GetFriendlyName()}.{nameof(Type)} not implemented.");
    // TODO make this abstract once all expressions have flow state implemented (also, not all names should have flow state)
    public virtual IFlowState FlowStateAfter
        => throw new NotImplementedException($"{GetType().GetFriendlyName()}.{nameof(FlowStateAfter)} not implemented.");

    private protected AmbiguousNameExpressionNode() { }

    internal override IPreviousValueId Next_PreviousValueId(SemanticNode before, IInheritanceContext ctx)
        => ValueId;
}
