using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Variables;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ExitNode : ControlFlowNode, IExitNode
{
    public override ControlFlowSet ControlFlowNext => ControlFlowSet.Empty;
    private BindingFlags<ILocalBindingNode>? definitelyAssigned;
    private bool definitelyAssignedCached;
    public BindingFlags<ILocalBindingNode> DefinitelyAssigned
        => GrammarAttribute.IsCached(in definitelyAssignedCached) ? definitelyAssigned!
            : this.Synthetic(ref definitelyAssignedCached, ref definitelyAssigned,
                AssignmentAspect.Exit_DefinitelyAssigned);
}
