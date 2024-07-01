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
    public INamedBindingNode ReferencedDefinition { get; }
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.VariableNameExpression_Antetype);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.VariableNameExpression_Type);
    private Circular<FlowState> flowStateAfter = new(FlowState.Empty);
    private bool flowStateAfterCached;
    public override FlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached)
            ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.VariableNameExpression_FlowStateAfter);

    public VariableNameExpressionNode(IIdentifierNameExpressionSyntax syntax, INamedBindingNode referencedDefinition)
    {
        Syntax = syntax;
        ReferencedDefinition = referencedDefinition;
    }

    public FlowState FlowStateBefore()
        => InheritedFlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
}
