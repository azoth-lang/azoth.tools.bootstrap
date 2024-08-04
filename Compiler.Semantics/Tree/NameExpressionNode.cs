using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class NameExpressionNode : AmbiguousNameExpressionNode, INameExpressionNode
{
    private FixedDictionary<IControlFlowNode, ControlFlowKind>? controlFlowNext;
    private bool controlFlowNextCached;
    public FixedDictionary<IControlFlowNode, ControlFlowKind> ControlFlowNext
        => GrammarAttribute.IsCached(in controlFlowNextCached)
            ? controlFlowNext!
            : this.Synthetic(ref controlFlowNextCached, ref controlFlowNext,
                ControlFlowAspect.Expression_ControlFlowNext);

    public FixedDictionary<IControlFlowNode, ControlFlowKind> ControlFlowFollowing()
        => InheritedControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());
}
