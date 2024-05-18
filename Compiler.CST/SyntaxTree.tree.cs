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
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
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
    typeof(IDefinitionSyntax),
    typeof(ITypeDefinitionSyntax),
    typeof(IGenericParameterSyntax),
    typeof(IAttributeSyntax),
    typeof(ICapabilityConstraintSyntax),
    typeof(IParameterSyntax),
    typeof(IReturnSyntax),
    typeof(ITypeSyntax),
    typeof(IParameterTypeSyntax),
    typeof(IReturnTypeSyntax),
    typeof(IStatementSyntax),
    typeof(IPatternSyntax),
    typeof(IExpressionSyntax))]
public partial interface IConcreteSyntax
{
    TextSpan Span { get; }
}

public partial interface ICompilationUnitSyntax : IConcreteSyntax
{
    CodeFile File { get; }
    NamespaceName ImplicitNamespaceName { get; }
    IFixedList<Diagnostic> Diagnostics { get; }
    IFixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
    IFixedList<INonMemberDefinitionSyntax> Definitions { get; }
}

public partial interface IUsingDirectiveSyntax : IConcreteSyntax
{
    NamespaceName Name { get; }
}

[Closed(
    typeof(IBodySyntax),
    typeof(IBlockExpressionSyntax))]
public partial interface IBodyOrBlockSyntax : IConcreteSyntax
{
    IFixedList<IStatementSyntax> Statements { get; }
}

[Closed(
    typeof(IBlockOrResultSyntax),
    typeof(IIfExpressionSyntax))]
public partial interface IElseClauseSyntax : IConcreteSyntax
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
    typeof(IFieldDefinitionSyntax))]
public partial interface IBindingSyntax : IConcreteSyntax
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
    IPromise<BindingSymbol> IBindingSyntax.Symbol => Symbol;
}

[Closed(
    typeof(IEntityDefinitionSyntax),
    typeof(INonMemberDefinitionSyntax))]
public partial interface IDefinitionSyntax : IHasContainingLexicalScope, IConcreteSyntax
{
    CodeFile File { get; }
    TypeName? Name { get; }
    TextSpan NameSpan { get; }
    IPromise<Symbol> Symbol { get; }
}

[Closed(
    typeof(IInvocableDefinitionSyntax),
    typeof(INonMemberEntityDefinitionSyntax),
    typeof(ITypeMemberDefinitionSyntax))]
public partial interface IEntityDefinitionSyntax : IDefinitionSyntax
{
    IAccessModifierToken? AccessModifier { get; }
}

[Closed(
    typeof(IConcreteInvocableDefinitionSyntax),
    typeof(IMethodDefinitionSyntax))]
public partial interface IInvocableDefinitionSyntax : IEntityDefinitionSyntax
{
    IFixedList<IConstructorOrInitializerParameterSyntax> Parameters { get; }
    new IPromise<InvocableSymbol> Symbol { get; }
    IPromise<Symbol> IDefinitionSyntax.Symbol => Symbol;
}

[Closed(
    typeof(IFunctionDefinitionSyntax),
    typeof(IConcreteMethodDefinitionSyntax),
    typeof(IConstructorDefinitionSyntax),
    typeof(IInitializerDefinitionSyntax),
    typeof(IAssociatedFunctionDefinitionSyntax))]
public partial interface IConcreteInvocableDefinitionSyntax : IInvocableDefinitionSyntax
{
    IBodySyntax Body { get; }
}

[Closed(
    typeof(INamespaceDefinitionSyntax),
    typeof(INonMemberEntityDefinitionSyntax))]
public partial interface INonMemberDefinitionSyntax : IDefinitionSyntax
{
    NamespaceName ContainingNamespaceName { get; }
}

public partial interface INamespaceDefinitionSyntax : INonMemberDefinitionSyntax
{
    bool IsGlobalQualified { get; }
    NamespaceName DeclaredNames { get; }
    NamespaceName FullName { get; }
    new Promise<NamespaceSymbol> Symbol { get; }
    IPromise<Symbol> IDefinitionSyntax.Symbol => Symbol;
    IFixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
    IFixedList<INonMemberDefinitionSyntax> Definitions { get; }
}

