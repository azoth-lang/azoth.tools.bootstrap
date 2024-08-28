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
}
