using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.AST;

[Closed(
    typeof(IBodyOrBlock),
    typeof(IElseClause),
    typeof(IBinding),
    typeof(IDeclaration),
    typeof(IAttribute),
    typeof(IParameter),
    typeof(IStatement),
    typeof(IPattern),
    typeof(IExpression))]
public partial interface IAbstractSyntax
{
    TextSpan Span { get; }
}

[Closed(
    typeof(IBody),
    typeof(IBlockExpression))]
public partial interface IBodyOrBlock : IAbstractSyntax
{
    IFixedList<IStatement> Statements { get; }
}

[Closed(
    typeof(IBlockOrResult),
    typeof(IIfExpression))]
public partial interface IElseClause : IAbstractSyntax
{
}

[Closed(
    typeof(IResultStatement),
    typeof(IBlockExpression))]
public partial interface IBlockOrResult : IElseClause
{
}

[Closed(
    typeof(ILocalBinding),
    typeof(IFieldDeclaration),
    typeof(IBindingParameter))]
public partial interface IBinding : IAbstractSyntax
{
    BindingSymbol Symbol { get; }
}

[Closed(
    typeof(INamedParameter),
    typeof(IVariableDeclarationStatement),
    typeof(IBindingPattern),
    typeof(IForeachExpression))]
public partial interface ILocalBinding : IBinding
{
    new NamedBindingSymbol Symbol { get; }
}

[Closed(
    typeof(IExecutableDeclaration),
    typeof(IInvocableDeclaration),
    typeof(INonMemberDeclaration),
    typeof(IMemberDeclaration))]
public partial interface IDeclaration : IAbstractSyntax
{
    CodeFile File { get; }
    Symbol Symbol { get; }
    TextSpan NameSpan { get; }
}

[Closed(
    typeof(IConcreteInvocableDeclaration),
    typeof(IFieldDeclaration))]
public partial interface IExecutableDeclaration : IDeclaration
{
}

[Closed(
    typeof(IConcreteInvocableDeclaration))]
public partial interface IInvocableDeclaration : IDeclaration
{
    new InvocableSymbol Symbol { get; }
    IFixedList<IConstructorOrInitializerParameter> Parameters { get; }
}

[Closed(
    typeof(IConcreteFunctionInvocableDeclaration),
    typeof(IConcreteMethodDeclaration),
    typeof(IConstructorDeclaration),
    typeof(IInitializerDeclaration))]
public partial interface IConcreteInvocableDeclaration : IInvocableDeclaration, IExecutableDeclaration
{
    IBody Body { get; }
}

[Closed(
    typeof(IFunctionDeclaration),
    typeof(IAssociatedFunctionDeclaration))]
public partial interface IConcreteFunctionInvocableDeclaration : IConcreteInvocableDeclaration
{
    new FunctionSymbol Symbol { get; }
    new IFixedList<INamedParameter> Parameters { get; }
}

[Closed(
    typeof(ITypeDeclaration),
    typeof(IFunctionDeclaration))]
public partial interface INonMemberDeclaration : IDeclaration
{
}

[Closed(
    typeof(IClassOrStructDeclaration),
    typeof(ITraitDeclaration))]
public partial interface ITypeDeclaration : INonMemberDeclaration
{
    new UserTypeSymbol Symbol { get; }
    IFixedList<ITypeDeclaration> Supertypes { get; }
    IFixedList<IMemberDeclaration> Members { get; }
}

[Closed(
    typeof(IClassDeclaration),
    typeof(IStructDeclaration))]
public partial interface IClassOrStructDeclaration : ITypeDeclaration
{
}

public partial interface IClassDeclaration : IClassOrStructDeclaration
{
    IClassDeclaration? BaseClass { get; }
    new IFixedList<IClassMemberDeclaration> Members { get; }
    ConstructorSymbol? DefaultConstructorSymbol { get; }
}

public partial interface IStructDeclaration : IClassOrStructDeclaration
{
    InitializerSymbol? DefaultInitializerSymbol { get; }
    new IFixedList<IStructMemberDeclaration> Members { get; }
}

public partial interface ITraitDeclaration : ITypeDeclaration
{
    new IFixedList<ITraitMemberDeclaration> Members { get; }
}

public partial interface IFunctionDeclaration : INonMemberDeclaration, IConcreteFunctionInvocableDeclaration
{
    IFixedList<IAttribute> Attributes { get; }
    new FunctionSymbol Symbol { get; }
}

[Closed(
    typeof(IClassMemberDeclaration),
    typeof(IStructMemberDeclaration),
    typeof(ITraitMemberDeclaration))]
