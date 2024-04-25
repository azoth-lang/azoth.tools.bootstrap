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
        => CreatePackage(from, TransformCompilationUnits(from.CompilationUnits), TransformCompilationUnits(from.TestingCompilationUnits));

    private partial IFixedSet<To.NamespaceMemberDeclaration> TransformCompilationUnits(
        IEnumerable<From.CompilationUnit> from)
        => from.SelectMany(TransformCompilationUnit).ToFixedSet();

    private partial IFixedList<To.NamespaceMemberDeclaration> TransformCompilationUnit(From.CompilationUnit from)
        => TransformNamespaceMemberDeclarations(from.Declarations, file: from.File);

    private partial IFixedSet<To.NamespaceMemberDeclaration> TransformNamespaceMemberDeclaration(
        From.NamespaceMemberDeclaration from, CodeFile file)
        => from switch
        {
            From.NamespaceDeclaration f => TransformNamespaceDeclaration(f, file),
            From.TypeDeclaration f => TransformTypeDeclaration(f, file).Yield().ToFixedSet(),
            From.FunctionDeclaration f => TransformFunctionDeclaration(f, file).Yield().ToFixedSet(),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.TypeDeclaration TransformTypeDeclaration(From.TypeDeclaration from, CodeFile file)
        => TransformTypeDeclaration(from, file, file);

    private partial IFixedSet<To.NamespaceMemberDeclaration> TransformNamespaceDeclaration(From.NamespaceDeclaration from, CodeFile file)
        => from.Declarations.SelectMany(d => TransformNamespaceMemberDeclaration(d, file)).ToFixedSet();
}