public partial interface IFunctionDefinitionSyntax : INonMemberEntityDefinitionSyntax, IConcreteInvocableDefinitionSyntax
{
    IFixedList<IAttributeSyntax> Attributes { get; }
    new IdentifierName Name { get; }
    TypeName INonMemberEntityDefinitionSyntax.Name => Name;
    TypeName? IDefinitionSyntax.Name => Name;
    new IFixedList<INamedParameterSyntax> Parameters { get; }
    IReturnSyntax? Return { get; }
    new AcyclicPromise<FunctionSymbol> Symbol { get; }
    IPromise<Symbol> IDefinitionSyntax.Symbol => Symbol;
    IPromise<InvocableSymbol> IInvocableDefinitionSyntax.Symbol => Symbol;
}

[Closed(
    typeof(IFunctionDefinitionSyntax),
    typeof(ITypeDefinitionSyntax))]
public partial interface INonMemberEntityDefinitionSyntax : IEntityDefinitionSyntax, INonMemberDefinitionSyntax
{
    new TypeName Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
}

[Closed(
    typeof(IClassOrStructDefinitionSyntax),
    typeof(ITraitDefinitionSyntax))]
public partial interface ITypeDefinitionSyntax : IConcreteSyntax, INonMemberEntityDefinitionSyntax, IClassMemberDefinitionSyntax, ITraitMemberDefinitionSyntax, IStructMemberDefinitionSyntax
{
    IConstKeywordToken? ConstModifier { get; }
    bool IsConst { get; }
    IMoveKeywordToken? MoveModifier { get; }
    bool IsMove { get; }
    new StandardName Name { get; }
    TypeName INonMemberEntityDefinitionSyntax.Name => Name;
    TypeName? IDefinitionSyntax.Name => Name;
    IFixedList<IGenericParameterSyntax> GenericParameters { get; }
    new AcyclicPromise<UserTypeSymbol> Symbol { get; }
    IPromise<Symbol> IDefinitionSyntax.Symbol => Symbol;
    IFixedList<IStandardTypeNameSyntax> SupertypeNames { get; }
    IFixedList<ITypeMemberDefinitionSyntax> Members { get; }
}

[Closed(
    typeof(IClassDefinitionSyntax),
    typeof(IStructDefinitionSyntax))]
public partial interface IClassOrStructDefinitionSyntax : ITypeDefinitionSyntax
{
}

public partial interface IClassDefinitionSyntax : IClassOrStructDefinitionSyntax
{
    IAbstractKeywordToken? AbstractModifier { get; }
    bool IsAbstract { get; }
    IStandardTypeNameSyntax? BaseTypeName { get; }
    ConstructorSymbol? DefaultConstructorSymbol { get; }
    new IFixedList<IClassMemberDefinitionSyntax> Members { get; }
    IFixedList<ITypeMemberDefinitionSyntax> ITypeDefinitionSyntax.Members => Members;
}

public partial interface IStructDefinitionSyntax : IClassOrStructDefinitionSyntax
{
    InitializerSymbol? DefaultInitializerSymbol { get; }
    new IFixedList<IStructMemberDefinitionSyntax> Members { get; }
    IFixedList<ITypeMemberDefinitionSyntax> ITypeDefinitionSyntax.Members => Members;
}

public partial interface ITraitDefinitionSyntax : ITypeDefinitionSyntax
{
    new IFixedList<ITraitMemberDefinitionSyntax> Members { get; }
    IFixedList<ITypeMemberDefinitionSyntax> ITypeDefinitionSyntax.Members => Members;
}

public partial interface IGenericParameterSyntax : IConcreteSyntax
{
    ICapabilityConstraintSyntax Constraint { get; }
    IdentifierName Name { get; }
    ParameterIndependence Independence { get; }
    ParameterVariance Variance { get; }
    Promise<GenericParameterTypeSymbol> Symbol { get; }
}

