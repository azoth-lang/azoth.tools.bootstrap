using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class SelfExpressionNode : AmbiguousNameExpressionNode, ISelfExpressionNode
{
    public override ISelfExpressionSyntax Syntax { get; }
    public bool IsImplicit => Syntax.IsImplicit;
    private ValueAttribute<IExecutableDefinitionNode> containingDeclaration;
    public IExecutableDefinitionNode ContainingDeclaration
        => containingDeclaration.TryGetValue(out var value) ? value
        : containingDeclaration.GetValue(() => (IExecutableDefinitionNode)InheritedContainingDeclaration());
    private ValueAttribute<ISelfParameterNode?> referencedDefinition;
    public ISelfParameterNode? ReferencedDefinition
        => referencedDefinition.TryGetValue(out var value) ? value
            : referencedDefinition.GetValue(this, BindingNamesAspect.SelfExpression_ReferencedDefinition);
    // TODO remove parameter symbols
    private ValueAttribute<SelfParameterSymbol?> referencedSymbol;
    public SelfParameterSymbol? ReferencedSymbol
        => referencedSymbol.TryGetValue(out var value) ? value
            : referencedSymbol.GetValue(this, SymbolAspect.SelfExpression_ReferencedSymbol);
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.SelfExpression_Antetype);
    private Circular<FlowState> flowStateAfter = new(FlowState.Empty);
    private bool flowStateAfterCached;
    public override FlowState FlowStateAfter
        => GrammarAttribute.IsCached(in flowStateAfterCached) ? flowStateAfter.UnsafeValue
            : this.Circular(ref flowStateAfterCached, ref flowStateAfter,
                ExpressionTypesAspect.SelfExpression_FlowStateAfter);
    private DataType? type;
    private bool typeCached;
    public override DataType Type
        => GrammarAttribute.IsCached(in typeCached) ? type!
            : this.Synthetic(ref typeCached, ref type, ExpressionTypesAspect.SelfExpression_Type);
    private Pseudotype? pseudotype;
    private bool pseudotypeCached;
    public Pseudotype Pseudotype
        => GrammarAttribute.IsCached(in pseudotypeCached) ? pseudotype!
            : this.Synthetic(ref pseudotypeCached, ref pseudotype, ExpressionTypesAspect.SelfExpression_Pseudotype);

    public SelfExpressionNode(ISelfExpressionSyntax syntax)
    {
        Syntax = syntax;
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        BindingNamesAspect.SelfExpression_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }

    public FlowState FlowStateBefore()
        => InheritedFlowStateBefore(GrammarAttribute.CurrentInheritanceContext());
}
