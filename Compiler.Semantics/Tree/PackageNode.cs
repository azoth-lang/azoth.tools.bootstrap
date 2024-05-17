using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class PackageNode : SemanticNode, IPackageNode
{
    public override IPackageSyntax Syntax { get; }
    /// <remarks>Implements <see cref="IPackageDeclarationNode.AliasOrName"/> which
    /// should be <see langword="null"/> for the current package.</remarks>
    public IdentifierName? AliasOrName => null;
    public IdentifierName Name => Syntax.Name;

    private ValueAttribute<PackageSymbol> symbol;
    public PackageSymbol Symbol
        => symbol.TryGetValue(out var value) ? value : symbol.GetValue(this, SymbolAttribute.Package);
    private ValueAttribute<FixedDictionary<IdentifierName, IPackageDeclarationNode>> packageDeclarations;
    public FixedDictionary<IdentifierName, IPackageDeclarationNode> PackageDeclarations
        => packageDeclarations.TryGetValue(out var value) ? value
            : packageDeclarations.GetValue(this, SymbolNodeAttributes.Package_PackageDeclarations);

    private ValueAttribute<IFixedList<Diagnostic>> diagnostics;
    public IFixedList<Diagnostic> Diagnostics
        => diagnostics.TryGetValue(out var value) ? value
            : diagnostics.GetValue(this, DiagnosticsAttribute.Package);

    public IFixedSet<IPackageReferenceNode> References { get; }
    private ValueAttribute<IPackageReferenceNode> intrinsicsReference;
    public IPackageReferenceNode IntrinsicsReference
        => intrinsicsReference.TryGetValue(out var value) ? value
            : intrinsicsReference.GetValue(this, IntrinsicsReferenceAttribute.Package);
    public IPackageFacetNode MainFacet { get; }
    public IPackageFacetNode TestingFacet { get; }

    public PackageNode(
        IPackageSyntax syntax,
        IEnumerable<IPackageReferenceNode> references,
        IPackageFacetNode mainFacet,
        IPackageFacetNode testingFacet)
    {
        Syntax = syntax;
        References = ChildSet.CreateFixedSet(this, references);
        MainFacet = Child.Attach(this, mainFacet);
        TestingFacet = Child.Attach(this, testingFacet);
    }

    internal override IPackageDeclarationNode InheritedPackage(IChildNode child, IChildNode descendant)
        => this;

    internal override PackageNameScope InheritedPackageNameScope(IChildNode child, IChildNode descendant)
    {
        // We are assuming these will be cached in the child nodes
        if (descendant == MainFacet)
            return LexicalScopingAspect.Package_InheritedPackageNameScope_MainFacet(this);
        if (descendant == TestingFacet)
            return LexicalScopingAspect.Package_InheritedPackageNameScope_TestingFacet(this);
        return base.InheritedPackageNameScope(child, descendant);
    }
}
