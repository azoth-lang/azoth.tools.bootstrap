using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithDeclarationLexicalScopes;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithoutCompilationUnits;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal partial class CompilationUnitRemover
{
    private partial To.Package TransformPackage(From.Package from)
        => Create(from, TransformCompilationUnits(from.CompilationUnits), TransformCompilationUnits(from.TestingCompilationUnits));

    private partial IFixedSet<To.NamespaceMemberDeclaration> TransformCompilationUnits(
        IEnumerable<From.CompilationUnit> from)
        => from.SelectMany(TransformCompilationUnit).ToFixedSet();

    private partial IFixedList<To.NamespaceMemberDeclaration> TransformCompilationUnit(From.CompilationUnit from)
        => TransformNamespaceMemberDeclarations(from.Declarations, from.File);

    private partial IFixedSet<To.NamespaceMemberDeclaration> TransformNamespaceMemberDeclaration(
        From.NamespaceMemberDeclaration from, CodeFile file)
        => from switch
        {
            From.NamespaceDeclaration f => TransformNamespaceDeclaration(f, file),
            From.TypeDeclaration f => TransformTypeDeclaration(f, file).Yield().ToFixedSet(),
            From.FunctionDeclaration f => TransformFunctionDeclaration(f, file).Yield().ToFixedSet(),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private partial IFixedSet<To.NamespaceMemberDeclaration> TransformNamespaceDeclaration(From.NamespaceDeclaration from, CodeFile file)
        => from.Declarations.SelectMany(d => TransformNamespaceMemberDeclaration(d, file)).ToFixedSet();

    private To.ClassDeclaration Create(From.ClassDeclaration from, CodeFile file)
        => Create(from, TransformClassMemberDeclarations(from.Members, file), file);

    private To.StructDeclaration Create(From.StructDeclaration from, CodeFile file)
        => Create(from, TransformStructMemberDeclarations(from.Members, file), file);

    private To.TraitDeclaration Create(From.TraitDeclaration from, CodeFile file)
        => Create(from, TransformTraitMemberDeclarations(from.Members, file), file);
}
