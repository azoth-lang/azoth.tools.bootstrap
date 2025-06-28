using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.SyntaxBinding;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Validation;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

public class SemanticAnalyzer
{
    private readonly IPackageSymbolLoader symbolLoader;

    public SemanticAnalyzer(IPackageSymbolLoader symbolLoader)
    {
        this.symbolLoader = symbolLoader;
    }

    public async ValueTask<IPackageFacetNode> CheckAsync(IPackageFacetSyntax packageFacetSyntax)
    {
        // If there are errors from the lex and parse phase, don't continue on
        packageFacetSyntax.Diagnostics.ThrowIfFatalErrors();

        var referenceSymbols = await LoadReferenceSymbolsAsync(packageFacetSyntax.Kind, packageFacetSyntax.References);

        // Build a semantic tree from the syntax tree
        var packageNode = SyntaxBinder.Bind(packageFacetSyntax, referenceSymbols);

#if DEBUG
        // Since the tree is lazy evaluated, walk it and force evaluation of many attributes to catch bugs
        SemanticTreeValidator.Validate(packageNode);
#endif

        // TODO use DataFlowAnalysis to check for unused variables and report use of variables starting with `_`

        // If the semantic tree reports any fatal errors, don't continue on
        packageNode.Diagnostics.ThrowIfFatalErrors();

        return packageNode;
    }

    private async ValueTask<IReadOnlyDictionary<PackageFacetReferenceSyntax, FixedSymbolTree>> LoadReferenceSymbolsAsync(
            FacetKind facet, IFixedSet<IPackageReferenceSyntax> references)
    {
        var symbolTrees = new Dictionary<PackageFacetReferenceSyntax, FixedSymbolTree>();
        var minimumRelation
            = facet == FacetKind.Main ? PackageReferenceRelation.Internal : PackageReferenceRelation.Dev;

        await LoadReferenceSymbolsAsync(references, minimumRelation, FacetKind.Main, symbolTrees);

        if (facet == FacetKind.Tests)
            await LoadReferenceSymbolsAsync(references, minimumRelation, FacetKind.Tests, symbolTrees);

        return symbolTrees;
    }

    private async Task LoadReferenceSymbolsAsync(
        IFixedSet<IPackageReferenceSyntax> references,
        PackageReferenceRelation minimumRelation,
        FacetKind facetToReference,
        Dictionary<PackageFacetReferenceSyntax, FixedSymbolTree> symbolTrees)
    {
        foreach (var reference in references.Where(r => r.Relation >= minimumRelation))
        {
            var symbolTree = await symbolLoader.LoadSymbolsAsync(reference.PackageName, facetToReference);
            symbolTrees.Add(new(reference, facetToReference), symbolTree);
        }
    }
}
