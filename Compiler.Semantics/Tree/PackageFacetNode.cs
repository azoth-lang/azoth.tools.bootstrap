using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal class PackageFacetNode : ChildNode, IPackageFacetNode
{
    public override IPackageSyntax Syntax { get; }
    /// <remarks>Implements <see cref="IPackageFacetDeclarationNode.PackageAliasOrName"/> which
    /// should be <see langword="null"/> for the current package.</remarks>
    public IdentifierName? PackageAliasOrName => null;
    public IdentifierName PackageName => Package.Name;
    private ValueAttribute<INamespaceDefinitionNode> globalNamespace;
    public INamespaceDefinitionNode GlobalNamespace
        => globalNamespace.TryGetValue(out var value) ? value
            : globalNamespace.GetValue(this, SymbolNodeAspect.PackageFacet_GlobalNamespace);
    public PackageSymbol PackageSymbol => Package.Symbol;
    public IFixedSet<ICompilationUnitNode> CompilationUnits { get; }

    private ValueAttribute<IFixedSet<IPackageMemberDefinitionNode>> definitions;
    public IFixedSet<IPackageMemberDefinitionNode> Definitions
        => definitions.TryGetValue(out var value) ? value
            : definitions.GetValue(this, DefinitionsAspect.PackageFacet_Definitions);

    private ValueAttribute<PackageNameScope> packageNameScope;
    public PackageNameScope PackageNameScope
        => packageNameScope.TryGetValue(out var value) ? value
            : packageNameScope.GetValue(() => Inherited_PackageNameScope(GrammarAttribute.CurrentInheritanceContext()));

    public PackageFacetNode(IPackageSyntax syntax, IEnumerable<ICompilationUnitNode> compilationUnits)
    {
        Syntax = syntax;
        CompilationUnits = ChildSet.Attach(this, compilationUnits);
    }

    internal override ISymbolDeclarationNode Inherited_ContainingDeclaration(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => this;

    internal override LexicalScope Inherited_ContainingLexicalScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => PackageNameScope.PackageGlobalScope;

    internal override IPackageFacetDeclarationNode Inherited_Facet(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => this;

    internal override PackageNameScope Inherited_PackageNameScope(SemanticNode child, SemanticNode descendant, IInheritanceContext ctx)
        => PackageNameScope;
}
