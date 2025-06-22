using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Lab.Build;

internal class PackageSymbolLoader : IPackageSymbolLoader
{
    private readonly FixedDictionary<IdentifierName, Task<IPackageNode>> packages;

    public PackageSymbolLoader(FixedDictionary<IdentifierName, Task<IPackageNode>> packages)
    {
        this.packages = packages;
    }

    public async ValueTask<FixedSymbolTree> LoadSymbolsAsync(IdentifierName packageName, FacetKind facet)
    {
        var packageNode = await packages[packageName];
        return facet switch
        {
            FacetKind.Main => packageNode.PackageSymbols.SymbolTree,
            FacetKind.Tests => packageNode.PackageSymbols.TestingSymbolTree,
            _ => throw ExhaustiveMatch.Failed(facet),
        };
    }
}
