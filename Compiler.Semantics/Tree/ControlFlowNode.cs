using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class ControlFlowNode : ChildNode, IControlFlowNode
{
    public sealed override IConcreteSyntax? Syntax => null;
    public CodeFile File => InheritedFile();
    public abstract FixedDictionary<IControlFlowNode, ControlFlowKind> ControlFlowNext { get; }
    private FixedDictionary<IControlFlowNode, ControlFlowKind>? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    public FixedDictionary<IControlFlowNode, ControlFlowKind> ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Inherited(ref controlFlowPreviousCached, ref controlFlowPrevious,
                ctx => CollectControlFlowPrevious(this, ctx));

    public virtual FixedDictionary<IControlFlowNode, ControlFlowKind> ControlFlowFollowing()
        => InheritedControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());

    protected override void CollectControlFlowPrevious(
        IControlFlowNode target,
        Dictionary<IControlFlowNode, ControlFlowKind> previous)
    {
        ControlFlowAspect.ControlFlow_ContributeControlFlowPrevious(this, target, previous);
        base.CollectControlFlowPrevious(target, previous);
    }
}
