using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal static class SymbolAttribute
{
    public static PackageSymbol Package(IPackageNode node) => new PackageSymbol(node.Name);

    public static TypeSymbol SpecialTypeName(ISpecialTypeNameNode node)
        => Primitive.SymbolTree.LookupSymbol(node.Name);
}
