using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST.Conversions;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

[Closed(
    typeof(ICompilationUnitSyntax),
    typeof(IUsingDirectiveSyntax),
    typeof(IBodyOrBlockSyntax),
    typeof(IElseClauseSyntax),
    typeof(IBindingSyntax),
    typeof(IDeclarationSyntax),
    typeof(IAttributeSyntax),
    typeof(IGenericParameterSyntax),
    typeof(IParameterSyntax),
    typeof(ISelfReferenceCapabilitySyntax),
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
    IFixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
    IFixedList<INonMemberDeclarationSyntax> Declarations { get; }
    IFixedList<Diagnostic> Diagnostics { get; }
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
    IFixedList<IConstructorParameterSyntax> Parameters { get; }
    new IPromise<InvocableSymbol> Symbol { get; }
}

[Closed(
    typeof(IFunctionDeclarationSyntax),
    typeof(IConcreteMethodDeclarationSyntax),
    typeof(IConstructorDeclarationSyntax),
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
    new TypeName Name { get; }
}

public partial interface INamespaceDeclarationSyntax : INonMemberDeclarationSyntax
{
    bool IsGlobalQualified { get; }
    NamespaceName DeclaredNames { get; }
    NamespaceName FullName { get; }
    new Promise<NamespaceOrPackageSymbol> Symbol { get; }
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
    typeof(IClassDeclarationSyntax),
    typeof(ITraitDeclarationSyntax))]
public partial interface ITypeDeclarationSyntax : INonMemberEntityDeclarationSyntax
{
    IConstKeywordToken? ConstModifier { get; }
    bool IsConst { get; }
    IMoveKeywordToken? MoveModifier { get; }
    bool IsMove { get; }
    new StandardTypeName Name { get; }
    IFixedList<IGenericParameterSyntax> GenericParameters { get; }
    new AcyclicPromise<ObjectTypeSymbol> Symbol { get; }
    IFixedList<ITypeNameSyntax> SupertypeNames { get; }
    IFixedList<IMemberDeclarationSyntax> Members { get; }
}

public partial interface IClassDeclarationSyntax : ITypeDeclarationSyntax
{
    IAbstractKeywordToken? AbstractModifier { get; }
    bool IsAbstract { get; }
    ITypeNameSyntax? BaseTypeName { get; }
    ConstructorSymbol? DefaultConstructorSymbol { get; }
    new IFixedList<IClassMemberDeclarationSyntax> Members { get; }
}

public partial interface ITraitDeclarationSyntax : ITypeDeclarationSyntax
{
    new IFixedList<ITraitMemberDeclarationSyntax> Members { get; }
}

public partial interface IFunctionDeclarationSyntax : INonMemberEntityDeclarationSyntax, IConcreteInvocableDeclarationSyntax
{
    IFixedList<IAttributeSyntax> Attributes { get; }
    new SimpleName Name { get; }
    new IFixedList<INamedParameterSyntax> Parameters { get; }
    IReturnSyntax? Return { get; }
    new AcyclicPromise<FunctionSymbol> Symbol { get; }
}

[Closed(
    typeof(IClassMemberDeclarationSyntax),
    typeof(ITraitMemberDeclarationSyntax))]
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
    typeof(IAbstractMethodDeclarationSyntax),
    typeof(IConcreteMethodDeclarationSyntax))]
public partial interface IMethodDeclarationSyntax : IClassMemberDeclarationSyntax, ITraitMemberDeclarationSyntax, IInvocableDeclarationSyntax
{
    new SimpleName Name { get; }
    IMethodSelfParameterSyntax SelfParameter { get; }
    new IFixedList<INamedParameterSyntax> Parameters { get; }
    IReturnSyntax? Return { get; }
    new AcyclicPromise<MethodSymbol> Symbol { get; }
}

public partial interface IAbstractMethodDeclarationSyntax : IMethodDeclarationSyntax
{
}

public partial interface IConcreteMethodDeclarationSyntax : IMethodDeclarationSyntax, IConcreteInvocableDeclarationSyntax
{
    new IFixedList<INamedParameterSyntax> Parameters { get; }
}

