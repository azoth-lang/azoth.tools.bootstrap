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

// [Closed(typeof(CompilationUnitSyntax))]
public partial interface ICompilationUnitSyntax : ICodeSyntax
{
    CodeFile File { get; }
    NamespaceName ImplicitNamespaceName { get; }
    DiagnosticCollection Diagnostics { get; }
    IFixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
    IFixedList<INonMemberDefinitionSyntax> Definitions { get; }
}

// [Closed(typeof(UsingDirectiveSyntax))]
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

// [Closed(typeof(PackageSyntax))]
public partial interface IPackageSyntax : ISyntax
{
    IdentifierName Name { get; }
    IFixedSet<ICompilationUnitSyntax> CompilationUnits { get; }
    IFixedSet<ICompilationUnitSyntax> TestingCompilationUnits { get; }
    IFixedSet<IPackageReferenceSyntax> References { get; }
    DiagnosticCollection Diagnostics { get; }
}

// [Closed(typeof(PackageReferenceSyntax))]
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

// [Closed(typeof(NamespaceDefinitionSyntax))]
public partial interface INamespaceDefinitionSyntax : INonMemberDefinitionSyntax
{
    bool IsGlobalQualified { get; }
    NamespaceName DeclaredNames { get; }
    IFixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
    IFixedList<INonMemberDefinitionSyntax> Definitions { get; }
}

// [Closed(typeof(FunctionDefinitionSyntax))]
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

// [Closed(typeof(ClassDefinitionSyntax))]
public partial interface IClassDefinitionSyntax : ITypeDefinitionSyntax
{
    IAbstractKeywordToken? AbstractModifier { get; }
    IStandardTypeNameSyntax? BaseTypeName { get; }
    new IFixedList<IClassMemberDefinitionSyntax> Members { get; }
    IFixedList<ITypeMemberDefinitionSyntax> ITypeDefinitionSyntax.Members => Members;
}

// [Closed(typeof(StructDefinitionSyntax))]
public partial interface IStructDefinitionSyntax : ITypeDefinitionSyntax
{
    new IFixedList<IStructMemberDefinitionSyntax> Members { get; }
    IFixedList<ITypeMemberDefinitionSyntax> ITypeDefinitionSyntax.Members => Members;
}

// [Closed(typeof(TraitDefinitionSyntax))]
public partial interface ITraitDefinitionSyntax : ITypeDefinitionSyntax
{
    new IFixedList<ITraitMemberDefinitionSyntax> Members { get; }
    IFixedList<ITypeMemberDefinitionSyntax> ITypeDefinitionSyntax.Members => Members;
}

// [Closed(typeof(GenericParameterSyntax))]
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

// [Closed(typeof(AbstractMethodDefinitionSyntax))]
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
}

// [Closed(typeof(StandardMethodDefinitionSyntax))]
public partial interface IStandardMethodDefinitionSyntax : IConcreteMethodDefinitionSyntax
{
}

// [Closed(typeof(GetterMethodDefinitionSyntax))]
public partial interface IGetterMethodDefinitionSyntax : IConcreteMethodDefinitionSyntax
{
    new IReturnSyntax Return { get; }
}

// [Closed(typeof(SetterMethodDefinitionSyntax))]
public partial interface ISetterMethodDefinitionSyntax : IConcreteMethodDefinitionSyntax
{
}

// [Closed(typeof(ConstructorDefinitionSyntax))]
public partial interface IConstructorDefinitionSyntax : IConcreteInvocableDefinitionSyntax, IClassMemberDefinitionSyntax
{
    new IdentifierName? Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    IConstructorSelfParameterSyntax SelfParameter { get; }
    new IBlockBodySyntax Body { get; }
    IBodySyntax IConcreteInvocableDefinitionSyntax.Body => Body;
}

// [Closed(typeof(InitializerDefinitionSyntax))]
public partial interface IInitializerDefinitionSyntax : IConcreteInvocableDefinitionSyntax, IStructMemberDefinitionSyntax
{
    new IdentifierName? Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    IInitializerSelfParameterSyntax SelfParameter { get; }
    new IBlockBodySyntax Body { get; }
    IBodySyntax IConcreteInvocableDefinitionSyntax.Body => Body;
}

// [Closed(typeof(FieldDefinitionSyntax))]
public partial interface IFieldDefinitionSyntax : IClassMemberDefinitionSyntax, IStructMemberDefinitionSyntax, IBindingSyntax
{
    new IdentifierName Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    ITypeSyntax Type { get; }
    IExpressionSyntax? Initializer { get; }
}

