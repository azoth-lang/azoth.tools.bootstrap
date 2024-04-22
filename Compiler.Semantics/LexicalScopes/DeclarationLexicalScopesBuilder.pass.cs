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
        var to = pass.Transform(from);
        pass.EndRun(to);
        return to;
    }

    static (To.Package, Void) ITransformPass<From.Package, Void, To.Package, Void>.Run(From.Package from, Void context)
        => (Run(from), default);


    partial void StartRun();

    partial void EndRun(To.Package to);

    private partial To.Package Transform(From.Package from);

    private partial To.CompilationUnit Transform(From.CompilationUnit from, PackageReferenceScope containingScope);

    private partial To.TypeDeclaration Transform(From.TypeDeclaration from, DeclarationLexicalScope containingScope);

    private partial To.NamespaceDeclaration Transform(From.NamespaceDeclaration from, DeclarationLexicalScope containingScope);

    [return: NotNullIfNotNull(nameof(from))]
    private To.UnresolvedSupertypeName? Transform(From.UnresolvedSupertypeName? from, DeclarationLexicalScope containingScope)
        => from is not null ? Create(from, containingScope) : null;

    private To.FunctionDeclaration Transform(From.FunctionDeclaration from, DeclarationLexicalScope containingScope)
        => Create(from, containingScope);

    private To.UnresolvedIdentifierTypeName Transform(From.UnresolvedIdentifierTypeName from, DeclarationLexicalScope containingScope)
        => Create(from, containingScope);

    private To.UnresolvedSpecialTypeName Transform(From.UnresolvedSpecialTypeName from, DeclarationLexicalScope containingScope)
        => Create(from, containingScope);

    private To.UnresolvedGenericTypeName Transform(From.UnresolvedGenericTypeName from, DeclarationLexicalScope containingScope)
        => Create(from, containingScope);

    private To.UnresolvedQualifiedTypeName Transform(From.UnresolvedQualifiedTypeName from, DeclarationLexicalScope containingScope)
        => Create(from, containingScope);

    private IFixedSet<To.CompilationUnit> Transform(IEnumerable<From.CompilationUnit> from, PackageReferenceScope containingScope)
        => from.Select(f => Transform(f, containingScope)).ToFixedSet();

    private IFixedList<To.NamespaceMemberDeclaration> Transform(IEnumerable<From.NamespaceMemberDeclaration> from, DeclarationLexicalScope containingScope)
        => from.Select(f => Transform(f, containingScope)).ToFixedList();

    private To.NamespaceMemberDeclaration Transform(From.NamespaceMemberDeclaration from, DeclarationLexicalScope containingScope)
        => from switch
        {
            From.NamespaceDeclaration f => Transform(f, containingScope),
            From.FunctionDeclaration f => Transform(f, containingScope),
            From.TypeDeclaration f => Transform(f, containingScope),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.ClassMemberDeclaration> Transform(IEnumerable<From.ClassMemberDeclaration> from, DeclarationLexicalScope containingScope)
        => from.Select(f => Transform(f, containingScope)).ToFixedList();

    private To.ClassMemberDeclaration Transform(From.ClassMemberDeclaration from, DeclarationLexicalScope containingScope)
        => from switch
        {
            From.TypeDeclaration f => Transform(f, containingScope),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.StructMemberDeclaration> Transform(IEnumerable<From.StructMemberDeclaration> from, DeclarationLexicalScope containingScope)
        => from.Select(f => Transform(f, containingScope)).ToFixedList();

    private To.StructMemberDeclaration Transform(From.StructMemberDeclaration from, DeclarationLexicalScope containingScope)
        => from switch
        {
            From.TypeDeclaration f => Transform(f, containingScope),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.TraitMemberDeclaration> Transform(IEnumerable<From.TraitMemberDeclaration> from, DeclarationLexicalScope containingScope)
        => from.Select(f => Transform(f, containingScope)).ToFixedList();

    private To.TraitMemberDeclaration Transform(From.TraitMemberDeclaration from, DeclarationLexicalScope containingScope)
        => from switch
        {
            From.TypeDeclaration f => Transform(f, containingScope),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.UnresolvedSupertypeName> Transform(IEnumerable<From.UnresolvedSupertypeName> from, DeclarationLexicalScope containingScope)
        => from.Select(f => Transform(f, containingScope)).ToFixedList();

    private IFixedList<To.UnresolvedType> Transform(IEnumerable<From.UnresolvedType> from, DeclarationLexicalScope containingScope)
        => from.Select(f => Transform(f, containingScope)).ToFixedList();

    private To.UnresolvedType Transform(From.UnresolvedType from, DeclarationLexicalScope containingScope)
        => from switch
        {
            From.UnresolvedTypeName f => Transform(f, containingScope),
            From.UnresolvedOptionalType f => Transform(f, containingScope),
            From.UnresolvedCapabilityType f => Transform(f, containingScope),
            From.UnresolvedFunctionType f => Transform(f, containingScope),
            From.UnresolvedViewpointType f => Transform(f, containingScope),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.UnresolvedOptionalType Transform(From.UnresolvedOptionalType from, DeclarationLexicalScope containingScope)
        => Create(from, containingScope);

    private To.UnresolvedCapabilityType Transform(From.UnresolvedCapabilityType from, DeclarationLexicalScope containingScope)
        => Create(from, containingScope);

    private To.UnresolvedFunctionType Transform(From.UnresolvedFunctionType from, DeclarationLexicalScope containingScope)
        => Create(from, containingScope);

    private To.UnresolvedParameterType Transform(From.UnresolvedParameterType from, DeclarationLexicalScope containingScope)
        => Create(from, containingScope);

    private IFixedList<To.UnresolvedParameterType> Transform(IEnumerable<From.UnresolvedParameterType> from, DeclarationLexicalScope containingScope)
        => from.Select(f => Transform(f, containingScope)).ToFixedList();

    private To.UnresolvedViewpointType Transform(From.UnresolvedViewpointType from, DeclarationLexicalScope containingScope)
        => from switch
        {
            From.UnresolvedCapabilityViewpointType f => Transform(f, containingScope),
            From.UnresolvedSelfViewpointType f => Transform(f, containingScope),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.UnresolvedCapabilityViewpointType Transform(From.UnresolvedCapabilityViewpointType from, DeclarationLexicalScope containingScope)
        => Create(from, containingScope);

    private To.UnresolvedSelfViewpointType Transform(From.UnresolvedSelfViewpointType from, DeclarationLexicalScope containingScope)
        => Create(from, containingScope);

    private To.UnresolvedTypeName Transform(From.UnresolvedTypeName from, DeclarationLexicalScope containingScope)
        => from switch
        {
            From.UnresolvedStandardTypeName f => Transform(f, containingScope),
            From.UnresolvedSimpleTypeName f => Transform(f, containingScope),
            From.UnresolvedQualifiedTypeName f => Transform(f, containingScope),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.UnresolvedStandardTypeName Transform(From.UnresolvedStandardTypeName from, DeclarationLexicalScope containingScope)
        => from switch
        {
            From.UnresolvedIdentifierTypeName f => Transform(f, containingScope),
            From.UnresolvedGenericTypeName f => Transform(f, containingScope),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.Package Create(From.Package from, PackageReferenceScope lexicalScope, IEnumerable<To.CompilationUnit> compilationUnits, IEnumerable<To.CompilationUnit> testingCompilationUnits)
        => To.Package.Create(lexicalScope, from.Syntax, from.Symbol, from.References, compilationUnits, testingCompilationUnits);

    private To.CompilationUnit Create(From.CompilationUnit from, DeclarationScope lexicalScope, IEnumerable<To.NamespaceMemberDeclaration> declarations)
        => To.CompilationUnit.Create(lexicalScope, from.Syntax, from.File, from.ImplicitNamespaceName, from.UsingDirectives, declarations);

    private To.NamespaceDeclaration Create(From.NamespaceDeclaration from, DeclarationScope newScope, IEnumerable<To.NamespaceMemberDeclaration> declarations, DeclarationLexicalScope containingScope)
        => To.NamespaceDeclaration.Create(newScope, from.ContainingNamespace, from.Symbol, from.Syntax, from.IsGlobalQualified, from.DeclaredNames, from.UsingDirectives, declarations, containingScope);

    private To.UnresolvedSupertypeName Create(From.UnresolvedSupertypeName from, DeclarationLexicalScope containingScope, IEnumerable<To.UnresolvedType> typeArguments)
        => To.UnresolvedSupertypeName.Create(containingScope, from.Syntax, from.Name, typeArguments);

    private To.FunctionDeclaration Create(From.FunctionDeclaration from, DeclarationLexicalScope containingScope)
        => To.FunctionDeclaration.Create(from.ContainingNamespace, from.Syntax, containingScope);

    private To.ClassDeclaration Create(From.ClassDeclaration from, To.UnresolvedSupertypeName? baseTypeName, IEnumerable<To.ClassMemberDeclaration> members, DeclarationScope newScope, IEnumerable<To.UnresolvedSupertypeName> supertypeNames, DeclarationLexicalScope containingScope)
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, baseTypeName, members, newScope, from.GenericParameters, supertypeNames, containingScope, from.ContainingNamespace);

    private To.StructDeclaration Create(From.StructDeclaration from, IEnumerable<To.StructMemberDeclaration> members, DeclarationScope newScope, IEnumerable<To.UnresolvedSupertypeName> supertypeNames, DeclarationLexicalScope containingScope)
        => To.StructDeclaration.Create(from.Syntax, members, newScope, from.GenericParameters, supertypeNames, containingScope, from.ContainingNamespace);

    private To.TraitDeclaration Create(From.TraitDeclaration from, IEnumerable<To.TraitMemberDeclaration> members, DeclarationScope newScope, IEnumerable<To.UnresolvedSupertypeName> supertypeNames, DeclarationLexicalScope containingScope)
        => To.TraitDeclaration.Create(from.Syntax, members, newScope, from.GenericParameters, supertypeNames, containingScope, from.ContainingNamespace);

    private To.UnresolvedIdentifierTypeName Create(From.UnresolvedIdentifierTypeName from, DeclarationLexicalScope containingScope)
        => To.UnresolvedIdentifierTypeName.Create(from.Syntax, from.Name, containingScope);

    private To.UnresolvedSpecialTypeName Create(From.UnresolvedSpecialTypeName from, DeclarationLexicalScope containingScope)
        => To.UnresolvedSpecialTypeName.Create(from.Syntax, from.Name, containingScope);

    private To.UnresolvedGenericTypeName Create(From.UnresolvedGenericTypeName from, IEnumerable<To.UnresolvedType> typeArguments, DeclarationLexicalScope containingScope)
        => To.UnresolvedGenericTypeName.Create(from.Syntax, from.Name, typeArguments, containingScope);

    private To.UnresolvedQualifiedTypeName Create(From.UnresolvedQualifiedTypeName from, To.UnresolvedTypeName context, To.UnresolvedStandardTypeName qualifiedName, DeclarationLexicalScope containingScope)
        => To.UnresolvedQualifiedTypeName.Create(from.Syntax, context, qualifiedName, containingScope, from.Name);

    private To.UnresolvedOptionalType Create(From.UnresolvedOptionalType from, To.UnresolvedType referent)
        => To.UnresolvedOptionalType.Create(from.Syntax, referent);

    private To.UnresolvedCapabilityType Create(From.UnresolvedCapabilityType from, To.UnresolvedType referent)
        => To.UnresolvedCapabilityType.Create(from.Syntax, from.Capability, referent);

    private To.UnresolvedFunctionType Create(From.UnresolvedFunctionType from, IEnumerable<To.UnresolvedParameterType> parameters, To.UnresolvedType @return)
        => To.UnresolvedFunctionType.Create(from.Syntax, parameters, @return);

    private To.UnresolvedParameterType Create(From.UnresolvedParameterType from, To.UnresolvedType referent)
        => To.UnresolvedParameterType.Create(from.Syntax, from.IsLent, referent);

    private To.UnresolvedCapabilityViewpointType Create(From.UnresolvedCapabilityViewpointType from, To.UnresolvedType referent)
        => To.UnresolvedCapabilityViewpointType.Create(from.Syntax, from.Capability, referent);

    private To.UnresolvedSelfViewpointType Create(From.UnresolvedSelfViewpointType from, To.UnresolvedType referent)
        => To.UnresolvedSelfViewpointType.Create(from.Syntax, referent);

}
