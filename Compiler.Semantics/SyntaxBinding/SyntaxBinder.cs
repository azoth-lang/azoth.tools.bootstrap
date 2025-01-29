using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Syntax;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.SyntaxBinding;

/// <summary>
/// Bind the syntax tree nodes to semantic tree nodes.
/// </summary>
/// <remarks>The method naming scheme in this class is that methods are named after the syntax node
/// that they bind. There is one exception to this around types. In situations where a name must be
/// a type, type semantic nodes are generated. In situations where a name is an expression,
/// expression semantic nodes are generated. These methods have "Type" or "Expression" in their name
/// to reflect that.</remarks>
internal static class SyntaxBinder
{
    public static IPackageNode Bind(IPackageSyntax syntax)
        => Package(syntax);

    #region Top Level
    private static IEnumerable<ICompilationUnitNode> CompilationUnits(IEnumerable<ICompilationUnitSyntax> syntax)
        => syntax.Select(syn => ICompilationUnitNode.Create(syn, ImportDirectives(syn.ImportDirectives),
            NamespaceMemberDefinitions(syn.Definitions)));

    private static IEnumerable<IImportDirectiveNode> ImportDirectives(IEnumerable<IImportDirectiveSyntax> syntax)
        => syntax.Select(IImportDirectiveNode.Create);
    #endregion

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

    #region Namespace Definitions
    private static INamespaceBlockDefinitionNode NamespaceBlockDefinition(INamespaceBlockDefinitionSyntax syntax)
        => INamespaceBlockDefinitionNode.Create(syntax, ImportDirectives(syntax.ImportDirectives),
            NamespaceMemberDefinitions(syntax.Definitions));

    private static IEnumerable<INamespaceBlockMemberDefinitionNode> NamespaceMemberDefinitions(IEnumerable<INamespaceBlockMemberDefinitionSyntax> syntax)
        => syntax.Select(NamespaceBlockMemberDefinition);