public partial interface IMemberDeclaration : IDeclaration
{
    ITypeDeclaration DeclaringType { get; }
}

[Closed(
    typeof(IMethodDeclaration),
    typeof(IConstructorDeclaration),
    typeof(IFieldDeclaration),
    typeof(IAssociatedFunctionDeclaration))]
public partial interface IClassMemberDeclaration : IMemberDeclaration
{
}

[Closed(
    typeof(IConcreteMethodDeclaration),
    typeof(IInitializerDeclaration),
    typeof(IFieldDeclaration),
    typeof(IAssociatedFunctionDeclaration))]
public partial interface IStructMemberDeclaration : IMemberDeclaration
{
}

[Closed(
    typeof(IMethodDeclaration),
    typeof(IAssociatedFunctionDeclaration))]
public partial interface ITraitMemberDeclaration : IMemberDeclaration
{
}

[Closed(
    typeof(IAbstractMethodDeclaration),
    typeof(IConcreteMethodDeclaration))]
public partial interface IMethodDeclaration : IClassMemberDeclaration, ITraitMemberDeclaration
{
    new MethodSymbol Symbol { get; }
    ISelfParameter SelfParameter { get; }
    IFixedList<INamedParameter> Parameters { get; }
}

public partial interface IAbstractMethodDeclaration : IMethodDeclaration
{
}

[Closed(
    typeof(IStandardMethodDeclaration),
    typeof(IGetterMethodDeclaration),
    typeof(ISetterMethodDeclaration))]
public partial interface IConcreteMethodDeclaration : IMethodDeclaration, IStructMemberDeclaration, IConcreteInvocableDeclaration
{
    new MethodSymbol Symbol { get; }
    new IFixedList<INamedParameter> Parameters { get; }
}

public partial interface IStandardMethodDeclaration : IConcreteMethodDeclaration
{
}

public partial interface IGetterMethodDeclaration : IConcreteMethodDeclaration
{
}

public partial interface ISetterMethodDeclaration : IConcreteMethodDeclaration
{
}

public partial interface IConstructorDeclaration : IClassMemberDeclaration, IConcreteInvocableDeclaration
{
    new IClassDeclaration DeclaringType { get; }
    new ConstructorSymbol Symbol { get; }
    ISelfParameter SelfParameter { get; }
}

public partial interface IInitializerDeclaration : IStructMemberDeclaration, IConcreteInvocableDeclaration
{
    new IStructDeclaration DeclaringType { get; }
    new InitializerSymbol Symbol { get; }
    ISelfParameter SelfParameter { get; }
}

public partial interface IFieldDeclaration : IClassMemberDeclaration, IStructMemberDeclaration, IExecutableDeclaration, IBinding
{
    new IClassOrStructDeclaration DeclaringType { get; }
    new FieldSymbol Symbol { get; }
}

public partial interface IAssociatedFunctionDeclaration : IClassMemberDeclaration, IStructMemberDeclaration, ITraitMemberDeclaration, IConcreteFunctionInvocableDeclaration
{
    new FunctionSymbol Symbol { get; }
}

public partial interface IAttribute : IAbstractSyntax
{
    TypeSymbol ReferencedSymbol { get; }
}

[Closed(
    typeof(IConstructorOrInitializerParameter),
    typeof(IBindingParameter),
    typeof(INamedParameter),
    typeof(ISelfParameter),
    typeof(IFieldParameter))]
public partial interface IParameter : IAbstractSyntax
{
    bool Unused { get; }
}

[Closed(
    typeof(INamedParameter),
    typeof(IFieldParameter))]
public partial interface IConstructorOrInitializerParameter : IParameter
{
}

[Closed(
    typeof(INamedParameter),
    typeof(ISelfParameter))]
public partial interface IBindingParameter : IParameter, IBinding
{
}

public partial interface INamedParameter : IParameter, IConstructorOrInitializerParameter, IBindingParameter, ILocalBinding
{
    new VariableSymbol Symbol { get; }
    IExpression? DefaultValue { get; }
}

public partial interface ISelfParameter : IParameter, IBindingParameter
{
    new SelfParameterSymbol Symbol { get; }
}

public partial interface IFieldParameter : IParameter, IConstructorOrInitializerParameter
{
    FieldSymbol ReferencedSymbol { get; }
    IExpression? DefaultValue { get; }
}

public partial interface IBody : IBodyOrBlock
{
    new IFixedList<IBodyStatement> Statements { get; }
}

[Closed(
    typeof(IResultStatement),
    typeof(IBodyStatement))]
public partial interface IStatement : IAbstractSyntax
{
}

public partial interface IResultStatement : IStatement, IBlockOrResult
{
    IExpression Expression { get; }
}

