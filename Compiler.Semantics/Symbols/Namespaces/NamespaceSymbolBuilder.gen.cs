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

    private To.ClassDeclaration CreateClassDeclaration(
        From.ClassDeclaration from,
        NamespaceSymbol? containingSymbol,
        NamespaceSymbol? childContainingSymbol)
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, from.BaseTypeName, TransformClassMemberDeclarations(from.Members, childContainingSymbol), from.GenericParameters, from.SupertypeNames, containingSymbol);

    private To.TraitDeclaration CreateTraitDeclaration(
        From.TraitDeclaration from,
        NamespaceSymbol? containingSymbol,
        NamespaceSymbol? childContainingSymbol)
        => To.TraitDeclaration.Create(from.Syntax, TransformTraitMemberDeclarations(from.Members, childContainingSymbol), from.GenericParameters, from.SupertypeNames, containingSymbol);

    private To.StructDeclaration CreateStructDeclaration(
        From.StructDeclaration from,
        NamespaceSymbol? containingSymbol,
        NamespaceSymbol? childContainingSymbol)
        => To.StructDeclaration.Create(from.Syntax, TransformStructMemberDeclarations(from.Members, childContainingSymbol), from.GenericParameters, from.SupertypeNames, containingSymbol);
}