public partial interface IConstructorDeclarationSyntax : IClassMemberDeclarationSyntax, IConcreteInvocableDeclarationSyntax
{
    new IClassDeclarationSyntax DeclaringType { get; }
    new SimpleName? Name { get; }
    IConstructorSelfParameterSyntax SelfParameter { get; }
    new IBlockBodySyntax Body { get; }
    new AcyclicPromise<ConstructorSymbol> Symbol { get; }
}

public partial interface IFieldDeclarationSyntax : IClassMemberDeclarationSyntax, IBindingSyntax
{
    new IClassDeclarationSyntax DeclaringType { get; }
    new SimpleName Name { get; }
    ITypeSyntax Type { get; }
    new AcyclicPromise<FieldSymbol> Symbol { get; }
    IExpressionSyntax? Initializer { get; }
}

public partial interface IAssociatedFunctionDeclarationSyntax : IClassMemberDeclarationSyntax, ITraitMemberDeclarationSyntax, IConcreteInvocableDeclarationSyntax
{
    new SimpleName Name { get; }
    new IFixedList<INamedParameterSyntax> Parameters { get; }
    IReturnSyntax? Return { get; }
    new AcyclicPromise<FunctionSymbol> Symbol { get; }
}

public partial interface IAttributeSyntax : ISyntax
{
    ITypeNameSyntax TypeName { get; }
}

public partial interface IGenericParameterSyntax : ISyntax
{
    SimpleName Name { get; }
    ParameterVariance ParameterVariance { get; }
    Promise<GenericParameterTypeSymbol> Symbol { get; }
}

[Closed(
    typeof(IConstructorParameterSyntax),
    typeof(INamedParameterSyntax),
    typeof(ISelfParameterSyntax),
    typeof(IFieldParameterSyntax))]
public partial interface IParameterSyntax : ISyntax
{
    SimpleName? Name { get; }
    IPromise<Pseudotype> DataType { get; }
    bool Unused { get; }
}

[Closed(
    typeof(INamedParameterSyntax),
    typeof(IFieldParameterSyntax))]
public partial interface IConstructorParameterSyntax : IParameterSyntax
{
}

public partial interface INamedParameterSyntax : IParameterSyntax, IConstructorParameterSyntax, ILocalBindingSyntax
{
    bool IsLentBinding { get; }
    new SimpleName Name { get; }
    Promise<int?> DeclarationNumber { get; }
    ITypeSyntax Type { get; }
    new IPromise<DataType> DataType { get; }
    new Promise<VariableSymbol> Symbol { get; }
    IExpressionSyntax? DefaultValue { get; }
}

[Closed(
    typeof(IConstructorSelfParameterSyntax),
    typeof(IMethodSelfParameterSyntax))]
public partial interface ISelfParameterSyntax : IParameterSyntax
{
    bool IsLentBinding { get; }
    Promise<SelfParameterSymbol> Symbol { get; }
}

public partial interface IConstructorSelfParameterSyntax : ISelfParameterSyntax
{
    IReferenceCapabilitySyntax Capability { get; }
    new IPromise<DataType> DataType { get; }
}

public partial interface IMethodSelfParameterSyntax : ISelfParameterSyntax
{
    ISelfReferenceCapabilitySyntax Capability { get; }
}

[Closed(
    typeof(IReferenceCapabilityConstraintSyntax),
    typeof(IReferenceCapabilitySyntax))]
public partial interface ISelfReferenceCapabilitySyntax : ISyntax
{
}

public partial interface IReferenceCapabilityConstraintSyntax : ISelfReferenceCapabilitySyntax
{
    ReferenceCapabilityConstraint Constraint { get; }
}

public partial interface IFieldParameterSyntax : IParameterSyntax, IConstructorParameterSyntax
{
    new SimpleName Name { get; }
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
    typeof(ISimpleTypeNameSyntax),
    typeof(IParameterizedTypeSyntax))]
public partial interface ITypeNameSyntax : ITypeSyntax, IHasContainingLexicalScope
{
    TypeName Name { get; }
    Promise<TypeSymbol?> ReferencedSymbol { get; }
}

public partial interface ISimpleTypeNameSyntax : ITypeNameSyntax
{
}