// [Closed(typeof(AssociatedFunctionDefinitionSyntax))]
public partial interface IAssociatedFunctionDefinitionSyntax : IClassMemberDefinitionSyntax, ITraitMemberDefinitionSyntax, IStructMemberDefinitionSyntax, IConcreteInvocableDefinitionSyntax
{
    new IdentifierName Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    new IFixedList<INamedParameterSyntax> Parameters { get; }
    IReturnSyntax? Return { get; }
}

// [Closed(typeof(AttributeSyntax))]
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

// [Closed(typeof(CapabilitySetSyntax))]
public partial interface ICapabilitySetSyntax : ICapabilityConstraintSyntax
{
    new CapabilitySet Constraint { get; }
    ICapabilityConstraint ICapabilityConstraintSyntax.Constraint => Constraint;
}

// [Closed(typeof(CapabilitySyntax))]
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
}

[Closed(
    typeof(INamedParameterSyntax),
    typeof(IFieldParameterSyntax))]
public partial interface IConstructorOrInitializerParameterSyntax : IParameterSyntax
{
}

// [Closed(typeof(NamedParameterSyntax))]
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

// [Closed(typeof(ConstructorSelfParameterSyntax))]
public partial interface IConstructorSelfParameterSyntax : ISelfParameterSyntax
{
    ICapabilitySyntax Capability { get; }
}

// [Closed(typeof(InitializerSelfParameterSyntax))]
public partial interface IInitializerSelfParameterSyntax : ISelfParameterSyntax
{
    ICapabilitySyntax Capability { get; }
}

// [Closed(typeof(MethodSelfParameterSyntax))]
public partial interface IMethodSelfParameterSyntax : ISelfParameterSyntax
{
    ICapabilityConstraintSyntax Capability { get; }
}

// [Closed(typeof(FieldParameterSyntax))]
public partial interface IFieldParameterSyntax : IConstructorOrInitializerParameterSyntax
{
    new IdentifierName Name { get; }
    IdentifierName? IParameterSyntax.Name => Name;
    IExpressionSyntax? DefaultValue { get; }
}

// [Closed(typeof(ReturnSyntax))]
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

// [Closed(typeof(BlockBodySyntax))]
public partial interface IBlockBodySyntax : IBodySyntax
{
    new IFixedList<IBodyStatementSyntax> Statements { get; }
    IFixedList<IStatementSyntax> IBodyOrBlockSyntax.Statements => Statements;
}

// [Closed(typeof(ExpressionBodySyntax))]
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

// [Closed(typeof(IdentifierTypeNameSyntax))]
public partial interface IIdentifierTypeNameSyntax : IStandardTypeNameSyntax, ISimpleTypeNameSyntax
{
    new IdentifierName Name { get; }
    StandardName IStandardTypeNameSyntax.Name => Name;
}

// [Closed(typeof(SpecialTypeNameSyntax))]
public partial interface ISpecialTypeNameSyntax : ISimpleTypeNameSyntax
{
    new SpecialTypeName Name { get; }
    TypeName ITypeNameSyntax.Name => Name;
}

// [Closed(typeof(GenericTypeNameSyntax))]
public partial interface IGenericTypeNameSyntax : IStandardTypeNameSyntax
{
    new GenericName Name { get; }
    StandardName IStandardTypeNameSyntax.Name => Name;
    IFixedList<ITypeSyntax> TypeArguments { get; }
}

// [Closed(typeof(QualifiedTypeNameSyntax))]
public partial interface IQualifiedTypeNameSyntax : ITypeNameSyntax
{
    ITypeNameSyntax Context { get; }
    IStandardTypeNameSyntax QualifiedName { get; }
}

// [Closed(typeof(OptionalTypeSyntax))]
public partial interface IOptionalTypeSyntax : ITypeSyntax
{
    ITypeSyntax Referent { get; }
}

// [Closed(typeof(CapabilityTypeSyntax))]
public partial interface ICapabilityTypeSyntax : ITypeSyntax
{
    ICapabilitySyntax Capability { get; }
    ITypeSyntax Referent { get; }
}

// [Closed(typeof(FunctionTypeSyntax))]
public partial interface IFunctionTypeSyntax : ITypeSyntax
{
    IFixedList<IParameterTypeSyntax> Parameters { get; }
    IReturnTypeSyntax Return { get; }
}

// [Closed(typeof(ParameterTypeSyntax))]
public partial interface IParameterTypeSyntax : ICodeSyntax
{
    bool IsLent { get; }
    ITypeSyntax Referent { get; }
}

// [Closed(typeof(ReturnTypeSyntax))]
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

