using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class ParameterNode : CodeNode, IParameterNode
{
    protected AttributeLock SyncLock;
    public abstract override IParameterSyntax Syntax { get; }
    public abstract IdentifierName? Name { get; }
    public bool Unused => Syntax.Unused;
    public abstract IMaybeAntetype BindingAntetype { get; }
    public abstract Pseudotype BindingType { get; }
    private ValueId valueId;
    private bool valueIdCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : GrammarAttribute.Synthetic(ref valueIdCached, this,
                TypeMemberDeclarationsAspect.Parameter_ValueId, ref valueId, ref SyncLock);
    public abstract FlowState FlowStateAfter { get; }

    private protected ParameterNode() { }

    public FlowState FlowStateBefore()
        => InheritedFlowStateBefore(GrammarAttribute.CurrentInheritanceContext());

    public IPreviousValueId PreviousValueId()
        => PreviousValueId(GrammarAttribute.CurrentInheritanceContext());

    internal override IPreviousValueId PreviousValueId(IChildNode before, IInheritanceContext ctx) => ValueId;
}
