using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal static class ContainingNamespaceAttribute
{
    public static NamespaceSymbol NamespaceDeclarationInherited(INamespaceDeclarationNode node)
        => node.Symbol;

    public static NamespaceSymbol? TypeDeclarationInherited(ITypeDeclarationNode _)
        => null;
}