[Closed(
    typeof(IClassMemberDefinitionSyntax),
    typeof(ITraitMemberDefinitionSyntax),
    typeof(IStructMemberDefinitionSyntax),
    typeof(IAlwaysTypeMemberDefinitionSyntax))]
public partial interface ITypeMemberDefinitionSyntax : IEntityDefinitionSyntax
{
    ITypeDefinitionSyntax? DefiningType { get; }
}

[Closed(
    typeof(ITypeDefinitionSyntax),
    typeof(IMethodDefinitionSyntax),
    typeof(IConstructorDefinitionSyntax),
    typeof(IFieldDefinitionSyntax),
    typeof(IAssociatedFunctionDefinitionSyntax))]
public partial interface IClassMemberDefinitionSyntax : ITypeMemberDefinitionSyntax
{
}

[Closed(
    typeof(ITypeDefinitionSyntax),
    typeof(IMethodDefinitionSyntax),
    typeof(IAssociatedFunctionDefinitionSyntax))]
public partial interface ITraitMemberDefinitionSyntax : ITypeMemberDefinitionSyntax
{
}

[Closed(
    typeof(ITypeDefinitionSyntax),
    typeof(IConcreteMethodDefinitionSyntax),
    typeof(IInitializerDefinitionSyntax),
    typeof(IFieldDefinitionSyntax),
    typeof(IAssociatedFunctionDefinitionSyntax))]
public partial interface IStructMemberDefinitionSyntax : ITypeMemberDefinitionSyntax
{
}

[Closed(
    typeof(IMethodDefinitionSyntax),
    typeof(IConstructorDefinitionSyntax),
    typeof(IInitializerDefinitionSyntax),
    typeof(IFieldDefinitionSyntax),
    typeof(IAssociatedFunctionDefinitionSyntax))]
public partial interface IAlwaysTypeMemberDefinitionSyntax : ITypeMemberDefinitionSyntax
{
    new ITypeDefinitionSyntax DefiningType { get; }
    ITypeDefinitionSyntax? ITypeMemberDefinitionSyntax.DefiningType => DefiningType;
}

[Closed(
    typeof(IAbstractMethodDefinitionSyntax),
    typeof(IConcreteMethodDefinitionSyntax))]
public partial interface IMethodDefinitionSyntax : IAlwaysTypeMemberDefinitionSyntax, IClassMemberDefinitionSyntax, ITraitMemberDefinitionSyntax, IInvocableDefinitionSyntax
{
    MethodKind Kind { get; }
    new IdentifierName Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    IMethodSelfParameterSyntax SelfParameter { get; }
    new IFixedList<INamedParameterSyntax> Parameters { get; }
    IFixedList<IConstructorOrInitializerParameterSyntax> IInvocableDefinitionSyntax.Parameters => Parameters;
    IReturnSyntax? Return { get; }
    new AcyclicPromise<MethodSymbol> Symbol { get; }
    IPromise<Symbol> IDefinitionSyntax.Symbol => Symbol;
    IPromise<InvocableSymbol> IInvocableDefinitionSyntax.Symbol => Symbol;
}

public partial interface IAbstractMethodDefinitionSyntax : IMethodDefinitionSyntax
{
}

[Closed(
    typeof(IStandardMethodDefinitionSyntax),
    typeof(IGetterMethodDefinitionSyntax),
    typeof(ISetterMethodDefinitionSyntax))]
public partial interface IConcreteMethodDefinitionSyntax : IMethodDefinitionSyntax, IStructMemberDefinitionSyntax, IConcreteInvocableDefinitionSyntax
{
    new IFixedList<INamedParameterSyntax> Parameters { get; }
    IFixedList<INamedParameterSyntax> IMethodDefinitionSyntax.Parameters => Parameters;
    IFixedList<IConstructorOrInitializerParameterSyntax> IInvocableDefinitionSyntax.Parameters => Parameters;
}

