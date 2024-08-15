using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class EntryNode : ControlFlowNode, IEntryNode
{
    private ControlFlowSet? controlFlowNext;
    private bool controlFlowNextCached;
    public override ControlFlowSet ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Entry_ControlFlowNext);
    public IFixedSet<IDataFlowNode> DataFlowPrevious => FixedSet.Empty<IDataFlowNode>();
    private BindingFlags<IVariableBindingNode>? definitelyAssigned;
    private bool definitelyAssignedCached;
    public BindingFlags<IVariableBindingNode> DefinitelyAssigned
        => GrammarAttribute.IsCached(in definitelyAssignedCached) ? definitelyAssigned!
            : this.Synthetic(ref definitelyAssignedCached, ref definitelyAssigned,
                DefiniteAssignmentAspect.Entry_DefinitelyAssigned);
    private BindingFlags<IVariableBindingNode>? definitelyUnassigned;
    private bool definitelyUnassignedCached;
    public BindingFlags<IVariableBindingNode> DefinitelyUnassigned
        => GrammarAttribute.IsCached(in definitelyUnassignedCached) ? definitelyUnassigned!
            : this.Synthetic(ref definitelyUnassignedCached, ref definitelyUnassigned,
                SingleAssignmentAspect.Entry_DefinitelyUnassigned);

    public FixedDictionary<IVariableBindingNode, int> VariableBindingsMap()
        => InheritedLocalBindingsMap(GrammarAttribute.CurrentInheritanceContext());

    public override IEntryNode ControlFlowEntry() => this;
}