public partial interface IParameterizedTypeSyntax : ITypeNameSyntax
{
    IFixedList<ITypeSyntax> TypeArguments { get; }
}

public partial interface IOptionalTypeSyntax : ITypeSyntax
{
    ITypeSyntax Referent { get; }
}

public partial interface ICapabilityTypeSyntax : ITypeSyntax
{
    IReferenceCapabilitySyntax Capability { get; }
    ITypeSyntax Referent { get; }
}

public partial interface IReferenceCapabilitySyntax : ISelfReferenceCapabilitySyntax
{
    IFixedList<ICapabilityToken> Tokens { get; }
    DeclaredReferenceCapability Declared { get; }
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
    IReferenceCapabilitySyntax Capability { get; }
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
    SimpleName Name { get; }
    Promise<int?> DeclarationNumber { get; }
    IReferenceCapabilitySyntax? Capability { get; }
    ITypeSyntax? Type { get; }
    new Promise<VariableSymbol> Symbol { get; }
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
    SimpleName Name { get; }
    Promise<int?> DeclarationNumber { get; }
    new Promise<VariableSymbol> Symbol { get; }
}

public partial interface IOptionalPatternSyntax : IOptionalOrBindingPatternSyntax
{
    IOptionalOrBindingPatternSyntax Pattern { get; }
}

[Closed(
    typeof(IAssignableExpressionSyntax),
    typeof(IBlockExpressionSyntax),
    typeof(INewObjectExpressionSyntax),
    typeof(IUnsafeExpressionSyntax),
    typeof(ILiteralExpressionSyntax),
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
    typeof(IBreakExpressionSyntax),
    typeof(INextExpressionSyntax),
    typeof(IReturnExpressionSyntax),
    typeof(IInvocationExpressionSyntax),
    typeof(IVariableNameExpressionSyntax),
    typeof(IMoveExpressionSyntax),
    typeof(IFreezeExpressionSyntax),
    typeof(IAsyncBlockExpressionSyntax),
    typeof(IAsyncStartExpressionSyntax),
    typeof(IAwaitExpressionSyntax))]
public partial interface IExpressionSyntax : ISyntax
{
    Conversion ImplicitConversion { get; }
    DataType? ConvertedDataType { get; }
    ExpressionSemantics? ConvertedSemantics { get; }
}

[Closed(
    typeof(INameExpressionSyntax))]
public partial interface IAssignableExpressionSyntax : IExpressionSyntax
{
}

public partial interface IBlockExpressionSyntax : IExpressionSyntax, IBlockOrResultSyntax, IBodyOrBlockSyntax
{
}

public partial interface INewObjectExpressionSyntax : IExpressionSyntax
{
    ITypeNameSyntax Type { get; }
    SimpleName? ConstructorName { get; }
    TextSpan? ConstructorNameSpan { get; }
    IFixedList<IExpressionSyntax> Arguments { get; }
    Promise<ConstructorSymbol?> ReferencedSymbol { get; }
}

public partial interface IUnsafeExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Expression { get; }
}

[Closed(
    typeof(IBoolLiteralExpressionSyntax),
    typeof(IIntegerLiteralExpressionSyntax),
    typeof(INoneLiteralExpressionSyntax),
    typeof(IStringLiteralExpressionSyntax))]
public partial interface ILiteralExpressionSyntax : IExpressionSyntax
{
}

public partial interface IBoolLiteralExpressionSyntax : ILiteralExpressionSyntax
{
    bool Value { get; }
}

public partial interface IIntegerLiteralExpressionSyntax : ILiteralExpressionSyntax
{
    BigInteger Value { get; }
}

public partial interface INoneLiteralExpressionSyntax : ILiteralExpressionSyntax
{
}

public partial interface IStringLiteralExpressionSyntax : ILiteralExpressionSyntax
{
    string Value { get; }
}

public partial interface IAssignmentExpressionSyntax : IExpressionSyntax
{
    IAssignableExpressionSyntax LeftOperand { get; }
    AssignmentOperator Operator { get; }
    IExpressionSyntax RightOperand { get; }
}

public partial interface IBinaryOperatorExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax LeftOperand { get; }
    BinaryOperator Operator { get; }
    IExpressionSyntax RightOperand { get; }
}

