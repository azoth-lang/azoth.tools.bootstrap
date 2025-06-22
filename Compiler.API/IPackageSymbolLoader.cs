using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;

namespace Azoth.Tools.Bootstrap.Compiler.API;

/// <summary>
/// Loads symbols for packages referenced for the package being currently compiled.
/// </summary>
public interface IPackageSymbolLoader
{
    public static IPackageSymbolLoader None = InvalidPackageSymbolLoader.Instance;

    // TODO add Package ID parameter which could be either a hash or path (hash is used when there is a lock file)
    ValueTask<FixedSymbolTree> LoadSymbolsAsync(IdentifierName packageName, FacetKind facet);
}
