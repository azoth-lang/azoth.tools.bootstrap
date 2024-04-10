using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST.Conversions;
using Azoth.Tools.Bootstrap.Compiler.CST.Semantics;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

// ReSharper disable PartialTypeWithSinglePart

[Closed(
    typeof(ICompilationUnitSyntax),
    typeof(IUsingDirectiveSyntax),
    typeof(IBodyOrBlockSyntax),
    typeof(IElseClauseSyntax),
    typeof(IBindingSyntax),
    typeof(IDeclarationSyntax),
    typeof(IGenericParameterSyntax),
    typeof(ISupertypeNameSyntax),
    typeof(IAttributeSyntax),
    typeof(IParameterSyntax),
    typeof(ICapabilityConstraintSyntax),
    typeof(IReturnSyntax),
    typeof(ITypeSyntax),
    typeof(IParameterTypeSyntax),
    typeof(IReturnTypeSyntax),
    typeof(IStatementSyntax),
    typeof(IPatternSyntax),
    typeof(IExpressionSyntax))]
public partial interface ISyntax
{
    TextSpan Span { get; }
}

public partial interface ICompilationUnitSyntax : ISyntax
{
    CodeFile File { get; }
    NamespaceName ImplicitNamespaceName { get; }
    IFixedList<Diagnostic> Diagnostics { get; }
    IFixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
    IFixedList<INonMemberDeclarationSyntax> Declarations { get; }
}

public partial interface IUsingDirectiveSyntax : ISyntax
{
    NamespaceName Name { get; }
}

[Closed(
    typeof(IBodySyntax),
    typeof(IBlockExpressionSyntax))]
public partial interface IBodyOrBlockSyntax : ISyntax
{
    IFixedList<IStatementSyntax> Statements { get; }
}

[Closed(
    typeof(IBlockOrResultSyntax),
    typeof(IIfExpressionSyntax))]
public partial interface IElseClauseSyntax : ISyntax
{
}

[Closed(
    typeof(IResultStatementSyntax),
    typeof(IBlockExpressionSyntax))]
public partial interface IBlockOrResultSyntax : IElseClauseSyntax
{
    IPromise<DataType?> DataType { get; }
}

[Closed(
    typeof(ILocalBindingSyntax),
    typeof(IFieldDeclarationSyntax))]
public partial interface IBindingSyntax : ISyntax
{
    bool IsMutableBinding { get; }
    IPromise<BindingSymbol> Symbol { get; }
}

[Closed(
    typeof(INamedParameterSyntax),
    typeof(IVariableDeclarationStatementSyntax),
    typeof(IBindingPatternSyntax),
    typeof(IForeachExpressionSyntax))]
public partial interface ILocalBindingSyntax : IBindingSyntax
{
    new IPromise<NamedBindingSymbol> Symbol { get; }
}

[Closed(
    typeof(IEntityDeclarationSyntax),
    typeof(INonMemberDeclarationSyntax))]
public partial interface IDeclarationSyntax : ISyntax, IHasContainingLexicalScope
{
    CodeFile File { get; }
    TypeName? Name { get; }
    TextSpan NameSpan { get; }
    IPromise<Symbol> Symbol { get; }
}

[Closed(
    typeof(IInvocableDeclarationSyntax),
    typeof(INonMemberEntityDeclarationSyntax),
    typeof(IMemberDeclarationSyntax))]
public partial interface IEntityDeclarationSyntax : IDeclarationSyntax
{
    IAccessModifierToken? AccessModifier { get; }
}

[Closed(
    typeof(IConcreteInvocableDeclarationSyntax),
    typeof(IMethodDeclarationSyntax))]
public partial interface IInvocableDeclarationSyntax : IEntityDeclarationSyntax
{
    IFixedList<IConstructorOrInitializerParameterSyntax> Parameters { get; }
    new IPromise<InvocableSymbol> Symbol { get; }
}

[Closed(
    typeof(IFunctionDeclarationSyntax),
    typeof(IConcreteMethodDeclarationSyntax),
    typeof(IConstructorDeclarationSyntax),
    typeof(IInitializerDeclarationSyntax),
    typeof(IAssociatedFunctionDeclarationSyntax))]
