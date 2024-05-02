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
    public static IPackageNode Bind(IPackageSyntax syntax)
        => Package(syntax);

    #region Packages
    private static IPackageNode Package(IPackageSyntax syntax)
        => new PackageNode(syntax, PackageReferences(syntax.References), CompilationUnits(syntax.CompilationUnits), CompilationUnits(syntax.TestingCompilationUnits));

    private static IEnumerable<IPackageReferenceNode> PackageReferences(IEnumerable<IPackageReferenceSyntax> syntax)
        => syntax.Select(syn => new PackageReferenceNode(syn));
    #endregion

    #region Code Files
    private static IEnumerable<ICompilationUnitNode> CompilationUnits(IEnumerable<ICompilationUnitSyntax> syntax)
        => syntax.Select(syn => new CompilationUnitNode(syn, UsingDirectives(syn.UsingDirectives),
            NamespaceMemberDeclarations(syn.Declarations)));

    private static IEnumerable<IUsingDirectiveNode> UsingDirectives(IEnumerable<IUsingDirectiveSyntax> syntax)
        => syntax.Select(syn => new UsingDirectiveNode(syn));
    #endregion

    #region Namespaces
    private static INamespaceDeclarationNode NamespaceDeclaration(INamespaceDeclarationSyntax syntax)
        => new NamespaceDeclarationNode(syntax, UsingDirectives(syntax.UsingDirectives),
            NamespaceMemberDeclarations(syntax.Declarations));

    private static IEnumerable<INamespaceMemberDeclarationNode> NamespaceMemberDeclarations(IEnumerable<INonMemberDeclarationSyntax> syntax)
        => syntax.Select(NamespaceMemberDeclaration);

    private static INamespaceMemberDeclarationNode NamespaceMemberDeclaration(INonMemberDeclarationSyntax syntax)
        => syntax switch
        {
            INamespaceDeclarationSyntax syn => NamespaceDeclaration(syn),
            ITypeDeclarationSyntax syn => TypeDeclaration(syn),
            IFunctionDeclarationSyntax syn => FunctionDeclaration(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };
    #endregion

    #region Type Declarations
    private static ITypeDeclarationNode TypeDeclaration(ITypeDeclarationSyntax syntax)
        => syntax switch
        {
            IClassDeclarationSyntax syn => ClassDeclaration(syn),
            IStructDeclarationSyntax syn => StructDeclaration(syn),
            ITraitDeclarationSyntax syn => TraitDeclaration(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IClassDeclarationNode ClassDeclaration(IClassDeclarationSyntax syntax)
        => new ClassDeclarationNode(syntax, GenericParameters(syntax.GenericParameters),
            SupertypeName(syntax.BaseTypeName), SupertypeNames(syntax.SupertypeNames),
            ClassMemberDeclarations(syntax.Members));

    private static IStructDeclarationNode StructDeclaration(IStructDeclarationSyntax syntax)
        => new StructDeclarationNode(syntax, GenericParameters(syntax.GenericParameters),
            SupertypeNames(syntax.SupertypeNames), StructMemberDeclarations(syntax.Members));

    private static ITraitDeclarationNode TraitDeclaration(ITraitDeclarationSyntax syntax)
        => new TraitDeclarationNode(syntax, GenericParameters(syntax.GenericParameters),
            SupertypeNames(syntax.SupertypeNames), TraitMemberDeclarations(syntax.Members));

    private static IEnumerable<ISupertypeNameNode> SupertypeNames(IFixedList<ISupertypeNameSyntax> syntax)
        => syntax.Select(syn => SupertypeName(syn));

    [return: NotNullIfNotNull(nameof(syntax))]
    private static ISupertypeNameNode? SupertypeName(ISupertypeNameSyntax? syntax)
        => syntax is not null ? new SupertypeNameNode(syntax) : null;
    #endregion

    #region Type Declaration Parts
    private static IEnumerable<IGenericParameterNode> GenericParameters(IEnumerable<IGenericParameterSyntax> syntax)
        => syntax.Select(GenericParameter);

    private static IGenericParameterNode GenericParameter(IGenericParameterSyntax syntax)
        => new GenericParameterNode(syntax, CapabilityConstraint(syntax.Constraint));
    #endregion

    #region Type Member Declarations
    private static IEnumerable<IClassMemberDeclarationNode> ClassMemberDeclarations(IEnumerable<IClassMemberDeclarationSyntax> syntax)
        => syntax.Select(ClassMemberDeclaration).WhereNotNull();

    private static IClassMemberDeclarationNode? ClassMemberDeclaration(IClassMemberDeclarationSyntax syntax)
        => syntax switch
        {
            ITypeDeclarationSyntax syn => TypeDeclaration(syn),
            IMethodDeclarationSyntax syn => null,
            IConstructorDeclarationSyntax syn => null,
            IFieldDeclarationSyntax syn => null,
            IAssociatedFunctionDeclarationSyntax syn => null,
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IEnumerable<IStructMemberDeclarationNode> StructMemberDeclarations(IEnumerable<IStructMemberDeclarationSyntax> syntax)
        => syntax.Select(StructMemberDeclaration).WhereNotNull();

    private static IStructMemberDeclarationNode? StructMemberDeclaration(IStructMemberDeclarationSyntax syntax)
        => syntax switch
        {
            ITypeDeclarationSyntax syn => TypeDeclaration(syn),
            IConcreteMethodDeclarationSyntax syn => null,
            IInitializerDeclarationSyntax syn => null,
            IFieldDeclarationSyntax syn => null,
            IAssociatedFunctionDeclarationSyntax syn => null,
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IEnumerable<ITraitMemberDeclarationNode> TraitMemberDeclarations(IEnumerable<ITraitMemberDeclarationSyntax> syntax)
        => syntax.Select(TraitMemberDeclaration).WhereNotNull();

    private static ITraitMemberDeclarationNode? TraitMemberDeclaration(ITraitMemberDeclarationSyntax syntax)
        => syntax switch
        {
            ITypeDeclarationSyntax syn => TypeDeclaration(syn),
            IMethodDeclarationSyntax syn => null,
            IAssociatedFunctionDeclarationSyntax syn => null,
            _ => throw ExhaustiveMatch.Failed(syntax)
        };
    #endregion

    #region Invocable Declarations
    private static IFunctionDeclarationNode FunctionDeclaration(IFunctionDeclarationSyntax syntax)
        => new FunctionDeclarationNode(syntax);
    #endregion

    #region Capabilities
    private static ICapabilityConstraintNode CapabilityConstraint(ICapabilityConstraintSyntax syntax)
        => syntax switch
        {
            ICapabilitySetSyntax syn => CapabilitySet(syn),
            ICapabilitySyntax syn => Capability(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static ICapabilityConstraintNode CapabilitySet(ICapabilitySetSyntax syntax)
        => new CapabilitySetNode(syntax);

    private static ICapabilityConstraintNode Capability(ICapabilitySyntax syntax)
        => new CapabilityNode(syntax);
    #endregion
}
