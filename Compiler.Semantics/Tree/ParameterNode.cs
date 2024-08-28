using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class ParameterNode : CodeNode, IParameterNode
{
    protected AttributeLock SyncLock;
    public abstract override IParameterSyntax Syntax { get; }
    public abstract IdentifierName? Name { get; }
    public bool Unused => Name?.Text.StartsWith('_') ?? false;
    public abstract IMaybeAntetype BindingAntetype { get; }
    public abstract Pseudotype BindingType { get; }
    private ValueId bindingValueId;
    private bool bindingValueIdCached;
    public ValueId BindingValueId
        => GrammarAttribute.IsCached(in bindingValueIdCached) ? bindingValueId
            : this.Synthetic(ref bindingValueIdCached, ref bindingValueId, ref SyncLock,
                ValueIdsAspect.Parameter_BindingValueId);
    public abstract IFlowState FlowStateAfter { get; }

    private protected ParameterNode() { }

    public IFlowState FlowStateBefore()
        => Inherited_FlowStateBefore(GrammarAttribute.CurrentInheritanceContext());

    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());

    internal override IPreviousValueId Previous_PreviousValueId(IChildNode before, IInheritanceContext ctx)
        => BindingValueId;
}