[Closed(
    typeof(IVariableDeclarationStatement),
    typeof(IExpressionStatement))]
public partial interface IBodyStatement : IStatement
{
}

public partial interface IVariableDeclarationStatement : IBodyStatement, ILocalBinding
{
    TextSpan NameSpan { get; }
    new VariableSymbol Symbol { get; }
    IExpression? Initializer { get; }
    Promise<bool> VariableIsLiveAfter { get; }
}

public partial interface IExpressionStatement : IBodyStatement
{
    IExpression Expression { get; }
}

[Closed(
    typeof(IBindingPattern),
    typeof(IBindingContextPattern),
    typeof(IOptionalPattern))]
public partial interface IPattern : IAbstractSyntax
{
}

public partial interface IBindingPattern : IPattern, ILocalBinding
{
    new VariableSymbol Symbol { get; }
    Promise<bool> VariableIsLiveAfter { get; }
}

public partial interface IBindingContextPattern : IPattern
{
    IPattern Pattern { get; }
    DataType Type { get; }
}

public partial interface IOptionalPattern : IPattern
{
    IPattern Pattern { get; }
}

[Closed(
    typeof(INameExpression),
    typeof(IAssignableExpression),
    typeof(IBlockExpression),
    typeof(INewObjectExpression),
    typeof(IUnsafeExpression),
    typeof(ILiteralExpression),
    typeof(IAssignmentExpression),
    typeof(IBinaryOperatorExpression),
    typeof(IUnaryOperatorExpression),
    typeof(IPatternMatchExpression),
    typeof(IIfExpression),
    typeof(ILoopExpression),
    typeof(IWhileExpression),
    typeof(IForeachExpression),
    typeof(IBreakExpression),
    typeof(INextExpression),
    typeof(IReturnExpression),
    typeof(IImplicitConversionExpression),
    typeof(IExplicitConversionExpression),
    typeof(IInvocationExpression),
    typeof(ISelfExpression),
    typeof(IMoveExpression),
    typeof(ITempMoveExpression),
    typeof(IFreezeExpression),
    typeof(ITempFreezeExpression),
    typeof(IIdExpression),
    typeof(IRecoverExpression),
    typeof(IAsyncBlockExpression),
    typeof(IAsyncStartExpression),
    typeof(IAwaitExpression))]
public partial interface IExpression : IAbstractSyntax
{
    DataType DataType { get; }
}

[Closed(
    typeof(IVariableNameExpression),
    typeof(IFunctionNameExpression))]
public partial interface INameExpression : IExpression
{
}

[Closed(
    typeof(IVariableNameExpression),
    typeof(IFieldAccessExpression))]
public partial interface IAssignableExpression : IExpression
{
}

public partial interface IBlockExpression : IExpression, IBlockOrResult, IBodyOrBlock
{
}

public partial interface INewObjectExpression : IExpression
{
    ConstructorSymbol ReferencedSymbol { get; }
    IFixedList<IExpression> Arguments { get; }
}

public partial interface IUnsafeExpression : IExpression
{
    IExpression Expression { get; }
}

[Closed(
    typeof(IBoolLiteralExpression),
    typeof(IIntegerLiteralExpression),
    typeof(INoneLiteralExpression),
    typeof(IStringLiteralExpression))]
public partial interface ILiteralExpression : IExpression
{
}

public partial interface IBoolLiteralExpression : ILiteralExpression
{
    bool Value { get; }
}

public partial interface IIntegerLiteralExpression : ILiteralExpression
{
    BigInteger Value { get; }
}

public partial interface INoneLiteralExpression : ILiteralExpression
{
}

public partial interface IStringLiteralExpression : ILiteralExpression
{
    string Value { get; }
}

public partial interface IAssignmentExpression : IExpression
{
    IAssignableExpression LeftOperand { get; }
    AssignmentOperator Operator { get; }
    IExpression RightOperand { get; }
}

public partial interface IBinaryOperatorExpression : IExpression
{
    IExpression LeftOperand { get; }
    BinaryOperator Operator { get; }
    IExpression RightOperand { get; }
}

public partial interface IUnaryOperatorExpression : IExpression
{
    UnaryOperatorFixity Fixity { get; }
    UnaryOperator Operator { get; }
    IExpression Operand { get; }
}

public partial interface IPatternMatchExpression : IExpression
{
    IExpression Referent { get; }
    IPattern Pattern { get; }
}

public partial interface IIfExpression : IExpression, IElseClause
{
    IExpression Condition { get; }
    IBlockOrResult ThenBlock { get; }
    IElseClause? ElseClause { get; }
}

public partial interface ILoopExpression : IExpression
{
    IBlockExpression Block { get; }
}