public partial interface IStandardMethodDefinitionSyntax : IConcreteMethodDefinitionSyntax
{
}

public partial interface IGetterMethodDefinitionSyntax : IConcreteMethodDefinitionSyntax
{
    new IReturnSyntax Return { get; }
}

public partial interface ISetterMethodDefinitionSyntax : IConcreteMethodDefinitionSyntax
{
}

public partial interface IConstructorDefinitionSyntax : IConcreteInvocableDefinitionSyntax, IAlwaysTypeMemberDefinitionSyntax, IClassMemberDefinitionSyntax
{
    new IClassDefinitionSyntax DefiningType { get; }
    ITypeDefinitionSyntax IAlwaysTypeMemberDefinitionSyntax.DefiningType => DefiningType;
    ITypeDefinitionSyntax? ITypeMemberDefinitionSyntax.DefiningType => DefiningType;
    new IdentifierName? Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    IConstructorSelfParameterSyntax SelfParameter { get; }
    new IBlockBodySyntax Body { get; }
    IBodySyntax IConcreteInvocableDefinitionSyntax.Body => Body;
    new AcyclicPromise<ConstructorSymbol> Symbol { get; }
    IPromise<InvocableSymbol> IInvocableDefinitionSyntax.Symbol => Symbol;
    IPromise<Symbol> IDefinitionSyntax.Symbol => Symbol;
}

public partial interface IInitializerDefinitionSyntax : IConcreteInvocableDefinitionSyntax, IAlwaysTypeMemberDefinitionSyntax, IStructMemberDefinitionSyntax
{
    new IStructDefinitionSyntax DefiningType { get; }
    ITypeDefinitionSyntax IAlwaysTypeMemberDefinitionSyntax.DefiningType => DefiningType;
    ITypeDefinitionSyntax? ITypeMemberDefinitionSyntax.DefiningType => DefiningType;
    new IdentifierName? Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    IInitializerSelfParameterSyntax SelfParameter { get; }
    new IBlockBodySyntax Body { get; }
    IBodySyntax IConcreteInvocableDefinitionSyntax.Body => Body;
    new AcyclicPromise<InitializerSymbol> Symbol { get; }
    IPromise<InvocableSymbol> IInvocableDefinitionSyntax.Symbol => Symbol;
    IPromise<Symbol> IDefinitionSyntax.Symbol => Symbol;
}

public partial interface IFieldDefinitionSyntax : IAlwaysTypeMemberDefinitionSyntax, IClassMemberDefinitionSyntax, IStructMemberDefinitionSyntax, IBindingSyntax
{
    new IClassOrStructDefinitionSyntax DefiningType { get; }
    ITypeDefinitionSyntax IAlwaysTypeMemberDefinitionSyntax.DefiningType => DefiningType;
    ITypeDefinitionSyntax? ITypeMemberDefinitionSyntax.DefiningType => DefiningType;
    new IdentifierName Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    ITypeSyntax Type { get; }
    new AcyclicPromise<FieldSymbol> Symbol { get; }
    IPromise<Symbol> IDefinitionSyntax.Symbol => Symbol;
    IPromise<BindingSymbol> IBindingSyntax.Symbol => Symbol;
    IExpressionSyntax? Initializer { get; }
}

public partial interface IAssociatedFunctionDefinitionSyntax : IAlwaysTypeMemberDefinitionSyntax, IClassMemberDefinitionSyntax, ITraitMemberDefinitionSyntax, IStructMemberDefinitionSyntax, IConcreteInvocableDefinitionSyntax
{
    new IdentifierName Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    new IFixedList<INamedParameterSyntax> Parameters { get; }
    IReturnSyntax? Return { get; }
    new AcyclicPromise<FunctionSymbol> Symbol { get; }
    IPromise<Symbol> IDefinitionSyntax.Symbol => Symbol;
    IPromise<InvocableSymbol> IInvocableDefinitionSyntax.Symbol => Symbol;
}

public partial interface IAttributeSyntax : IConcreteSyntax
{
    IStandardTypeNameSyntax TypeName { get; }
}

