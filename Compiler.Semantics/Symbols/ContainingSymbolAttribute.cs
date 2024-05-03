using System;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal static class ContainingSymbolAttribute
{
    public static NamespaceSymbol NamespaceDeclarationInherited(INamespaceDeclarationNode node)
        => node.Symbol;

    public static Symbol TypeDeclarationInherited(ITypeDeclarationNode node)
        => throw new NotImplementedException();
}
