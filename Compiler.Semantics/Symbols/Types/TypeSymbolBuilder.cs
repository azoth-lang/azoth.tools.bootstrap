using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Contexts;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithTypeDeclarationPromises;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithTypeDeclarationSymbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Types;

internal partial class TypeSymbolBuilder
{
    private readonly SymbolBuilderContext context;

    private TypeSymbolBuilder(SymbolBuilderContext context)
    {
        this.context = context;
    }

    private partial SymbolBuilderContext EndRun(To.Package to)
        => context;

    private partial To.Package TransformPackage(From.Package from)
    {
        var typeDeclarations = TypeDeclarationsDictionary(from.Declarations);
        //var declarations = TransformNamespaceMemberDeclarations(from.Declarations,
        throw new System.NotImplementedException();
    }

    private partial To.ClassDeclaration TransformClassDeclaration(
        From.ClassDeclaration from,
        TypeLookup typeDeclarations)
        => throw new System.NotImplementedException();

    private partial To.StructDeclaration TransformStructDeclaration(
        From.StructDeclaration from,
        TypeLookup typeDeclarations)
        => throw new System.NotImplementedException();

    private partial To.TraitDeclaration TransformTraitDeclaration(
        From.TraitDeclaration from,
        TypeLookup typeDeclarations)
        => throw new System.NotImplementedException();


    private static IEnumerable<From.TypeDeclaration> TypeDeclarations(IEnumerable<From.NamespaceMemberDeclaration> declarations)
        => declarations
           .OfType<From.TypeDeclaration>()
           .SelectMany(t => MoreEnumerable.TraverseDepthFirst(t, c => c.Members.OfType<From.TypeDeclaration>()));

    private static TypeLookup TypeDeclarationsDictionary(
        IEnumerable<From.NamespaceMemberDeclaration> declarations)
        => new(TypeDeclarations(declarations).ToFixedDictionary(t => (IPromise<UserTypeSymbol>)t.Symbol));

    private struct TypeLookup
    {
        private readonly FixedDictionary<IPromise<UserTypeSymbol>, From.TypeDeclaration> typeDeclarations;
        public TypeLookup(FixedDictionary<IPromise<UserTypeSymbol>, From.TypeDeclaration> typeDeclarations)
        {
            this.typeDeclarations = typeDeclarations;
        }
    }
}