[Closed(
    typeof(ICapabilitySetSyntax),
    typeof(ICapabilitySyntax))]
public partial interface ICapabilityConstraintSyntax : IConcreteSyntax
{
    ICapabilityConstraint Constraint { get; }
}

public partial interface ICapabilitySetSyntax : ICapabilityConstraintSyntax
{
    new CapabilitySet Constraint { get; }
    ICapabilityConstraint ICapabilityConstraintSyntax.Constraint => Constraint;
}

public partial interface ICapabilitySyntax : ICapabilityConstraintSyntax
{
    IFixedList<ICapabilityToken> Tokens { get; }
    DeclaredCapability Declared { get; }
    Capability Capability { get; }
}

[Closed(
    typeof(IConstructorOrInitializerParameterSyntax),
    typeof(ISelfParameterSyntax))]
public partial interface IParameterSyntax : IConcreteSyntax
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
    IdentifierName? IParameterSyntax.Name => Name;
    Promise<int?> DeclarationNumber { get; }
    ITypeSyntax Type { get; }
    new IPromise<DataType> DataType { get; }
    IPromise<Pseudotype> IParameterSyntax.DataType => DataType;
    new Promise<NamedVariableSymbol> Symbol { get; }
    IPromise<NamedBindingSymbol> ILocalBindingSyntax.Symbol => Symbol;
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
    IPromise<Pseudotype> IParameterSyntax.DataType => DataType;
}

public partial interface IInitializerSelfParameterSyntax : ISelfParameterSyntax
{
    ICapabilitySyntax Capability { get; }
    new IPromise<DataType> DataType { get; }
    IPromise<Pseudotype> IParameterSyntax.DataType => DataType;
}

public partial interface IMethodSelfParameterSyntax : ISelfParameterSyntax
{
    ICapabilityConstraintSyntax Capability { get; }
}

public partial interface IFieldParameterSyntax : IConstructorOrInitializerParameterSyntax
{
    new IdentifierName Name { get; }
    IdentifierName? IParameterSyntax.Name => Name;
    Promise<FieldSymbol?> ReferencedSymbol { get; }
    IExpressionSyntax? DefaultValue { get; }
}

public partial interface IReturnSyntax : IConcreteSyntax
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
    IFixedList<IStatementSyntax> IBodyOrBlockSyntax.Statements => Statements;
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
public partial interface ITypeSyntax : IConcreteSyntax
{
}

[Closed(
    typeof(IStandardTypeNameSyntax),
    typeof(ISimpleTypeNameSyntax),
    typeof(IQualifiedTypeNameSyntax))]
public partial interface ITypeNameSyntax : ITypeSyntax
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
    TypeName ITypeNameSyntax.Name => Name;
    BareReferenceType? NamedBareType { get; }
}

[Closed(
    typeof(IIdentifierTypeNameSyntax),
    typeof(ISpecialTypeNameSyntax))]
public partial interface ISimpleTypeNameSyntax : ITypeNameSyntax
{
}

public partial interface IIdentifierTypeNameSyntax : IStandardTypeNameSyntax, ISimpleTypeNameSyntax
{
    new IdentifierName Name { get; }
    StandardName IStandardTypeNameSyntax.Name => Name;
    TypeName ITypeNameSyntax.Name => Name;
}

public partial interface ISpecialTypeNameSyntax : ISimpleTypeNameSyntax
{
    new SpecialTypeName Name { get; }
    TypeName ITypeNameSyntax.Name => Name;
}

public partial interface IGenericTypeNameSyntax : IStandardTypeNameSyntax
{
    new GenericName Name { get; }
    StandardName IStandardTypeNameSyntax.Name => Name;
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

public partial interface IFunctionTypeSyntax : ITypeSyntax
{
    IFixedList<IParameterTypeSyntax> Parameters { get; }
    IReturnTypeSyntax Return { get; }
}

public partial interface IParameterTypeSyntax : IConcreteSyntax
{
    bool IsLent { get; }
    ITypeSyntax Referent { get; }
}

public partial interface IReturnTypeSyntax : IConcreteSyntax
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
public partial interface IStatementSyntax : IConcreteSyntax
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
    IPromise<NamedBindingSymbol> ILocalBindingSyntax.Symbol => Symbol;
    IExpressionSyntax? Initializer { get; }
}

