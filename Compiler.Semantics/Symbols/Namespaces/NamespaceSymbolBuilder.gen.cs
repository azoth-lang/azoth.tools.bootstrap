using Azoth.Tools.Bootstrap.Compiler.Symbols;
using ExhaustiveMatching;
using From = Azoth.Tools.Bootstrap.Compiler.IST.Concrete;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithNamespaceSymbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Namespaces;

internal sealed partial class NamespaceSymbolBuilder
{
    private To.TypeDeclaration CreateTypeDeclaration(From.TypeDeclaration from, NamespaceSymbol? containingSymbol, NamespaceSymbol? childContainingSymbol)
    {
        return from switch
        {
            From.ClassDeclaration f => CreateClassDeclaration(f, containingSymbol, childContainingSymbol),
            From.TraitDeclaration f => CreateTraitDeclaration(f, containingSymbol, childContainingSymbol),
            From.StructDeclaration f => CreateStructDeclaration(f, containingSymbol, childContainingSymbol),
            _ => throw ExhaustiveMatch.Failed(from)
        };
    }
}
