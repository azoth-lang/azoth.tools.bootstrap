using System.CodeDom.Compiler;
using System.Collections.Generic;
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

    static (To.Package, Void) ITransformPass<From.Package, Void, To.Package, Void>.Run(From.Package from, Void _)
        => (Run(from), default);


    partial void StartRun();

    partial void EndRun(To.Package to);

    private partial To.Package Transform(From.Package from);

    private partial To.CompilationUnit Transform(From.CompilationUnit from, PackageReferenceScope containingScope);

    private partial To.TypeDeclaration Transform(From.TypeDeclaration from, DeclarationLexicalScope containingScope);

    private IFixedSet<To.CompilationUnit> Transform(IEnumerable<From.CompilationUnit> from, PackageReferenceScope containingScope)
        => from.Select(f => Transform(f, containingScope)).ToFixedSet();

    private To.Package Create(From.Package from, PackageReferenceScope lexicalScope, IEnumerable<To.CompilationUnit> compilationUnits, IEnumerable<To.CompilationUnit> testingCompilationUnits)
        => To.Package.Create(lexicalScope, from.Syntax, from.Symbol, from.References, compilationUnits, testingCompilationUnits);

    private To.CompilationUnit Create(From.CompilationUnit from, DeclarationScope lexicalScope, IEnumerable<To.NamespaceMemberDeclaration> declarations)
        => To.CompilationUnit.Create(lexicalScope, from.Syntax, from.File, from.ImplicitNamespaceName, from.UsingDirectives, declarations);

    private To.NamespaceDeclaration Create(From.NamespaceDeclaration from, DeclarationScope lexicalScope, IEnumerable<To.NamespaceMemberDeclaration> declarations, DeclarationLexicalScope containingLexicalScope)
        => To.NamespaceDeclaration.Create(lexicalScope, from.ContainingSymbol, from.Symbol, from.Syntax, from.IsGlobalQualified, from.DeclaredNames, from.UsingDirectives, declarations, containingLexicalScope);

    private To.UnresolvedSupertypeName Create(From.UnresolvedSupertypeName from, DeclarationLexicalScope containingLexicalScope, IEnumerable<To.UnresolvedType> typeArguments)
        => To.UnresolvedSupertypeName.Create(containingLexicalScope, from.Syntax, from.Name, typeArguments);

    private To.FunctionDeclaration Create(From.FunctionDeclaration from, DeclarationLexicalScope containingLexicalScope)
        => To.FunctionDeclaration.Create(from.ContainingSymbol, from.Syntax, containingLexicalScope);

    private To.ClassDeclaration Create(From.ClassDeclaration from, To.UnresolvedSupertypeName? baseTypeName, IEnumerable<To.ClassMemberDeclaration> members, DeclarationScope lexicalScope, IEnumerable<To.UnresolvedSupertypeName> supertypeNames, DeclarationLexicalScope containingLexicalScope)
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, baseTypeName, members, lexicalScope, from.GenericParameters, supertypeNames, containingLexicalScope, from.ContainingSymbol);

    private To.StructDeclaration Create(From.StructDeclaration from, IEnumerable<To.StructMemberDeclaration> members, DeclarationScope lexicalScope, IEnumerable<To.UnresolvedSupertypeName> supertypeNames, DeclarationLexicalScope containingLexicalScope)
        => To.StructDeclaration.Create(from.Syntax, members, lexicalScope, from.GenericParameters, supertypeNames, containingLexicalScope, from.ContainingSymbol);

    private To.TraitDeclaration Create(From.TraitDeclaration from, IEnumerable<To.TraitMemberDeclaration> members, DeclarationScope lexicalScope, IEnumerable<To.UnresolvedSupertypeName> supertypeNames, DeclarationLexicalScope containingLexicalScope)
        => To.TraitDeclaration.Create(from.Syntax, members, lexicalScope, from.GenericParameters, supertypeNames, containingLexicalScope, from.ContainingSymbol);

    private To.UnresolvedIdentifierTypeName Create(From.UnresolvedIdentifierTypeName from, DeclarationLexicalScope containingLexicalScope)
        => To.UnresolvedIdentifierTypeName.Create(from.Syntax, from.Name, containingLexicalScope);

    private To.UnresolvedSpecialTypeName Create(From.UnresolvedSpecialTypeName from, DeclarationLexicalScope containingLexicalScope)
        => To.UnresolvedSpecialTypeName.Create(from.Syntax, from.Name, containingLexicalScope);

    private To.UnresolvedGenericTypeName Create(From.UnresolvedGenericTypeName from, IEnumerable<To.UnresolvedType> typeArguments, DeclarationLexicalScope containingLexicalScope)
        => To.UnresolvedGenericTypeName.Create(from.Syntax, from.Name, typeArguments, containingLexicalScope);

    private To.UnresolvedQualifiedTypeName Create(From.UnresolvedQualifiedTypeName from, To.UnresolvedTypeName context, To.UnresolvedStandardTypeName qualifiedName, DeclarationLexicalScope containingLexicalScope)
        => To.UnresolvedQualifiedTypeName.Create(from.Syntax, context, qualifiedName, containingLexicalScope, from.Name);

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
