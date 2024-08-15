using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

// ReSharper disable PartialTypeWithSinglePart

[Closed(
    typeof(ICodeSyntax),
    typeof(IPackageSyntax),
    typeof(IPackageReferenceSyntax))]
public partial interface ISyntax
{
}

[Closed(
    typeof(ICompilationUnitSyntax),
    typeof(IUsingDirectiveSyntax),
    typeof(IBodyOrBlockSyntax),
    typeof(IElseClauseSyntax),
    typeof(IBindingSyntax),
    typeof(IDefinitionSyntax),
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
public partial interface ICodeSyntax : ISyntax
{
    TextSpan Span { get; }
}

public partial interface ICompilationUnitSyntax : ICodeSyntax
{
    CodeFile File { get; }
    NamespaceName ImplicitNamespaceName { get; }
    DiagnosticCollection Diagnostics { get; }
    IFixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
    IFixedList<INonMemberDefinitionSyntax> Definitions { get; }
}

public partial interface IUsingDirectiveSyntax : ICodeSyntax
{
    NamespaceName Name { get; }
}

[Closed(
    typeof(IBodySyntax),
    typeof(IBlockExpressionSyntax))]
public partial interface IBodyOrBlockSyntax : ICodeSyntax
{
    IFixedList<IStatementSyntax> Statements { get; }
}

[Closed(
    typeof(IBlockOrResultSyntax),
    typeof(IIfExpressionSyntax))]
public partial interface IElseClauseSyntax : ICodeSyntax
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
    typeof(IFieldDefinitionSyntax))]
public partial interface IBindingSyntax : ICodeSyntax
{
    bool IsMutableBinding { get; }
}

[Closed(
    typeof(INamedParameterSyntax),
    typeof(IVariableDeclarationStatementSyntax),
    typeof(IBindingPatternSyntax),
    typeof(IForeachExpressionSyntax))]
public partial interface ILocalBindingSyntax : IBindingSyntax
{
    TextSpan NameSpan { get; }
}

public partial interface IPackageSyntax : ISyntax
{
    IdentifierName Name { get; }
    IFixedSet<ICompilationUnitSyntax> CompilationUnits { get; }
    IFixedSet<ICompilationUnitSyntax> TestingCompilationUnits { get; }
    IFixedSet<IPackageReferenceSyntax> References { get; }
    DiagnosticCollection Diagnostics { get; }
}

public partial interface IPackageReferenceSyntax : ISyntax
{
    IdentifierName AliasOrName { get; }
    IPackageSymbols Package { get; }
    bool IsTrusted { get; }
}

[Closed(
    typeof(IEntityDefinitionSyntax),
    typeof(INonMemberDefinitionSyntax))]
public partial interface IDefinitionSyntax : ICodeSyntax
{
    CodeFile File { get; }
    TypeName? Name { get; }
    TextSpan NameSpan { get; }
}

[Closed(
    typeof(IInvocableDefinitionSyntax),
    typeof(ITypeDefinitionSyntax),
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
    typeof(IFunctionDefinitionSyntax),
    typeof(ITypeDefinitionSyntax))]
public partial interface INonMemberDefinitionSyntax : IDefinitionSyntax
{
}

public partial interface INamespaceDefinitionSyntax : INonMemberDefinitionSyntax
{
    bool IsGlobalQualified { get; }
    NamespaceName DeclaredNames { get; }
    IFixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
    IFixedList<INonMemberDefinitionSyntax> Definitions { get; }
}

public partial interface IFunctionDefinitionSyntax : IConcreteInvocableDefinitionSyntax, INonMemberDefinitionSyntax
{
    IFixedList<IAttributeSyntax> Attributes { get; }
    new IdentifierName Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    new IFixedList<INamedParameterSyntax> Parameters { get; }
    IReturnSyntax? Return { get; }
}

[Closed(
    typeof(IClassDefinitionSyntax),
    typeof(IStructDefinitionSyntax),
    typeof(ITraitDefinitionSyntax))]
public partial interface ITypeDefinitionSyntax : IEntityDefinitionSyntax, INonMemberDefinitionSyntax, IClassMemberDefinitionSyntax, ITraitMemberDefinitionSyntax, IStructMemberDefinitionSyntax
{
    IConstKeywordToken? ConstModifier { get; }
    IMoveKeywordToken? MoveModifier { get; }
    new StandardName Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    IFixedList<IGenericParameterSyntax> GenericParameters { get; }
    IFixedList<IStandardTypeNameSyntax> SupertypeNames { get; }
    IFixedList<ITypeMemberDefinitionSyntax> Members { get; }
}

public partial interface IClassDefinitionSyntax : ITypeDefinitionSyntax
{
    IAbstractKeywordToken? AbstractModifier { get; }
    IStandardTypeNameSyntax? BaseTypeName { get; }
    new IFixedList<IClassMemberDefinitionSyntax> Members { get; }
    IFixedList<ITypeMemberDefinitionSyntax> ITypeDefinitionSyntax.Members => Members;
}