public partial interface IConcreteInvocableDeclarationSyntax : IInvocableDeclarationSyntax
{
    IBodySyntax Body { get; }
}

[Closed(
    typeof(INamespaceDeclarationSyntax),
    typeof(INonMemberEntityDeclarationSyntax))]
public partial interface INonMemberDeclarationSyntax : IDeclarationSyntax
{
    NamespaceName ContainingNamespaceName { get; }
}

public partial interface INamespaceDeclarationSyntax : INonMemberDeclarationSyntax
{
    bool IsGlobalQualified { get; }
    NamespaceName DeclaredNames { get; }
    NamespaceName FullName { get; }
    new Promise<NamespaceSymbol> Symbol { get; }
    IFixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
    IFixedList<INonMemberDeclarationSyntax> Declarations { get; }
}

[Closed(
    typeof(ITypeDeclarationSyntax),
    typeof(IFunctionDeclarationSyntax))]
public partial interface INonMemberEntityDeclarationSyntax : IEntityDeclarationSyntax, INonMemberDeclarationSyntax
{
    new TypeName Name { get; }
}

[Closed(
    typeof(IClassOrStructDeclarationSyntax),
    typeof(ITraitDeclarationSyntax))]
public partial interface ITypeDeclarationSyntax : INonMemberEntityDeclarationSyntax
{
    IConstKeywordToken? ConstModifier { get; }
    bool IsConst { get; }
    IMoveKeywordToken? MoveModifier { get; }
    bool IsMove { get; }
    new StandardName Name { get; }
    IFixedList<IGenericParameterSyntax> GenericParameters { get; }
    new AcyclicPromise<UserTypeSymbol> Symbol { get; }
    IFixedList<ISupertypeNameSyntax> SupertypeNames { get; }
    IFixedList<IMemberDeclarationSyntax> Members { get; }
}

[Closed(
    typeof(IClassDeclarationSyntax),
    typeof(IStructDeclarationSyntax))]
public partial interface IClassOrStructDeclarationSyntax : ITypeDeclarationSyntax
{
}

public partial interface IClassDeclarationSyntax : IClassOrStructDeclarationSyntax
{
    IAbstractKeywordToken? AbstractModifier { get; }
    bool IsAbstract { get; }
    ISupertypeNameSyntax? BaseTypeName { get; }
    ConstructorSymbol? DefaultConstructorSymbol { get; }
    new IFixedList<IClassMemberDeclarationSyntax> Members { get; }
}

public partial interface IStructDeclarationSyntax : IClassOrStructDeclarationSyntax
{
    InitializerSymbol? DefaultInitializerSymbol { get; }
    new IFixedList<IStructMemberDeclarationSyntax> Members { get; }
}

public partial interface ITraitDeclarationSyntax : ITypeDeclarationSyntax
{
    new IFixedList<ITraitMemberDeclarationSyntax> Members { get; }
}

public partial interface IFunctionDeclarationSyntax : INonMemberEntityDeclarationSyntax, IConcreteInvocableDeclarationSyntax
{
    IFixedList<IAttributeSyntax> Attributes { get; }
    new IdentifierName Name { get; }
    new IFixedList<INamedParameterSyntax> Parameters { get; }
    IReturnSyntax? Return { get; }
    new AcyclicPromise<FunctionSymbol> Symbol { get; }
}

public partial interface IGenericParameterSyntax : ISyntax
{
    ICapabilityConstraintSyntax Constraint { get; }
    IdentifierName Name { get; }
    ParameterIndependence Independence { get; }
    ParameterVariance Variance { get; }
    Promise<GenericParameterTypeSymbol> Symbol { get; }
}

public partial interface ISupertypeNameSyntax : ISyntax, IHasContainingLexicalScope
{
    TypeName Name { get; }
    IFixedList<ITypeSyntax> TypeArguments { get; }
    Promise<UserTypeSymbol?> ReferencedSymbol { get; }
}

[Closed(
    typeof(IClassMemberDeclarationSyntax),
    typeof(ITraitMemberDeclarationSyntax),
    typeof(IStructMemberDeclarationSyntax),
    typeof(IMethodDeclarationSyntax),
    typeof(IFieldDeclarationSyntax),
    typeof(IAssociatedFunctionDeclarationSyntax))]
