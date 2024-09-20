using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class AmbiguousExpressionNode : CodeNode, IAmbiguousExpressionNode
{
    protected AttributeLock SyncLock;

    public abstract override IExpressionSyntax Syntax { get; }
    private ValueId valueId;
    private bool valueIdCached;
    public ValueId ValueId
        => GrammarAttribute.IsCached(in valueIdCached) ? valueId
            : this.Synthetic(ref valueIdCached, ref valueId, ref SyncLock,
                ValueIdsAspect.AmbiguousExpression_ValueId);
    public virtual LexicalScope ContainingLexicalScope
        => Inherited_ContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());

    private protected AmbiguousExpressionNode() { }

    LexicalScope IAmbiguousExpressionNode.ContainingLexicalScope() => ContainingLexicalScope;

    public virtual ConditionalLexicalScope FlowLexicalScope()
        => LexicalScopingAspect.AmbiguousExpression_FlowLexicalScope(this);

    public IPreviousValueId PreviousValueId()
        => Previous_PreviousValueId(GrammarAttribute.CurrentInheritanceContext());

    internal override bool Inherited_ImplicitRecoveryAllowed(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
    {
        //if (ReferenceEquals(child, descendant)) return false;
        //return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);

        //// Avoid issues with a hack that affects MethodGroupName
        if (this is not IMethodGroupNameNode)
            // By default, implicit recovery is not allowed
            return false;
        return base.Inherited_ImplicitRecoveryAllowed(child, descendant, ctx);
    }

    internal override bool Inherited_ShouldPrepareToReturn(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => false;
}
