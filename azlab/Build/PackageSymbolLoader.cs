using System.Collections.Generic;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Lab.Build;

internal class PackageSymbolLoader : IPackageSymbolLoader
{
    private readonly FixedDictionary<(IdentifierName, FacetKind), Task<IPackageFacetNode>> packageFacets;

    public PackageSymbolLoader(IEnumerable<KeyValuePair<(IdentifierName, FacetKind), Task<IPackageFacetNode>>> packageFacets)
    {
        this.packageFacets = new(packageFacets);
    }

    public async ValueTask<FixedSymbolTree> LoadSymbolsAsync(IdentifierName packageName, FacetKind facet)
        => (await packageFacets[(packageName, facet)]).Symbols;
}
