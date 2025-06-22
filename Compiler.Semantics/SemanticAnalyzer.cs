using System.Collections.Generic;
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

    public async ValueTask<IPackageNode> CheckAsync(
        IPackageFacetSyntax packageMainSyntax,
        IPackageFacetSyntax packageTestsSyntax)
    {
        // If there are errors from the lex and parse phase, don't continue on
        packageMainSyntax.Diagnostics.ThrowIfFatalErrors();
        packageTestsSyntax.Diagnostics.ThrowIfFatalErrors();

        var referenceSymbols = await LoadReferenceSymbolsAsync(packageMainSyntax.References);

        // Build a semantic tree from the syntax tree
        var packageNode = SyntaxBinder.Bind(packageMainSyntax, packageTestsSyntax, referenceSymbols);

#if DEBUG
        // Since the tree is lazy evaluated, walk it and force evaluation of many attributes to catch bugs
        SemanticTreeValidator.Validate(packageNode);
#endif

        // TODO use DataFlowAnalysis to check for unused variables and report use of variables starting with `_`

        // If the semantic tree reports any fatal errors, don't continue on
        packageNode.Diagnostics.ThrowIfFatalErrors();

        return packageNode;
    }

    private async ValueTask<IReadOnlyDictionary<IPackageReferenceSyntax, FixedSymbolTree>> LoadReferenceSymbolsAsync(IFixedSet<IPackageReferenceSyntax> references)
    {
        var symbolTrees = new Dictionary<IPackageReferenceSyntax, FixedSymbolTree>();

        foreach (var reference in references)
        {
            var symbolTree = await symbolLoader.LoadSymbolsAsync(reference.PackageName, FacetKind.Main);
            symbolTrees.Add(reference, symbolTree);
        }

        return symbolTrees;
    }
}
