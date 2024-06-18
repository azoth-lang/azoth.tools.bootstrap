using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class VariableNameExpressionNode : AmbiguousNameExpressionNode, IVariableNameExpressionNode
{
    public override IIdentifierNameExpressionSyntax Syntax { get; }
    public IdentifierName Name => Syntax.Name;
    public INamedBindingNode ReferencedDeclaration { get; }
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.VariableNameExpression_Antetype);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : GrammarAttribute.Synthetic(ref typeCached, this,
                ExpressionTypesAspect.VariableNameExpression_Type, ref type);
    private Circular<FlowState> flowStateAfter = new(FlowState.Empty);
    private bool flowStateAfterCached;
    public override FlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached)
            ? flowStateAfter.UnsafeValue
            : GrammarAttribute.Circular(ref flowStateAfterCached, this,
                ExpressionTypesAspect.VariableNameExpression_FlowStateAfter, ref flowStateAfter);

    public VariableNameExpressionNode(IIdentifierNameExpressionSyntax syntax, INamedBindingNode referencedDeclaration)
    {
        Syntax = syntax;
        ReferencedDeclaration = referencedDeclaration;
    }

    public FlowState FlowStateBefore() => InheritedFlowStateBefore();
}