public partial interface IMemberDeclarationSyntax : IEntityDeclarationSyntax
{
    ITypeDeclarationSyntax DeclaringType { get; }
}

[Closed(
    typeof(IMethodDeclarationSyntax),
    typeof(IConstructorDeclarationSyntax),
    typeof(IFieldDeclarationSyntax),
    typeof(IAssociatedFunctionDeclarationSyntax))]
public partial interface IClassMemberDeclarationSyntax : IMemberDeclarationSyntax
{
}

[Closed(
    typeof(IMethodDeclarationSyntax),
    typeof(IAssociatedFunctionDeclarationSyntax))]
public partial interface ITraitMemberDeclarationSyntax : IMemberDeclarationSyntax
{
}

[Closed(
    typeof(IConcreteMethodDeclarationSyntax),
    typeof(IInitializerDeclarationSyntax),
    typeof(IFieldDeclarationSyntax),
    typeof(IAssociatedFunctionDeclarationSyntax))]
public partial interface IStructMemberDeclarationSyntax : IMemberDeclarationSyntax
{
}

[Closed(
    typeof(IAbstractMethodDeclarationSyntax),
    typeof(IConcreteMethodDeclarationSyntax))]
public partial interface IMethodDeclarationSyntax : IMemberDeclarationSyntax, IClassMemberDeclarationSyntax, ITraitMemberDeclarationSyntax, IInvocableDeclarationSyntax
{
    MethodKind Kind { get; }
    new IdentifierName Name { get; }
    IMethodSelfParameterSyntax SelfParameter { get; }
    new IFixedList<INamedParameterSyntax> Parameters { get; }
    IReturnSyntax? Return { get; }
    new AcyclicPromise<MethodSymbol> Symbol { get; }
}

public partial interface IAbstractMethodDeclarationSyntax : IMethodDeclarationSyntax
{
}

[Closed(
    typeof(IStandardMethodDeclarationSyntax),
    typeof(IGetterMethodDeclarationSyntax),
    typeof(ISetterMethodDeclarationSyntax))]
public partial interface IConcreteMethodDeclarationSyntax : IMethodDeclarationSyntax, IStructMemberDeclarationSyntax, IConcreteInvocableDeclarationSyntax
{
    new IFixedList<INamedParameterSyntax> Parameters { get; }
}

public partial interface IStandardMethodDeclarationSyntax : IConcreteMethodDeclarationSyntax
{
}

public partial interface IGetterMethodDeclarationSyntax : IConcreteMethodDeclarationSyntax
{
    new IReturnSyntax Return { get; }
}

public partial interface ISetterMethodDeclarationSyntax : IConcreteMethodDeclarationSyntax
{
}

public partial interface IConstructorDeclarationSyntax : IConcreteInvocableDeclarationSyntax, IClassMemberDeclarationSyntax
{
    new IClassDeclarationSyntax DeclaringType { get; }
    new IdentifierName? Name { get; }
    IConstructorSelfParameterSyntax SelfParameter { get; }
    new IBlockBodySyntax Body { get; }
    new AcyclicPromise<ConstructorSymbol> Symbol { get; }
}

public partial interface IInitializerDeclarationSyntax : IConcreteInvocableDeclarationSyntax, IStructMemberDeclarationSyntax
{
    new IStructDeclarationSyntax DeclaringType { get; }
    new IdentifierName? Name { get; }
    IInitializerSelfParameterSyntax SelfParameter { get; }
    new IBlockBodySyntax Body { get; }
    new AcyclicPromise<InitializerSymbol> Symbol { get; }
}

public partial interface IFieldDeclarationSyntax : IMemberDeclarationSyntax, IClassMemberDeclarationSyntax, IStructMemberDeclarationSyntax, IBindingSyntax
{
    new IClassOrStructDeclarationSyntax DeclaringType { get; }
    new IdentifierName Name { get; }
    ITypeSyntax Type { get; }
    new AcyclicPromise<FieldSymbol> Symbol { get; }
    IExpressionSyntax? Initializer { get; }
}

