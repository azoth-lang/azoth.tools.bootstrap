using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class AmbiguousMoveExpressionNode : ExpressionNode, IAmbiguousMoveExpressionNode
{
    protected override bool MayHaveRewrite => true;

    public override IMoveExpressionSyntax Syntax { get; }
    private RewritableChild<ISimpleNameNode> referent;
    private bool referentCached;
    public ISimpleNameNode Referent
        => GrammarAttribute.IsCached(in referentCached) ? referent.UnsafeValue
            : this.RewritableChild(ref referentCached, ref referent);
    public INameExpressionNode? IntermediateReferent => Referent as INameExpressionNode;

    public AmbiguousMoveExpressionNode(IMoveExpressionSyntax syntax, ISimpleNameNode referent)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
    }

    public override ConditionalLexicalScope GetFlowLexicalScope() => Referent.GetFlowLexicalScope();

    protected override IChildNode? Rewrite()
        => CapabilityExpressionsAspect.AmbiguousMoveExpression_Rewrite_Variable(this)
        ?? CapabilityExpressionsAspect.AmbiguousMoveExpression_Rewrite_Value(this)
        ?? base.Rewrite();
}