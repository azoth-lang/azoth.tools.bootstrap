using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.IST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Contexts;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using From = Azoth.Tools.Bootstrap.Compiler.IST.Concrete;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithNamespaceSymbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Namespaces;

internal sealed partial class NamespaceSymbolBuilder : ITransformPass<From.Package, SymbolBuilderContext, To.Package, SymbolBuilderContext>
{
    public static (To.Package, SymbolBuilderContext) Run(From.Package from, SymbolBuilderContext context)
    {
        var pass = new NamespaceSymbolBuilder(context);
        pass.StartRun();
        var to = pass.Transform(from);
        return (to, pass.EndRun(to));
    }

    partial void StartRun();

    private partial SymbolBuilderContext EndRun(To.Package package);

    private partial To.Package Transform(From.Package value);

    private static To.Package Create(
        From.Package from,
        IEnumerable<To.CompilationUnit> compilationUnits,
        IEnumerable<To.CompilationUnit> testingCompilationUnits)
    {
        return To.Package.Create(from.Syntax, from.Symbol, from.References,
                                  compilationUnits.ToFixedSet(),
                                  testingCompilationUnits.ToFixedSet());
    }

    private IFixedSet<To.CompilationUnit> Transform(IEnumerable<From.CompilationUnit> from, PackageSymbol containingSymbol, ISymbolTreeBuilder treeBuilder)
        => from.Select(cu => Transform(cu, containingSymbol, treeBuilder)).ToFixedSet();

    private partial To.CompilationUnit Transform(From.CompilationUnit from, PackageSymbol containingSymbol, ISymbolTreeBuilder treeBuilder);

    private static To.CompilationUnit Create(
        From.CompilationUnit from,
        IEnumerable<To.NamespaceMemberDeclaration> declarations)
        => To.CompilationUnit.Create(from.Syntax, from.File, from.ImplicitNamespaceName, from.UsingDirectives, declarations);

    private IFixedList<To.NamespaceMemberDeclaration> Transform(IEnumerable<From.NamespaceMemberDeclaration> from, NamespaceOrPackageSymbol containingSymbol, ISymbolTreeBuilder treeBuilder)
        => from.Select(f => Transform(f, containingSymbol, treeBuilder)).ToFixedList();

    private To.NamespaceMemberDeclaration Transform(From.NamespaceMemberDeclaration from, NamespaceOrPackageSymbol containingSymbol, ISymbolTreeBuilder treeBuilder)
    {
        return from switch
        {
            From.NamespaceDeclaration f => Transform(f, containingSymbol, treeBuilder),
            From.FunctionDeclaration f => Transform(f, containingSymbol),
            From.TypeDeclaration f => Transform(f, containingSymbol),
            _ => throw ExhaustiveMatch.Failed(from)
        };
    }

    private partial To.NamespaceDeclaration Transform(
        From.NamespaceDeclaration from,
        NamespaceOrPackageSymbol containingSymbol,
        ISymbolTreeBuilder treeBuilder);

    private static To.NamespaceDeclaration Create(
        From.NamespaceDeclaration from,
        NamespaceOrPackageSymbol containingSymbol,
        NamespaceOrPackageSymbol symbol,
        IEnumerable<To.NamespaceMemberDeclaration> declarations)
        => To.NamespaceDeclaration.Create(symbol, containingSymbol, from.Syntax, from.IsGlobalQualified, from.DeclaredNames, from.UsingDirectives, declarations);

    private partial To.FunctionDeclaration Transform(
        From.FunctionDeclaration from,
        NamespaceOrPackageSymbol containingSymbol);

    private static To.FunctionDeclaration Create(
        From.FunctionDeclaration from,
        NamespaceOrPackageSymbol containingSymbol)
        => To.FunctionDeclaration.Create(containingSymbol, from.Syntax);

    private partial To.TypeDeclaration Transform(
        From.TypeDeclaration from,
        NamespaceOrPackageSymbol? containingSymbol);

    private static To.TypeDeclaration Create(From.TypeDeclaration from, NamespaceOrPackageSymbol? containingSymbol, NamespaceOrPackageSymbol? childContainingSymbol)
    {
        return from switch
        {
            From.ClassDeclaration f => To.ClassDeclaration.Create(f.Syntax, f.IsAbstract, Transform(f.Members, childContainingSymbol), containingSymbol),
            From.TraitDeclaration f => To.TraitDeclaration.Create(f.Syntax, Transform(f.Members, childContainingSymbol), containingSymbol),
            From.StructDeclaration f => To.StructDeclaration.Create(f.Syntax, Transform(f.Members, childContainingSymbol), containingSymbol),
            _ => throw ExhaustiveMatch.Failed(from)
        };
    }

    private static IFixedList<To.ClassMemberDeclaration> Transform(IEnumerable<From.ClassMemberDeclaration> from, NamespaceOrPackageSymbol? containingSymbol)
        => from.Select(f => Transform(f, containingSymbol)).ToFixedList();

    private static To.ClassMemberDeclaration Transform(From.ClassMemberDeclaration from, NamespaceOrPackageSymbol? containingSymbol)
        => To.ClassMemberDeclaration.Create(from.Syntax, containingSymbol);

    private static IEnumerable<To.TraitMemberDeclaration> Transform(
        IEnumerable<From.TraitMemberDeclaration> from,
        NamespaceOrPackageSymbol? containingSymbol)
        => from.Select(f => Transform(f, containingSymbol)).ToFixedList();

    private static To.TraitMemberDeclaration Transform(From.TraitMemberDeclaration from, NamespaceOrPackageSymbol? containingSymbol)
        => To.TraitMemberDeclaration.Create(from.Syntax, containingSymbol);

    private static IEnumerable<To.StructMemberDeclaration> Transform(
        IEnumerable<From.StructMemberDeclaration> from,
        NamespaceOrPackageSymbol? containingSymbol)
        => from.Select(f => Transform(f, containingSymbol)).ToFixedList();

    private static To.StructMemberDeclaration Transform(From.StructMemberDeclaration from, NamespaceOrPackageSymbol? containingSymbol)
        => To.StructMemberDeclaration.Create(from.Syntax, containingSymbol);
}
