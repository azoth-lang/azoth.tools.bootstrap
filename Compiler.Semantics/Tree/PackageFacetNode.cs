using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal class PackageFacetNode : ChildNode, IPackageFacetNode
{
    public override IPackageSyntax Syntax { get; }
    public IdentifierName PackageName => Package.Name;
    public PackageSymbol PackageSymbol => Package.Symbol;
    private ValueAttribute<IPackageFacetDeclarationNode> symbolNode;
    public IPackageFacetDeclarationNode SymbolNode
        => symbolNode.TryGetValue(out var value) ? value
            : symbolNode.GetValue(this, SymbolNodeAttributes.PackageFacet_SymbolNode);
    public IFixedSet<ICompilationUnitNode> CompilationUnits { get; }

    private ValueAttribute<IFixedSet<IPackageMemberDefinitionNode>> definitions;
    public IFixedSet<IPackageMemberDefinitionNode> Definitions
        => definitions.TryGetValue(out var value) ? value
            : definitions.GetValue(this, DeclarationsAttribute.PackageFacet);

    private ValueAttribute<PackageNameScope> packageNameScope;
    public PackageNameScope PackageNameScope
        => packageNameScope.TryGetValue(out var value) ? value
            : packageNameScope.GetValue(InheritedPackageNameScope);

    public PackageFacetNode(IPackageSyntax syntax, IEnumerable<ICompilationUnitNode> compilationUnits)
    {
        Syntax = syntax;
        CompilationUnits = ChildSet.CreateFixedSet(this, compilationUnits);
    }

    internal override IDeclarationNode InheritedContainingDeclaration(IChildNode caller, IChildNode child)
        => SymbolNode;

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode caller, IChildNode child)
        => PackageNameScope.PackageGlobalScope;
}
