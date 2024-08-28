using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class ControlFlowNode : ChildNode, IControlFlowNode
{
    public sealed override ICodeSyntax? Syntax => null;
    public CodeFile File => Inherited_File();
    public abstract ControlFlowSet ControlFlowNext { get; }
    private ControlFlowSet? controlFlowPrevious;
    private bool controlFlowPreviousCached;
    public ControlFlowSet ControlFlowPrevious
        => GrammarAttribute.IsCached(in controlFlowPreviousCached) ? controlFlowPrevious!
            : this.Inherited(ref controlFlowPreviousCached, ref controlFlowPrevious,
                ctx => CollectControlFlowPrevious(this, ctx));

    public abstract IEntryNode ControlFlowEntry();

    public virtual ControlFlowSet ControlFlowFollowing()
        => Inherited_ControlFlowFollowing(GrammarAttribute.CurrentInheritanceContext());

    protected override void CollectControlFlowPrevious(
        IControlFlowNode target,
        Dictionary<IControlFlowNode, ControlFlowKind> previous)
    {
        ControlFlowAspect.ControlFlow_ContributeControlFlowPrevious(this, target, previous);
        base.CollectControlFlowPrevious(target, previous);
    }
}
