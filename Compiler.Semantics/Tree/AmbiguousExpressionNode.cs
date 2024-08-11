using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

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
                ExpressionTypesAspect.AmbiguousExpression_ValueId);

    private protected AmbiguousExpressionNode() { }

    public LexicalScope GetContainingLexicalScope()
        => InheritedContainingLexicalScope(GrammarAttribute.CurrentInheritanceContext());

    public virtual ConditionalLexicalScope GetFlowLexicalScope()
        => LexicalScopingAspect.UntypedExpression_GetFlowLexicalScope(this);

    public IPreviousValueId PreviousValueId()
        => PreviousValueId(GrammarAttribute.CurrentInheritanceContext());
}
