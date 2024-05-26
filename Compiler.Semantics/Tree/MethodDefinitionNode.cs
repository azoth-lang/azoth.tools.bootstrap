using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class MethodDefinitionNode : TypeMemberDefinitionNode, IMethodDefinitionNode
{
    public abstract override IMethodDefinitionSyntax Syntax { get; }
    public override UserTypeSymbol ContainingSymbol => (UserTypeSymbol)base.ContainingSymbol;
    public MethodKind Kind => Syntax.Kind;
    public override IdentifierName Name => Syntax.Name;
    StandardName INamedDeclarationNode.Name => Name;
    public IMethodSelfParameterNode SelfParameter { get; }
    public IFixedList<INamedParameterNode> Parameters { get; }
    public virtual ITypeNode? Return { get; }
    private ValueAttribute<MethodSymbol> symbol;
    public override MethodSymbol Symbol
    => symbol.TryGetValue(out var value) ? value
        : symbol.GetValue(this, SymbolAspect.MethodDeclaration);

    private protected MethodDefinitionNode(
        IMethodSelfParameterNode selfParameter,
        IEnumerable<INamedParameterNode> parameters,
        ITypeNode? @return)
    {
        SelfParameter = Child.Attach(this, selfParameter);
        Parameters = ChildList.Attach(this, parameters);
        Return = Child.Attach(this, @return);
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        TypeMemberDeclarationsAspect.MethodDeclaration_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }

    internal override ISemanticNode? InheritedPredecessor(IChildNode child, IChildNode descendant)
    {
        if (descendant == SelfParameter) return null;
        if (descendant is INamedParameterNode parameter
            && Parameters.IndexOf(parameter) is int index)
            return index == 0 ? SelfParameter : Parameters[index - 1];

        return base.InheritedPredecessor(child, descendant);
    }
}