public partial interface IAssociatedFunctionDeclarationSyntax : IMemberDeclarationSyntax, IClassMemberDeclarationSyntax, ITraitMemberDeclarationSyntax, IStructMemberDeclarationSyntax, IConcreteInvocableDeclarationSyntax
{
    new IdentifierName Name { get; }
    new IFixedList<INamedParameterSyntax> Parameters { get; }
    IReturnSyntax? Return { get; }
    new AcyclicPromise<FunctionSymbol> Symbol { get; }
}

public partial interface IAttributeSyntax : ISyntax
{
    ITypeNameSyntax TypeName { get; }
}

[Closed(
    typeof(IConstructorOrInitializerParameterSyntax),
    typeof(ISelfParameterSyntax))]
public partial interface IParameterSyntax : ISyntax
{
    IdentifierName? Name { get; }
    IPromise<Pseudotype> DataType { get; }
    bool Unused { get; }
}

[Closed(
    typeof(INamedParameterSyntax),
    typeof(IFieldParameterSyntax))]
public partial interface IConstructorOrInitializerParameterSyntax : IParameterSyntax
{
}

public partial interface INamedParameterSyntax : IConstructorOrInitializerParameterSyntax, ILocalBindingSyntax
{
    bool IsLentBinding { get; }
    new IdentifierName Name { get; }
    Promise<int?> DeclarationNumber { get; }
    ITypeSyntax Type { get; }
    new IPromise<DataType> DataType { get; }
    new Promise<NamedVariableSymbol> Symbol { get; }
    IExpressionSyntax? DefaultValue { get; }
}

[Closed(
    typeof(IConstructorSelfParameterSyntax),
    typeof(IInitializerSelfParameterSyntax),
    typeof(IMethodSelfParameterSyntax))]
public partial interface ISelfParameterSyntax : IParameterSyntax
{
    bool IsLentBinding { get; }
    Promise<SelfParameterSymbol> Symbol { get; }
}

public partial interface IConstructorSelfParameterSyntax : ISelfParameterSyntax
{
    ICapabilitySyntax Capability { get; }
    new IPromise<DataType> DataType { get; }
}

public partial interface IInitializerSelfParameterSyntax : ISelfParameterSyntax
{
    ICapabilitySyntax Capability { get; }
    new IPromise<DataType> DataType { get; }
}

public partial interface IMethodSelfParameterSyntax : ISelfParameterSyntax
{
    ICapabilityConstraintSyntax Capability { get; }
}

[Closed(
    typeof(ICapabilitySetSyntax),
    typeof(ICapabilitySyntax))]
public partial interface ICapabilityConstraintSyntax : ISyntax
{
    ICapabilityConstraint Constraint { get; }
}

public partial interface ICapabilitySetSyntax : ICapabilityConstraintSyntax
{
    new CapabilitySet Constraint { get; }
}

public partial interface IFieldParameterSyntax : IConstructorOrInitializerParameterSyntax
{
    new IdentifierName Name { get; }
    Promise<FieldSymbol?> ReferencedSymbol { get; }
    IExpressionSyntax? DefaultValue { get; }
}

public partial interface IReturnSyntax : ISyntax
{
    ITypeSyntax Type { get; }
}

[Closed(
    typeof(IBlockBodySyntax),
    typeof(IExpressionBodySyntax))]
public partial interface IBodySyntax : IBodyOrBlockSyntax
{
}

public partial interface IBlockBodySyntax : IBodySyntax
{
    new IFixedList<IBodyStatementSyntax> Statements { get; }
}

public partial interface IExpressionBodySyntax : IBodySyntax
{
    IResultStatementSyntax ResultStatement { get; }
}

[Closed(
    typeof(ITypeNameSyntax),
    typeof(IOptionalTypeSyntax),
    typeof(ICapabilityTypeSyntax),
    typeof(IFunctionTypeSyntax),
    typeof(IViewpointTypeSyntax))]
public partial interface ITypeSyntax : ISyntax
{
}

[Closed(
    typeof(IStandardTypeNameSyntax),
    typeof(ISimpleTypeNameSyntax),
    typeof(IIdentifierTypeNameSyntax),
    typeof(IQualifiedTypeNameSyntax))]
public partial interface ITypeNameSyntax : ITypeSyntax, IHasContainingLexicalScope
{
    TypeName Name { get; }
    Promise<TypeSymbol?> ReferencedSymbol { get; }
}

[Closed(
    typeof(IIdentifierTypeNameSyntax),
    typeof(IGenericTypeNameSyntax))]