public partial interface IStructDefinitionSyntax : ITypeDefinitionSyntax
{
    new IFixedList<IStructMemberDefinitionSyntax> Members { get; }
    IFixedList<ITypeMemberDefinitionSyntax> ITypeDefinitionSyntax.Members => Members;
}

public partial interface ITraitDefinitionSyntax : ITypeDefinitionSyntax
{
    new IFixedList<ITraitMemberDefinitionSyntax> Members { get; }
    IFixedList<ITypeMemberDefinitionSyntax> ITypeDefinitionSyntax.Members => Members;
}

public partial interface IGenericParameterSyntax : ICodeSyntax
{
    ICapabilityConstraintSyntax Constraint { get; }
    IdentifierName Name { get; }
    TypeParameterIndependence Independence { get; }
    TypeParameterVariance Variance { get; }
}

[Closed(
    typeof(IClassMemberDefinitionSyntax),
    typeof(ITraitMemberDefinitionSyntax),
    typeof(IStructMemberDefinitionSyntax))]
public partial interface ITypeMemberDefinitionSyntax : IEntityDefinitionSyntax
{
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
    typeof(IAbstractMethodDefinitionSyntax),
    typeof(IConcreteMethodDefinitionSyntax))]
public partial interface IMethodDefinitionSyntax : IClassMemberDefinitionSyntax, ITraitMemberDefinitionSyntax, IInvocableDefinitionSyntax
{
    MethodKind Kind { get; }
    new IdentifierName Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    IMethodSelfParameterSyntax SelfParameter { get; }
    new IFixedList<INamedParameterSyntax> Parameters { get; }
    IFixedList<IConstructorOrInitializerParameterSyntax> IInvocableDefinitionSyntax.Parameters => Parameters;
    IReturnSyntax? Return { get; }
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

public partial interface IConstructorDefinitionSyntax : IConcreteInvocableDefinitionSyntax, IClassMemberDefinitionSyntax
{
    new IdentifierName? Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    IConstructorSelfParameterSyntax SelfParameter { get; }
    new IBlockBodySyntax Body { get; }
    IBodySyntax IConcreteInvocableDefinitionSyntax.Body => Body;
}

public partial interface IInitializerDefinitionSyntax : IConcreteInvocableDefinitionSyntax, IStructMemberDefinitionSyntax
{
    new IdentifierName? Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    IInitializerSelfParameterSyntax SelfParameter { get; }
    new IBlockBodySyntax Body { get; }
    IBodySyntax IConcreteInvocableDefinitionSyntax.Body => Body;
}

public partial interface IFieldDefinitionSyntax : IClassMemberDefinitionSyntax, IStructMemberDefinitionSyntax, IBindingSyntax
{
    new IdentifierName Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    ITypeSyntax Type { get; }
    IExpressionSyntax? Initializer { get; }
}

public partial interface IAssociatedFunctionDefinitionSyntax : IClassMemberDefinitionSyntax, ITraitMemberDefinitionSyntax, IStructMemberDefinitionSyntax, IConcreteInvocableDefinitionSyntax
{
    new IdentifierName Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    new IFixedList<INamedParameterSyntax> Parameters { get; }
    IReturnSyntax? Return { get; }
}

public partial interface IAttributeSyntax : ICodeSyntax
{
    IStandardTypeNameSyntax TypeName { get; }
}

[Closed(
    typeof(ICapabilitySetSyntax),
    typeof(ICapabilitySyntax))]
public partial interface ICapabilityConstraintSyntax : ICodeSyntax
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
public partial interface IParameterSyntax : ICodeSyntax
{
    IdentifierName? Name { get; }
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
    ITypeSyntax Type { get; }
    IExpressionSyntax? DefaultValue { get; }
}

[Closed(
    typeof(IConstructorSelfParameterSyntax),
    typeof(IInitializerSelfParameterSyntax),
    typeof(IMethodSelfParameterSyntax))]
public partial interface ISelfParameterSyntax : IParameterSyntax
{
    bool IsLentBinding { get; }
}

public partial interface IConstructorSelfParameterSyntax : ISelfParameterSyntax
{
    ICapabilitySyntax Capability { get; }
}

public partial interface IInitializerSelfParameterSyntax : ISelfParameterSyntax
{
    ICapabilitySyntax Capability { get; }
}

public partial interface IMethodSelfParameterSyntax : ISelfParameterSyntax
{
    ICapabilityConstraintSyntax Capability { get; }
}

public partial interface IFieldParameterSyntax : IConstructorOrInitializerParameterSyntax
{
    new IdentifierName Name { get; }
    IdentifierName? IParameterSyntax.Name => Name;
    IExpressionSyntax? DefaultValue { get; }
}

public partial interface IReturnSyntax : ICodeSyntax
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
public partial interface ITypeSyntax : ICodeSyntax
{
}

[Closed(
    typeof(IStandardTypeNameSyntax),
    typeof(ISimpleTypeNameSyntax),
    typeof(IQualifiedTypeNameSyntax))]
public partial interface ITypeNameSyntax : ITypeSyntax
{
    TypeName Name { get; }
}

[Closed(
    typeof(IIdentifierTypeNameSyntax),
    typeof(IGenericTypeNameSyntax))]
public partial interface IStandardTypeNameSyntax : ITypeNameSyntax
{
    new StandardName Name { get; }
    TypeName ITypeNameSyntax.Name => Name;
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

public partial interface IParameterTypeSyntax : ICodeSyntax
{
    bool IsLent { get; }
    ITypeSyntax Referent { get; }
}

public partial interface IReturnTypeSyntax : ICodeSyntax
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
}

[Closed(
    typeof(IResultStatementSyntax),
    typeof(IBodyStatementSyntax))]
public partial interface IStatementSyntax : ICodeSyntax
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
    IdentifierName Name { get; }
    ICapabilitySyntax? Capability { get; }
    ITypeSyntax? Type { get; }
    IExpressionSyntax? Initializer { get; }
}