public partial interface IWhileExpression : IExpression
{
    IExpression Condition { get; }
    IBlockExpression Block { get; }
}

public partial interface IForeachExpression : IExpression, ILocalBinding
{
    new VariableSymbol Symbol { get; }
    IExpression InExpression { get; }
    MethodSymbol? IterateMethod { get; }
    MethodSymbol NextMethod { get; }
    IBlockExpression Block { get; }
    Promise<bool> VariableIsLiveAfterAssignment { get; }
}

public partial interface IBreakExpression : IExpression
{
    IExpression? Value { get; }
}

public partial interface INextExpression : IExpression
{
}

public partial interface IReturnExpression : IExpression
{
    IExpression? Value { get; }
}

[Closed(
    typeof(IImplicitSimpleTypeConversionExpression),
    typeof(IImplicitOptionalConversionExpression),
    typeof(IImplicitLiftedConversionExpression))]
public partial interface IImplicitConversionExpression : IExpression
{
    IExpression Expression { get; }
}

public partial interface IImplicitSimpleTypeConversionExpression : IImplicitConversionExpression
{
    SimpleType ConvertToType { get; }
}

public partial interface IImplicitOptionalConversionExpression : IImplicitConversionExpression
{
    OptionalType ConvertToType { get; }
}

public partial interface IImplicitLiftedConversionExpression : IImplicitConversionExpression
{
    OptionalType ConvertToType { get; }
}

[Closed(
    typeof(IExplicitSimpleTypeConversionExpression))]
public partial interface IExplicitConversionExpression : IExpression
{
    IExpression Expression { get; }
    bool IsOptional { get; }
}

public partial interface IExplicitSimpleTypeConversionExpression : IExplicitConversionExpression
{
    SimpleType ConvertToType { get; }
}

[Closed(
    typeof(IFunctionInvocationExpression),
    typeof(IInitializerInvocationExpression),
    typeof(IMethodInvocationExpression),
    typeof(IFunctionReferenceInvocationExpression))]
public partial interface IInvocationExpression : IExpression
{
    IFixedList<IExpression> Arguments { get; }
}

public partial interface IFunctionInvocationExpression : IInvocationExpression
{
    FunctionSymbol ReferencedSymbol { get; }
}

public partial interface IInitializerInvocationExpression : IInvocationExpression
{
    InitializerSymbol ReferencedSymbol { get; }
}

public partial interface IMethodInvocationExpression : IInvocationExpression
{
    IExpression Context { get; }
    MethodSymbol ReferencedSymbol { get; }
}

public partial interface IFunctionReferenceInvocationExpression : IInvocationExpression
{
    IExpression Referent { get; }
}

public partial interface IVariableNameExpression : IAssignableExpression, INameExpression
{
    NamedBindingSymbol ReferencedSymbol { get; }
    bool IsMove { get; }
    Promise<bool> VariableIsLiveAfter { get; }
}

public partial interface ISelfExpression : IExpression
{
    SelfParameterSymbol ReferencedSymbol { get; }
    bool IsImplicit { get; }
}

public partial interface IFieldAccessExpression : IAssignableExpression
{
    IExpression Context { get; }
    AccessOperator AccessOperator { get; }
    FieldSymbol ReferencedSymbol { get; }
}

public partial interface IMoveExpression : IExpression
{
    BindingSymbol ReferencedSymbol { get; }
    IExpression Referent { get; }
}

public partial interface ITempMoveExpression : IExpression
{
    IExpression Referent { get; }
}

public partial interface IFreezeExpression : IExpression
{
    BindingSymbol ReferencedSymbol { get; }
    IExpression Referent { get; }
}

public partial interface ITempFreezeExpression : IExpression
{
    IExpression Referent { get; }
}

public partial interface IIdExpression : IExpression
{
    IExpression Referent { get; }
}

[Closed(
    typeof(IRecoverConstExpression),
    typeof(IRecoverIsolationExpression))]
public partial interface IRecoverExpression : IExpression
{
    IExpression Value { get; }
}

public partial interface IRecoverConstExpression : IRecoverExpression
{
}

public partial interface IRecoverIsolationExpression : IRecoverExpression
{
}

public partial interface IAsyncBlockExpression : IExpression
{
    IBlockExpression Block { get; }
}

public partial interface IAsyncStartExpression : IExpression
{
    bool Scheduled { get; }
    IExpression Expression { get; }
}

public partial interface IAwaitExpression : IExpression
{
    IExpression Expression { get; }
}

public partial interface IFunctionNameExpression : INameExpression
{
    FunctionSymbol ReferencedSymbol { get; }
}

