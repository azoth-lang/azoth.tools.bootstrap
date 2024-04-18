using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithDeclarationLexicalScopes;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithoutCompilationUnits;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

internal sealed partial class CompilationUnitRemover
{
    partial void StartRun();

    partial void EndRun(To.Package package);

    private partial To.Package Transform(From.Package from);

    private To.Package Create(
        From.Package from,
        IFixedSet<To.NamespaceMemberDeclaration> declarations,
        IFixedSet<To.NamespaceMemberDeclaration> testingDeclarations)
        => To.Package.Create(declarations, testingDeclarations, from.LexicalScope, from.Syntax,
                       from.Symbol, from.References);

    private IFixedSet<To.NamespaceMemberDeclaration> Transform(IEnumerable<From.CompilationUnit> from)
        => from.SelectMany(Transform).ToFixedSet();

    private partial IEnumerable<To.NamespaceMemberDeclaration> Transform(From.CompilationUnit from);

    private IFixedList<To.NamespaceMemberDeclaration> Transform(IFixedList<From.NamespaceMemberDeclaration> from, CodeFile file)
        => from.SelectMany(f => Transform(f, file)).ToFixedList();

    private IEnumerable<To.NamespaceMemberDeclaration> Transform(From.NamespaceMemberDeclaration from, CodeFile file)
        => Create(from, file);

    private IEnumerable<To.NamespaceMemberDeclaration> Create(From.NamespaceMemberDeclaration from, CodeFile file)
        => from switch
        {
            From.NamespaceDeclaration f => Transform(f, file),
            From.TypeDeclaration f => Transform(f, file),
            From.FunctionDeclaration f => Transform(f, file),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.NamespaceMemberDeclaration> Transform(From.NamespaceDeclaration from, CodeFile file)
        => from.Declarations.SelectMany(d => Transform(d, file)).ToFixedList();

    private IEnumerable<To.NamespaceMemberDeclaration> Transform(
        From.TypeDeclaration from,
        CodeFile file)
        => Create(from, file).Yield();

    private To.TypeDeclaration Create(From.TypeDeclaration from, CodeFile file)
        => from switch
        {
            From.ClassDeclaration f => Create(f, file),
            From.TraitDeclaration f => Create(f, file),
            From.StructDeclaration f => Create(f, file),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.ClassDeclaration Create(From.ClassDeclaration from, CodeFile file)
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, from.BaseTypeName, Transform(from.Members, file),
            from.LexicalScope, from.GenericParameters, from.SupertypeNames, file, from.ContainingLexicalScope,
            from.ContainingSymbol);

    private IFixedList<To.ClassMemberDeclaration> Transform(IEnumerable<From.ClassMemberDeclaration> from, CodeFile file)
        => from.Select(f => Transform(f, file)).ToFixedList();

    private To.ClassMemberDeclaration Transform(From.ClassMemberDeclaration from, CodeFile file)
        => from switch
        {
            From.ClassDeclaration f => Create(f, file),
            From.TraitDeclaration f => Create(f, file),
            From.StructDeclaration f => Create(f, file),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.TraitDeclaration Create(From.TraitDeclaration from, CodeFile file)
        => To.TraitDeclaration.Create(from.Syntax, Transform(from.Members, file), from.LexicalScope,
            from.GenericParameters, from.SupertypeNames, file, from.ContainingLexicalScope, from.ContainingSymbol);

    private IFixedList<To.TraitMemberDeclaration> Transform(IEnumerable<From.TraitMemberDeclaration> from, CodeFile file)
        => from.Select(f => Transform(f, file)).ToFixedList();

    private To.TraitMemberDeclaration Transform(From.TraitMemberDeclaration from, CodeFile file)
        => from switch
        {
            From.ClassDeclaration f => Create(f, file),
            From.TraitDeclaration f => Create(f, file),
            From.StructDeclaration f => Create(f, file),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.StructDeclaration Create(From.StructDeclaration from, CodeFile file)
        => To.StructDeclaration.Create(from.Syntax, Transform(from.Members, file), from.LexicalScope,
            from.GenericParameters, from.SupertypeNames, file, from.ContainingLexicalScope, from.ContainingSymbol);

    private IFixedList<To.StructMemberDeclaration> Transform(
        IEnumerable<From.StructMemberDeclaration> from,
        CodeFile file)
        => from.Select(f => Transform(f, file)).ToFixedList();

    private To.StructMemberDeclaration Transform(From.StructMemberDeclaration from, CodeFile file)
        => from switch
        {
            From.ClassDeclaration f => Create(f, file),
            From.TraitDeclaration f => Create(f, file),
            From.StructDeclaration f => Create(f, file),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IEnumerable<To.FunctionDeclaration> Transform(
        From.FunctionDeclaration from,
        CodeFile file)
        => Create(from, file).Yield();

    private To.FunctionDeclaration Create(From.FunctionDeclaration from, CodeFile file)
        => To.FunctionDeclaration.Create(from.ContainingSymbol, from.Syntax, file, from.ContainingLexicalScope);
}
