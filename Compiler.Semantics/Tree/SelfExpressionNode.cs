using System;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
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
    // TODO remove parameter symbols
    private ValueAttribute<SelfParameterSymbol?> referencedSymbol;
    public SelfParameterSymbol? ReferencedSymbol
        => referencedSymbol.TryGetValue(out var value) ? value
        : referencedSymbol.GetValue(this, SymbolAttribute.SelfExpression_ReferencedSymbol);

    public SelfExpressionNode(ISelfExpressionSyntax syntax)
    {
        Syntax = syntax;
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        BindingAmbiguousNamesAspect.SelfExpression_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
