using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.IST;
using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithNamespaceSymbols;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithDeclarationLexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class DeclarationLexicalScopesBuilder : ITransformPass<From.Package, Void, To.Package, Void>
{
    public static To.Package Run(From.Package from)
    {
        var pass = new DeclarationLexicalScopesBuilder();
        pass.StartRun();
        var to = pass.TransformPackage(from);
        pass.EndRun(to);
        return to;
    }

    static (To.Package, Void) ITransformPass<From.Package, Void, To.Package, Void>.Run(From.Package from, Void context)
        => (Run(from), default);


    partial void StartRun();

    partial void EndRun(To.Package to);

    private partial To.Package TransformPackage(From.Package from);

    private partial To.CompilationUnit TransformCompilationUnit(From.CompilationUnit from, PackageReferenceScope containingScope);

    private partial To.TypeDeclaration TransformTypeDeclaration(From.TypeDeclaration from, DeclarationLexicalScope containingScope);

    private partial To.NamespaceDeclaration TransformNamespaceDeclaration(From.NamespaceDeclaration from, DeclarationLexicalScope containingScope);

    [return: NotNullIfNotNull(nameof(from))]
    private To.UnresolvedSupertypeName? TransformUnresolvedSupertypeName(From.UnresolvedSupertypeName? from, DeclarationLexicalScope containingScope, DeclarationLexicalScope childContainingScope)
        => from is not null ? CreateUnresolvedSupertypeName(from, containingScope, childContainingScope) : null;

    private To.FunctionDeclaration TransformFunctionDeclaration(From.FunctionDeclaration from, DeclarationLexicalScope containingScope)
        => CreateFunctionDeclaration(from, containingScope);

    private To.UnresolvedIdentifierTypeName TransformUnresolvedIdentifierTypeName(From.UnresolvedIdentifierTypeName from, DeclarationLexicalScope containingScope)
        => CreateUnresolvedIdentifierTypeName(from, containingScope);

    private To.UnresolvedSpecialTypeName TransformUnresolvedSpecialTypeName(From.UnresolvedSpecialTypeName from, DeclarationLexicalScope containingScope)
        => CreateUnresolvedSpecialTypeName(from, containingScope);

    private To.UnresolvedGenericTypeName TransformUnresolvedGenericTypeName(From.UnresolvedGenericTypeName from, DeclarationLexicalScope containingScope, DeclarationLexicalScope childContainingScope)
        => CreateUnresolvedGenericTypeName(from, containingScope, childContainingScope);

    private To.UnresolvedQualifiedTypeName TransformUnresolvedQualifiedTypeName(From.UnresolvedQualifiedTypeName from, DeclarationLexicalScope containingScope, DeclarationLexicalScope childContainingScope)
        => CreateUnresolvedQualifiedTypeName(from, containingScope, childContainingScope);

    private IFixedSet<To.CompilationUnit> TransformCompilationUnits(IEnumerable<From.CompilationUnit> from, PackageReferenceScope containingScope)
        => from.Select(f => TransformCompilationUnit(f, containingScope)).ToFixedSet();

    private To.NamespaceMemberDeclaration TransformNamespaceMemberDeclaration(From.NamespaceMemberDeclaration from, DeclarationLexicalScope containingScope)
        => from switch
        {
            From.NamespaceDeclaration f => TransformNamespaceDeclaration(f, containingScope),
            From.FunctionDeclaration f => TransformFunctionDeclaration(f, containingScope),
            From.TypeDeclaration f => TransformTypeDeclaration(f, containingScope),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.ClassMemberDeclaration TransformClassMemberDeclaration(From.ClassMemberDeclaration from, DeclarationLexicalScope containingScope)
        => from switch
        {
            From.TypeDeclaration f => TransformTypeDeclaration(f, containingScope),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.StructMemberDeclaration TransformStructMemberDeclaration(From.StructMemberDeclaration from, DeclarationLexicalScope containingScope)
        => from switch
        {
            From.TypeDeclaration f => TransformTypeDeclaration(f, containingScope),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.TraitMemberDeclaration TransformTraitMemberDeclaration(From.TraitMemberDeclaration from, DeclarationLexicalScope containingScope)
        => from switch
        {
            From.TypeDeclaration f => TransformTypeDeclaration(f, containingScope),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.UnresolvedSupertypeName> TransformUnresolvedSupertypeNames(IEnumerable<From.UnresolvedSupertypeName> from, DeclarationLexicalScope containingScope, DeclarationLexicalScope childContainingScope)
        => from.Select(f => TransformUnresolvedSupertypeName(f, containingScope, childContainingScope)).ToFixedList();

    private To.ClassDeclaration TransformClassDeclaration(From.ClassDeclaration from, DeclarationScope newScope, DeclarationLexicalScope containingScope, DeclarationLexicalScope childContainingScope)
        => CreateClassDeclaration(from, newScope, containingScope, childContainingScope);

    private To.StructDeclaration TransformStructDeclaration(From.StructDeclaration from, DeclarationScope newScope, DeclarationLexicalScope containingScope, DeclarationLexicalScope childContainingScope)
        => CreateStructDeclaration(from, newScope, containingScope, childContainingScope);

    private To.TraitDeclaration TransformTraitDeclaration(From.TraitDeclaration from, DeclarationScope newScope, DeclarationLexicalScope containingScope, DeclarationLexicalScope childContainingScope)
        => CreateTraitDeclaration(from, newScope, containingScope, childContainingScope);

    private To.UnresolvedStandardTypeName TransformUnresolvedStandardTypeName(From.UnresolvedStandardTypeName from, DeclarationLexicalScope containingScope, DeclarationLexicalScope childContainingScope)
        => from switch
        {
            From.UnresolvedIdentifierTypeName f => TransformUnresolvedIdentifierTypeName(f, containingScope),
            From.UnresolvedGenericTypeName f => TransformUnresolvedGenericTypeName(f, containingScope, childContainingScope),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.UnresolvedTypeName TransformUnresolvedTypeName(From.UnresolvedTypeName from, DeclarationLexicalScope containingScope, DeclarationLexicalScope childContainingScope)
        => from switch
        {
            From.UnresolvedStandardTypeName f => TransformUnresolvedStandardTypeName(f, containingScope, childContainingScope),
            From.UnresolvedSimpleTypeName f => TransformUnresolvedSimpleTypeName(f, containingScope),
            From.UnresolvedQualifiedTypeName f => TransformUnresolvedQualifiedTypeName(f, containingScope, childContainingScope),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.UnresolvedType TransformUnresolvedType(From.UnresolvedType from, DeclarationLexicalScope containingScope, DeclarationLexicalScope childContainingScope)
        => from switch
        {
            From.UnresolvedTypeName f => TransformUnresolvedTypeName(f, containingScope, childContainingScope),
            From.UnresolvedOptionalType f => TransformUnresolvedOptionalType(f, childContainingScope),
            From.UnresolvedCapabilityType f => TransformUnresolvedCapabilityType(f, childContainingScope),
            From.UnresolvedFunctionType f => TransformUnresolvedFunctionType(f, childContainingScope),
            From.UnresolvedViewpointType f => TransformUnresolvedViewpointType(f, childContainingScope),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.UnresolvedType> TransformUnresolvedTypes(IEnumerable<From.UnresolvedType> from, DeclarationLexicalScope containingScope, DeclarationLexicalScope childContainingScope)
        => from.Select(f => TransformUnresolvedType(f, containingScope, childContainingScope)).ToFixedList();

    private To.UnresolvedOptionalType TransformUnresolvedOptionalType(From.UnresolvedOptionalType from, DeclarationLexicalScope childContainingScope)
        => CreateUnresolvedOptionalType(from, childContainingScope);

    private To.UnresolvedCapabilityType TransformUnresolvedCapabilityType(From.UnresolvedCapabilityType from, DeclarationLexicalScope childContainingScope)
        => CreateUnresolvedCapabilityType(from, childContainingScope);

    private To.UnresolvedFunctionType TransformUnresolvedFunctionType(From.UnresolvedFunctionType from, DeclarationLexicalScope childContainingScope)
        => CreateUnresolvedFunctionType(from, childContainingScope);

    private To.UnresolvedParameterType TransformUnresolvedParameterType(From.UnresolvedParameterType from, DeclarationLexicalScope childContainingScope)
        => CreateUnresolvedParameterType(from, childContainingScope);

    private IFixedList<To.UnresolvedParameterType> TransformUnresolvedParameterTypes(IEnumerable<From.UnresolvedParameterType> from, DeclarationLexicalScope childContainingScope)
        => from.Select(f => TransformUnresolvedParameterType(f, childContainingScope)).ToFixedList();

    private To.UnresolvedViewpointType TransformUnresolvedViewpointType(From.UnresolvedViewpointType from, DeclarationLexicalScope childContainingScope)
        => from switch
        {
            From.UnresolvedCapabilityViewpointType f => TransformUnresolvedCapabilityViewpointType(f, childContainingScope),
            From.UnresolvedSelfViewpointType f => TransformUnresolvedSelfViewpointType(f, childContainingScope),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.UnresolvedCapabilityViewpointType TransformUnresolvedCapabilityViewpointType(From.UnresolvedCapabilityViewpointType from, DeclarationLexicalScope childContainingScope)
        => CreateUnresolvedCapabilityViewpointType(from, childContainingScope);

    private To.UnresolvedSelfViewpointType TransformUnresolvedSelfViewpointType(From.UnresolvedSelfViewpointType from, DeclarationLexicalScope childContainingScope)
        => CreateUnresolvedSelfViewpointType(from, childContainingScope);

    private To.UnresolvedSimpleTypeName TransformUnresolvedSimpleTypeName(From.UnresolvedSimpleTypeName from, DeclarationLexicalScope containingScope)
        => from switch
        {
            From.UnresolvedIdentifierTypeName f => TransformUnresolvedIdentifierTypeName(f, containingScope),
            From.UnresolvedSpecialTypeName f => TransformUnresolvedSpecialTypeName(f, containingScope),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.NamespaceMemberDeclaration> TransformNamespaceMemberDeclarations(IEnumerable<From.NamespaceMemberDeclaration> from, DeclarationLexicalScope containingScope)
        => from.Select(f => TransformNamespaceMemberDeclaration(f, containingScope)).ToFixedList();

    private IFixedList<To.ClassMemberDeclaration> TransformClassMemberDeclarations(IEnumerable<From.ClassMemberDeclaration> from, DeclarationLexicalScope containingScope)
        => from.Select(f => TransformClassMemberDeclaration(f, containingScope)).ToFixedList();

    private IFixedList<To.StructMemberDeclaration> TransformStructMemberDeclarations(IEnumerable<From.StructMemberDeclaration> from, DeclarationLexicalScope containingScope)
        => from.Select(f => TransformStructMemberDeclaration(f, containingScope)).ToFixedList();

    private IFixedList<To.TraitMemberDeclaration> TransformTraitMemberDeclarations(IEnumerable<From.TraitMemberDeclaration> from, DeclarationLexicalScope containingScope)
        => from.Select(f => TransformTraitMemberDeclaration(f, containingScope)).ToFixedList();

    #region Create() methods
    private To.Package CreatePackage(From.Package from, PackageReferenceScope lexicalScope, IEnumerable<To.CompilationUnit> compilationUnits, IEnumerable<To.CompilationUnit> testingCompilationUnits)
        => To.Package.Create(lexicalScope, from.Syntax, from.Symbol, from.References, compilationUnits, testingCompilationUnits);

    private To.CompilationUnit CreateCompilationUnit(From.CompilationUnit from, DeclarationScope lexicalScope, IEnumerable<To.NamespaceMemberDeclaration> declarations)
        => To.CompilationUnit.Create(lexicalScope, from.Syntax, from.File, from.ImplicitNamespaceName, from.UsingDirectives, declarations);

    private To.NamespaceDeclaration CreateNamespaceDeclaration(From.NamespaceDeclaration from, DeclarationScope newScope, IEnumerable<To.NamespaceMemberDeclaration> declarations, DeclarationLexicalScope containingScope)
        => To.NamespaceDeclaration.Create(newScope, from.ContainingNamespace, from.Symbol, from.Syntax, from.IsGlobalQualified, from.DeclaredNames, from.UsingDirectives, declarations, containingScope);

    private To.UnresolvedSupertypeName CreateUnresolvedSupertypeName(From.UnresolvedSupertypeName from, DeclarationLexicalScope containingScope, IEnumerable<To.UnresolvedType> typeArguments)
        => To.UnresolvedSupertypeName.Create(containingScope, from.Syntax, from.Name, typeArguments);

    private To.FunctionDeclaration CreateFunctionDeclaration(From.FunctionDeclaration from, DeclarationLexicalScope containingScope)
        => To.FunctionDeclaration.Create(from.ContainingNamespace, from.Syntax, containingScope);

    private To.ClassDeclaration CreateClassDeclaration(From.ClassDeclaration from, To.UnresolvedSupertypeName? baseTypeName, IEnumerable<To.ClassMemberDeclaration> members, DeclarationScope newScope, IEnumerable<To.UnresolvedSupertypeName> supertypeNames, DeclarationLexicalScope containingScope)
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, baseTypeName, members, newScope, from.GenericParameters, supertypeNames, containingScope, from.ContainingNamespace);

    private To.StructDeclaration CreateStructDeclaration(From.StructDeclaration from, IEnumerable<To.StructMemberDeclaration> members, DeclarationScope newScope, IEnumerable<To.UnresolvedSupertypeName> supertypeNames, DeclarationLexicalScope containingScope)
        => To.StructDeclaration.Create(from.Syntax, members, newScope, from.GenericParameters, supertypeNames, containingScope, from.ContainingNamespace);

    private To.TraitDeclaration CreateTraitDeclaration(From.TraitDeclaration from, IEnumerable<To.TraitMemberDeclaration> members, DeclarationScope newScope, IEnumerable<To.UnresolvedSupertypeName> supertypeNames, DeclarationLexicalScope containingScope)
        => To.TraitDeclaration.Create(from.Syntax, members, newScope, from.GenericParameters, supertypeNames, containingScope, from.ContainingNamespace);

    private To.UnresolvedIdentifierTypeName CreateUnresolvedIdentifierTypeName(From.UnresolvedIdentifierTypeName from, DeclarationLexicalScope containingScope)
        => To.UnresolvedIdentifierTypeName.Create(from.Syntax, from.Name, containingScope);

    private To.UnresolvedSpecialTypeName CreateUnresolvedSpecialTypeName(From.UnresolvedSpecialTypeName from, DeclarationLexicalScope containingScope)
        => To.UnresolvedSpecialTypeName.Create(from.Syntax, from.Name, containingScope);

    private To.UnresolvedGenericTypeName CreateUnresolvedGenericTypeName(From.UnresolvedGenericTypeName from, IEnumerable<To.UnresolvedType> typeArguments, DeclarationLexicalScope containingScope)
        => To.UnresolvedGenericTypeName.Create(from.Syntax, from.Name, typeArguments, containingScope);

    private To.UnresolvedQualifiedTypeName CreateUnresolvedQualifiedTypeName(From.UnresolvedQualifiedTypeName from, To.UnresolvedTypeName context, To.UnresolvedStandardTypeName qualifiedName, DeclarationLexicalScope containingScope)
        => To.UnresolvedQualifiedTypeName.Create(from.Syntax, context, qualifiedName, containingScope, from.Name);

    private To.UnresolvedOptionalType CreateUnresolvedOptionalType(From.UnresolvedOptionalType from, To.UnresolvedType referent)
        => To.UnresolvedOptionalType.Create(from.Syntax, referent);

    private To.UnresolvedCapabilityType CreateUnresolvedCapabilityType(From.UnresolvedCapabilityType from, To.UnresolvedType referent)
        => To.UnresolvedCapabilityType.Create(from.Syntax, from.Capability, referent);

    private To.UnresolvedFunctionType CreateUnresolvedFunctionType(From.UnresolvedFunctionType from, IEnumerable<To.UnresolvedParameterType> parameters, To.UnresolvedType @return)
        => To.UnresolvedFunctionType.Create(from.Syntax, parameters, @return);

    private To.UnresolvedParameterType CreateUnresolvedParameterType(From.UnresolvedParameterType from, To.UnresolvedType referent)
        => To.UnresolvedParameterType.Create(from.Syntax, from.IsLent, referent);

    private To.UnresolvedCapabilityViewpointType CreateUnresolvedCapabilityViewpointType(From.UnresolvedCapabilityViewpointType from, To.UnresolvedType referent)
        => To.UnresolvedCapabilityViewpointType.Create(from.Syntax, from.Capability, referent);

    private To.UnresolvedSelfViewpointType CreateUnresolvedSelfViewpointType(From.UnresolvedSelfViewpointType from, To.UnresolvedType referent)
        => To.UnresolvedSelfViewpointType.Create(from.Syntax, referent);

    #endregion

    #region CreateX() methods
    private To.Package CreatePackage(From.Package from, PackageReferenceScope lexicalScope, PackageReferenceScope childContainingScope)
        => To.Package.Create(lexicalScope, from.Syntax, from.Symbol, from.References, TransformCompilationUnits(from.CompilationUnits, childContainingScope), TransformCompilationUnits(from.TestingCompilationUnits, childContainingScope));

    private To.CompilationUnit CreateCompilationUnit(From.CompilationUnit from, DeclarationScope lexicalScope, DeclarationLexicalScope childContainingScope)
        => To.CompilationUnit.Create(lexicalScope, from.Syntax, from.File, from.ImplicitNamespaceName, from.UsingDirectives, TransformNamespaceMemberDeclarations(from.Declarations, childContainingScope));

    private To.TypeDeclaration CreateTypeDeclaration(From.TypeDeclaration from, DeclarationScope newScope, DeclarationLexicalScope containingScope, DeclarationLexicalScope childContainingScope)
        => from switch
        {
            From.ClassDeclaration f => CreateClassDeclaration(f, newScope, containingScope, childContainingScope),
            From.StructDeclaration f => CreateStructDeclaration(f, newScope, containingScope, childContainingScope),
            From.TraitDeclaration f => CreateTraitDeclaration(f, newScope, containingScope, childContainingScope),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.NamespaceDeclaration CreateNamespaceDeclaration(From.NamespaceDeclaration from, DeclarationScope newScope, DeclarationLexicalScope containingScope, DeclarationLexicalScope childContainingScope)
        => To.NamespaceDeclaration.Create(newScope, from.ContainingNamespace, from.Symbol, from.Syntax, from.IsGlobalQualified, from.DeclaredNames, from.UsingDirectives, TransformNamespaceMemberDeclarations(from.Declarations, childContainingScope), containingScope);

    private To.UnresolvedSupertypeName CreateUnresolvedSupertypeName(From.UnresolvedSupertypeName from, DeclarationLexicalScope containingScope, DeclarationLexicalScope childContainingScope)
        => To.UnresolvedSupertypeName.Create(containingScope, from.Syntax, from.Name, TransformUnresolvedTypes(from.TypeArguments, childContainingScope, childContainingScope));

    private To.UnresolvedGenericTypeName CreateUnresolvedGenericTypeName(From.UnresolvedGenericTypeName from, DeclarationLexicalScope containingScope, DeclarationLexicalScope childContainingScope)
        => To.UnresolvedGenericTypeName.Create(from.Syntax, from.Name, TransformUnresolvedTypes(from.TypeArguments, childContainingScope, childContainingScope), containingScope);

    private To.UnresolvedQualifiedTypeName CreateUnresolvedQualifiedTypeName(From.UnresolvedQualifiedTypeName from, DeclarationLexicalScope containingScope, DeclarationLexicalScope childContainingScope)
        => To.UnresolvedQualifiedTypeName.Create(from.Syntax, TransformUnresolvedTypeName(from.Context, childContainingScope, childContainingScope), TransformUnresolvedStandardTypeName(from.QualifiedName, childContainingScope, childContainingScope), containingScope, from.Name);

    private To.ClassDeclaration CreateClassDeclaration(From.ClassDeclaration from, DeclarationScope newScope, DeclarationLexicalScope containingScope, DeclarationLexicalScope childContainingScope)
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, TransformUnresolvedSupertypeName(from.BaseTypeName, childContainingScope, childContainingScope), TransformClassMemberDeclarations(from.Members, childContainingScope), newScope, from.GenericParameters, TransformUnresolvedSupertypeNames(from.SupertypeNames, childContainingScope, childContainingScope), containingScope, from.ContainingNamespace);

    private To.StructDeclaration CreateStructDeclaration(From.StructDeclaration from, DeclarationScope newScope, DeclarationLexicalScope containingScope, DeclarationLexicalScope childContainingScope)
        => To.StructDeclaration.Create(from.Syntax, TransformStructMemberDeclarations(from.Members, childContainingScope), newScope, from.GenericParameters, TransformUnresolvedSupertypeNames(from.SupertypeNames, childContainingScope, childContainingScope), containingScope, from.ContainingNamespace);

    private To.TraitDeclaration CreateTraitDeclaration(From.TraitDeclaration from, DeclarationScope newScope, DeclarationLexicalScope containingScope, DeclarationLexicalScope childContainingScope)
        => To.TraitDeclaration.Create(from.Syntax, TransformTraitMemberDeclarations(from.Members, childContainingScope), newScope, from.GenericParameters, TransformUnresolvedSupertypeNames(from.SupertypeNames, childContainingScope, childContainingScope), containingScope, from.ContainingNamespace);

    private To.UnresolvedOptionalType CreateUnresolvedOptionalType(From.UnresolvedOptionalType from, DeclarationLexicalScope childContainingScope)
        => To.UnresolvedOptionalType.Create(from.Syntax, TransformUnresolvedType(from.Referent, childContainingScope, childContainingScope));

    private To.UnresolvedCapabilityType CreateUnresolvedCapabilityType(From.UnresolvedCapabilityType from, DeclarationLexicalScope childContainingScope)
        => To.UnresolvedCapabilityType.Create(from.Syntax, from.Capability, TransformUnresolvedType(from.Referent, childContainingScope, childContainingScope));

    private To.UnresolvedFunctionType CreateUnresolvedFunctionType(From.UnresolvedFunctionType from, DeclarationLexicalScope childContainingScope)
        => To.UnresolvedFunctionType.Create(from.Syntax, TransformUnresolvedParameterTypes(from.Parameters, childContainingScope), TransformUnresolvedType(from.Return, childContainingScope, childContainingScope));

    private To.UnresolvedParameterType CreateUnresolvedParameterType(From.UnresolvedParameterType from, DeclarationLexicalScope childContainingScope)
        => To.UnresolvedParameterType.Create(from.Syntax, from.IsLent, TransformUnresolvedType(from.Referent, childContainingScope, childContainingScope));

    private To.UnresolvedViewpointType CreateUnresolvedViewpointType(From.UnresolvedViewpointType from, DeclarationLexicalScope childContainingScope)
        => from switch
        {
            From.UnresolvedCapabilityViewpointType f => CreateUnresolvedCapabilityViewpointType(f, childContainingScope),
            From.UnresolvedSelfViewpointType f => CreateUnresolvedSelfViewpointType(f, childContainingScope),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.UnresolvedCapabilityViewpointType CreateUnresolvedCapabilityViewpointType(From.UnresolvedCapabilityViewpointType from, DeclarationLexicalScope childContainingScope)
        => To.UnresolvedCapabilityViewpointType.Create(from.Syntax, from.Capability, TransformUnresolvedType(from.Referent, childContainingScope, childContainingScope));

    private To.UnresolvedSelfViewpointType CreateUnresolvedSelfViewpointType(From.UnresolvedSelfViewpointType from, DeclarationLexicalScope childContainingScope)
        => To.UnresolvedSelfViewpointType.Create(from.Syntax, TransformUnresolvedType(from.Referent, childContainingScope, childContainingScope));

    #endregion
}
