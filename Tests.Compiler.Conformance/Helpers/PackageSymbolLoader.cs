using System;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Tests.Conformance.Helpers;

internal class PackageSymbolLoader : IPackageSymbolLoader
{
    private readonly IPackageNode supportPackage;

    public PackageSymbolLoader(IPackageNode supportPackage)
    {
        this.supportPackage = supportPackage;
    }

    public ValueTask<FixedSymbolTree> LoadSymbolsAsync(IdentifierName packageName, FacetKind facet)
    {
        if (packageName == supportPackage.Name && facet == FacetKind.Main)
            return ValueTask.FromResult(supportPackage.PackageSymbols.SymbolTree);

        throw new InvalidOperationException($"Invalid package and facet combination: {packageName}, {facet}");
    }
}