public partial interface IExpressionStatementSyntax : IBodyStatementSyntax
{
    IExpressionSyntax Expression { get; }
}

[Closed(
    typeof(IBindingContextPatternSyntax),
    typeof(IOptionalOrBindingPatternSyntax))]
public partial interface IPatternSyntax : IConcreteSyntax
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
    IPromise<NamedBindingSymbol> ILocalBindingSyntax.Symbol => Symbol;
}

public partial interface IOptionalPatternSyntax : IOptionalOrBindingPatternSyntax
{
    IOptionalOrBindingPatternSyntax Pattern { get; }
}

[Closed(
    typeof(ITypedExpressionSyntax),
    typeof(INameExpressionSyntax))]
public partial interface IExpressionSyntax : IConcreteSyntax
{
    IPromise<DataType?> DataType { get; }
    Conversion ImplicitConversion { get; }
    DataType? ConvertedDataType { get; }
}

[Closed(
    typeof(IDataTypedExpressionSyntax),
    typeof(IAssignableExpressionSyntax),
    typeof(INeverTypedExpressionSyntax),
    typeof(ILiteralExpressionSyntax),
    typeof(ISelfExpressionSyntax))]
public partial interface ITypedExpressionSyntax : IExpressionSyntax
{
    new IPromise<DataType> DataType { get; }
    IPromise<DataType?> IExpressionSyntax.DataType => DataType;
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
    IPromise<DataType> ITypedExpressionSyntax.DataType => DataType;
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
    Promise<DataType> IDataTypedExpressionSyntax.DataType => DataType;
    IPromise<DataType?> IBlockOrResultSyntax.DataType => DataType;
    IPromise<DataType> ITypedExpressionSyntax.DataType => DataType;
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
    typeof(IBreakExpressionSyntax),
    typeof(INextExpressionSyntax),
    typeof(IReturnExpressionSyntax))]
public partial interface INeverTypedExpressionSyntax : ITypedExpressionSyntax
{
    new Promise<NeverType> DataType { get; }
    IPromise<DataType> ITypedExpressionSyntax.DataType => DataType;
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
    IPromise<DataType> ITypedExpressionSyntax.DataType => DataType;
}

public partial interface IIntegerLiteralExpressionSyntax : ILiteralExpressionSyntax
{
    BigInteger Value { get; }
    new Promise<IntegerConstValueType> DataType { get; }
    IPromise<DataType> ITypedExpressionSyntax.DataType => DataType;
}

public partial interface INoneLiteralExpressionSyntax : ILiteralExpressionSyntax
{
    new Promise<OptionalType> DataType { get; }
    IPromise<DataType> ITypedExpressionSyntax.DataType => DataType;
}

