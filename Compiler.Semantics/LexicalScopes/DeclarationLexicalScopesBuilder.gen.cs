using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.IST;
using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithNamespaceSymbols;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithDeclarationLexicalScopes;
using Void = Azoth.Tools.Bootstrap.Framework.Void;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

public sealed partial class DeclarationLexicalScopesBuilder : ITransformPass<From.Package, Void, To.Package, Void>
{
    public static To.Package Run(From.Package from)
    {
        var pass = new DeclarationLexicalScopesBuilder();
        pass.StartRun();
        var to = pass.Transform(from);
        pass.EndRun(to);
        return to;
    }

    static (To.Package, Void) ITransformPass<From.Package, Void, To.Package, Void>.Run(From.Package from, Void context)
        => (Run(from), default);

    partial void StartRun();

    partial void EndRun(To.Package package);

    private partial To.Package Transform(From.Package from);

    private To.Package Create(From.Package from, PackageReferenceScope lexicalScope, PackageReferenceScope containingScope)
        => Create(from, lexicalScope, Transform(from.CompilationUnits, containingScope), Transform(from.TestingCompilationUnits, containingScope));

    private To.Package Create(
        From.Package from,
        PackageReferenceScope lexicalScope,
        IEnumerable<To.CompilationUnit> compilationUnits,
        IEnumerable<To.CompilationUnit> testingCompilationUnits)
        => To.Package.Create(lexicalScope, from.Syntax, from.Symbol, from.References, compilationUnits, testingCompilationUnits);

    private IFixedSet<To.CompilationUnit> Transform(IEnumerable<From.CompilationUnit> from, PackageReferenceScope containingScope)
        => from.Select(f => Transform(f, containingScope)).ToFixedSet();

    private partial To.CompilationUnit Transform(From.CompilationUnit from, PackageReferenceScope containingScope);

    private To.CompilationUnit Create(
        From.CompilationUnit from,
        DeclarationScope scope)
        => Create(from, scope, Transform(from.Declarations, scope));

    private To.CompilationUnit Create(
        From.CompilationUnit from,
        DeclarationScope lexicalScope,
        IEnumerable<To.NamespaceMemberDeclaration> declarations)
        => To.CompilationUnit.Create(lexicalScope, from.Syntax, from.File, from.ImplicitNamespaceName,
            from.UsingDirectives, declarations);

    private IFixedList<To.NamespaceMemberDeclaration> Transform(
        IEnumerable<From.NamespaceMemberDeclaration> from,
        DeclarationScope containingScope)
        => from.Select(f => Transform(f, containingScope)).ToFixedList();

    private To.NamespaceMemberDeclaration Transform(From.NamespaceMemberDeclaration from, DeclarationLexicalScope containingScope)
        => from switch
        {
            From.NamespaceDeclaration f => Transform(f, containingScope),
            From.TypeDeclaration f => Transform(f, containingScope),
            From.FunctionDeclaration f => Transform(f, containingScope),
            _ => throw ExhaustiveMatch.Failed(from)
        };

    private partial To.NamespaceDeclaration Transform(From.NamespaceDeclaration from, DeclarationLexicalScope containingScope);

    private To.NamespaceDeclaration Create(
        From.NamespaceDeclaration from,
        DeclarationLexicalScope containingScope,
        DeclarationScope lexicalScope)
        => Create(from, containingScope, lexicalScope, Transform(from.Declarations, lexicalScope));

    private To.NamespaceDeclaration Create(
        From.NamespaceDeclaration from,
        DeclarationLexicalScope containingScope,
        DeclarationScope lexicalScope,
        IEnumerable<To.NamespaceMemberDeclaration> declarations)
        => To.NamespaceDeclaration.Create(lexicalScope, from.ContainingSymbol, from.Symbol, from.Syntax, from.IsGlobalQualified, from.DeclaredNames, from.UsingDirectives, declarations, containingScope);

    private To.NamespaceMemberDeclaration Transform(From.TypeDeclaration from, DeclarationLexicalScope containingScope)
        => throw new System.NotImplementedException();

    private To.NamespaceMemberDeclaration Transform(From.FunctionDeclaration from, DeclarationLexicalScope containingScope)
        => throw new System.NotImplementedException();
}
