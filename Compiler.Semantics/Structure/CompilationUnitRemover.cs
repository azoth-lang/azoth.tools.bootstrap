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
    private partial To.Package Transform(From.Package from)
        => Create(from, Transform(from.CompilationUnits), Transform(from.TestingCompilationUnits));

    private partial IFixedSet<To.NamespaceMemberDeclaration> Transform(
        IEnumerable<From.CompilationUnit> from)
        => from.SelectMany(Transform).ToFixedSet();

    private partial IFixedList<To.NamespaceMemberDeclaration> Transform(From.CompilationUnit from)
        => Transform(from.Declarations, from.File);

    private partial IFixedSet<To.NamespaceMemberDeclaration> Transform(
        From.NamespaceMemberDeclaration from, CodeFile file)
        => from switch
        {
            From.NamespaceDeclaration f => Transform(f, file),
            From.TypeDeclaration f => Transform(f, file).Yield().ToFixedSet(),
            From.FunctionDeclaration f => Transform(f, file).Yield().ToFixedSet(),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private partial IFixedSet<To.NamespaceMemberDeclaration> Transform(From.NamespaceDeclaration from, CodeFile file)
        => from.Declarations.SelectMany(d => Transform(d, file)).ToFixedSet();

    private To.ClassDeclaration Create(From.ClassDeclaration from, CodeFile file)
        => Create(from, Transform(from.Members, file), file);

    private To.StructDeclaration Create(From.StructDeclaration from, CodeFile file)
        => Create(from, Transform(from.Members, file), file);

    private To.TraitDeclaration Create(From.TraitDeclaration from, CodeFile file)
        => Create(from, Transform(from.Members, file), file);
}
