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

    public IdentifierName Name => Syntax.Name;

    private ValueAttribute<PackageSymbol> symbol;
    public PackageSymbol Symbol
        => symbol.TryGetValue(out var value) ? value : symbol.GetValue(this, SymbolAttribute.Package);

    private ValueAttribute<IPackageSymbolNode> symbolNode;
    public IPackageSymbolNode SymbolNode
        => symbolNode.TryGetValue(out var value) ? value
            : symbolNode.GetValue(this, SymbolNodeAttributes.Package);

    private ValueAttribute<FixedDictionary<IdentifierName, IPackageSymbolNode>> symbolNodes;
    public FixedDictionary<IdentifierName, IPackageSymbolNode> SymbolNodes
        => symbolNodes.TryGetValue(out var value) ? value
            : symbolNodes.GetValue(this, SymbolNodeAttributes.Package_SymbolNodes);

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

    internal override IPackageNode InheritedPackage(IChildNode caller, IChildNode child) => this;

    internal override PackageNameScope InheritedPackageNameScope(IChildNode caller, IChildNode child)
    {
        // We are assuming these will be cached in the child nodes
        if (child == MainFacet)
            return LexicalScopeAttributes.PackageInheritedMainFacet(this);
        if (child == TestingFacet)
            return LexicalScopeAttributes.PackageInheritedTestingFacet(this);
        return base.InheritedPackageNameScope(caller, child);
    }
}