// [Closed(typeof(CapabilityViewpointTypeSyntax))]
public partial interface ICapabilityViewpointTypeSyntax : IViewpointTypeSyntax
{
    ICapabilitySyntax Capability { get; }
}

// [Closed(typeof(SelfViewpointTypeSyntax))]
public partial interface ISelfViewpointTypeSyntax : IViewpointTypeSyntax
{
}

[Closed(
    typeof(IResultStatementSyntax),
    typeof(IBodyStatementSyntax))]
public partial interface IStatementSyntax : ICodeSyntax
{
}

// [Closed(typeof(ResultStatementSyntax))]
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

// [Closed(typeof(VariableDeclarationStatementSyntax))]
public partial interface IVariableDeclarationStatementSyntax : IBodyStatementSyntax, ILocalBindingSyntax
{
    IdentifierName Name { get; }
    ICapabilitySyntax? Capability { get; }
    ITypeSyntax? Type { get; }
    IExpressionSyntax? Initializer { get; }
}

// [Closed(typeof(ExpressionStatementSyntax))]
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

// [Closed(typeof(BindingContextPatternSyntax))]
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

// [Closed(typeof(BindingPatternSyntax))]
public partial interface IBindingPatternSyntax : IOptionalOrBindingPatternSyntax, ILocalBindingSyntax
{
    IdentifierName Name { get; }
}

// [Closed(typeof(OptionalPatternSyntax))]
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

// [Closed(typeof(BlockExpressionSyntax))]
public partial interface IBlockExpressionSyntax : IExpressionSyntax, IBlockOrResultSyntax, IBodyOrBlockSyntax
{
}

// [Closed(typeof(NewObjectExpressionSyntax))]
public partial interface INewObjectExpressionSyntax : IExpressionSyntax
{
    ITypeNameSyntax Type { get; }
    IdentifierName? ConstructorName { get; }
    TextSpan? ConstructorNameSpan { get; }
    IFixedList<IExpressionSyntax> Arguments { get; }
}

// [Closed(typeof(UnsafeExpressionSyntax))]
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

// [Closed(typeof(BoolLiteralExpressionSyntax))]
public partial interface IBoolLiteralExpressionSyntax : ILiteralExpressionSyntax
{
    bool Value { get; }
}

// [Closed(typeof(IntegerLiteralExpressionSyntax))]
public partial interface IIntegerLiteralExpressionSyntax : ILiteralExpressionSyntax
{
    BigInteger Value { get; }
}

// [Closed(typeof(NoneLiteralExpressionSyntax))]
public partial interface INoneLiteralExpressionSyntax : ILiteralExpressionSyntax
{
}

// [Closed(typeof(StringLiteralExpressionSyntax))]
public partial interface IStringLiteralExpressionSyntax : ILiteralExpressionSyntax
{
    string Value { get; }
}

// [Closed(typeof(AssignmentExpressionSyntax))]
public partial interface IAssignmentExpressionSyntax : IExpressionSyntax
{
    IAssignableExpressionSyntax LeftOperand { get; }
    AssignmentOperator Operator { get; }
    IExpressionSyntax RightOperand { get; }
}

// [Closed(typeof(BinaryOperatorExpressionSyntax))]
public partial interface IBinaryOperatorExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax LeftOperand { get; }
    BinaryOperator Operator { get; }
    IExpressionSyntax RightOperand { get; }
}

// [Closed(typeof(UnaryOperatorExpressionSyntax))]
public partial interface IUnaryOperatorExpressionSyntax : IExpressionSyntax
{
    UnaryOperatorFixity Fixity { get; }
    UnaryOperator Operator { get; }
    IExpressionSyntax Operand { get; }
}

// [Closed(typeof(IdExpressionSyntax))]
public partial interface IIdExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Referent { get; }
}

// [Closed(typeof(ConversionExpressionSyntax))]
public partial interface IConversionExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Referent { get; }
    ConversionOperator Operator { get; }
    ITypeSyntax ConvertToType { get; }
}

// [Closed(typeof(PatternMatchExpressionSyntax))]
public partial interface IPatternMatchExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Referent { get; }
    IPatternSyntax Pattern { get; }
}

// [Closed(typeof(IfExpressionSyntax))]
public partial interface IIfExpressionSyntax : IExpressionSyntax, IElseClauseSyntax
{
    IExpressionSyntax Condition { get; }
    IBlockOrResultSyntax ThenBlock { get; }
    IElseClauseSyntax? ElseClause { get; }
}

// [Closed(typeof(LoopExpressionSyntax))]
public partial interface ILoopExpressionSyntax : IExpressionSyntax
{
    IBlockExpressionSyntax Block { get; }
}

