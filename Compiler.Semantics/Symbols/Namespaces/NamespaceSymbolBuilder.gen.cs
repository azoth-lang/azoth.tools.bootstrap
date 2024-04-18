using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using From = Azoth.Tools.Bootstrap.Compiler.IST.Concrete;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithNamespaceSymbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Namespaces;

internal sealed partial class NamespaceSymbolBuilder
{
    private IFixedSet<To.CompilationUnit> Transform(IEnumerable<From.CompilationUnit> from, PackageSymbol containingSymbol, ISymbolTreeBuilder treeBuilder)
        => from.Select(cu => Transform(cu, containingSymbol, treeBuilder)).ToFixedSet();

    private To.CompilationUnit Create(
        From.CompilationUnit from,
        NamespaceSymbol containingSymbol,
        ISymbolTreeBuilder treeBuilder)
        => Create(from, Transform(from.Declarations, containingSymbol, treeBuilder));

    private IFixedList<To.NamespaceMemberDeclaration> Transform(IEnumerable<From.NamespaceMemberDeclaration> from, NamespaceSymbol containingSymbol, ISymbolTreeBuilder treeBuilder)
        => from.Select(f => Transform(f, containingSymbol, treeBuilder)).ToFixedList();

    private To.NamespaceMemberDeclaration Transform(From.NamespaceMemberDeclaration from, NamespaceSymbol containingSymbol, ISymbolTreeBuilder treeBuilder)
    {
        return from switch
        {
            From.NamespaceDeclaration f => Transform(f, containingSymbol, treeBuilder),
            From.FunctionDeclaration f => Transform(f, containingSymbol),
            From.TypeDeclaration f => Transform(f, containingSymbol),
            _ => throw ExhaustiveMatch.Failed(from)
        };
    }

    private To.NamespaceDeclaration Create(
        From.NamespaceDeclaration from,
        NamespaceSymbol containingSymbol,
        NamespaceSymbol childContainingSymbol,
        ISymbolTreeBuilder treeBuilder)
    {
        var declarations = Transform(from.Declarations, childContainingSymbol, treeBuilder);
        return Create(from, containingSymbol, childContainingSymbol, declarations);
    }

    private To.TypeDeclaration Create(From.TypeDeclaration from, NamespaceSymbol? containingSymbol, NamespaceSymbol? childContainingSymbol)
    {
        return from switch
        {
            From.ClassDeclaration f => Create(f, containingSymbol, childContainingSymbol),
            From.TraitDeclaration f => Create(f, containingSymbol, childContainingSymbol),
            From.StructDeclaration f => Create(f, containingSymbol, childContainingSymbol),
            _ => throw ExhaustiveMatch.Failed(from)
        };
    }

    private To.ClassDeclaration Create(
        From.ClassDeclaration from,
        NamespaceSymbol? containingSymbol,
        NamespaceSymbol? childContainingSymbol)
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, from.BaseTypeName, Transform(from.Members, childContainingSymbol), from.GenericParameters, from.SupertypeNames, containingSymbol);

    private To.TraitDeclaration Create(
        From.TraitDeclaration from,
        NamespaceSymbol? containingSymbol,
        NamespaceSymbol? childContainingSymbol)
        => To.TraitDeclaration.Create(from.Syntax, Transform(from.Members, childContainingSymbol), from.GenericParameters, from.SupertypeNames, containingSymbol);

    private To.StructDeclaration Create(
        From.StructDeclaration from,
        NamespaceSymbol? containingSymbol,
        NamespaceSymbol? childContainingSymbol)
        => To.StructDeclaration.Create(from.Syntax, Transform(from.Members, childContainingSymbol), from.GenericParameters, from.SupertypeNames, containingSymbol);

    private IFixedList<To.ClassMemberDeclaration> Transform(IEnumerable<From.ClassMemberDeclaration> from, NamespaceSymbol? containingSymbol)
        => from.Select(f => Transform(f, containingSymbol)).ToFixedList();

    private To.ClassMemberDeclaration Transform(From.ClassMemberDeclaration from, NamespaceSymbol? containingSymbol)
    {
        return from switch
        {
            From.TypeDeclaration f => Transform(f, containingSymbol),
            _ => throw ExhaustiveMatch.Failed(from)
        };
    }

    private IEnumerable<To.TraitMemberDeclaration> Transform(
        IEnumerable<From.TraitMemberDeclaration> from,
        NamespaceSymbol? containingSymbol)
        => from.Select(f => Transform(f, containingSymbol)).ToFixedList();

    private To.TraitMemberDeclaration Transform(From.TraitMemberDeclaration from, NamespaceSymbol? containingSymbol)
    {
        return from switch
        {
            From.TypeDeclaration f => Transform(f, containingSymbol),
            _ => throw ExhaustiveMatch.Failed(from)
        };
    }

    private IEnumerable<To.StructMemberDeclaration> Transform(
        IEnumerable<From.StructMemberDeclaration> from,
        NamespaceSymbol? containingSymbol)
        => from.Select(f => Transform(f, containingSymbol)).ToFixedList();

    private To.StructMemberDeclaration Transform(From.StructMemberDeclaration from, NamespaceSymbol? containingSymbol)
    {
        return from switch
        {
            From.TypeDeclaration f => Transform(f, containingSymbol),
            _ => throw ExhaustiveMatch.Failed(from)
        };
    }
}
