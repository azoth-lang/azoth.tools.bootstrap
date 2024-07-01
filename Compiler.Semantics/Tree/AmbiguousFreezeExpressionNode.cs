using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class AmbiguousFreezeExpressionNode : AmbiguousExpressionNode, IAmbiguousFreezeExpressionNode
{
    protected override bool MayHaveRewrite => true;

    public override IFreezeExpressionSyntax Syntax { get; }
    private RewritableChild<ISimpleNameNode> referent;
    private bool referentCached;
    public ISimpleNameNode Referent
        => GrammarAttribute.IsCached(in referentCached) ? referent.UnsafeValue
            : this.RewritableChild(ref referentCached, ref referent);
    public INameExpressionNode? IntermediateReferent => Referent as INameExpressionNode;
    //private DataType? type;
    //private bool typeCached;
    //public override DataType Type
    //    => GrammarAttribute.IsCached(in typeCached) ? type!
    //        : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.FreezeExpression_Type);
    //private Circular<FlowState> flowStateAfter = new(FlowState.Empty);
    //private bool flowStateAfterCached;
    //public override FlowState FlowStateAfter
    //    => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
    //        : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
    //            ExpressionTypesAspect.FreezeExpression_FlowStateAfter);

    public AmbiguousFreezeExpressionNode(IFreezeExpressionSyntax syntax, ISimpleNameNode referent)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
    }

    public override ConditionalLexicalScope GetFlowLexicalScope() => Referent.GetFlowLexicalScope();

    protected override IChildNode? Rewrite()
        => CapabilityExpressionsAspect.AmbiguousFreezeExpression_Rewrite_Variable(this)
        ?? CapabilityExpressionsAspect.AmbiguousFreezeExpression_Rewrite_Value(this)
        ?? base.Rewrite();
}