// [Closed(typeof(WhileExpressionSyntax))]
public partial interface IWhileExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Condition { get; }
    IBlockExpressionSyntax Block { get; }
}

// [Closed(typeof(ForeachExpressionSyntax))]
public partial interface IForeachExpressionSyntax : IExpressionSyntax, ILocalBindingSyntax
{
    IdentifierName VariableName { get; }
    IExpressionSyntax InExpression { get; }
    ITypeSyntax? Type { get; }
    IBlockExpressionSyntax Block { get; }
}

// [Closed(typeof(BreakExpressionSyntax))]
public partial interface IBreakExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax? Value { get; }
}

// [Closed(typeof(NextExpressionSyntax))]
public partial interface INextExpressionSyntax : IExpressionSyntax
{
}

// [Closed(typeof(ReturnExpressionSyntax))]
public partial interface IReturnExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax? Value { get; }
}

// [Closed(typeof(InvocationExpressionSyntax))]
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

// [Closed(typeof(IdentifierNameExpressionSyntax))]
public partial interface IIdentifierNameExpressionSyntax : IStandardNameExpressionSyntax, ISimpleNameSyntax, IAssignableExpressionSyntax
{
    new IdentifierName Name { get; }
    StandardName IStandardNameExpressionSyntax.Name => Name;
}

// [Closed(typeof(SpecialTypeNameExpressionSyntax))]
public partial interface ISpecialTypeNameExpressionSyntax : INameExpressionSyntax
{
    SpecialTypeName Name { get; }
}

// [Closed(typeof(GenericNameExpressionSyntax))]
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

// [Closed(typeof(SelfExpressionSyntax))]
public partial interface ISelfExpressionSyntax : INameExpressionSyntax, IInstanceExpressionSyntax
{
    bool IsImplicit { get; }
}

// [Closed(typeof(MemberAccessExpressionSyntax))]
public partial interface IMemberAccessExpressionSyntax : INameExpressionSyntax, IAssignableExpressionSyntax
{
    IExpressionSyntax Context { get; }
    StandardName MemberName { get; }
    IFixedList<ITypeSyntax> TypeArguments { get; }
    TextSpan MemberNameSpan { get; }
}

// [Closed(typeof(MissingNameSyntax))]
public partial interface IMissingNameSyntax : ISimpleNameSyntax, IAssignableExpressionSyntax
{
}

// [Closed(typeof(MoveExpressionSyntax))]
public partial interface IMoveExpressionSyntax : IExpressionSyntax
{
    ISimpleNameSyntax Referent { get; }
}

// [Closed(typeof(FreezeExpressionSyntax))]
public partial interface IFreezeExpressionSyntax : IExpressionSyntax
{
    ISimpleNameSyntax Referent { get; }
}

// [Closed(typeof(AsyncBlockExpressionSyntax))]
public partial interface IAsyncBlockExpressionSyntax : IExpressionSyntax
{
    IBlockExpressionSyntax Block { get; }
}

// [Closed(typeof(AsyncStartExpressionSyntax))]
public partial interface IAsyncStartExpressionSyntax : IExpressionSyntax
{
    bool Scheduled { get; }
    IExpressionSyntax Expression { get; }
}

// [Closed(typeof(AwaitExpressionSyntax))]
public partial interface IAwaitExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Expression { get; }
}