public partial interface IUnaryOperatorExpressionSyntax : IExpressionSyntax
{
    UnaryOperatorFixity Fixity { get; }
    UnaryOperator Operator { get; }
    IExpressionSyntax Operand { get; }
}

public partial interface IIdExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Referent { get; }
}

public partial interface IConversionExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Referent { get; }
    ConversionOperator Operator { get; }
    ITypeSyntax ConvertToType { get; }
}

public partial interface IPatternMatchExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Referent { get; }
    IPatternSyntax Pattern { get; }
}

public partial interface IIfExpressionSyntax : IExpressionSyntax, IElseClauseSyntax
{
    IExpressionSyntax Condition { get; }
    IBlockOrResultSyntax ThenBlock { get; }
    IElseClauseSyntax? ElseClause { get; }
}

public partial interface ILoopExpressionSyntax : IExpressionSyntax
{
    IBlockExpressionSyntax Block { get; }
}

public partial interface IWhileExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Condition { get; }
    IBlockExpressionSyntax Block { get; }
}

public partial interface IForeachExpressionSyntax : IExpressionSyntax, ILocalBindingSyntax
{
    SimpleName VariableName { get; }
    Promise<int?> DeclarationNumber { get; }
    IExpressionSyntax InExpression { get; }
    Promise<MethodSymbol?> IterateMethod { get; }
    Promise<MethodSymbol> NextMethod { get; }
    ITypeSyntax? Type { get; }
    new Promise<VariableSymbol> Symbol { get; }
    IBlockExpressionSyntax Block { get; }
}

public partial interface IBreakExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax? Value { get; }
}

public partial interface INextExpressionSyntax : IExpressionSyntax
{
}

public partial interface IReturnExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax? Value { get; }
}

public partial interface IInvocationExpressionSyntax : IExpressionSyntax, IHasContainingLexicalScope
{
    IExpressionSyntax Expression { get; }
    IFixedList<IExpressionSyntax> Arguments { get; }
    Promise<Symbol?> ReferencedSymbol { get; }
}

[Closed(
    typeof(ISimpleNameExpressionSyntax),
    typeof(IQualifiedNameExpressionSyntax))]
public partial interface INameExpressionSyntax : IAssignableExpressionSyntax
{
    Promise<Symbol?> ReferencedSymbol { get; }
}

[Closed(
    typeof(ISimpleNameExpressionSyntax),
    typeof(ISelfExpressionSyntax))]
public partial interface IVariableNameExpressionSyntax : IExpressionSyntax
{
    IPromise<Symbol?> ReferencedSymbol { get; }
}

public partial interface ISimpleNameExpressionSyntax : INameExpressionSyntax, IVariableNameExpressionSyntax, IHasContainingLexicalScope
{
    SimpleName? Name { get; }
    new Promise<Symbol?> ReferencedSymbol { get; }
}

public partial interface ISelfExpressionSyntax : IVariableNameExpressionSyntax
{
    bool IsImplicit { get; }
    new Promise<SelfParameterSymbol?> ReferencedSymbol { get; }
}

public partial interface IQualifiedNameExpressionSyntax : INameExpressionSyntax
{
    IExpressionSyntax Context { get; }
    AccessOperator AccessOperator { get; }
    ISimpleNameExpressionSyntax Member { get; }
}

public partial interface IMoveExpressionSyntax : IExpressionSyntax
{
    IVariableNameExpressionSyntax Referent { get; }
    Promise<BindingSymbol?> ReferencedSymbol { get; }
}

public partial interface IFreezeExpressionSyntax : IExpressionSyntax
{
    IVariableNameExpressionSyntax Referent { get; }
    Promise<BindingSymbol?> ReferencedSymbol { get; }
}

public partial interface IAsyncBlockExpressionSyntax : IExpressionSyntax
{
    IBlockExpressionSyntax Block { get; }
}

public partial interface IAsyncStartExpressionSyntax : IExpressionSyntax
{
    bool Scheduled { get; }
    IExpressionSyntax Expression { get; }
}

public partial interface IAwaitExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Expression { get; }
}

