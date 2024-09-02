using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class AmbiguousMoveExpressionNode : ExpressionNode, IAmbiguousMoveExpressionNode
{
    public override IMoveExpressionSyntax Syntax { get; }
    private RewritableChild<IUnresolvedSimpleNameNode> referent;
    private bool referentCached;
    public IUnresolvedSimpleNameNode TempReferent
        => GrammarAttribute.IsCached(in referentCached) ? referent.UnsafeValue
            : this.RewritableChild(ref referentCached, ref referent);
    public ISimpleNameExpressionNode? Referent => TempReferent as ISimpleNameExpressionNode;
    public IUnresolvedSimpleNameNode CurrentReferent => referent.UnsafeValue;

    public AmbiguousMoveExpressionNode(IMoveExpressionSyntax syntax, IUnresolvedSimpleNameNode referent)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
    }

    public override ConditionalLexicalScope FlowLexicalScope() => TempReferent.FlowLexicalScope();

    protected override IChildTreeNode Rewrite()
        => CapabilityExpressionsAspect.AmbiguousMoveExpression_Rewrite_Variable(this)
        ?? CapabilityExpressionsAspect.AmbiguousMoveExpression_Rewrite_Value(this)
        ?? base.Rewrite();
}