file class CompilationUnitSyntax // : ICompilationUnitSyntax
{
    public CodeFile File { get; } = default!;
    public NamespaceName ImplicitNamespaceName { get; } = default!;
    public DiagnosticCollection Diagnostics { get; } = default!;
    public IFixedList<IUsingDirectiveSyntax> UsingDirectives { get; } = default!;
    public IFixedList<INonMemberDefinitionSyntax> Definitions { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class UsingDirectiveSyntax // : IUsingDirectiveSyntax
{
    public NamespaceName Name { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class PackageSyntax // : IPackageSyntax
{
    public IdentifierName Name { get; } = default!;
    public IFixedSet<ICompilationUnitSyntax> CompilationUnits { get; } = default!;
    public IFixedSet<ICompilationUnitSyntax> TestingCompilationUnits { get; } = default!;
    public IFixedSet<IPackageReferenceSyntax> References { get; } = default!;
    public DiagnosticCollection Diagnostics { get; } = default!;
}

file class PackageReferenceSyntax // : IPackageReferenceSyntax
{
    public IdentifierName AliasOrName { get; } = default!;
    public IPackageSymbols Package { get; } = default!;
    public bool IsTrusted { get; } = default!;
}

file class NamespaceDefinitionSyntax // : INamespaceDefinitionSyntax
{
    public bool IsGlobalQualified { get; } = default!;
    public NamespaceName DeclaredNames { get; } = default!;
    public IFixedList<IUsingDirectiveSyntax> UsingDirectives { get; } = default!;
    public IFixedList<INonMemberDefinitionSyntax> Definitions { get; } = default!;
    public CodeFile File { get; } = default!;
    public TypeName? Name { get; } = default!;
    public TextSpan NameSpan { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class FunctionDefinitionSyntax // : IFunctionDefinitionSyntax
{
    public IFixedList<IAttributeSyntax> Attributes { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public IFixedList<INamedParameterSyntax> Parameters { get; } = default!;
    public IReturnSyntax? Return { get; } = default!;
    public IBodySyntax Body { get; } = default!;
    public IAccessModifierToken? AccessModifier { get; } = default!;
    public CodeFile File { get; } = default!;
    public TextSpan NameSpan { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class ClassDefinitionSyntax // : IClassDefinitionSyntax
{
    public IAbstractKeywordToken? AbstractModifier { get; } = default!;
    public IFixedList<IGenericParameterSyntax> GenericParameters { get; } = default!;
    public IStandardTypeNameSyntax? BaseTypeName { get; } = default!;
    public IFixedList<IStandardTypeNameSyntax> SupertypeNames { get; } = default!;
    public IFixedList<IClassMemberDefinitionSyntax> Members { get; } = default!;
    public IConstKeywordToken? ConstModifier { get; } = default!;
    public IMoveKeywordToken? MoveModifier { get; } = default!;
    public StandardName Name { get; } = default!;
    public IAccessModifierToken? AccessModifier { get; } = default!;
    public CodeFile File { get; } = default!;
    public TextSpan NameSpan { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class StructDefinitionSyntax // : IStructDefinitionSyntax
{
    public IFixedList<IGenericParameterSyntax> GenericParameters { get; } = default!;
    public IFixedList<IStandardTypeNameSyntax> SupertypeNames { get; } = default!;
    public IFixedList<IStructMemberDefinitionSyntax> Members { get; } = default!;
    public IConstKeywordToken? ConstModifier { get; } = default!;
    public IMoveKeywordToken? MoveModifier { get; } = default!;
    public StandardName Name { get; } = default!;
    public IAccessModifierToken? AccessModifier { get; } = default!;
    public CodeFile File { get; } = default!;
    public TextSpan NameSpan { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class TraitDefinitionSyntax // : ITraitDefinitionSyntax
{
    public IFixedList<IGenericParameterSyntax> GenericParameters { get; } = default!;
    public IFixedList<IStandardTypeNameSyntax> SupertypeNames { get; } = default!;
    public IFixedList<ITraitMemberDefinitionSyntax> Members { get; } = default!;
    public IConstKeywordToken? ConstModifier { get; } = default!;
    public IMoveKeywordToken? MoveModifier { get; } = default!;
    public StandardName Name { get; } = default!;
    public IAccessModifierToken? AccessModifier { get; } = default!;
    public CodeFile File { get; } = default!;
    public TextSpan NameSpan { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class GenericParameterSyntax // : IGenericParameterSyntax
{
    public ICapabilityConstraintSyntax Constraint { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public TypeParameterIndependence Independence { get; } = default!;
    public TypeParameterVariance Variance { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class AbstractMethodDefinitionSyntax // : IAbstractMethodDefinitionSyntax
{
    public IMethodSelfParameterSyntax SelfParameter { get; } = default!;
    public IFixedList<INamedParameterSyntax> Parameters { get; } = default!;
    public IReturnSyntax? Return { get; } = default!;
    public MethodKind Kind { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public IAccessModifierToken? AccessModifier { get; } = default!;
    public CodeFile File { get; } = default!;
    public TextSpan NameSpan { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class StandardMethodDefinitionSyntax // : IStandardMethodDefinitionSyntax
{
    public IMethodSelfParameterSyntax SelfParameter { get; } = default!;
    public IFixedList<INamedParameterSyntax> Parameters { get; } = default!;
    public IReturnSyntax? Return { get; } = default!;
    public IBodySyntax Body { get; } = default!;
    public MethodKind Kind { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public IAccessModifierToken? AccessModifier { get; } = default!;
    public CodeFile File { get; } = default!;
    public TextSpan NameSpan { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class GetterMethodDefinitionSyntax // : IGetterMethodDefinitionSyntax
{
    public IMethodSelfParameterSyntax SelfParameter { get; } = default!;
    public IFixedList<INamedParameterSyntax> Parameters { get; } = default!;
    public IReturnSyntax Return { get; } = default!;
    public IBodySyntax Body { get; } = default!;
    public MethodKind Kind { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public IAccessModifierToken? AccessModifier { get; } = default!;
    public CodeFile File { get; } = default!;
    public TextSpan NameSpan { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class SetterMethodDefinitionSyntax // : ISetterMethodDefinitionSyntax
{
    public IMethodSelfParameterSyntax SelfParameter { get; } = default!;
    public IFixedList<INamedParameterSyntax> Parameters { get; } = default!;
    public IReturnSyntax? Return { get; } = default!;
    public IBodySyntax Body { get; } = default!;
    public MethodKind Kind { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public IAccessModifierToken? AccessModifier { get; } = default!;
    public CodeFile File { get; } = default!;
    public TextSpan NameSpan { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class ConstructorDefinitionSyntax // : IConstructorDefinitionSyntax
{
    public IdentifierName? Name { get; } = default!;
    public IConstructorSelfParameterSyntax SelfParameter { get; } = default!;
    public IFixedList<IConstructorOrInitializerParameterSyntax> Parameters { get; } = default!;
    public IBlockBodySyntax Body { get; } = default!;
    public IAccessModifierToken? AccessModifier { get; } = default!;
    public CodeFile File { get; } = default!;
    public TextSpan NameSpan { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class InitializerDefinitionSyntax // : IInitializerDefinitionSyntax
{
    public IdentifierName? Name { get; } = default!;
    public IInitializerSelfParameterSyntax SelfParameter { get; } = default!;
    public IFixedList<IConstructorOrInitializerParameterSyntax> Parameters { get; } = default!;
    public IBlockBodySyntax Body { get; } = default!;
    public IAccessModifierToken? AccessModifier { get; } = default!;
    public CodeFile File { get; } = default!;
    public TextSpan NameSpan { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class FieldDefinitionSyntax // : IFieldDefinitionSyntax
{
    public IdentifierName Name { get; } = default!;
    public ITypeSyntax Type { get; } = default!;
    public IExpressionSyntax? Initializer { get; } = default!;
    public IAccessModifierToken? AccessModifier { get; } = default!;
    public CodeFile File { get; } = default!;
    public TextSpan NameSpan { get; } = default!;
    public TextSpan Span { get; } = default!;
    public bool IsMutableBinding { get; } = default!;
}

file class AssociatedFunctionDefinitionSyntax // : IAssociatedFunctionDefinitionSyntax
{
    public IdentifierName Name { get; } = default!;
    public IFixedList<INamedParameterSyntax> Parameters { get; } = default!;
    public IReturnSyntax? Return { get; } = default!;
    public IBodySyntax Body { get; } = default!;
    public IAccessModifierToken? AccessModifier { get; } = default!;
    public CodeFile File { get; } = default!;
    public TextSpan NameSpan { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class AttributeSyntax // : IAttributeSyntax
{
    public IStandardTypeNameSyntax TypeName { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class CapabilitySetSyntax // : ICapabilitySetSyntax
{
    public CapabilitySet Constraint { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class CapabilitySyntax // : ICapabilitySyntax
{
    public IFixedList<ICapabilityToken> Tokens { get; } = default!;
    public DeclaredCapability Declared { get; } = default!;
    public Capability Capability { get; } = default!;
    public ICapabilityConstraint Constraint { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class NamedParameterSyntax // : INamedParameterSyntax
{
    public bool IsMutableBinding { get; } = default!;
    public bool IsLentBinding { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public ITypeSyntax Type { get; } = default!;
    public IExpressionSyntax? DefaultValue { get; } = default!;
    public TextSpan Span { get; } = default!;
    public TextSpan NameSpan { get; } = default!;
}

file class ConstructorSelfParameterSyntax // : IConstructorSelfParameterSyntax
{
    public ICapabilitySyntax Capability { get; } = default!;
    public bool IsLentBinding { get; } = default!;
    public IdentifierName? Name { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class InitializerSelfParameterSyntax // : IInitializerSelfParameterSyntax
{
    public ICapabilitySyntax Capability { get; } = default!;
    public bool IsLentBinding { get; } = default!;
    public IdentifierName? Name { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class MethodSelfParameterSyntax // : IMethodSelfParameterSyntax
{
    public ICapabilityConstraintSyntax Capability { get; } = default!;
    public bool IsLentBinding { get; } = default!;
    public IdentifierName? Name { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class FieldParameterSyntax // : IFieldParameterSyntax
{
    public IdentifierName Name { get; } = default!;
    public IExpressionSyntax? DefaultValue { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class ReturnSyntax // : IReturnSyntax
{
    public ITypeSyntax Type { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class BlockBodySyntax // : IBlockBodySyntax
{
    public IFixedList<IBodyStatementSyntax> Statements { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class ExpressionBodySyntax // : IExpressionBodySyntax
{
    public IResultStatementSyntax ResultStatement { get; } = default!;
    public IFixedList<IStatementSyntax> Statements { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class IdentifierTypeNameSyntax // : IIdentifierTypeNameSyntax
{
    public IdentifierName Name { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class SpecialTypeNameSyntax // : ISpecialTypeNameSyntax
{
    public SpecialTypeName Name { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class GenericTypeNameSyntax // : IGenericTypeNameSyntax
{
    public GenericName Name { get; } = default!;
    public IFixedList<ITypeSyntax> TypeArguments { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class QualifiedTypeNameSyntax // : IQualifiedTypeNameSyntax
{
    public ITypeNameSyntax Context { get; } = default!;
    public IStandardTypeNameSyntax QualifiedName { get; } = default!;
    public TypeName Name { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class OptionalTypeSyntax // : IOptionalTypeSyntax
{
    public ITypeSyntax Referent { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class CapabilityTypeSyntax // : ICapabilityTypeSyntax
{
    public ICapabilitySyntax Capability { get; } = default!;
    public ITypeSyntax Referent { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class FunctionTypeSyntax // : IFunctionTypeSyntax
{
    public IFixedList<IParameterTypeSyntax> Parameters { get; } = default!;
    public IReturnTypeSyntax Return { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class ParameterTypeSyntax // : IParameterTypeSyntax
{
    public bool IsLent { get; } = default!;
    public ITypeSyntax Referent { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class ReturnTypeSyntax // : IReturnTypeSyntax
{
    public ITypeSyntax Referent { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class CapabilityViewpointTypeSyntax // : ICapabilityViewpointTypeSyntax
{
    public ICapabilitySyntax Capability { get; } = default!;
    public ITypeSyntax Referent { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class SelfViewpointTypeSyntax // : ISelfViewpointTypeSyntax
{
    public ITypeSyntax Referent { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class ResultStatementSyntax // : IResultStatementSyntax
{
    public IExpressionSyntax Expression { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class VariableDeclarationStatementSyntax // : IVariableDeclarationStatementSyntax
{
    public TextSpan NameSpan { get; } = default!;
    public IdentifierName Name { get; } = default!;
    public ICapabilitySyntax? Capability { get; } = default!;
    public ITypeSyntax? Type { get; } = default!;
    public IExpressionSyntax? Initializer { get; } = default!;
    public TextSpan Span { get; } = default!;
    public bool IsMutableBinding { get; } = default!;
}

file class ExpressionStatementSyntax // : IExpressionStatementSyntax
{
    public IExpressionSyntax Expression { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class BindingContextPatternSyntax // : IBindingContextPatternSyntax
{
    public bool IsMutableBinding { get; } = default!;
    public IPatternSyntax Pattern { get; } = default!;
    public ITypeSyntax? Type { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class BindingPatternSyntax // : IBindingPatternSyntax
{
    public IdentifierName Name { get; } = default!;
    public TextSpan Span { get; } = default!;
    public TextSpan NameSpan { get; } = default!;
    public bool IsMutableBinding { get; } = default!;
}

file class OptionalPatternSyntax // : IOptionalPatternSyntax
{
    public IOptionalOrBindingPatternSyntax Pattern { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class BlockExpressionSyntax // : IBlockExpressionSyntax
{
    public IFixedList<IStatementSyntax> Statements { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class NewObjectExpressionSyntax // : INewObjectExpressionSyntax
{
    public ITypeNameSyntax Type { get; } = default!;
    public IdentifierName? ConstructorName { get; } = default!;
    public TextSpan? ConstructorNameSpan { get; } = default!;
    public IFixedList<IExpressionSyntax> Arguments { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class UnsafeExpressionSyntax // : IUnsafeExpressionSyntax
{
    public IExpressionSyntax Expression { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class BoolLiteralExpressionSyntax // : IBoolLiteralExpressionSyntax
{
    public bool Value { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class IntegerLiteralExpressionSyntax // : IIntegerLiteralExpressionSyntax
{
    public BigInteger Value { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class NoneLiteralExpressionSyntax // : INoneLiteralExpressionSyntax
{
    public TextSpan Span { get; } = default!;
}

file class StringLiteralExpressionSyntax // : IStringLiteralExpressionSyntax
{
    public string Value { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class AssignmentExpressionSyntax // : IAssignmentExpressionSyntax
{
    public IAssignableExpressionSyntax LeftOperand { get; } = default!;
    public AssignmentOperator Operator { get; } = default!;
    public IExpressionSyntax RightOperand { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class BinaryOperatorExpressionSyntax // : IBinaryOperatorExpressionSyntax
{
    public IExpressionSyntax LeftOperand { get; } = default!;
    public BinaryOperator Operator { get; } = default!;
    public IExpressionSyntax RightOperand { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class UnaryOperatorExpressionSyntax // : IUnaryOperatorExpressionSyntax
{
    public UnaryOperatorFixity Fixity { get; } = default!;
    public UnaryOperator Operator { get; } = default!;
    public IExpressionSyntax Operand { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class IdExpressionSyntax // : IIdExpressionSyntax
{
    public IExpressionSyntax Referent { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class ConversionExpressionSyntax // : IConversionExpressionSyntax
{
    public IExpressionSyntax Referent { get; } = default!;
    public ConversionOperator Operator { get; } = default!;
    public ITypeSyntax ConvertToType { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class PatternMatchExpressionSyntax // : IPatternMatchExpressionSyntax
{
    public IExpressionSyntax Referent { get; } = default!;
    public IPatternSyntax Pattern { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class IfExpressionSyntax // : IIfExpressionSyntax
{
    public IExpressionSyntax Condition { get; } = default!;
    public IBlockOrResultSyntax ThenBlock { get; } = default!;
    public IElseClauseSyntax? ElseClause { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class LoopExpressionSyntax // : ILoopExpressionSyntax
{
    public IBlockExpressionSyntax Block { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class WhileExpressionSyntax // : IWhileExpressionSyntax
{
    public IExpressionSyntax Condition { get; } = default!;
    public IBlockExpressionSyntax Block { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class ForeachExpressionSyntax // : IForeachExpressionSyntax
{
    public IdentifierName VariableName { get; } = default!;
    public IExpressionSyntax InExpression { get; } = default!;
    public ITypeSyntax? Type { get; } = default!;
    public IBlockExpressionSyntax Block { get; } = default!;
    public TextSpan Span { get; } = default!;
    public TextSpan NameSpan { get; } = default!;
    public bool IsMutableBinding { get; } = default!;
}

file class BreakExpressionSyntax // : IBreakExpressionSyntax
{
    public IExpressionSyntax? Value { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class NextExpressionSyntax // : INextExpressionSyntax
{
    public TextSpan Span { get; } = default!;
}

file class ReturnExpressionSyntax // : IReturnExpressionSyntax
{
    public IExpressionSyntax? Value { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class InvocationExpressionSyntax // : IInvocationExpressionSyntax
{
    public IExpressionSyntax Expression { get; } = default!;
    public IFixedList<IExpressionSyntax> Arguments { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class IdentifierNameExpressionSyntax // : IIdentifierNameExpressionSyntax
{
    public IdentifierName Name { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class SpecialTypeNameExpressionSyntax // : ISpecialTypeNameExpressionSyntax
{
    public SpecialTypeName Name { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class GenericNameExpressionSyntax // : IGenericNameExpressionSyntax
{
    public GenericName Name { get; } = default!;
    public IFixedList<ITypeSyntax> TypeArguments { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class SelfExpressionSyntax // : ISelfExpressionSyntax
{
    public bool IsImplicit { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class MemberAccessExpressionSyntax // : IMemberAccessExpressionSyntax
{
    public IExpressionSyntax Context { get; } = default!;
    public StandardName MemberName { get; } = default!;
    public IFixedList<ITypeSyntax> TypeArguments { get; } = default!;
    public TextSpan MemberNameSpan { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class MissingNameSyntax // : IMissingNameSyntax
{
    public TextSpan Span { get; } = default!;
}

file class MoveExpressionSyntax // : IMoveExpressionSyntax
{
    public ISimpleNameSyntax Referent { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class FreezeExpressionSyntax // : IFreezeExpressionSyntax
{
    public ISimpleNameSyntax Referent { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class AsyncBlockExpressionSyntax // : IAsyncBlockExpressionSyntax
{
    public IBlockExpressionSyntax Block { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class AsyncStartExpressionSyntax // : IAsyncStartExpressionSyntax
{
    public bool Scheduled { get; } = default!;
    public IExpressionSyntax Expression { get; } = default!;
    public TextSpan Span { get; } = default!;
}

file class AwaitExpressionSyntax // : IAwaitExpressionSyntax
{
    public IExpressionSyntax Expression { get; } = default!;
    public TextSpan Span { get; } = default!;
}

