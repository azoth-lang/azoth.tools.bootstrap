using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using ExhaustiveMatching;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithNamespaceSymbols;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithDeclarationLexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

internal sealed partial class DeclarationLexicalScopesBuilder
{
    private To.Package Create(From.Package from, PackageReferenceScope lexicalScope, PackageReferenceScope containingScope)
        => Create(from, lexicalScope, Transform(from.CompilationUnits, containingScope), Transform(from.TestingCompilationUnits, containingScope));

    private To.CompilationUnit Create(
        From.CompilationUnit from,
        DeclarationScope scope)
        => Create(from, scope, Transform(from.Declarations, scope));

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
        => To.NamespaceDeclaration.Create(lexicalScope, from.ContainingNamespace, from.Symbol, from.Syntax, from.IsGlobalQualified, from.DeclaredNames, from.UsingDirectives, declarations, containingScope);

    private To.TypeDeclaration Create(
        From.TypeDeclaration from,
        DeclarationLexicalScope containingScope,
        DeclarationScope lexicalScope)
        => from switch
        {
            From.ClassDeclaration f => Create(f, containingScope, lexicalScope),
            From.TraitDeclaration f => Create(f, containingScope, lexicalScope),
            From.StructDeclaration f => Create(f, containingScope, lexicalScope),
            _ => throw ExhaustiveMatch.Failed(from)
        };

    private To.ClassDeclaration Create(
        From.ClassDeclaration from,
        DeclarationLexicalScope containingScope,
        DeclarationScope lexicalScope)
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, Transform(from.BaseTypeName, containingScope),
            Transform(from.Members, containingScope), lexicalScope, from.GenericParameters, Transform(from.SupertypeNames, containingScope),
            containingScope, from.ContainingNamespace);

    private To.TraitDeclaration Create(
        From.TraitDeclaration from,
        DeclarationLexicalScope containingScope,
        DeclarationScope lexicalScope)
        => To.TraitDeclaration.Create(from.Syntax,
            Transform(from.Members, containingScope), lexicalScope, from.GenericParameters, Transform(from.SupertypeNames, containingScope),
            containingScope, from.ContainingNamespace);

    private To.StructDeclaration Create(
        From.StructDeclaration from,
        DeclarationLexicalScope containingScope,
        DeclarationScope lexicalScope)
        => To.StructDeclaration.Create(from.Syntax, Transform(from.Members, containingScope), lexicalScope, from.GenericParameters,
            Transform(from.SupertypeNames, containingScope), containingScope, from.ContainingNamespace);

    private To.UnresolvedSupertypeName Create(
        From.UnresolvedSupertypeName from,
        DeclarationLexicalScope containingScope)
        => Create(from, containingScope, Transform(from.TypeArguments, containingScope));

    private To.UnresolvedGenericTypeName Create(
        From.UnresolvedGenericTypeName from,
        DeclarationLexicalScope containingScope)
        => Create(from, Transform(from.TypeArguments, containingScope), containingScope);

    private To.UnresolvedQualifiedTypeName Create(
        From.UnresolvedQualifiedTypeName from,
        DeclarationLexicalScope containingScope)
        => Create(from, Transform(from.Context, containingScope), Transform(from.QualifiedName, containingScope), containingScope);

    private To.UnresolvedOptionalType Create(
        From.UnresolvedOptionalType from,
        DeclarationLexicalScope containingScope)
        => Create(from, Transform(from.Referent, containingScope));

    private To.UnresolvedCapabilityType Create(From.UnresolvedCapabilityType from, DeclarationLexicalScope containingScope)
        => Create(from, Transform(from.Referent, containingScope));

    private To.UnresolvedParameterType Create(
        From.UnresolvedParameterType from,
        DeclarationLexicalScope containingScope)
        => Create(from, Transform(from.Referent, containingScope));

    private To.UnresolvedCapabilityViewpointType Create(
        From.UnresolvedCapabilityViewpointType from,
        DeclarationLexicalScope containingScope)
        => Create(from, Transform(from.Referent, containingScope));

    private To.UnresolvedSelfViewpointType Create(
        From.UnresolvedSelfViewpointType from,
        DeclarationLexicalScope containingScope)
        => Create(from, Transform(from.Referent, containingScope));

    private To.UnresolvedFunctionType Create(
        From.UnresolvedFunctionType from,
        DeclarationLexicalScope containingScope)
        => Create(from, Transform(from.Parameters, containingScope), Transform(from.Return, containingScope));
}