public partial interface IStandardTypeNameSyntax : ITypeNameSyntax
{
    new StandardName Name { get; }
}

[Closed(
    typeof(IIdentifierTypeNameSyntax),
    typeof(ISpecialTypeNameSyntax))]
public partial interface ISimpleTypeNameSyntax : ITypeNameSyntax
{
}

public partial interface IIdentifierTypeNameSyntax : ITypeNameSyntax, IStandardTypeNameSyntax, ISimpleTypeNameSyntax
{
    new IdentifierName Name { get; }
}

public partial interface ISpecialTypeNameSyntax : ISimpleTypeNameSyntax
{
    new SpecialTypeName Name { get; }
}

public partial interface IGenericTypeNameSyntax : IStandardTypeNameSyntax
{
    new GenericName Name { get; }
    IFixedList<ITypeSyntax> TypeArguments { get; }
}

public partial interface IQualifiedTypeNameSyntax : ITypeNameSyntax
{
    ITypeNameSyntax Context { get; }
    IStandardTypeNameSyntax QualifiedName { get; }
}

public partial interface IOptionalTypeSyntax : ITypeSyntax
{
    ITypeSyntax Referent { get; }
}

public partial interface ICapabilityTypeSyntax : ITypeSyntax
{
    ICapabilitySyntax Capability { get; }
    ITypeSyntax Referent { get; }
}

public partial interface ICapabilitySyntax : ICapabilityConstraintSyntax
{
    IFixedList<ICapabilityToken> Tokens { get; }
    DeclaredCapability Declared { get; }
    Capability Capability { get; }
}

public partial interface IFunctionTypeSyntax : ITypeSyntax
{
    IFixedList<IParameterTypeSyntax> Parameters { get; }
    IReturnTypeSyntax Return { get; }
}

public partial interface IParameterTypeSyntax : ISyntax
{
    bool IsLent { get; }
    ITypeSyntax Referent { get; }
}

public partial interface IReturnTypeSyntax : ISyntax
{
    ITypeSyntax Referent { get; }
}

[Closed(
    typeof(ICapabilityViewpointTypeSyntax),
    typeof(ISelfViewpointTypeSyntax))]
public partial interface IViewpointTypeSyntax : ITypeSyntax
{
    ITypeSyntax Referent { get; }
}

public partial interface ICapabilityViewpointTypeSyntax : IViewpointTypeSyntax
{
    ICapabilitySyntax Capability { get; }
}

public partial interface ISelfViewpointTypeSyntax : IViewpointTypeSyntax
{
    Promise<SelfParameterSymbol?> ReferencedSymbol { get; }
}

[Closed(
    typeof(IResultStatementSyntax),
    typeof(IBodyStatementSyntax))]
public partial interface IStatementSyntax : ISyntax
{
}

public partial interface IResultStatementSyntax : IStatementSyntax, IBlockOrResultSyntax
{
    IExpressionSyntax Expression { get; }
}

[Closed(
    typeof(IVariableDeclarationStatementSyntax),
    typeof(IExpressionStatementSyntax))]
public partial interface IBodyStatementSyntax : IStatementSyntax
{
}

public partial interface IVariableDeclarationStatementSyntax : IBodyStatementSyntax, ILocalBindingSyntax
{
    TextSpan NameSpan { get; }
    IdentifierName Name { get; }
    Promise<int?> DeclarationNumber { get; }
    ICapabilitySyntax? Capability { get; }
    ITypeSyntax? Type { get; }
    new Promise<NamedVariableSymbol> Symbol { get; }
    IExpressionSyntax? Initializer { get; }
}

public partial interface IExpressionStatementSyntax : IBodyStatementSyntax
{
    IExpressionSyntax Expression { get; }
}

[Closed(
    typeof(IBindingContextPatternSyntax),
    typeof(IOptionalOrBindingPatternSyntax))]
public partial interface IPatternSyntax : ISyntax
{
}

public partial interface IBindingContextPatternSyntax : IPatternSyntax
{
    bool IsMutableBinding { get; }
    IPatternSyntax Pattern { get; }
    ITypeSyntax? Type { get; }
}

[Closed(
    typeof(IBindingPatternSyntax),
    typeof(IOptionalPatternSyntax))]
