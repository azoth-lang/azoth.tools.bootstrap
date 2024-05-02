using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal static class SymbolDefinitions
{
    public static PackageSymbol Package(IPackageNode package) => new PackageSymbol(package.Name);
}
