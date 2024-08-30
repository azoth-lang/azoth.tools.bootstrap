using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.SyntaxBinding;

/// <summary>
/// Bind the syntax tree nodes to semantic tree nodes.
/// </summary>
/// <remarks>The method naming scheme in this class is that methods are named after the syntax node
/// that they bind.</remarks>
internal static class SyntaxBinder
{
    public static IPackageNode Bind(IPackageSyntax syntax)
        => Package(syntax);

    #region Special Parts
    [return: NotNullIfNotNull(nameof(syntax))]
    private static IElseClauseNode? ElseClause(IElseClauseSyntax? syntax)
        => syntax switch
        {
            null => null,
            IBlockOrResultSyntax syn => BlockOrResult(syn),
            IIfExpressionSyntax syn => IfExpression(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IBlockOrResultNode BlockOrResult(IBlockOrResultSyntax syntax)
        => syntax switch
        {
            IResultStatementSyntax syn => ResultStatement(syn),
            IBlockExpressionSyntax syn => BlockExpression(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };
    #endregion

    #region Packages
    private static IPackageNode Package(IPackageSyntax syntax)
        => IPackageNode.Create(syntax, PackageReferences(syntax.References),
            PackageFacet(syntax, syntax.CompilationUnits),
            PackageFacet(syntax, syntax.TestingCompilationUnits));

    private static IPackageFacetNode PackageFacet(IPackageSyntax syntax, IFixedSet<ICompilationUnitSyntax> compilationUnits)
        => IPackageFacetNode.Create(syntax, CompilationUnits(compilationUnits));

    private static IEnumerable<IStandardPackageReferenceNode> PackageReferences(IEnumerable<IPackageReferenceSyntax> syntax)
        => syntax.Select(IStandardPackageReferenceNode.Create);
    #endregion

    #region Code Files
    private static IEnumerable<ICompilationUnitNode> CompilationUnits(IEnumerable<ICompilationUnitSyntax> syntax)
        => syntax.Select(syn => new CompilationUnitNode(syn, UsingDirectives(syn.UsingDirectives),
            NamespaceMemberDefinitions(syn.Definitions)));

    private static IEnumerable<IUsingDirectiveNode> UsingDirectives(IEnumerable<IUsingDirectiveSyntax> syntax)
        => syntax.Select(syn => new UsingDirectiveNode(syn));
    #endregion

    #region Namespace Declarations
    private static INamespaceBlockDefinitionNode NamespaceDefinition(INamespaceDefinitionSyntax syntax)
        => new NamespaceBlockDefinitionNode(syntax, UsingDirectives(syntax.UsingDirectives),
            NamespaceMemberDefinitions(syntax.Definitions));

    private static IEnumerable<INamespaceBlockMemberDefinitionNode> NamespaceMemberDefinitions(IEnumerable<INonMemberDefinitionSyntax> syntax)
        => syntax.Select(NonMemberDefinition);

    private static INamespaceBlockMemberDefinitionNode NonMemberDefinition(INonMemberDefinitionSyntax syntax)
        => syntax switch
        {
            INamespaceDefinitionSyntax syn => NamespaceDefinition(syn),
            ITypeDefinitionSyntax syn => TypeDefinition(syn),
            IFunctionDefinitionSyntax syn => FunctionDefinition(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };
    #endregion

    #region Function Definition
    private static IFunctionDefinitionNode FunctionDefinition(IFunctionDefinitionSyntax syntax)
        => new FunctionDefinitionNode(syntax, Attributes(syntax.Attributes), NamedParameters(syntax.Parameters), Type(syntax.Return?.Type), Body(syntax.Body));
    #endregion

    #region Type Definitions
    private static ITypeDefinitionNode TypeDefinition(ITypeDefinitionSyntax syntax)
        => syntax switch
        {
            IClassDefinitionSyntax syn => ClassDefinition(syn),
            IStructDefinitionSyntax syn => StructDefinition(syn),
            ITraitDefinitionSyntax syn => TraitDefinition(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IClassDefinitionNode ClassDefinition(IClassDefinitionSyntax syntax)
        => new ClassDefinitionNode(syntax, GenericParameters(syntax.GenericParameters),
            StandardTypeName(syntax.BaseTypeName), SupertypeNames(syntax.SupertypeNames),
            ClassMemberDefinitions(syntax.Members));

    private static IStructDefinitionNode StructDefinition(IStructDefinitionSyntax syntax)
        => new StructDefinitionNode(syntax, GenericParameters(syntax.GenericParameters),
            SupertypeNames(syntax.SupertypeNames), StructMemberDefinitions(syntax.Members));

    private static ITraitDefinitionNode TraitDefinition(ITraitDefinitionSyntax syntax)
        => new TraitDefinitionNode(syntax, GenericParameters(syntax.GenericParameters),
            SupertypeNames(syntax.SupertypeNames), TraitMemberDefinitions(syntax.Members));

    private static IEnumerable<IStandardTypeNameNode> SupertypeNames(IEnumerable<IStandardTypeNameSyntax> syntax)
        => syntax.Select(syn => StandardTypeName(syn));
    #endregion

    #region Type Definition Parts
    private static IEnumerable<IGenericParameterNode> GenericParameters(IEnumerable<IGenericParameterSyntax> syntax)
        => syntax.Select(GenericParameter);

    private static IGenericParameterNode GenericParameter(IGenericParameterSyntax syntax)
        => new GenericParameterNode(syntax, CapabilityConstraint(syntax.Constraint));
    #endregion

    #region Type Member Definitions
    private static IEnumerable<IClassMemberDefinitionNode> ClassMemberDefinitions(IEnumerable<IClassMemberDefinitionSyntax> syntax)
        => syntax.Select(ClassMemberDefinition);

    private static IClassMemberDefinitionNode ClassMemberDefinition(IClassMemberDefinitionSyntax syntax)
        => syntax switch
        {
            ITypeDefinitionSyntax syn => TypeDefinition(syn),
            IMethodDefinitionSyntax syn => MethodDefinition(syn),
            IConstructorDefinitionSyntax syn => ConstructorDefinition(syn),
            IFieldDefinitionSyntax syn => FieldDefinition(syn),
            IAssociatedFunctionDefinitionSyntax syn => AssociatedFunctionDefinition(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IEnumerable<IStructMemberDefinitionNode> StructMemberDefinitions(IEnumerable<IStructMemberDefinitionSyntax> syntax)
        => syntax.Select(StructMemberDefinition);

    private static IStructMemberDefinitionNode StructMemberDefinition(IStructMemberDefinitionSyntax syntax)
        => syntax switch
        {
            ITypeDefinitionSyntax syn => TypeDefinition(syn),
            IConcreteMethodDefinitionSyntax syn => ConcreteMethodDefinition(syn),
            IInitializerDefinitionSyntax syn => InitializerDefinition(syn),
            IFieldDefinitionSyntax syn => FieldDefinition(syn),
            IAssociatedFunctionDefinitionSyntax syn => AssociatedFunctionDefinition(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IEnumerable<ITraitMemberDefinitionNode> TraitMemberDefinitions(IEnumerable<ITraitMemberDefinitionSyntax> syntax)
        => syntax.Select(TraitMemberDefinition);

    private static ITraitMemberDefinitionNode TraitMemberDefinition(ITraitMemberDefinitionSyntax syntax)
        => syntax switch
        {
            ITypeDefinitionSyntax syn => TypeDefinition(syn),
            IMethodDefinitionSyntax syn => MethodDefinition(syn),
            IAssociatedFunctionDefinitionSyntax syn => AssociatedFunctionDefinition(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };
    #endregion

    #region Member Definitions
    private static IMethodDefinitionNode MethodDefinition(IMethodDefinitionSyntax syntax)
        => syntax switch
        {
            IConcreteMethodDefinitionSyntax syn => ConcreteMethodDefinition(syn),
            IAbstractMethodDefinitionSyntax syn => AbstractMethodDefinition(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IAbstractMethodDefinitionNode AbstractMethodDefinition(IAbstractMethodDefinitionSyntax syntax)
        => new AbstractMethodDefinitionNode(syntax, MethodSelfParameter(syntax.SelfParameter),
                       NamedParameters(syntax.Parameters), Type(syntax.Return?.Type));

    private static IConcreteMethodDefinitionNode ConcreteMethodDefinition(IConcreteMethodDefinitionSyntax syntax)
        => syntax switch
        {
            IStandardMethodDefinitionSyntax syn => StandardMethodDefinition(syn),
            IGetterMethodDefinitionSyntax syn => GetterMethodDefinition(syn),
            ISetterMethodDefinitionSyntax syn => SetterMethodDefinition(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IStandardMethodDefinitionNode StandardMethodDefinition(IStandardMethodDefinitionSyntax syntax)
        => new StandardMethodDefinitionNode(syntax, MethodSelfParameter(syntax.SelfParameter),
            NamedParameters(syntax.Parameters), Type(syntax.Return?.Type), Body(syntax.Body));

    private static IGetterMethodDefinitionNode GetterMethodDefinition(IGetterMethodDefinitionSyntax syntax)
        => new GetterMethodDefinitionNode(syntax, MethodSelfParameter(syntax.SelfParameter), NamedParameters(syntax.Parameters), Type(syntax.Return.Type), Body(syntax.Body));

    private static ISetterMethodDefinitionNode SetterMethodDefinition(ISetterMethodDefinitionSyntax syntax)
        => new SetterMethodDefinitionNode(syntax, MethodSelfParameter(syntax.SelfParameter), NamedParameters(syntax.Parameters), Type(syntax.Return?.Type), Body(syntax.Body));

    private static IConstructorDefinitionNode ConstructorDefinition(IConstructorDefinitionSyntax syntax)
        => new SourceConstructorDefinitionNode(syntax, ConstructorSelfParameter(syntax.SelfParameter), ConstructorOrInitializerParameters(syntax.Parameters), BlockBody(syntax.Body));

    private static IInitializerDefinitionNode InitializerDefinition(IInitializerDefinitionSyntax syntax)
        => new SourceInitializerDefinitionNode(syntax, InitializerSelfParameter(syntax.SelfParameter), ConstructorOrInitializerParameters(syntax.Parameters), BlockBody(syntax.Body));

    private static IFieldDefinitionNode FieldDefinition(IFieldDefinitionSyntax syntax)
        => new FieldDefinitionNode(syntax, Type(syntax.Type), Expression(syntax.Initializer));
    private static IAssociatedFunctionDefinitionNode AssociatedFunctionDefinition(IAssociatedFunctionDefinitionSyntax syntax)
        => new AssociatedFunctionDefinitionNode(syntax, NamedParameters(syntax.Parameters), Type(syntax.Return?.Type), Body(syntax.Body));
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

    [return: NotNullIfNotNull(nameof(syntax))]
    private static ICapabilityNode? Capability(ICapabilitySyntax? syntax)
        => syntax is not null ? new CapabilityNode(syntax) : null;
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

    private static IBlockBodyNode BlockBody(IBlockBodySyntax syntax)
        => new BlockBodyNode(syntax, BodyStatements(syntax.Statements));

    private static IExpressionBodyNode ExpressionBody(IExpressionBodySyntax syntax)
        => new ExpressionBodyNode(syntax, ResultStatement(syntax.ResultStatement));
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
        => new QualifiedTypeNameNode(syntax, TypeName(syntax.Context), StandardTypeName(syntax.QualifiedName));

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
    private static IEnumerable<IStatementNode> Statements(IEnumerable<IStatementSyntax> syntax)
        => syntax.Select(Statement);

    private static IStatementNode Statement(IStatementSyntax syntax)
        => syntax switch
        {
            IResultStatementSyntax syn => ResultStatement(syn),
            IBodyStatementSyntax syn => BodyStatement(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IEnumerable<IBodyStatementNode> BodyStatements(IEnumerable<IBodyStatementSyntax> syntax)
        => syntax.Select(BodyStatement);

    private static IBodyStatementNode BodyStatement(IBodyStatementSyntax syntax)
        => syntax switch
        {
            IVariableDeclarationStatementSyntax syn => VariableDeclarationStatement(syn),
            IExpressionStatementSyntax syn => ExpressionStatement(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IResultStatementNode ResultStatement(IResultStatementSyntax syntax)
        => new ResultStatementNode(syntax, Expression(syntax.Expression));

    private static IVariableDeclarationStatementNode VariableDeclarationStatement(
        IVariableDeclarationStatementSyntax syntax)
        => new VariableDeclarationStatementNode(syntax, Capability(syntax.Capability), Type(syntax.Type), Expression(syntax.Initializer));

    private static IExpressionStatementNode ExpressionStatement(IExpressionStatementSyntax syntax)
        => new ExpressionStatementNode(syntax, Expression(syntax.Expression));
    #endregion

    #region Patterns
    private static IPatternNode Pattern(IPatternSyntax syntax)
        => syntax switch
        {
            IBindingContextPatternSyntax syn => BindingContextPattern(syn),
            IOptionalOrBindingPatternSyntax syn => OptionalOrBindingPattern(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IBindingContextPatternNode BindingContextPattern(IBindingContextPatternSyntax syntax)
        => new BindingContextPatternNode(syntax, Pattern(syntax.Pattern), Type(syntax.Type));

    private static IOptionalOrBindingPatternNode OptionalOrBindingPattern(IOptionalOrBindingPatternSyntax syntax)
        => syntax switch
        {

            IBindingPatternSyntax syn => BindingPattern(syn),
            IOptionalPatternSyntax syn => OptionalPattern(syn),
            _ => throw ExhaustiveMatch.Failed(syntax),
        };

    private static IBindingPatternNode BindingPattern(IBindingPatternSyntax syntax)
        => new BindingPatternNode(syntax);

    private static IOptionalPatternNode OptionalPattern(IOptionalPatternSyntax syntax)
        => new OptionalPatternNode(syntax, OptionalOrBindingPattern(syntax.Pattern));
    #endregion

    #region Expressions
    private static IEnumerable<IAmbiguousExpressionNode> Expressions(IEnumerable<IExpressionSyntax> syntax)
        => syntax.Select(syn => Expression(syn));

    [return: NotNullIfNotNull(nameof(syntax))]
    private static IAmbiguousExpressionNode? Expression(IExpressionSyntax? syntax)
        => syntax switch
        {
            null => null,
            IAssignableExpressionSyntax syn => AssignableExpression(syn),
            IBlockExpressionSyntax syn => BlockExpression(syn),
            INewObjectExpressionSyntax syn => NewObjectExpression(syn),
            IUnsafeExpressionSyntax syn => UnsafeExpression(syn),
            ILiteralExpressionSyntax syn => LiteralExpression(syn),
            IAssignmentExpressionSyntax syn => AssignmentExpression(syn),
            IBinaryOperatorExpressionSyntax syn => BinaryOperatorExpression(syn),
            IUnaryOperatorExpressionSyntax syn => UnaryOperatorExpression(syn),
            IIdExpressionSyntax syn => IdExpression(syn),
            IConversionExpressionSyntax syn => ConversionExpression(syn),
            IPatternMatchExpressionSyntax syn => PatternMatchExpression(syn),
            IIfExpressionSyntax syn => IfExpression(syn),
            ILoopExpressionSyntax syn => LoopExpression(syn),
            IWhileExpressionSyntax syn => WhileExpression(syn),
            IForeachExpressionSyntax syn => ForeachExpression(syn),
            IBreakExpressionSyntax syn => BreakExpression(syn),
            INextExpressionSyntax syn => NextExpression(syn),
            IReturnExpressionSyntax syn => ReturnExpression(syn),
            IInvocationExpressionSyntax syn => InvocationExpression(syn),
            ISelfExpressionSyntax syn => SelfExpression(syn),
            IMoveExpressionSyntax syn => MoveExpression(syn),
            IFreezeExpressionSyntax syn => FreezeExpression(syn),
            IAsyncBlockExpressionSyntax syn => AsyncBlockExpression(syn),
            IAsyncStartExpressionSyntax syn => AsyncStartExpression(syn),
            IAwaitExpressionSyntax syn => AwaitExpression(syn),
            ISpecialTypeNameExpressionSyntax syn => SpecialTypeNameExpression(syn),
            IGenericNameExpressionSyntax syn => GenericNameExpression(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IAmbiguousAssignableExpressionNode AssignableExpression(IAssignableExpressionSyntax syntax)
        => syntax switch
        {
            IIdentifierNameExpressionSyntax syn => IdentifierNameExpression(syn),
            IMemberAccessExpressionSyntax syn => MemberAccessExpression(syn),
            IMissingNameSyntax syn => MissingName(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IBlockExpressionNode BlockExpression(IBlockExpressionSyntax syntax)
        => new BlockExpressionNode(syntax, Statements(syntax.Statements));

    private static INewObjectExpressionNode NewObjectExpression(INewObjectExpressionSyntax syntax)
        => new NewObjectExpressionNode(syntax, TypeName(syntax.Type), Expressions(syntax.Arguments));

    private static IUnsafeExpressionNode UnsafeExpression(IUnsafeExpressionSyntax syntax)
        => new UnsafeExpressionNode(syntax, Expression(syntax.Expression));
    #endregion

    #region Literal Expressions
    private static ILiteralExpressionNode LiteralExpression(ILiteralExpressionSyntax syntax)
        => syntax switch
        {
            IBoolLiteralExpressionSyntax syn => BoolLiteralExpression(syn),
            IIntegerLiteralExpressionSyntax syn => IntegerLiteralExpression(syn),
            INoneLiteralExpressionSyntax syn => NoneLiteralExpression(syn),
            IStringLiteralExpressionSyntax syn => StringLiteralExpression(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IBoolLiteralExpressionNode BoolLiteralExpression(IBoolLiteralExpressionSyntax syntax)
        => new BoolLiteralExpressionNode(syntax);

    private static IIntegerLiteralExpressionNode IntegerLiteralExpression(IIntegerLiteralExpressionSyntax syntax)
        => new IntegerLiteralExpressionNode(syntax);

    private static INoneLiteralExpressionNode NoneLiteralExpression(INoneLiteralExpressionSyntax syntax)
        => new NoneLiteralExpressionNode(syntax);

    private static IStringLiteralExpressionNode StringLiteralExpression(IStringLiteralExpressionSyntax syntax)
        => new StringLiteralExpressionNode(syntax);
    #endregion

    #region Operator Expressions
    private static IAssignmentExpressionNode AssignmentExpression(IAssignmentExpressionSyntax syntax)
        => new AssignmentExpressionNode(syntax, AssignableExpression(syntax.LeftOperand), Expression(syntax.RightOperand));

    private static IBinaryOperatorExpressionNode BinaryOperatorExpression(IBinaryOperatorExpressionSyntax syntax)
        => new BinaryOperatorExpressionNode(syntax, Expression(syntax.LeftOperand), Expression(syntax.RightOperand));

    private static IUnaryOperatorExpressionNode UnaryOperatorExpression(IUnaryOperatorExpressionSyntax syntax)
        => new UnaryOperatorExpressionNode(syntax, Expression(syntax.Operand));

    private static IIdExpressionNode IdExpression(IIdExpressionSyntax syntax)
        => new IdExpressionNode(syntax, Expression(syntax.Referent));

    private static IConversionExpressionNode ConversionExpression(IConversionExpressionSyntax syntax)
        => new ConversionExpressionNode(syntax, Expression(syntax.Referent), Type(syntax.ConvertToType));

    private static IPatternMatchExpressionNode PatternMatchExpression(IPatternMatchExpressionSyntax syntax)
        => new PatternMatchExpressionNode(syntax, Expression(syntax.Referent), Pattern(syntax.Pattern));
    #endregion

    #region Control Flow Expressions
    private static IIfExpressionNode IfExpression(IIfExpressionSyntax syntax)
        => new IfExpressionNode(syntax, Expression(syntax.Condition), BlockOrResult(syntax.ThenBlock), ElseClause(syntax.ElseClause));

    private static ILoopExpressionNode LoopExpression(ILoopExpressionSyntax syntax)
        => new LoopExpressionNode(syntax, BlockExpression(syntax.Block));

    private static IWhileExpressionNode WhileExpression(IWhileExpressionSyntax syntax)
        => new WhileExpressionNode(syntax, Expression(syntax.Condition), BlockExpression(syntax.Block));

    private static IForeachExpressionNode ForeachExpression(IForeachExpressionSyntax syntax)
        => new ForeachExpressionNode(syntax, Expression(syntax.InExpression), Type(syntax.Type), BlockExpression(syntax.Block));

    private static IBreakExpressionNode BreakExpression(IBreakExpressionSyntax syntax)
        => new BreakExpressionNode(syntax, Expression(syntax.Value));

    private static INextExpressionNode NextExpression(INextExpressionSyntax syntax)
        => new NextExpressionNode(syntax);

    private static IReturnExpressionNode ReturnExpression(IReturnExpressionSyntax syntax)
        => new ReturnExpressionNode(syntax, Expression(syntax.Value));
    #endregion

    #region Invocation Expressions
    private static IUnresolvedInvocationExpressionNode InvocationExpression(IInvocationExpressionSyntax syntax)
        => new UnresolvedInvocationExpressionNode(syntax, Expression(syntax.Expression), Expressions(syntax.Arguments));
    #endregion

    #region Name Expressions
    private static IUnresolvedSimpleNameNode SimpleName(ISimpleNameSyntax syntax)
        => syntax switch
        {
            IIdentifierNameExpressionSyntax syn => IdentifierNameExpression(syn),
            ISelfExpressionSyntax syn => SelfExpression(syn),
            IMissingNameSyntax syn => MissingName(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IIdentifierNameExpressionNode IdentifierNameExpression(IIdentifierNameExpressionSyntax syntax)
        => new IdentifierNameExpressionNode(syntax);

    private static ISpecialTypeNameExpressionNode SpecialTypeNameExpression(ISpecialTypeNameExpressionSyntax syntax)
        => new SpecialTypeNameExpressionNode(syntax);

    private static IGenericNameExpressionNode GenericNameExpression(IGenericNameExpressionSyntax syntax)
        => new GenericNameExpressionNode(syntax, Types(syntax.TypeArguments));

    private static IMemberAccessExpressionNode MemberAccessExpression(IMemberAccessExpressionSyntax syntax)
        => new MemberAccessExpressionNode(syntax, Expression(syntax.Context), Types(syntax.TypeArguments));

    private static ISelfExpressionNode SelfExpression(ISelfExpressionSyntax syntax)
        => new SelfExpressionNode(syntax);

    private static IMissingNameExpressionNode MissingName(IMissingNameSyntax syn)
        => new MissingNameExpressionNode(syn);
    #endregion

    #region Capability Expressions
    private static IAmbiguousMoveExpressionNode MoveExpression(IMoveExpressionSyntax syntax)
        => new AmbiguousMoveExpressionNode(syntax, SimpleName(syntax.Referent));

    private static IAmbiguousFreezeExpressionNode FreezeExpression(IFreezeExpressionSyntax syntax)
        => new AmbiguousFreezeExpressionNode(syntax, SimpleName(syntax.Referent));
    #endregion

    #region Async Expressions
    private static IAsyncBlockExpressionNode AsyncBlockExpression(IAsyncBlockExpressionSyntax syntax)
        => new AsyncBlockExpressionNode(syntax, BlockExpression(syntax.Block));

    private static IAsyncStartExpressionNode AsyncStartExpression(IAsyncStartExpressionSyntax syntax)
        => new AsyncStartExpressionNode(syntax, Expression(syntax.Expression));

    private static IAwaitExpressionNode AwaitExpression(IAwaitExpressionSyntax syntax)
        => new AwaitExpressionNode(syntax, Expression(syntax.Expression));
    #endregion
}