public partial interface IOptionalOrBindingPatternSyntax : IPatternSyntax
{
}

public partial interface IBindingPatternSyntax : IOptionalOrBindingPatternSyntax, ILocalBindingSyntax
{
    IdentifierName Name { get; }
    Promise<int?> DeclarationNumber { get; }
    new Promise<NamedVariableSymbol> Symbol { get; }
}

public partial interface IOptionalPatternSyntax : IOptionalOrBindingPatternSyntax
{
    IOptionalOrBindingPatternSyntax Pattern { get; }
}

[Closed(
    typeof(ITypedExpressionSyntax),
    typeof(INameExpressionSyntax))]
public partial interface IExpressionSyntax : ISyntax
{
    IPromise<DataType?> DataType { get; }
    Conversion ImplicitConversion { get; }
    DataType? ConvertedDataType { get; }
}

[Closed(
    typeof(IDataTypedExpressionSyntax),
    typeof(IAssignableExpressionSyntax),
    typeof(ILiteralExpressionSyntax),
    typeof(INeverTypedExpressionSyntax),
    typeof(ISelfExpressionSyntax))]
public partial interface ITypedExpressionSyntax : IExpressionSyntax
{
    new IPromise<DataType> DataType { get; }
}

[Closed(
    typeof(IBlockExpressionSyntax),
    typeof(INewObjectExpressionSyntax),
    typeof(IUnsafeExpressionSyntax),
    typeof(IAssignmentExpressionSyntax),
    typeof(IBinaryOperatorExpressionSyntax),
    typeof(IUnaryOperatorExpressionSyntax),
    typeof(IIdExpressionSyntax),
    typeof(IConversionExpressionSyntax),
    typeof(IPatternMatchExpressionSyntax),
    typeof(IIfExpressionSyntax),
    typeof(ILoopExpressionSyntax),
    typeof(IWhileExpressionSyntax),
    typeof(IForeachExpressionSyntax),
    typeof(IInvocationExpressionSyntax),
    typeof(IMoveExpressionSyntax),
    typeof(IFreezeExpressionSyntax),
    typeof(IAsyncBlockExpressionSyntax),
    typeof(IAsyncStartExpressionSyntax),
    typeof(IAwaitExpressionSyntax))]
public partial interface IDataTypedExpressionSyntax : ITypedExpressionSyntax
{
    new Promise<DataType> DataType { get; }
}

[Closed(
    typeof(IIdentifierNameExpressionSyntax),
    typeof(IMemberAccessExpressionSyntax))]
public partial interface IAssignableExpressionSyntax : ITypedExpressionSyntax
{
    IPromise<Symbol?> ReferencedSymbol { get; }
}

public partial interface IBlockExpressionSyntax : IDataTypedExpressionSyntax, IBlockOrResultSyntax, IBodyOrBlockSyntax
{
    new Promise<DataType> DataType { get; }
}

public partial interface INewObjectExpressionSyntax : IDataTypedExpressionSyntax
{
    ITypeNameSyntax Type { get; }
    IdentifierName? ConstructorName { get; }
    TextSpan? ConstructorNameSpan { get; }
    IFixedList<IExpressionSyntax> Arguments { get; }
    Promise<ConstructorSymbol?> ReferencedSymbol { get; }
}

public partial interface IUnsafeExpressionSyntax : IDataTypedExpressionSyntax
{
    IExpressionSyntax Expression { get; }
}

[Closed(
    typeof(IBoolLiteralExpressionSyntax),
    typeof(IIntegerLiteralExpressionSyntax),
    typeof(INoneLiteralExpressionSyntax),
    typeof(IStringLiteralExpressionSyntax))]
public partial interface ILiteralExpressionSyntax : ITypedExpressionSyntax
{
}

public partial interface IBoolLiteralExpressionSyntax : ILiteralExpressionSyntax
{
    bool Value { get; }
    new Promise<BoolConstValueType> DataType { get; }
}

public partial interface IIntegerLiteralExpressionSyntax : ILiteralExpressionSyntax
{
    BigInteger Value { get; }
    new Promise<IntegerConstValueType> DataType { get; }
}

public partial interface INoneLiteralExpressionSyntax : ILiteralExpressionSyntax
{
    new Promise<OptionalType> DataType { get; }
}