public partial interface IExpressionStatementSyntax : IBodyStatementSyntax
{
    IExpressionSyntax Expression { get; }
}

[Closed(
    typeof(IBindingContextPatternSyntax),
    typeof(IOptionalOrBindingPatternSyntax))]
public partial interface IPatternSyntax : ICodeSyntax
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
    typeof(INameExpressionSyntax),
    typeof(IMoveExpressionSyntax),
    typeof(IFreezeExpressionSyntax),
    typeof(IAsyncBlockExpressionSyntax),
    typeof(IAsyncStartExpressionSyntax),
    typeof(IAwaitExpressionSyntax))]
public partial interface IExpressionSyntax : ICodeSyntax
{
}

[Closed(
    typeof(IIdentifierNameExpressionSyntax),
    typeof(IMemberAccessExpressionSyntax),
    typeof(IMissingNameSyntax))]
public partial interface IAssignableExpressionSyntax : IExpressionSyntax
{
}

public partial interface IBlockExpressionSyntax : IExpressionSyntax, IBlockOrResultSyntax, IBodyOrBlockSyntax
{
}

public partial interface INewObjectExpressionSyntax : IExpressionSyntax
{
    ITypeNameSyntax Type { get; }
    IdentifierName? ConstructorName { get; }
    TextSpan? ConstructorNameSpan { get; }
    IFixedList<IExpressionSyntax> Arguments { get; }
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
    IdentifierName VariableName { get; }
    IExpressionSyntax InExpression { get; }
    ITypeSyntax? Type { get; }
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

public partial interface IInvocationExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Expression { get; }
    IFixedList<IExpressionSyntax> Arguments { get; }
}

[Closed(
    typeof(ISimpleNameSyntax),
    typeof(IStandardNameExpressionSyntax),
    typeof(ISpecialTypeNameExpressionSyntax),
    typeof(ISelfExpressionSyntax),
    typeof(IMemberAccessExpressionSyntax))]
public partial interface INameExpressionSyntax : IExpressionSyntax
{
}

[Closed(
    typeof(IIdentifierNameExpressionSyntax),
    typeof(IInstanceExpressionSyntax),
    typeof(IMissingNameSyntax))]
public partial interface ISimpleNameSyntax : INameExpressionSyntax
{
}

[Closed(
    typeof(IIdentifierNameExpressionSyntax),
    typeof(IGenericNameExpressionSyntax))]
public partial interface IStandardNameExpressionSyntax : INameExpressionSyntax
{
    StandardName Name { get; }
}

public partial interface IIdentifierNameExpressionSyntax : IStandardNameExpressionSyntax, ISimpleNameSyntax, IAssignableExpressionSyntax
{
    new IdentifierName Name { get; }
    StandardName IStandardNameExpressionSyntax.Name => Name;
}

public partial interface ISpecialTypeNameExpressionSyntax : INameExpressionSyntax
{
    SpecialTypeName Name { get; }
}

public partial interface IGenericNameExpressionSyntax : IStandardNameExpressionSyntax
{
    new GenericName Name { get; }
    StandardName IStandardNameExpressionSyntax.Name => Name;
    IFixedList<ITypeSyntax> TypeArguments { get; }
}

[Closed(
    typeof(ISelfExpressionSyntax))]
public partial interface IInstanceExpressionSyntax : ISimpleNameSyntax
{
}

public partial interface ISelfExpressionSyntax : INameExpressionSyntax, IInstanceExpressionSyntax
{
    bool IsImplicit { get; }
}

public partial interface IMemberAccessExpressionSyntax : INameExpressionSyntax, IAssignableExpressionSyntax
{
    IExpressionSyntax Context { get; }
    StandardName MemberName { get; }
    IFixedList<ITypeSyntax> TypeArguments { get; }
    TextSpan MemberNameSpan { get; }
}

public partial interface IMissingNameSyntax : ISimpleNameSyntax, IAssignableExpressionSyntax
{
}

public partial interface IMoveExpressionSyntax : IExpressionSyntax
{
    ISimpleNameSyntax Referent { get; }
}

public partial interface IFreezeExpressionSyntax : IExpressionSyntax
{
    ISimpleNameSyntax Referent { get; }
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

