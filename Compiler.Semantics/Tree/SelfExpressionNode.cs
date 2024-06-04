using System;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class SelfExpressionNode : AmbiguousNameExpressionNode, ISelfExpressionNode
{
    public override ISelfExpressionSyntax Syntax { get; }
    public bool IsImplicit => Syntax.IsImplicit;
    public Pseudotype Pseudotype => throw new NotImplementedException();
    private ValueAttribute<IExecutableDefinitionNode> containingDeclaration;
    public IExecutableDefinitionNode ContainingDeclaration
        => containingDeclaration.TryGetValue(out var value) ? value
        : containingDeclaration.GetValue(() => (IExecutableDefinitionNode)InheritedContainingDeclaration());
    private ValueAttribute<ISelfParameterNode?> referencedParameter;
    public ISelfParameterNode? ReferencedParameter
        => referencedParameter.TryGetValue(out var value) ? value
        : referencedParameter.GetValue(this, BindingNamesAspect.SelfExpression_ReferencedParameter);
    // TODO remove parameter symbols
    private ValueAttribute<SelfParameterSymbol?> referencedSymbol;
    public SelfParameterSymbol? ReferencedSymbol
        => referencedSymbol.TryGetValue(out var value) ? value
        : referencedSymbol.GetValue(this, SymbolAspect.SelfExpression_ReferencedSymbol);
    private ValueAttribute<IMaybeExpressionAntetype> antetype;
    public override IMaybeExpressionAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.SelfExpression_Antetype);

    public SelfExpressionNode(ISelfExpressionSyntax syntax)
    {
        Syntax = syntax;
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        BindingNamesAspect.SelfExpression_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