public partial interface IStringLiteralExpressionSyntax : ILiteralExpressionSyntax
{
    string Value { get; }
    new Promise<DataType> DataType { get; }
}

public partial interface IAssignmentExpressionSyntax : IDataTypedExpressionSyntax
{
    IAssignableExpressionSyntax LeftOperand { get; }
    AssignmentOperator Operator { get; }
    IExpressionSyntax RightOperand { get; }
}

public partial interface IBinaryOperatorExpressionSyntax : IDataTypedExpressionSyntax
{
    IExpressionSyntax LeftOperand { get; }
    BinaryOperator Operator { get; }
    IExpressionSyntax RightOperand { get; }
}

public partial interface IUnaryOperatorExpressionSyntax : IDataTypedExpressionSyntax
{
    UnaryOperatorFixity Fixity { get; }
    UnaryOperator Operator { get; }
    IExpressionSyntax Operand { get; }
}

public partial interface IIdExpressionSyntax : IDataTypedExpressionSyntax
{
    IExpressionSyntax Referent { get; }
}

public partial interface IConversionExpressionSyntax : IDataTypedExpressionSyntax
{
    IExpressionSyntax Referent { get; }
    ConversionOperator Operator { get; }
    ITypeSyntax ConvertToType { get; }
}

public partial interface IPatternMatchExpressionSyntax : IDataTypedExpressionSyntax
{
    IExpressionSyntax Referent { get; }
    IPatternSyntax Pattern { get; }
}

public partial interface IIfExpressionSyntax : IDataTypedExpressionSyntax, IElseClauseSyntax
{
    IExpressionSyntax Condition { get; }
    IBlockOrResultSyntax ThenBlock { get; }
    IElseClauseSyntax? ElseClause { get; }
}

public partial interface ILoopExpressionSyntax : IDataTypedExpressionSyntax
{
    IBlockExpressionSyntax Block { get; }
}

public partial interface IWhileExpressionSyntax : IDataTypedExpressionSyntax
{
    IExpressionSyntax Condition { get; }
    IBlockExpressionSyntax Block { get; }
}

public partial interface IForeachExpressionSyntax : IDataTypedExpressionSyntax, ILocalBindingSyntax
{
    IdentifierName VariableName { get; }
    Promise<int?> DeclarationNumber { get; }
    IExpressionSyntax InExpression { get; }
    Promise<MethodSymbol?> IterateMethod { get; }
    Promise<MethodSymbol> NextMethod { get; }
    ITypeSyntax? Type { get; }
    new Promise<NamedVariableSymbol> Symbol { get; }
    IBlockExpressionSyntax Block { get; }
}

[Closed(
    typeof(IBreakExpressionSyntax),
    typeof(INextExpressionSyntax),
    typeof(IReturnExpressionSyntax))]
public partial interface INeverTypedExpressionSyntax : ITypedExpressionSyntax
{
    new Promise<NeverType> DataType { get; }
}

public partial interface IBreakExpressionSyntax : INeverTypedExpressionSyntax
{
    IExpressionSyntax? Value { get; }
}

public partial interface INextExpressionSyntax : INeverTypedExpressionSyntax
{
}

public partial interface IReturnExpressionSyntax : INeverTypedExpressionSyntax
{
    IExpressionSyntax? Value { get; }
}

public partial interface IInvocationExpressionSyntax : IDataTypedExpressionSyntax, IHasContainingLexicalScope
{
    IExpressionSyntax Expression { get; }
    IFixedList<IExpressionSyntax> Arguments { get; }
    Promise<Symbol?> ReferencedSymbol { get; }
}

[Closed(
    typeof(IInvocableNameExpressionSyntax),
    typeof(IVariableNameExpressionSyntax),
    typeof(IStandardNameExpressionSyntax),
    typeof(ISimpleNameExpressionSyntax),
    typeof(IIdentifierNameExpressionSyntax),
    typeof(ISpecialTypeNameExpressionSyntax),
    typeof(IGenericNameExpressionSyntax),
    typeof(ISelfExpressionSyntax),
    typeof(IMemberAccessExpressionSyntax))]
