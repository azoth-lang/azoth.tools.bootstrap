using System;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.Symbols;

internal class InvalidPackageSymbolLoader : IPackageSymbolLoader
{
    #region Singleton
    public static InvalidPackageSymbolLoader Instance = new InvalidPackageSymbolLoader();

    private InvalidPackageSymbolLoader() { }
    #endregion

    public ValueTask<FixedSymbolTree> LoadSymbolsAsync(IdentifierName packageName, FacetKind facet)
        => throw new InvalidOperationException($"Cannot load symbols for package '{packageName}'. (Facet {facet})");
}
