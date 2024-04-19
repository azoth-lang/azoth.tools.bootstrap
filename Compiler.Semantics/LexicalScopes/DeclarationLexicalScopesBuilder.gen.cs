using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using Azoth.Tools.Bootstrap.Framework;
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
            containingScope, from.ContainingSymbol);

    private To.ClassMemberDeclaration Transform(From.ClassMemberDeclaration from, DeclarationLexicalScope containingScope)
        => from switch
        {
            From.TypeDeclaration f => Transform(f, containingScope),
            _ => throw ExhaustiveMatch.Failed(from)
        };

    private To.TraitDeclaration Create(
        From.TraitDeclaration from,
        DeclarationLexicalScope containingScope,
        DeclarationScope lexicalScope)
        => To.TraitDeclaration.Create(from.Syntax,
            Transform(from.Members, containingScope), lexicalScope, from.GenericParameters, Transform(from.SupertypeNames, containingScope),
            containingScope, from.ContainingSymbol);

    private To.TraitMemberDeclaration Transform(
        From.TraitMemberDeclaration from,
        DeclarationLexicalScope containingScope)
        => from switch
        {
            From.TypeDeclaration f => Transform(f, containingScope),
            _ => throw ExhaustiveMatch.Failed(from)
        };

    private To.StructDeclaration Create(
        From.StructDeclaration from,
        DeclarationLexicalScope containingScope,
        DeclarationScope lexicalScope)
        => To.StructDeclaration.Create(from.Syntax, Transform(from.Members, containingScope), lexicalScope, from.GenericParameters,
            Transform(from.SupertypeNames, containingScope), containingScope, from.ContainingSymbol);

    private To.StructMemberDeclaration Transform(
        From.StructMemberDeclaration from,
        DeclarationLexicalScope containingScope)
        => from switch
        {
            From.TypeDeclaration f => Transform(f, containingScope),
            _ => throw ExhaustiveMatch.Failed(from)
        };

    private IFixedList<To.UnresolvedSupertypeName> Transform(
        IEnumerable<From.UnresolvedSupertypeName> from,
        DeclarationLexicalScope containingScope)
        => from.Select(f => Transform(f, containingScope)).ToFixedList();

    [return: NotNullIfNotNull(nameof(from))]
    private To.UnresolvedSupertypeName? Transform(From.UnresolvedSupertypeName? from, DeclarationLexicalScope containingScope)
    {
        if (from is null) return null;
        return To.UnresolvedSupertypeName.Create(containingScope, from.Syntax, from.Name, Transform(from.TypeArguments, containingScope));
    }

    // TODO for types, it is possible there is not change. Check if the child is the same instance and don't recreate parent in that case.

    private IFixedList<To.UnresolvedType> Transform(IEnumerable<From.UnresolvedType> from, DeclarationLexicalScope containingScope)
        => from.Select(f => Transform(f, containingScope)).ToFixedList();

    private To.UnresolvedType Transform(From.UnresolvedType from, DeclarationLexicalScope containingScope)
        => from switch
        {
            From.UnresolvedTypeName f => Transform(f, containingScope),
            From.UnresolvedCapabilityType f => Transform(f, containingScope),
            From.UnresolvedFunctionType f => Transform(f, containingScope),
            From.UnresolvedOptionalType f => Transform(f, containingScope),
            From.UnresolvedCapabilityViewpointType f => Transform(f, containingScope),
            From.UnresolvedSelfViewpointType f => Transform(f, containingScope),
            _ => throw ExhaustiveMatch.Failed(from)
        };

    private To.UnresolvedTypeName Transform(From.UnresolvedTypeName from,
        DeclarationLexicalScope containingScope)
        => from switch
        {
            From.UnresolvedSimpleTypeName f => Transform(f, containingScope),
            From.UnresolvedGenericTypeName f => Transform(f, containingScope),
            From.UnresolvedQualifiedTypeName f => Transform(f, containingScope),
            _ => throw ExhaustiveMatch.Failed(from)
        };

    private To.UnresolvedSimpleTypeName Transform(From.UnresolvedSimpleTypeName from,
        DeclarationLexicalScope containingScope)
        => from switch
        {
            From.UnresolvedIdentifierTypeName f => Transform(f, containingScope),
            From.UnresolvedSpecialTypeName f => Transform(f, containingScope),
            _ => throw ExhaustiveMatch.Failed(from)
        };

    private To.UnresolvedIdentifierTypeName Transform(From.UnresolvedIdentifierTypeName from,
        DeclarationLexicalScope containingScope)
        => To.UnresolvedIdentifierTypeName.Create(from.Syntax, from.Name, containingScope);

    private To.UnresolvedSpecialTypeName Transform(From.UnresolvedSpecialTypeName from,
        DeclarationLexicalScope containingScope)
        => To.UnresolvedSpecialTypeName.Create(from.Syntax, from.Name, containingScope);

    private To.UnresolvedGenericTypeName Transform(From.UnresolvedGenericTypeName from,
        DeclarationLexicalScope containingScope)
        => To.UnresolvedGenericTypeName.Create(from.Syntax, from.Name, Transform(from.TypeArguments, containingScope), containingScope);

    private To.UnresolvedQualifiedTypeName Transform(
        From.UnresolvedQualifiedTypeName from,
        DeclarationLexicalScope containingScope)
        => To.UnresolvedQualifiedTypeName.Create(from.Syntax, Transform(from.Context, containingScope),
            Transform(from.QualifiedName, containingScope), containingScope, from.Name);

    private To.UnresolvedStandardTypeName Transform(
        From.UnresolvedStandardTypeName from,
        DeclarationLexicalScope containingScope)
        => from switch
        {
            From.UnresolvedIdentifierTypeName f => Transform(f, containingScope),
            From.UnresolvedGenericTypeName f => Transform(f, containingScope),
            _ => throw ExhaustiveMatch.Failed(from)
        };

    private To.UnresolvedCapabilityType Transform(From.UnresolvedCapabilityType from,
        DeclarationLexicalScope containingScope)
        => To.UnresolvedCapabilityType.Create(from.Syntax, from.Capability, Transform(from.Referent, containingScope));

    private To.UnresolvedFunctionType Transform(
        From.UnresolvedFunctionType from,
        DeclarationLexicalScope containingScope)
        => To.UnresolvedFunctionType.Create(from.Syntax, Transform(from.Parameters, containingScope),
            Transform(from.Return, containingScope));

    private IFixedList<To.UnresolvedParameterType> Transform(IEnumerable<From.UnresolvedParameterType> from, DeclarationLexicalScope containingScope)
        => from.Select(f => Transform(f, containingScope)).ToFixedList();

    private To.UnresolvedParameterType Transform(From.UnresolvedParameterType from, DeclarationLexicalScope containingScope)
        => To.UnresolvedParameterType.Create(from.Syntax, from.IsLent, Transform(from.Referent, containingScope));

    private To.UnresolvedOptionalType Transform(From.UnresolvedOptionalType from,
        DeclarationLexicalScope containingScope)
        => To.UnresolvedOptionalType.Create(from.Syntax, Transform(from.Referent, containingScope));

    private To.UnresolvedCapabilityViewpointType Transform(From.UnresolvedCapabilityViewpointType from,
        DeclarationLexicalScope containingScope)
        => To.UnresolvedCapabilityViewpointType.Create(from.Syntax, from.Capability, Transform(from.Referent, containingScope));

    private To.UnresolvedSelfViewpointType Transform(From.UnresolvedSelfViewpointType from, DeclarationLexicalScope containingScope)
        => To.UnresolvedSelfViewpointType.Create(from.Syntax, Transform(from.Referent, containingScope));

    private To.FunctionDeclaration Transform(From.FunctionDeclaration from, DeclarationLexicalScope containingScope)
        => To.FunctionDeclaration.Create(from.ContainingSymbol, from.Syntax, containingScope);
}
