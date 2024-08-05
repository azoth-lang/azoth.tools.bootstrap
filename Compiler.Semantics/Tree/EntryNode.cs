using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class EntryNode : ControlFlowNode, IEntryNode
{
    private FixedDictionary<IControlFlowNode, ControlFlowKind>? controlFlowNext;
    private bool controlFlowNextCached;
    public override FixedDictionary<IControlFlowNode, ControlFlowKind> ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached) ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Entry_ControlFlowNext);
    private BindingFlags<ILocalBindingNode>? definitelyAssigned;
    private bool definitelyAssignedCached;
    public BindingFlags<ILocalBindingNode> DefinitelyAssigned
        => GrammarAttribute.IsCached(in definitelyAssignedCached) ? definitelyAssigned!
            : this.Synthetic(ref definitelyAssignedCached, ref definitelyAssigned,
                AssignmentAspect.Entry_DefinitelyAssigned);

    public FixedDictionary<ILocalBindingNode, int> LocalBindingsMap()
        => InheritedLocalBindingsMap(GrammarAttribute.CurrentInheritanceContext());
}
