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
using From = Azoth.Tools.Bootstrap.Compiler.IST.WithDeclarationLexicalScopes;
using To = Azoth.Tools.Bootstrap.Compiler.IST.WithoutCompilationUnits;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class CompilationUnitRemover : ITransformPass<From.Package, Void, To.Package, Void>
{
    public static To.Package Run(From.Package from)
    {
        var pass = new CompilationUnitRemover();
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

    private partial IFixedList<To.NamespaceMemberDeclaration> TransformCompilationUnit(From.CompilationUnit from);

    private partial IFixedSet<To.NamespaceMemberDeclaration> TransformCompilationUnits(IEnumerable<From.CompilationUnit> from);

    private partial IFixedSet<To.NamespaceMemberDeclaration> TransformNamespaceDeclaration(From.NamespaceDeclaration from, CodeFile file);

    private partial IFixedSet<To.NamespaceMemberDeclaration> TransformNamespaceMemberDeclaration(From.NamespaceMemberDeclaration from, CodeFile file);

    private To.TypeDeclaration TransformTypeDeclaration(From.TypeDeclaration from, CodeFile file, CodeFile childFile)
        => from switch
        {
            From.ClassDeclaration f => TransformClassDeclaration(f, file, childFile),
            From.StructDeclaration f => TransformStructDeclaration(f, file, childFile),
            From.TraitDeclaration f => TransformTraitDeclaration(f, file, childFile),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.FunctionDeclaration TransformFunctionDeclaration(From.FunctionDeclaration from, CodeFile file)
        => CreateFunctionDeclaration(from, file);

    private IFixedList<To.NamespaceMemberDeclaration> TransformNamespaceMemberDeclarations(IEnumerable<From.NamespaceMemberDeclaration> from, CodeFile file)
        => from.SelectMany(f => TransformNamespaceMemberDeclaration(f, file)).ToFixedList();

    private To.ClassMemberDeclaration TransformClassMemberDeclaration(From.ClassMemberDeclaration from, CodeFile file, CodeFile childFile)
        => from switch
        {
            From.TypeDeclaration f => TransformTypeDeclaration(f, file, childFile),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.ClassMemberDeclaration> TransformClassMemberDeclarations(IEnumerable<From.ClassMemberDeclaration> from, CodeFile file, CodeFile childFile)
        => from.Select(f => TransformClassMemberDeclaration(f, file, childFile)).ToFixedList();

    private To.ClassDeclaration TransformClassDeclaration(From.ClassDeclaration from, CodeFile file, CodeFile childFile)
        => CreateClassDeclaration(from, file, childFile);

    private To.StructMemberDeclaration TransformStructMemberDeclaration(From.StructMemberDeclaration from, CodeFile file, CodeFile childFile)
        => from switch
        {
            From.TypeDeclaration f => TransformTypeDeclaration(f, file, childFile),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.StructMemberDeclaration> TransformStructMemberDeclarations(IEnumerable<From.StructMemberDeclaration> from, CodeFile file, CodeFile childFile)
        => from.Select(f => TransformStructMemberDeclaration(f, file, childFile)).ToFixedList();

    private To.StructDeclaration TransformStructDeclaration(From.StructDeclaration from, CodeFile file, CodeFile childFile)
        => CreateStructDeclaration(from, file, childFile);

    private To.TraitMemberDeclaration TransformTraitMemberDeclaration(From.TraitMemberDeclaration from, CodeFile file, CodeFile childFile)
        => from switch
        {
            From.TypeDeclaration f => TransformTypeDeclaration(f, file, childFile),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private IFixedList<To.TraitMemberDeclaration> TransformTraitMemberDeclarations(IEnumerable<From.TraitMemberDeclaration> from, CodeFile file, CodeFile childFile)
        => from.Select(f => TransformTraitMemberDeclaration(f, file, childFile)).ToFixedList();

    private To.TraitDeclaration TransformTraitDeclaration(From.TraitDeclaration from, CodeFile file, CodeFile childFile)
        => CreateTraitDeclaration(from, file, childFile);

    #region Create() methods
    private To.Package CreatePackage(From.Package from, IEnumerable<To.NamespaceMemberDeclaration> declarations, IEnumerable<To.NamespaceMemberDeclaration> testingDeclarations)
        => To.Package.Create(declarations, testingDeclarations, from.LexicalScope, from.Syntax, from.Symbol, from.References);

    private To.UnresolvedSupertypeName CreateUnresolvedSupertypeName(From.UnresolvedSupertypeName from)
        => To.UnresolvedSupertypeName.Create(from.ContainingScope, from.Syntax, from.Name, from.TypeArguments);

    private To.FunctionDeclaration CreateFunctionDeclaration(From.FunctionDeclaration from, CodeFile file)
        => To.FunctionDeclaration.Create(from.ContainingNamespace, from.Syntax, file, from.ContainingScope);

    private To.PackageReference CreatePackageReference(From.PackageReference from)
        => To.PackageReference.Create(from.Syntax, from.AliasOrName, from.Package, from.IsTrusted);

    private To.ClassDeclaration CreateClassDeclaration(From.ClassDeclaration from, IEnumerable<To.ClassMemberDeclaration> members, CodeFile file)
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, from.BaseTypeName, members, from.NewScope, from.GenericParameters, from.SupertypeNames, file, from.ContainingScope, from.ContainingNamespace);

    private To.StructDeclaration CreateStructDeclaration(From.StructDeclaration from, IEnumerable<To.StructMemberDeclaration> members, CodeFile file)
        => To.StructDeclaration.Create(from.Syntax, members, from.NewScope, from.GenericParameters, from.SupertypeNames, file, from.ContainingScope, from.ContainingNamespace);

    private To.TraitDeclaration CreateTraitDeclaration(From.TraitDeclaration from, IEnumerable<To.TraitMemberDeclaration> members, CodeFile file)
        => To.TraitDeclaration.Create(from.Syntax, members, from.NewScope, from.GenericParameters, from.SupertypeNames, file, from.ContainingScope, from.ContainingNamespace);

    private To.GenericParameter CreateGenericParameter(From.GenericParameter from)
        => To.GenericParameter.Create(from.Syntax, from.Constraint, from.Name, from.Independence, from.Variance);

    private To.CapabilitySet CreateCapabilitySet(From.CapabilitySet from)
        => To.CapabilitySet.Create(from.Syntax, from.Constraint);

    private To.Capability CreateCapability(From.Capability from)
        => To.Capability.Create(from.Syntax, from.Capability, from.Constraint);

    private To.UnresolvedIdentifierTypeName CreateUnresolvedIdentifierTypeName(From.UnresolvedIdentifierTypeName from)
        => To.UnresolvedIdentifierTypeName.Create(from.Syntax, from.Name, from.ContainingScope);

    private To.UnresolvedSpecialTypeName CreateUnresolvedSpecialTypeName(From.UnresolvedSpecialTypeName from)
        => To.UnresolvedSpecialTypeName.Create(from.Syntax, from.Name, from.ContainingScope);

    private To.UnresolvedGenericTypeName CreateUnresolvedGenericTypeName(From.UnresolvedGenericTypeName from)
        => To.UnresolvedGenericTypeName.Create(from.Syntax, from.Name, from.TypeArguments, from.ContainingScope);

    private To.UnresolvedQualifiedTypeName CreateUnresolvedQualifiedTypeName(From.UnresolvedQualifiedTypeName from)
        => To.UnresolvedQualifiedTypeName.Create(from.Syntax, from.Context, from.QualifiedName, from.ContainingScope, from.Name);

    private To.UnresolvedOptionalType CreateUnresolvedOptionalType(From.UnresolvedOptionalType from)
        => To.UnresolvedOptionalType.Create(from.Syntax, from.Referent);

    private To.UnresolvedCapabilityType CreateUnresolvedCapabilityType(From.UnresolvedCapabilityType from)
        => To.UnresolvedCapabilityType.Create(from.Syntax, from.Capability, from.Referent);

    private To.UnresolvedFunctionType CreateUnresolvedFunctionType(From.UnresolvedFunctionType from)
        => To.UnresolvedFunctionType.Create(from.Syntax, from.Parameters, from.Return);

    private To.UnresolvedParameterType CreateUnresolvedParameterType(From.UnresolvedParameterType from)
        => To.UnresolvedParameterType.Create(from.Syntax, from.IsLent, from.Referent);

    private To.UnresolvedCapabilityViewpointType CreateUnresolvedCapabilityViewpointType(From.UnresolvedCapabilityViewpointType from)
        => To.UnresolvedCapabilityViewpointType.Create(from.Syntax, from.Capability, from.Referent);

    private To.UnresolvedSelfViewpointType CreateUnresolvedSelfViewpointType(From.UnresolvedSelfViewpointType from)
        => To.UnresolvedSelfViewpointType.Create(from.Syntax, from.Referent);

    #endregion

    #region CreateX() methods
    private To.TypeDeclaration CreateTypeDeclaration(From.TypeDeclaration from, CodeFile file, CodeFile childFile)
        => from switch
        {
            From.ClassDeclaration f => CreateClassDeclaration(f, file, childFile),
            From.StructDeclaration f => CreateStructDeclaration(f, file, childFile),
            From.TraitDeclaration f => CreateTraitDeclaration(f, file, childFile),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.ClassMemberDeclaration CreateClassMemberDeclaration(From.ClassMemberDeclaration from, CodeFile file, CodeFile childFile)
        => from switch
        {
            From.TypeDeclaration f => CreateTypeDeclaration(f, file, childFile),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.ClassDeclaration CreateClassDeclaration(From.ClassDeclaration from, CodeFile file, CodeFile childFile)
        => To.ClassDeclaration.Create(from.Syntax, from.IsAbstract, from.BaseTypeName, TransformClassMemberDeclarations(from.Members, childFile, childFile), from.NewScope, from.GenericParameters, from.SupertypeNames, file, from.ContainingScope, from.ContainingNamespace);

    private To.StructMemberDeclaration CreateStructMemberDeclaration(From.StructMemberDeclaration from, CodeFile file, CodeFile childFile)
        => from switch
        {
            From.TypeDeclaration f => CreateTypeDeclaration(f, file, childFile),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.StructDeclaration CreateStructDeclaration(From.StructDeclaration from, CodeFile file, CodeFile childFile)
        => To.StructDeclaration.Create(from.Syntax, TransformStructMemberDeclarations(from.Members, childFile, childFile), from.NewScope, from.GenericParameters, from.SupertypeNames, file, from.ContainingScope, from.ContainingNamespace);

    private To.TraitMemberDeclaration CreateTraitMemberDeclaration(From.TraitMemberDeclaration from, CodeFile file, CodeFile childFile)
        => from switch
        {
            From.TypeDeclaration f => CreateTypeDeclaration(f, file, childFile),
            _ => throw ExhaustiveMatch.Failed(from),
        };

    private To.TraitDeclaration CreateTraitDeclaration(From.TraitDeclaration from, CodeFile file, CodeFile childFile)
        => To.TraitDeclaration.Create(from.Syntax, TransformTraitMemberDeclarations(from.Members, childFile, childFile), from.NewScope, from.GenericParameters, from.SupertypeNames, file, from.ContainingScope, from.ContainingNamespace);

    #endregion
}