public partial interface IStringLiteralExpressionSyntax : ILiteralExpressionSyntax
{
    string Value { get; }
    new Promise<DataType> DataType { get; }
    IPromise<DataType> ITypedExpressionSyntax.DataType => DataType;
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
    IPromise<NamedBindingSymbol> ILocalBindingSyntax.Symbol => Symbol;
    IBlockExpressionSyntax Block { get; }
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

public partial interface IInvocationExpressionSyntax : IHasContainingLexicalScope, IDataTypedExpressionSyntax
{
    IExpressionSyntax Expression { get; }
    IFixedList<IExpressionSyntax> Arguments { get; }
    Promise<Symbol?> ReferencedSymbol { get; }
}

[Closed(
    typeof(IVariableNameExpressionSyntax),
    typeof(IStandardNameExpressionSyntax),
    typeof(ISpecialTypeNameExpressionSyntax),
    typeof(IMemberAccessExpressionSyntax))]
public partial interface INameExpressionSyntax : IExpressionSyntax
{
    IPromise<ISyntaxSemantics> Semantics { get; }
    IPromise<Symbol?> ReferencedSymbol { get; }
}

[Closed(
    typeof(IIdentifierNameExpressionSyntax),
    typeof(ISelfExpressionSyntax))]
public partial interface IVariableNameExpressionSyntax : INameExpressionSyntax
{
    new IPromise<IVariableNameExpressionSyntaxSemantics> Semantics { get; }
    IPromise<ISyntaxSemantics> INameExpressionSyntax.Semantics => Semantics;
}

[Closed(
    typeof(IIdentifierNameExpressionSyntax),
    typeof(IGenericNameExpressionSyntax))]
public partial interface IStandardNameExpressionSyntax : IHasContainingLexicalScope, INameExpressionSyntax
{
    StandardName? Name { get; }
}

public partial interface IIdentifierNameExpressionSyntax : IStandardNameExpressionSyntax, IVariableNameExpressionSyntax, IAssignableExpressionSyntax
{
    new IdentifierName? Name { get; }
    StandardName? IStandardNameExpressionSyntax.Name => Name;
    new Promise<IIdentifierNameExpressionSyntaxSemantics> Semantics { get; }
    IPromise<ISyntaxSemantics> INameExpressionSyntax.Semantics => Semantics;
    IPromise<IVariableNameExpressionSyntaxSemantics> IVariableNameExpressionSyntax.Semantics => Semantics;
}

public partial interface ISpecialTypeNameExpressionSyntax : INameExpressionSyntax
{
    SpecialTypeName Name { get; }
    new Promise<SpecialTypeNameExpressionSyntaxSemantics> Semantics { get; }
    IPromise<ISyntaxSemantics> INameExpressionSyntax.Semantics => Semantics;
    new Promise<DataType?> DataType { get; }
    IPromise<DataType?> IExpressionSyntax.DataType => DataType;
    new Promise<TypeSymbol?> ReferencedSymbol { get; }
    IPromise<Symbol?> INameExpressionSyntax.ReferencedSymbol => ReferencedSymbol;
}

public partial interface IGenericNameExpressionSyntax : IStandardNameExpressionSyntax
{
    new GenericName Name { get; }
    StandardName? IStandardNameExpressionSyntax.Name => Name;
    IFixedList<ITypeSyntax> TypeArguments { get; }
    new Promise<DataType?> DataType { get; }
    IPromise<DataType?> IExpressionSyntax.DataType => DataType;
}

public partial interface ISelfExpressionSyntax : IVariableNameExpressionSyntax, ITypedExpressionSyntax
{
    bool IsImplicit { get; }
    new Promise<ISelfExpressionSyntaxSemantics> Semantics { get; }
    IPromise<IVariableNameExpressionSyntaxSemantics> IVariableNameExpressionSyntax.Semantics => Semantics;
    new IPromise<SelfParameterSymbol?> ReferencedSymbol { get; }
    IPromise<Symbol?> INameExpressionSyntax.ReferencedSymbol => ReferencedSymbol;
    IPromise<Pseudotype> Pseudotype { get; }
}

public partial interface IMemberAccessExpressionSyntax : INameExpressionSyntax, IAssignableExpressionSyntax
{
    IExpressionSyntax Context { get; }
    AccessOperator AccessOperator { get; }
    StandardName MemberName { get; }
    IFixedList<ITypeSyntax> TypeArguments { get; }
    TextSpan MemberNameSpan { get; }
    new Promise<IMemberAccessSyntaxSemantics> Semantics { get; }
    IPromise<ISyntaxSemantics> INameExpressionSyntax.Semantics => Semantics;
    new IPromise<Symbol?> ReferencedSymbol { get; }
    IPromise<Symbol?> INameExpressionSyntax.ReferencedSymbol => ReferencedSymbol;
    IPromise<Symbol?> IAssignableExpressionSyntax.ReferencedSymbol => ReferencedSymbol;
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

