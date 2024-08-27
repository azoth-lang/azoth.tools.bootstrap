using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class PackageNode : SemanticNode, IPackageNode
{
    public override IPackageSyntax Syntax { get; }
    private PackageSymbol? symbol;
    private bool symbolCached;
    public PackageSymbol Symbol
        => GrammarAttribute.IsCached(in symbolCached) ? symbol!
            : this.Synthetic(ref symbolCached, ref symbol, SymbolsAspect.Package_Symbol);
    private ValueAttribute<FixedDictionary<IdentifierName, IPackageDeclarationNode>> packageDeclarations;
    public FixedDictionary<IdentifierName, IPackageDeclarationNode> PackageDeclarations
        => packageDeclarations.TryGetValue(out var value) ? value
            : packageDeclarations.GetValue(this, SymbolNodeAspect.Package_PackageDeclarations);
    private DiagnosticCollection? diagnostics;
    private bool diagnosticsCached;
    public DiagnosticCollection Diagnostics
        => GrammarAttribute.IsCached(in diagnosticsCached) ? diagnostics!
            : this.Synthetic(ref diagnosticsCached, ref diagnostics,
                DiagnosticsAspect.Package);
    private ValueAttribute<IFixedSet<ITypeDeclarationNode>> primitivesDeclarations;
    public IFixedSet<ITypeDeclarationNode> PrimitivesDeclarations
        => primitivesDeclarations.TryGetValue(out var value) ? value
            : primitivesDeclarations.GetValue(this, n => ChildSet.Attach(this, BuiltInsAspect.Package_PrimitivesDeclarations(n)));
    private IFunctionDefinitionNode? entryPoint;
    private bool entryPointCached;
    public IFunctionDefinitionNode? EntryPoint
        => GrammarAttribute.IsCached(in entryPointCached) ? entryPoint
            : this.Synthetic(ref entryPointCached, ref entryPoint,
                DefinitionsAspect.Package_EntryPoint, ReferenceEqualityComparer.Instance);
    private IPackageSymbols? packageSymbols;
    private bool packageSymbolsCached;
    public IPackageSymbols PackageSymbols
        => GrammarAttribute.IsCached(in packageSymbolsCached) ? packageSymbols!
            : this.Synthetic(ref packageSymbolsCached, ref packageSymbols,
                SymbolsAspect.Package_PackageSymbols, ReferenceEqualityComparer.Instance);
    public IFixedSet<IPackageReferenceNode> References { get; }
    private ValueAttribute<IPackageReferenceNode> intrinsicsReference;
    public IPackageReferenceNode IntrinsicsReference
        => intrinsicsReference.TryGetValue(out var value) ? value
            : intrinsicsReference.GetValue(this, n => Child.Attach(this, BuiltInsAspect.Package_IntrinsicsReference(n)));
    public IPackageFacetNode MainFacet { get; }
    public IPackageFacetNode TestingFacet { get; }

    public PackageNode(
        IPackageSyntax syntax,
        IEnumerable<IPackageReferenceNode> references,
        IPackageFacetNode mainFacet,
        IPackageFacetNode testingFacet)
    {
        Syntax = syntax;
        References = ChildSet.Attach(this, references);
        MainFacet = Child.Attach(this, mainFacet);
        TestingFacet = Child.Attach(this, testingFacet);
    }

    internal override IPackageDeclarationNode InheritedPackage(IChildNode child, IChildNode descendant)
        => this;

    internal override PackageNameScope InheritedPackageNameScope(IChildNode child, IChildNode descendant)
    {
        if (descendant == MainFacet)
            return LexicalScopingAspect.Package_MainFacet_PackageNameScope(this);
        if (descendant == TestingFacet)
            return LexicalScopingAspect.Package_TestingFacet_PackageNameScope(this);
        return base.InheritedPackageNameScope(child, descendant);
    }
}
