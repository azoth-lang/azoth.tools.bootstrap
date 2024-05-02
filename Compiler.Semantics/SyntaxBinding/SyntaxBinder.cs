using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.SyntaxBinding;

internal static class SyntaxBinder
{
    #region Packages
    public static IPackage Package(IPackageSyntax syntax)
        => new PackageNode(syntax, PackageReferences(syntax.References), CompilationUnits(syntax.CompilationUnits), CompilationUnits(syntax.TestingCompilationUnits));

    public static IEnumerable<IPackageReference> PackageReferences(IEnumerable<IPackageReferenceSyntax> syntax)
        => syntax.Select(syn => new PackageReferenceNode(syn));
    #endregion

    #region Code Files
    public static IEnumerable<ICompilationUnit> CompilationUnits(IEnumerable<ICompilationUnitSyntax> syntax)
        => syntax.Select(syn => new CompilationUnitNode(syn, UsingDirectives(syn.UsingDirectives),
            NamespaceMemberDeclarations(syn.Declarations)));

    public static IEnumerable<IUsingDirective> UsingDirectives(IEnumerable<IUsingDirectiveSyntax> syntax)
        => syntax.Select(syn => new UsingDirectiveNode(syn));
    #endregion

    #region Namespaces

    private static INamespaceDeclaration NamespaceDeclaration(INamespaceDeclarationSyntax syntax)
        => new NamespaceDeclarationNode(syntax, UsingDirectives(syntax.UsingDirectives),
            NamespaceMemberDeclarations(syntax.Declarations));

    public static IEnumerable<INamespaceMemberDeclaration> NamespaceMemberDeclarations(IEnumerable<INonMemberDeclarationSyntax> syntax)
        => syntax.Select(NamespaceMemberDeclaration);

    public static INamespaceMemberDeclaration NamespaceMemberDeclaration(INonMemberDeclarationSyntax syntax)
        => syntax switch
        {
            INamespaceDeclarationSyntax syn => NamespaceDeclaration(syn),
            ITypeDeclarationSyntax syn => TypeDeclaration(syn),
            IFunctionDeclarationSyntax syn => throw new NotImplementedException(),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };
    #endregion

    #region Type Declarations
    private static ITypeDeclaration TypeDeclaration(ITypeDeclarationSyntax syntax)
        => syntax switch
        {
            IClassDeclarationSyntax syn => ClassDeclaration(syn),
            IStructDeclarationSyntax syn => StructDeclaration(syn),
            ITraitDeclarationSyntax syn => TraitDeclaration(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IClassDeclaration ClassDeclaration(IClassDeclarationSyntax syntax)
        => new ClassDeclarationNode(syntax, GenericParameters(syntax.GenericParameters),
            SupertypeName(syntax.BaseTypeName), SupertypeNames(syntax.SupertypeNames),
            ClassMemberDeclarations(syntax.Members));

    private static IStructDeclaration StructDeclaration(IStructDeclarationSyntax syntax)
        => new StructDeclarationNode(syntax, GenericParameters(syntax.GenericParameters),
            SupertypeNames(syntax.SupertypeNames), StructMemberDeclarations(syntax.Members));

    private static ITraitDeclaration TraitDeclaration(ITraitDeclarationSyntax syntax)
        => new TraitDeclarationNode(syntax, GenericParameters(syntax.GenericParameters),
            SupertypeNames(syntax.SupertypeNames), TraitMemberDeclarations(syntax.Members));

    private static IEnumerable<ISupertypeName> SupertypeNames(IFixedList<ISupertypeNameSyntax> syntax)
        => syntax.Select(syn => SupertypeName(syn));

    [return: NotNullIfNotNull(nameof(syntax))]
    private static ISupertypeName? SupertypeName(ISupertypeNameSyntax? syntax)
        => syntax is not null ? new SupertypeNameNode(syntax) : null;
    #endregion

    #region Type Declaration Parts
    private static IEnumerable<IGenericParameter> GenericParameters(IEnumerable<IGenericParameterSyntax> syntax)
        => syntax.Select(GenericParameter);

    private static IGenericParameter GenericParameter(IGenericParameterSyntax syntax)
        => new GenericParameterNode(syntax, CapabilityConstraint(syntax.Constraint));
    #endregion

    #region Type Member Declarations
    private static IEnumerable<IClassMemberDeclaration> ClassMemberDeclarations(IEnumerable<IClassMemberDeclarationSyntax> syntax)
        => syntax.Select(ClassMemberDeclaration);

    private static IClassMemberDeclaration ClassMemberDeclaration(IClassMemberDeclarationSyntax syntax)
        => syntax switch
        {
            ITypeDeclarationSyntax syn => TypeDeclaration(syn),
            IMethodDeclarationSyntax syn => throw new NotImplementedException(),
            IConstructorDeclarationSyntax syn => throw new NotImplementedException(),
            IFieldDeclarationSyntax syn => throw new NotImplementedException(),
            IAssociatedFunctionDeclarationSyntax syn => throw new NotImplementedException(),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IEnumerable<IStructMemberDeclaration> StructMemberDeclarations(IEnumerable<IStructMemberDeclarationSyntax> syntax)
        => syntax.Select(StructMemberDeclaration);

    private static IStructMemberDeclaration StructMemberDeclaration(IStructMemberDeclarationSyntax syntax)
        => syntax switch
        {
            ITypeDeclarationSyntax syn => TypeDeclaration(syn),
            IConcreteMethodDeclarationSyntax syn => throw new NotImplementedException(),
            IInitializerDeclarationSyntax syn => throw new NotImplementedException(),
            IFieldDeclarationSyntax syn => throw new NotImplementedException(),
            IAssociatedFunctionDeclarationSyntax syn => throw new NotImplementedException(),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IEnumerable<ITraitMemberDeclaration> TraitMemberDeclarations(IEnumerable<ITraitMemberDeclarationSyntax> syntax)
        => syntax.Select(TraitMemberDeclaration);

    private static ITraitMemberDeclaration TraitMemberDeclaration(ITraitMemberDeclarationSyntax syntax)
        => syntax switch
        {
            ITypeDeclarationSyntax syn => TypeDeclaration(syn),
            IMethodDeclarationSyntax syn => throw new NotImplementedException(),
            IAssociatedFunctionDeclarationSyntax syn => throw new NotImplementedException(),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };
    #endregion

    #region Capabilities
    private static ICapabilityConstraint CapabilityConstraint(ICapabilityConstraintSyntax syntax)
        => syntax switch
        {
            ICapabilitySetSyntax syn => CapabilitySet(syn),
            ICapabilitySyntax syn => Capability(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static ICapabilityConstraint CapabilitySet(ICapabilitySetSyntax syntax)
    {
        throw new NotImplementedException();
    }

    private static ICapabilityConstraint Capability(ICapabilitySyntax syntax)
    {
        throw new NotImplementedException();
    }
    #endregion
}
