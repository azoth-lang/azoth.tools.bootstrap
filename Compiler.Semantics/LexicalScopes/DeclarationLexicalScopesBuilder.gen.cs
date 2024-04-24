using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using ExhaustiveMatching;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithNamespaceSymbols;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithDeclarationLexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

internal sealed partial class DeclarationLexicalScopesBuilder
{
    private To.Package Create(From.Package from, PackageReferenceScope lexicalScope, PackageReferenceScope containingScope)
        => Create(from, lexicalScope, TransformCompilationUnits(from.CompilationUnits, containingScope),
            TransformCompilationUnits(from.TestingCompilationUnits, containingScope));

    private To.CompilationUnit Create(
        From.CompilationUnit from,
        DeclarationScope scope)
        => Create(from, scope, TransformNamespaceMemberDeclarations(from.Declarations, scope));

    private To.NamespaceDeclaration Create(
        From.NamespaceDeclaration from,
        DeclarationLexicalScope containingScope,
        DeclarationScope lexicalScope)
        => Create(from, containingScope, lexicalScope, TransformNamespaceMemberDeclarations(from.Declarations, lexicalScope));

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
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, TransformUnresolvedSupertypeName(from.BaseTypeName, containingScope),
            TransformClassMemberDeclarations(from.Members, containingScope), lexicalScope, from.GenericParameters, TransformUnresolvedSupertypeNames(from.SupertypeNames, containingScope),
            containingScope, from.ContainingNamespace);

    private To.TraitDeclaration Create(
        From.TraitDeclaration from,
        DeclarationLexicalScope containingScope,
        DeclarationScope lexicalScope)
        => To.TraitDeclaration.Create(from.Syntax,
            TransformTraitMemberDeclarations(from.Members, containingScope), lexicalScope, from.GenericParameters, TransformUnresolvedSupertypeNames(from.SupertypeNames, containingScope),
            containingScope, from.ContainingNamespace);

    private To.StructDeclaration Create(
        From.StructDeclaration from,
        DeclarationLexicalScope containingScope,
        DeclarationScope lexicalScope)
        => To.StructDeclaration.Create(from.Syntax, TransformStructMemberDeclarations(from.Members, containingScope), lexicalScope, from.GenericParameters,
            TransformUnresolvedSupertypeNames(from.SupertypeNames, containingScope), containingScope, from.ContainingNamespace);

    private To.UnresolvedSupertypeName Create(
        From.UnresolvedSupertypeName from,
        DeclarationLexicalScope containingScope)
        => Create(from, containingScope, TransformUnresolvedTypes(from.TypeArguments, containingScope));

    private To.UnresolvedGenericTypeName Create(
        From.UnresolvedGenericTypeName from,
        DeclarationLexicalScope containingScope)
        => Create(from, TransformUnresolvedTypes(from.TypeArguments, containingScope), containingScope);

    private To.UnresolvedQualifiedTypeName Create(
        From.UnresolvedQualifiedTypeName from,
        DeclarationLexicalScope containingScope)
        => Create(from, TransformUnresolvedTypeName(from.Context, containingScope), TransformUnresolvedStandardTypeName(from.QualifiedName, containingScope), containingScope);

    private To.UnresolvedOptionalType Create(
        From.UnresolvedOptionalType from,
        DeclarationLexicalScope containingScope)
        => Create(from, TransformUnresolvedType(from.Referent, containingScope));

    private To.UnresolvedCapabilityType Create(From.UnresolvedCapabilityType from, DeclarationLexicalScope containingScope)
        => Create(from, TransformUnresolvedType(from.Referent, containingScope));

    private To.UnresolvedParameterType Create(
        From.UnresolvedParameterType from,
        DeclarationLexicalScope containingScope)
        => Create(from, TransformUnresolvedType(from.Referent, containingScope));

    private To.UnresolvedCapabilityViewpointType Create(
        From.UnresolvedCapabilityViewpointType from,
        DeclarationLexicalScope containingScope)
        => Create(from, TransformUnresolvedType(from.Referent, containingScope));

    private To.UnresolvedSelfViewpointType Create(
        From.UnresolvedSelfViewpointType from,
        DeclarationLexicalScope containingScope)
        => Create(from, TransformUnresolvedType(from.Referent, containingScope));

    private To.UnresolvedFunctionType Create(
        From.UnresolvedFunctionType from,
        DeclarationLexicalScope containingScope)
        => Create(from, TransformUnresolvedParameterTypes(from.Parameters, containingScope), TransformUnresolvedType(from.Return, containingScope));
}