public partial interface INameExpressionSyntax : IExpressionSyntax
{
    IPromise<ISyntaxSemantics> Semantics { get; }
    IPromise<Symbol?> ReferencedSymbol { get; }
}

[Closed(
    typeof(IIdentifierNameExpressionSyntax),
    typeof(IGenericNameExpressionSyntax),
    typeof(IMemberAccessExpressionSyntax))]
public partial interface IInvocableNameExpressionSyntax : INameExpressionSyntax
{
}

[Closed(
    typeof(IIdentifierNameExpressionSyntax),
    typeof(ISelfExpressionSyntax))]
public partial interface IVariableNameExpressionSyntax : INameExpressionSyntax
{
    new IPromise<IVariableNameExpressionSyntaxSemantics> Semantics { get; }
}

[Closed(
    typeof(IIdentifierNameExpressionSyntax),
    typeof(IGenericNameExpressionSyntax))]
public partial interface IStandardNameExpressionSyntax : INameExpressionSyntax, IHasContainingLexicalScope
{
    StandardName? Name { get; }
}

[Closed(
    typeof(IIdentifierNameExpressionSyntax),
    typeof(ISpecialTypeNameExpressionSyntax))]
public partial interface ISimpleNameExpressionSyntax : INameExpressionSyntax
{
}

public partial interface IIdentifierNameExpressionSyntax : INameExpressionSyntax, IInvocableNameExpressionSyntax, ISimpleNameExpressionSyntax, IStandardNameExpressionSyntax, IVariableNameExpressionSyntax, IAssignableExpressionSyntax
{
    new IdentifierName? Name { get; }
    new Promise<IIdentifierNameExpressionSyntaxSemantics> Semantics { get; }
}

public partial interface ISpecialTypeNameExpressionSyntax : INameExpressionSyntax, ISimpleNameExpressionSyntax
{
    SpecialTypeName Name { get; }
    new Promise<SpecialTypeNameExpressionSyntaxSemantics> Semantics { get; }
    new Promise<DataType?> DataType { get; }
    new Promise<TypeSymbol?> ReferencedSymbol { get; }
}

public partial interface IGenericNameExpressionSyntax : INameExpressionSyntax, IInvocableNameExpressionSyntax, IStandardNameExpressionSyntax
{
    new GenericName Name { get; }
    IFixedList<ITypeSyntax> TypeArguments { get; }
    new Promise<DataType?> DataType { get; }
}

public partial interface ISelfExpressionSyntax : INameExpressionSyntax, IVariableNameExpressionSyntax, ITypedExpressionSyntax
{
    bool IsImplicit { get; }
    new Promise<ISelfExpressionSyntaxSemantics> Semantics { get; }
    new IPromise<SelfParameterSymbol?> ReferencedSymbol { get; }
    IPromise<Pseudotype> Pseudotype { get; }
}

public partial interface IMemberAccessExpressionSyntax : INameExpressionSyntax, IInvocableNameExpressionSyntax, IAssignableExpressionSyntax
{
    IExpressionSyntax Context { get; }
    AccessOperator AccessOperator { get; }
    StandardName MemberName { get; }
    IFixedList<ITypeSyntax> TypeArguments { get; }
    TextSpan MemberNameSpan { get; }
    new Promise<IMemberAccessSyntaxSemantics> Semantics { get; }
    new IPromise<Symbol?> ReferencedSymbol { get; }
}

public partial interface IMoveExpressionSyntax : IDataTypedExpressionSyntax
{
    IVariableNameExpressionSyntax Referent { get; }
    Promise<BindingSymbol?> ReferencedSymbol { get; }
}

public partial interface IFreezeExpressionSyntax : IDataTypedExpressionSyntax
{
    IVariableNameExpressionSyntax Referent { get; }
    Promise<BindingSymbol?> ReferencedSymbol { get; }
}

public partial interface IAsyncBlockExpressionSyntax : IDataTypedExpressionSyntax
{
    IBlockExpressionSyntax Block { get; }
}

public partial interface IAsyncStartExpressionSyntax : IDataTypedExpressionSyntax
{
    bool Scheduled { get; }
    IExpressionSyntax Expression { get; }
}

public partial interface IAwaitExpressionSyntax : IDataTypedExpressionSyntax
{
    IExpressionSyntax Expression { get; }
}

