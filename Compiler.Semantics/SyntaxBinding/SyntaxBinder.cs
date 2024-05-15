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
        => new PackageNode(syntax, PackageReferences(syntax.References),
            PackageFacet(syntax, syntax.CompilationUnits),
            PackageFacet(syntax, syntax.TestingCompilationUnits));

    private static IPackageFacetNode PackageFacet(IPackageSyntax syntax, IFixedSet<ICompilationUnitSyntax> compilationUnits)
        => new PackageFacetNode(syntax, CompilationUnits(compilationUnits));

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

    #region Namespace Declarations
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

    #region Function Declaration
    private static IFunctionDeclarationNode FunctionDeclaration(IFunctionDeclarationSyntax syntax)
        => new FunctionDeclarationNode(syntax, Attributes(syntax.Attributes), NamedParameters(syntax.Parameters), Type(syntax.Return?.Type), Body(syntax.Body));
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
            StandardTypeName(syntax.BaseTypeName), SupertypeNames(syntax.SupertypeNames),
            ClassMemberDeclarations(syntax.Members));

    private static IStructDeclarationNode StructDeclaration(IStructDeclarationSyntax syntax)
        => new StructDeclarationNode(syntax, GenericParameters(syntax.GenericParameters),
            SupertypeNames(syntax.SupertypeNames), StructMemberDeclarations(syntax.Members));

    private static ITraitDeclarationNode TraitDeclaration(ITraitDeclarationSyntax syntax)
        => new TraitDeclarationNode(syntax, GenericParameters(syntax.GenericParameters),
            SupertypeNames(syntax.SupertypeNames), TraitMemberDeclarations(syntax.Members));

    private static IEnumerable<IStandardTypeNameNode> SupertypeNames(IEnumerable<IStandardTypeNameSyntax> syntax)
        => syntax.Select(syn => StandardTypeName(syn));
    #endregion

    #region Type Declaration Parts
    private static IEnumerable<IGenericParameterNode> GenericParameters(IEnumerable<IGenericParameterSyntax> syntax)
        => syntax.Select(GenericParameter);

    private static IGenericParameterNode GenericParameter(IGenericParameterSyntax syntax)
        => new GenericParameterNode(syntax, CapabilityConstraint(syntax.Constraint));
    #endregion

    #region Type Member Declarations
    private static IEnumerable<IClassMemberDeclarationNode> ClassMemberDeclarations(IEnumerable<IClassMemberDeclarationSyntax> syntax)
        => syntax.Select(ClassMemberDeclaration);

    private static IClassMemberDeclarationNode ClassMemberDeclaration(IClassMemberDeclarationSyntax syntax)
        => syntax switch
        {
            ITypeDeclarationSyntax syn => TypeDeclaration(syn),
            IMethodDeclarationSyntax syn => MethodDeclaration(syn),
            IConstructorDeclarationSyntax syn => ConstructorDeclaration(syn),
            IFieldDeclarationSyntax syn => FieldDeclaration(syn),
            IAssociatedFunctionDeclarationSyntax syn => AssociatedFunctionDeclaration(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IEnumerable<IStructMemberDeclarationNode> StructMemberDeclarations(IEnumerable<IStructMemberDeclarationSyntax> syntax)
        => syntax.Select(StructMemberDeclaration);

    private static IStructMemberDeclarationNode StructMemberDeclaration(IStructMemberDeclarationSyntax syntax)
        => syntax switch
        {
            ITypeDeclarationSyntax syn => TypeDeclaration(syn),
            IConcreteMethodDeclarationSyntax syn => ConcreteMethodDeclaration(syn),
            IInitializerDeclarationSyntax syn => InitializerDeclaration(syn),
            IFieldDeclarationSyntax syn => FieldDeclaration(syn),
            IAssociatedFunctionDeclarationSyntax syn => AssociatedFunctionDeclaration(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IEnumerable<ITraitMemberDeclarationNode> TraitMemberDeclarations(IEnumerable<ITraitMemberDeclarationSyntax> syntax)
        => syntax.Select(TraitMemberDeclaration);

    private static ITraitMemberDeclarationNode TraitMemberDeclaration(ITraitMemberDeclarationSyntax syntax)
        => syntax switch
        {
            ITypeDeclarationSyntax syn => TypeDeclaration(syn),
            IMethodDeclarationSyntax syn => MethodDeclaration(syn),
            IAssociatedFunctionDeclarationSyntax syn => AssociatedFunctionDeclaration(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };
    #endregion

    #region Member Declarations
    private static IMethodDeclarationNode MethodDeclaration(IMethodDeclarationSyntax syntax)
        => syntax switch
        {
            IConcreteMethodDeclarationSyntax syn => ConcreteMethodDeclaration(syn),
            IAbstractMethodDeclarationSyntax syn => AbstractMethodDeclaration(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IAbstractMethodDeclarationNode AbstractMethodDeclaration(IAbstractMethodDeclarationSyntax syntax)
        => new AbstractMethodDeclarationNode(syntax, MethodSelfParameter(syntax.SelfParameter),
                       NamedParameters(syntax.Parameters), Type(syntax.Return?.Type));

    private static IConcreteMethodDeclarationNode ConcreteMethodDeclaration(IConcreteMethodDeclarationSyntax syntax)
        => syntax switch
        {
            IStandardMethodDeclarationSyntax syn => StandardMethodDeclaration(syn),
            IGetterMethodDeclarationSyntax syn => GetterMethodDeclaration(syn),
            ISetterMethodDeclarationSyntax syn => SetterMethodDeclaration(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IStandardMethodDeclarationNode StandardMethodDeclaration(IStandardMethodDeclarationSyntax syntax)
        => new StandardMethodDeclarationNode(syntax, MethodSelfParameter(syntax.SelfParameter),
            NamedParameters(syntax.Parameters), Type(syntax.Return?.Type), Body(syntax.Body));

    private static IGetterMethodDeclarationNode GetterMethodDeclaration(IGetterMethodDeclarationSyntax syntax)
        => new GetterMethodDeclarationNode(syntax, MethodSelfParameter(syntax.SelfParameter), NamedParameters(syntax.Parameters), Type(syntax.Return.Type), Body(syntax.Body));

    private static ISetterMethodDeclarationNode SetterMethodDeclaration(ISetterMethodDeclarationSyntax syntax)
        => new SetterMethodDeclarationNode(syntax, MethodSelfParameter(syntax.SelfParameter), NamedParameters(syntax.Parameters), Type(syntax.Return?.Type), Body(syntax.Body));

    private static IConstructorDeclarationNode ConstructorDeclaration(IConstructorDeclarationSyntax syntax)
        => new ConstructorDeclarationNode(syntax, ConstructorSelfParameter(syntax.SelfParameter), ConstructorOrInitializerParameters(syntax.Parameters), BlockBody(syntax.Body));

    private static IInitializerDeclarationNode InitializerDeclaration(IInitializerDeclarationSyntax syntax)
        => new InitializerDeclarationNode(syntax, InitializerSelfParameter(syntax.SelfParameter), ConstructorOrInitializerParameters(syntax.Parameters), BlockBody(syntax.Body));

    private static IFieldDeclarationNode FieldDeclaration(IFieldDeclarationSyntax syntax)
        => new FieldDeclarationNode(syntax, Type(syntax.Type), UntypedExpression(syntax.Initializer));
    private static IAssociatedFunctionDeclarationNode AssociatedFunctionDeclaration(IAssociatedFunctionDeclarationSyntax syntax)
        => new AssociatedFunctionDeclarationNode(syntax, NamedParameters(syntax.Parameters), Type(syntax.Return?.Type), Body(syntax.Body));
    #endregion

    #region Attributes
    private static IEnumerable<IAttributeNode> Attributes(IEnumerable<IAttributeSyntax> syntax)
        => syntax.Select(Attribute);

    private static IAttributeNode Attribute(IAttributeSyntax syntax)
        => new AttributeNode(syntax, StandardTypeName(syntax.TypeName));
    #endregion

    #region Capabilities
    private static ICapabilityConstraintNode CapabilityConstraint(ICapabilityConstraintSyntax syntax)
        => syntax switch
        {
            ICapabilitySetSyntax syn => CapabilitySet(syn),
            ICapabilitySyntax syn => Capability(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static ICapabilitySetNode CapabilitySet(ICapabilitySetSyntax syntax)
        => new CapabilitySetNode(syntax);

    private static ICapabilityNode Capability(ICapabilitySyntax syntax)
        => new CapabilityNode(syntax);
    #endregion

    #region Parameters
    private static IEnumerable<IConstructorOrInitializerParameterNode> ConstructorOrInitializerParameters(
        IEnumerable<IConstructorOrInitializerParameterSyntax> syntax)
        => syntax.Select(ConstructorOrInitializerParameter);

    private static IConstructorOrInitializerParameterNode ConstructorOrInitializerParameter(
        IConstructorOrInitializerParameterSyntax syntax)
        => syntax switch
        {
            INamedParameterSyntax syn => NamedParameter(syn),
            IFieldParameterSyntax syn => FieldParameter(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IEnumerable<INamedParameterNode> NamedParameters(IEnumerable<INamedParameterSyntax> syntax)
        => syntax.Select(NamedParameter);

    private static INamedParameterNode NamedParameter(INamedParameterSyntax syntax)
        => new NamedParameterNode(syntax, Type(syntax.Type));

    private static IConstructorSelfParameterNode ConstructorSelfParameter(IConstructorSelfParameterSyntax syntax)
        => new ConstructorSelfParameterNode(syntax, Capability(syntax.Capability));

    private static IInitializerSelfParameterNode InitializerSelfParameter(IInitializerSelfParameterSyntax syntax)
        => new InitializerSelfParameterNode(syntax, Capability(syntax.Capability));

    private static IMethodSelfParameterNode MethodSelfParameter(IMethodSelfParameterSyntax syntax)
        => new MethodSelfParameterNode(syntax, CapabilityConstraint(syntax.Capability));

    private static IFieldParameterNode FieldParameter(IFieldParameterSyntax syntax)
        => new FieldParameterNode(syntax);
    #endregion

    #region Function Parts
    private static IBodyNode Body(IBodySyntax syntax)
        => syntax switch
        {
            IBlockBodySyntax syn => BlockBody(syn),
            IExpressionBodySyntax syn => ExpressionBody(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IBlockBodyNode BlockBody(IBlockBodySyntax syntax) => new BlockBodyNode(syntax);

    private static IExpressionBodyNode ExpressionBody(IExpressionBodySyntax syntax)
        => new ExpressionBodyNode(syntax);
    #endregion

    #region Types
    private static IEnumerable<ITypeNode> Types(IFixedList<ITypeSyntax> syntax)
        => syntax.Select(syn => Type(syn));

    [return: NotNullIfNotNull(nameof(syntax))]
    private static ITypeNode? Type(ITypeSyntax? syntax)
        => syntax switch
        {
            null => null,
            ITypeNameSyntax syn => TypeName(syn),
            IOptionalTypeSyntax syn => OptionalType(syn),
            ICapabilityTypeSyntax syn => CapabilityType(syn),
            IFunctionTypeSyntax syn => FunctionType(syn),
            IViewpointTypeSyntax syn => ViewpointType(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static ITypeNameNode TypeName(ITypeNameSyntax syntax)
        => syntax switch
        {
            IStandardTypeNameSyntax syn => StandardTypeName(syn),
            ISimpleTypeNameSyntax syn => SimpleTypeName(syn),
            IQualifiedTypeNameSyntax syn => QualifiedTypeName(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    [return: NotNullIfNotNull(nameof(syntax))]
    private static IStandardTypeNameNode? StandardTypeName(IStandardTypeNameSyntax? syntax)
        => syntax switch
        {
            null => null,
            IIdentifierTypeNameSyntax syn => IdentifierTypeName(syn),
            IGenericTypeNameSyntax syn => GenericTypeName(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IIdentifierTypeNameNode IdentifierTypeName(IIdentifierTypeNameSyntax syntax)
        => new IdentifierTypeNameNode(syntax);

    private static IGenericTypeNameNode GenericTypeName(IGenericTypeNameSyntax syntax)
        => new GenericTypeNameNode(syntax, Types(syntax.TypeArguments));

    private static ISimpleTypeNameNode SimpleTypeName(ISimpleTypeNameSyntax syntax)
        => syntax switch
        {
            IIdentifierTypeNameSyntax syn => IdentifierTypeName(syn),
            ISpecialTypeNameSyntax syn => SpecialTypeName(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static ISpecialTypeNameNode SpecialTypeName(ISpecialTypeNameSyntax syntax)
        => new SpecialTypeNameNode(syntax);

    private static IQualifiedTypeNameNode QualifiedTypeName(IQualifiedTypeNameSyntax syntax)
        => throw new System.NotImplementedException();

    private static IOptionalTypeNode OptionalType(IOptionalTypeSyntax syntax)
        => new OptionalTypeNode(syntax, Type(syntax.Referent));

    private static ICapabilityTypeNode CapabilityType(ICapabilityTypeSyntax syntax)
        => new CapabilityTypeNode(syntax, Capability(syntax.Capability), Type(syntax.Referent));

    private static IFunctionTypeNode FunctionType(IFunctionTypeSyntax syntax)
        => new FunctionTypeNode(syntax, ParameterTypes(syntax.Parameters), Type(syntax.Return.Referent));

    private static IEnumerable<IParameterTypeNode> ParameterTypes(IEnumerable<IParameterTypeSyntax> syntax)
        => syntax.Select(ParameterType);

    private static IParameterTypeNode ParameterType(IParameterTypeSyntax syntax)
        => new ParameterTypeNode(syntax, Type(syntax.Referent));

    private static IViewpointTypeNode ViewpointType(IViewpointTypeSyntax syntax)
        => syntax switch
        {
            ICapabilityViewpointTypeSyntax syn => CapabilityViewpointType(syn),
            ISelfViewpointTypeSyntax syn => SelfViewpointType(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static ICapabilityViewpointTypeNode CapabilityViewpointType(ICapabilityViewpointTypeSyntax syntax)
        => new CapabilityViewpointTypeNode(syntax, Capability(syntax.Capability), Type(syntax.Referent));

    private static ISelfViewpointTypeNode SelfViewpointType(ISelfViewpointTypeSyntax syntax)
        => new SelfViewpointTypeNode(syntax, Type(syntax.Referent));
    #endregion

    #region Statements
    #endregion

    #region Patterns
    #endregion

    #region Expressions
    [return: NotNullIfNotNull(nameof(syntax))]
    private static IUntypedExpressionNode? UntypedExpression(IExpressionSyntax? syntax)
        => syntax switch
        {
            null => null,
            ITypedExpressionSyntax syn => Expression(syn),
            INameExpressionSyntax syn => NameExpression(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IExpressionNode Expression(ITypedExpressionSyntax syntax)
        => throw new System.NotImplementedException();
    #endregion

    #region Literal Expressions

    #endregion

    #region Operator Expressions

    #endregion

    #region Control Flow Expressions

    #endregion

    #region Invocation Expressions

    #endregion

    #region Name Expressions
    private static INameExpressionNode NameExpression(INameExpressionSyntax syntax)
        => throw new System.NotImplementedException();
    #endregion

    #region Capability Expressions

    #endregion

    #region Async Expressions

    #endregion
}