    private static INamespaceBlockMemberDefinitionNode NamespaceBlockMemberDefinition(INamespaceBlockMemberDefinitionSyntax syntax)
        => syntax switch
        {
            INamespaceBlockDefinitionSyntax syn => NamespaceBlockDefinition(syn),
            ITypeDefinitionSyntax syn => TypeDefinition(syn),
            IFunctionDefinitionSyntax syn => FunctionDefinition(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };
    #endregion

    #region Function Definition
    private static IFunctionDefinitionNode FunctionDefinition(IFunctionDefinitionSyntax syntax)
        => IFunctionDefinitionNode.Create(syntax, Attributes(syntax.Attributes),
            NamedParameters(syntax.Parameters), Type(syntax.Return?.Type), Body(syntax.Body));
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
        // TODO support attributes on class
        => IClassDefinitionNode.Create(syntax, [], GenericParameters(syntax.GenericParameters),
            TypeName(syntax.BaseTypeName), SupertypeNames(syntax.SupertypeNames),
            ClassMemberDefinitions(syntax.Members));

    private static IStructDefinitionNode StructDefinition(IStructDefinitionSyntax syntax)
        // TODO support attributes on struct
        => IStructDefinitionNode.Create(syntax, [], GenericParameters(syntax.GenericParameters),
            SupertypeNames(syntax.SupertypeNames), StructMemberDefinitions(syntax.Members));

    private static ITraitDefinitionNode TraitDefinition(ITraitDefinitionSyntax syntax)
        // TODO support attributes on trait
        => ITraitDefinitionNode.Create(syntax, [], GenericParameters(syntax.GenericParameters),
            SupertypeNames(syntax.SupertypeNames), TraitMemberDefinitions(syntax.Members));

    private static IEnumerable<ITypeNameNode> SupertypeNames(IEnumerable<INameSyntax> syntax)
        => syntax.Select(syn => TypeName(syn));
    #endregion

    #region Type Definition Parts
    private static IEnumerable<IGenericParameterNode> GenericParameters(IEnumerable<IGenericParameterSyntax> syntax)
        => syntax.Select(GenericParameter);

    private static IGenericParameterNode GenericParameter(IGenericParameterSyntax syntax)
        => IGenericParameterNode.Create(syntax, CapabilityConstraint(syntax.Constraint));
    #endregion

    #region Type Member Definitions
    private static IEnumerable<IClassMemberDefinitionNode> ClassMemberDefinitions(IEnumerable<IClassMemberDefinitionSyntax> syntax)
        => syntax.Select(ClassMemberDefinition);

    private static IClassMemberDefinitionNode ClassMemberDefinition(IClassMemberDefinitionSyntax syntax)
        => syntax switch
        {
            ITypeDefinitionSyntax syn => TypeDefinition(syn),
            IMethodDefinitionSyntax syn => MethodDefinition(syn),
            IInitializerDefinitionSyntax syn => InitializerDefinition(syn),
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
            IMethodDefinitionSyntax syn => MethodDefinition(syn),
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
            IAbstractMethodDefinitionSyntax syn => AbstractMethodDefinition(syn),
            IOrdinaryMethodDefinitionSyntax syn => StandardMethodDefinition(syn),
            IGetterMethodDefinitionSyntax syn => GetterMethodDefinition(syn),
            ISetterMethodDefinitionSyntax syn => SetterMethodDefinition(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IAbstractMethodDefinitionNode AbstractMethodDefinition(IAbstractMethodDefinitionSyntax syntax)
        => IAbstractMethodDefinitionNode.Create(syntax, MethodSelfParameter(syntax.SelfParameter),
            NamedParameters(syntax.Parameters), Type(syntax.Return?.Type));

    private static IOrdinaryMethodDefinitionNode StandardMethodDefinition(IOrdinaryMethodDefinitionSyntax syntax)
        => IOrdinaryMethodDefinitionNode.Create(syntax, MethodSelfParameter(syntax.SelfParameter),
            NamedParameters(syntax.Parameters), Type(syntax.Return?.Type), Body(syntax.Body));

    private static IGetterMethodDefinitionNode GetterMethodDefinition(IGetterMethodDefinitionSyntax syntax)
        => IGetterMethodDefinitionNode.Create(syntax, MethodSelfParameter(syntax.SelfParameter),
            NamedParameters(syntax.Parameters), Type(syntax.Return.Type), Body(syntax.Body));

    private static ISetterMethodDefinitionNode SetterMethodDefinition(ISetterMethodDefinitionSyntax syntax)
        => ISetterMethodDefinitionNode.Create(syntax, MethodSelfParameter(syntax.SelfParameter),
            NamedParameters(syntax.Parameters), Type(syntax.Return?.Type), Body(syntax.Body));

    private static IInitializerDefinitionNode InitializerDefinition(IInitializerDefinitionSyntax syntax)
        => IOrdinaryInitializerDefinitionNode.Create(syntax, InitializerSelfParameter(syntax.SelfParameter),
            InitializerParameters(syntax.Parameters), BlockBody(syntax.Body));

    private static IFieldDefinitionNode FieldDefinition(IFieldDefinitionSyntax syntax)
        => IFieldDefinitionNode.Create(syntax, Type(syntax.Type), Expression(syntax.Initializer));

    private static IAssociatedFunctionDefinitionNode AssociatedFunctionDefinition(IAssociatedFunctionDefinitionSyntax syntax)
        => IAssociatedFunctionDefinitionNode.Create(syntax, NamedParameters(syntax.Parameters),
            Type(syntax.Return?.Type), Body(syntax.Body));
    #endregion

    #region Attributes
    private static IEnumerable<IAttributeNode> Attributes(IEnumerable<IAttributeSyntax> syntax)
        => syntax.Select(Attribute);

    private static IAttributeNode Attribute(IAttributeSyntax syntax)
        => IAttributeNode.Create(syntax, TypeName(syntax.TypeName));
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
        => ICapabilitySetNode.Create(syntax);

    [return: NotNullIfNotNull(nameof(syntax))]
    private static ICapabilityNode? Capability(ICapabilitySyntax? syntax)
        => syntax is not null ? ICapabilityNode.Create(syntax) : null;
    #endregion

    #region Parameters
    private static IEnumerable<IInitializerParameterNode> InitializerParameters(IEnumerable<IInitializerParameterSyntax> syntax)
        => syntax.Select(InitializerParameter);

    private static IInitializerParameterNode InitializerParameter(IInitializerParameterSyntax syntax)
        => syntax switch
        {
            INamedParameterSyntax syn => NamedParameter(syn),
            IFieldParameterSyntax syn => FieldParameter(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IEnumerable<INamedParameterNode> NamedParameters(IEnumerable<INamedParameterSyntax> syntax)
        => syntax.Select(NamedParameter);

    private static INamedParameterNode NamedParameter(INamedParameterSyntax syntax)
        => INamedParameterNode.Create(syntax, Type(syntax.Type));

    private static IInitializerSelfParameterNode InitializerSelfParameter(IInitializerSelfParameterSyntax syntax)
        => IInitializerSelfParameterNode.Create(syntax, Capability(syntax.Constraint));

    private static IMethodSelfParameterNode MethodSelfParameter(IMethodSelfParameterSyntax syntax)
        => IMethodSelfParameterNode.Create(syntax, CapabilityConstraint(syntax.Constraint));

    private static IFieldParameterNode FieldParameter(IFieldParameterSyntax syntax)
        => IFieldParameterNode.Create(syntax);
    #endregion

    #region Function Parts
    [return: NotNullIfNotNull(nameof(syntax))]
    private static IBodyNode? Body(IBodySyntax? syntax)
        => syntax switch
        {
            null => null,
            IBlockBodySyntax syn => BlockBody(syn),
            IExpressionBodySyntax syn => ExpressionBody(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IBlockBodyNode BlockBody(IBlockBodySyntax syntax)
        => IBlockBodyNode.Create(syntax, BodyStatements(syntax.Statements));

    private static IExpressionBodyNode ExpressionBody(IExpressionBodySyntax syntax)
        => IExpressionBodyNode.Create(syntax, ResultStatement(syntax.ResultStatement));
    #endregion

    #region Types
    private static IEnumerable<ITypeNode> Types(IFixedList<ITypeSyntax> syntax)
        => syntax.Select(syn => Type(syn));

    [return: NotNullIfNotNull(nameof(syntax))]
    private static ITypeNode? Type(ITypeSyntax? syntax)
        => syntax switch
        {
            null => null,
            INameSyntax syn => TypeName(syn),
            IOptionalTypeSyntax syn => OptionalType(syn),
            ICapabilityTypeSyntax syn => CapabilityType(syn),
            IFunctionTypeSyntax syn => FunctionType(syn),
            IViewpointTypeSyntax syn => ViewpointType(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IOptionalTypeNode OptionalType(IOptionalTypeSyntax syntax)
        => IOptionalTypeNode.Create(syntax, Type(syntax.Referent));

    private static ICapabilityTypeNode CapabilityType(ICapabilityTypeSyntax syntax)
        => ICapabilityTypeNode.Create(syntax, Capability(syntax.Capability), Type(syntax.Referent));

    private static IFunctionTypeNode FunctionType(IFunctionTypeSyntax syntax)
        => IFunctionTypeNode.Create(syntax, ParameterTypes(syntax.Parameters), Type(syntax.Return));

    private static IEnumerable<IParameterTypeNode> ParameterTypes(IEnumerable<IParameterTypeSyntax> syntax)
        => syntax.Select(ParameterType);

    private static IParameterTypeNode ParameterType(IParameterTypeSyntax syntax)
        => IParameterTypeNode.Create(syntax, Type(syntax.Referent));

    private static IViewpointTypeNode ViewpointType(IViewpointTypeSyntax syntax)
        => syntax switch
        {
            ICapabilityViewpointTypeSyntax syn => CapabilityViewpointType(syn),
            ISelfViewpointTypeSyntax syn => SelfViewpointType(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static ICapabilityViewpointTypeNode CapabilityViewpointType(ICapabilityViewpointTypeSyntax syntax)
        => ICapabilityViewpointTypeNode.Create(syntax, Capability(syntax.Capability), Type(syntax.Referent));

    private static ISelfViewpointTypeNode SelfViewpointType(ISelfViewpointTypeSyntax syntax)
        => ISelfViewpointTypeNode.Create(syntax, Type(syntax.Referent));
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
        => IResultStatementNode.Create(syntax, Expression(syntax.Expression));

    private static IVariableDeclarationStatementNode VariableDeclarationStatement(IVariableDeclarationStatementSyntax syntax)
        => IVariableDeclarationStatementNode.Create(syntax, Capability(syntax.Capability),
            Type(syntax.Type), Expression(syntax.Initializer));

    private static IExpressionStatementNode ExpressionStatement(IExpressionStatementSyntax syntax)
        => IExpressionStatementNode.Create(syntax, Expression(syntax.Expression));
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
        => IBindingContextPatternNode.Create(syntax, Pattern(syntax.Pattern), Type(syntax.Type));

    private static IOptionalOrBindingPatternNode OptionalOrBindingPattern(IOptionalOrBindingPatternSyntax syntax)
        => syntax switch
        {

            IBindingPatternSyntax syn => BindingPattern(syn),
            IOptionalPatternSyntax syn => OptionalPattern(syn),
            _ => throw ExhaustiveMatch.Failed(syntax),
        };

    private static IBindingPatternNode BindingPattern(IBindingPatternSyntax syntax)
        => IBindingPatternNode.Create(syntax);

    private static IOptionalPatternNode OptionalPattern(IOptionalPatternSyntax syntax)
        => IOptionalPatternNode.Create(syntax, OptionalOrBindingPattern(syntax.Pattern));
    #endregion

    #region Expressions
    private static IEnumerable<IAmbiguousExpressionNode> Expressions(IEnumerable<IExpressionSyntax> syntax)
        => syntax.Select(syn => Expression(syn));

    [return: NotNullIfNotNull(nameof(syntax))]
    private static IAmbiguousExpressionNode? Expression(IExpressionSyntax? syntax)
        => syntax switch
        {
            null => null,
            INameSyntax syn => NameExpression(syn),
            IMemberAccessExpressionSyntax syn => MemberAccessExpression(syn),
            IMissingNameExpressionSyntax syn => MissingNameExpression(syn),
            IBlockExpressionSyntax syn => BlockExpression(syn),
            IUnsafeExpressionSyntax syn => UnsafeExpression(syn),
            ILiteralExpressionSyntax syn => LiteralExpression(syn),
            IAssignmentExpressionSyntax syn => AssignmentExpression(syn),
            IBinaryOperatorExpressionSyntax syn => BinaryOperatorExpression(syn),
            IUnaryOperatorExpressionSyntax syn => UnaryOperatorExpression(syn),
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
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IBlockExpressionNode BlockExpression(IBlockExpressionSyntax syntax)
        => IBlockExpressionNode.Create(syntax, Statements(syntax.Statements));

    private static IUnsafeExpressionNode UnsafeExpression(IUnsafeExpressionSyntax syntax)
        => IUnsafeExpressionNode.Create(syntax, Expression(syntax.Expression));
    #endregion

    #region Instance Member Access Expressions
    private static IUnresolvedMemberAccessExpressionNode MemberAccessExpression(IMemberAccessExpressionSyntax syntax)
        => IUnresolvedMemberAccessExpressionNode.Create(syntax, Expression(syntax.Context), Types(syntax.GenericArguments));
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
        => IBoolLiteralExpressionNode.Create(syntax);

    private static IIntegerLiteralExpressionNode IntegerLiteralExpression(IIntegerLiteralExpressionSyntax syntax)
        => IIntegerLiteralExpressionNode.Create(syntax);

    private static INoneLiteralExpressionNode NoneLiteralExpression(INoneLiteralExpressionSyntax syntax)
        => INoneLiteralExpressionNode.Create(syntax);

    private static IStringLiteralExpressionNode StringLiteralExpression(IStringLiteralExpressionSyntax syntax)
        => IStringLiteralExpressionNode.Create(syntax);
    #endregion

    #region Operator Expressions
    private static IAssignmentExpressionNode AssignmentExpression(IAssignmentExpressionSyntax syntax)
        => IAssignmentExpressionNode.Create(syntax, Expression(syntax.LeftOperand), Expression(syntax.RightOperand));

    private static IBinaryOperatorExpressionNode BinaryOperatorExpression(IBinaryOperatorExpressionSyntax syntax)
        => IBinaryOperatorExpressionNode.Create(syntax, Expression(syntax.LeftOperand), Expression(syntax.RightOperand));

    private static IUnaryOperatorExpressionNode UnaryOperatorExpression(IUnaryOperatorExpressionSyntax syntax)
        => IUnaryOperatorExpressionNode.Create(syntax, Expression(syntax.Operand));

    private static IConversionExpressionNode ConversionExpression(IConversionExpressionSyntax syntax)
        => IConversionExpressionNode.Create(syntax, Expression(syntax.Referent), Type(syntax.ConvertToType));

    private static IPatternMatchExpressionNode PatternMatchExpression(IPatternMatchExpressionSyntax syntax)
        => IPatternMatchExpressionNode.Create(syntax, Expression(syntax.Referent), Pattern(syntax.Pattern));
    #endregion

    #region Control Flow Expressions
    private static IIfExpressionNode IfExpression(IIfExpressionSyntax syntax)
        => IIfExpressionNode.Create(syntax, Expression(syntax.Condition),
            BlockOrResult(syntax.ThenBlock), ElseClause(syntax.ElseClause));

    private static ILoopExpressionNode LoopExpression(ILoopExpressionSyntax syntax)
        => ILoopExpressionNode.Create(syntax, BlockExpression(syntax.Block));

    private static IWhileExpressionNode WhileExpression(IWhileExpressionSyntax syntax)
        => IWhileExpressionNode.Create(syntax, Expression(syntax.Condition), BlockExpression(syntax.Block));

    private static IForeachExpressionNode ForeachExpression(IForeachExpressionSyntax syntax)
        => IForeachExpressionNode.Create(syntax, Expression(syntax.InExpression), Type(syntax.Type),
            BlockExpression(syntax.Block));

    private static IBreakExpressionNode BreakExpression(IBreakExpressionSyntax syntax)
        => IBreakExpressionNode.Create(syntax, Expression(syntax.Value));

    private static INextExpressionNode NextExpression(INextExpressionSyntax syntax)
        => INextExpressionNode.Create(syntax);

    private static IReturnExpressionNode ReturnExpression(IReturnExpressionSyntax syntax)
        => IReturnExpressionNode.Create(syntax, Expression(syntax.Value));
    #endregion

    #region Invocation Expressions
    private static IUnresolvedInvocationExpressionNode InvocationExpression(IInvocationExpressionSyntax syntax)
        => IUnresolvedInvocationExpressionNode.Create(syntax, Expression(syntax.Expression), Expressions(syntax.Arguments));
    #endregion

    #region Name Expressions
    private static ISelfExpressionNode SelfExpression(ISelfExpressionSyntax syntax)
        => ISelfExpressionNode.Create(syntax);

    private static IMissingNameExpressionNode MissingNameExpression(IMissingNameExpressionSyntax syn)
        => IMissingNameExpressionNode.Create(syn);
    #endregion

    #region Names

    #region Semantic Nodes: Name Expressions
    private static INameExpressionNode NameExpression(INameSyntax syntax)
        => syntax switch
        {
            IBuiltInTypeNameSyntax syn => BuiltInTypeName(syn),
            IIdentifierNameSyntax syn => IdentifierNameExpression(syn),
            IGenericNameSyntax syn => GenericNameExpression(syn),
            IQualifiedNameSyntax syn => QualifiedNameExpression(syn),
            _ => throw ExhaustiveMatch.Failed(syntax),
        };

    private static IUnresolvedIdentifierNameExpressionNode IdentifierNameExpression(IIdentifierNameSyntax syntax)
        => IUnresolvedIdentifierNameExpressionNode.Create(syntax);

    private static IUnresolvedGenericNameExpressionNode GenericNameExpression(IGenericNameSyntax syntax)
        => IUnresolvedGenericNameExpressionNode.Create(syntax, Types(syntax.GenericArguments));

    private static IUnresolvedNameExpressionQualifiedNameExpressionNode QualifiedNameExpression(IQualifiedNameSyntax syntax)
        => IUnresolvedNameExpressionQualifiedNameExpressionNode.Create(syntax, NameExpression(syntax.Context), Types(syntax.GenericArguments));
    #endregion

    #region Semantic Nodes: Names
    private static INameNode Name(INameSyntax syntax)
        => syntax switch
        {
            IBuiltInTypeNameSyntax syn => BuiltInTypeName(syn),
            IIdentifierNameSyntax syn => IdentifierName(syn),
            IGenericNameSyntax syn => GenericName(syn),
            IQualifiedNameSyntax syn => QualifiedName(syn),
            _ => throw ExhaustiveMatch.Failed(syntax),
        };

    private static IUnresolvedIdentifierNameNode IdentifierName(IIdentifierNameSyntax syntax)
        => IUnresolvedIdentifierNameNode.Create(syntax);

    private static IUnresolvedGenericNameNode GenericName(IGenericNameSyntax syntax)
        => IUnresolvedGenericNameNode.Create(syntax, Types(syntax.GenericArguments));

    private static IUnresolvedNameQualifiedNameNode QualifiedName(IQualifiedNameSyntax syntax)
        => IUnresolvedNameQualifiedNameNode.Create(syntax, Name(syntax.Context), Types(syntax.GenericArguments));
    #endregion

    #region Semantic Nodes: Type Names

    [return: NotNullIfNotNull(nameof(syntax))]
    private static ITypeNameNode? TypeName(INameSyntax? syntax)
        => syntax switch
        {
            null => null,
            IBuiltInTypeNameSyntax syn => BuiltInTypeName(syn),
            IOrdinaryNameSyntax syn => OrdinaryTypeName(syn),
            IQualifiedNameSyntax syn => QualifiedTypeName(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IBuiltInTypeNameNode BuiltInTypeName(IBuiltInTypeNameSyntax syntax)
        => IBuiltInTypeNameNode.Create(syntax);

    private static IOrdinaryTypeNameNode OrdinaryTypeName(IOrdinaryNameSyntax? syntax)
        => syntax switch
        {
            IIdentifierNameSyntax syn => IdentifierTypeName(syn),
            IGenericNameSyntax syn => GenericTypeName(syn),
            _ => throw ExhaustiveMatch.Failed(syntax)
        };

    private static IIdentifierTypeNameNode IdentifierTypeName(IIdentifierNameSyntax syntax)
        => IIdentifierTypeNameNode.Create(syntax);

    private static IGenericTypeNameNode GenericTypeName(IGenericNameSyntax syntax)
        => IGenericTypeNameNode.Create(syntax, Types(syntax.GenericArguments));

    private static IQualifiedTypeNameNode QualifiedTypeName(IQualifiedNameSyntax syntax)
        => IQualifiedTypeNameNode.Create(syntax, Name(syntax.Context), Types(syntax.GenericArguments));
    #endregion

    #endregion

    #region Capability Expressions
    private static IAmbiguousMoveExpressionNode MoveExpression(IMoveExpressionSyntax syntax)
        => IAmbiguousMoveExpressionNode.Create(syntax, Expression(syntax.Referent));

    private static IAmbiguousFreezeExpressionNode FreezeExpression(IFreezeExpressionSyntax syntax)
        => IAmbiguousFreezeExpressionNode.Create(syntax, Expression(syntax.Referent));
    #endregion

    #region Async Expressions
    private static IAsyncBlockExpressionNode AsyncBlockExpression(IAsyncBlockExpressionSyntax syntax)
        => IAsyncBlockExpressionNode.Create(syntax, BlockExpression(syntax.Block));

    private static IAsyncStartExpressionNode AsyncStartExpression(IAsyncStartExpressionSyntax syntax)
        => IAsyncStartExpressionNode.Create(syntax, Expression(syntax.Expression));

    private static IAwaitExpressionNode AwaitExpression(IAwaitExpressionSyntax syntax)
        => IAwaitExpressionNode.Create(syntax, Expression(syntax.Expression));
    #endregion
}
