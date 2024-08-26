using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class AmbiguousFreezeExpressionNode : AmbiguousExpressionNode, IAmbiguousFreezeExpressionNode
{
    protected override bool MayHaveRewrite => true;

    public override IFreezeExpressionSyntax Syntax { get; }
    private RewritableChild<ISimpleNameNode> referent;
    private bool referentCached;
    public ISimpleNameNode TempReferent
        => GrammarAttribute.IsCached(in referentCached) ? referent.UnsafeValue
            : this.RewritableChild(ref referentCached, ref referent);
    public INameExpressionNode? Referent => TempReferent as INameExpressionNode;

    public AmbiguousFreezeExpressionNode(IFreezeExpressionSyntax syntax, ISimpleNameNode referent)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
    }

    public override ConditionalLexicalScope FlowLexicalScope() => TempReferent.FlowLexicalScope();

    protected override IChildNode? Rewrite()
        => CapabilityExpressionsAspect.AmbiguousFreezeExpression_Rewrite_Variable(this)
        ?? CapabilityExpressionsAspect.AmbiguousFreezeExpression_Rewrite_Value(this)
        ?? base.Rewrite();
}
