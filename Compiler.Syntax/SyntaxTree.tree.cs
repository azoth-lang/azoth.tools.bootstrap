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
    public CodeFile File { get; }
    public NamespaceName ImplicitNamespaceName { get; }
    public DiagnosticCollection Diagnostics { get; }
    public IFixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
    public IFixedList<INonMemberDefinitionSyntax> Definitions { get; }
    public TextSpan Span { get; }

    public CompilationUnitSyntax(CodeFile file, NamespaceName implicitNamespaceName, DiagnosticCollection diagnostics, IFixedList<IUsingDirectiveSyntax> usingDirectives, IFixedList<INonMemberDefinitionSyntax> definitions, TextSpan span)
    {
        File = file;
        ImplicitNamespaceName = implicitNamespaceName;
        Diagnostics = diagnostics;
        UsingDirectives = usingDirectives;
        Definitions = definitions;
        Span = span;
    }
}

file class UsingDirectiveSyntax // : IUsingDirectiveSyntax
{
    public NamespaceName Name { get; }
    public TextSpan Span { get; }

    public UsingDirectiveSyntax(NamespaceName name, TextSpan span)
    {
        Name = name;
        Span = span;
    }
}

file class PackageSyntax // : IPackageSyntax
{
    public IdentifierName Name { get; }
    public IFixedSet<ICompilationUnitSyntax> CompilationUnits { get; }
    public IFixedSet<ICompilationUnitSyntax> TestingCompilationUnits { get; }
    public IFixedSet<IPackageReferenceSyntax> References { get; }
    public DiagnosticCollection Diagnostics { get; }

    public PackageSyntax(IdentifierName name, IFixedSet<ICompilationUnitSyntax> compilationUnits, IFixedSet<ICompilationUnitSyntax> testingCompilationUnits, IFixedSet<IPackageReferenceSyntax> references, DiagnosticCollection diagnostics)
    {
        Name = name;
        CompilationUnits = compilationUnits;
        TestingCompilationUnits = testingCompilationUnits;
        References = references;
        Diagnostics = diagnostics;
    }
}

file class PackageReferenceSyntax // : IPackageReferenceSyntax
{
    public IdentifierName AliasOrName { get; }
    public IPackageSymbols Package { get; }
    public bool IsTrusted { get; }

    public PackageReferenceSyntax(IdentifierName aliasOrName, IPackageSymbols package, bool isTrusted)
    {
        AliasOrName = aliasOrName;
        Package = package;
        IsTrusted = isTrusted;
    }
}

file class NamespaceDefinitionSyntax // : INamespaceDefinitionSyntax
{
    public bool IsGlobalQualified { get; }
    public NamespaceName DeclaredNames { get; }
    public IFixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
    public IFixedList<INonMemberDefinitionSyntax> Definitions { get; }
    public CodeFile File { get; }
    public TypeName? Name { get; }
    public TextSpan NameSpan { get; }
    public TextSpan Span { get; }

    public NamespaceDefinitionSyntax(bool isGlobalQualified, NamespaceName declaredNames, IFixedList<IUsingDirectiveSyntax> usingDirectives, IFixedList<INonMemberDefinitionSyntax> definitions, CodeFile file, TypeName? name, TextSpan nameSpan, TextSpan span)
    {
        IsGlobalQualified = isGlobalQualified;
        DeclaredNames = declaredNames;
        UsingDirectives = usingDirectives;
        Definitions = definitions;
        File = file;
        Name = name;
        NameSpan = nameSpan;
        Span = span;
    }
}

file class FunctionDefinitionSyntax // : IFunctionDefinitionSyntax
{
    public IFixedList<IAttributeSyntax> Attributes { get; }
    public IdentifierName Name { get; }
    public IFixedList<INamedParameterSyntax> Parameters { get; }
    public IReturnSyntax? Return { get; }
    public IBodySyntax Body { get; }
    public IAccessModifierToken? AccessModifier { get; }
    public CodeFile File { get; }
    public TextSpan NameSpan { get; }
    public TextSpan Span { get; }

    public FunctionDefinitionSyntax(IFixedList<IAttributeSyntax> attributes, IdentifierName name, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax? @return, IBodySyntax body, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span)
    {
        Attributes = attributes;
        Name = name;
        Parameters = parameters;
        Return = @return;
        Body = body;
        AccessModifier = accessModifier;
        File = file;
        NameSpan = nameSpan;
        Span = span;
    }
}

file class ClassDefinitionSyntax // : IClassDefinitionSyntax
{
    public IAbstractKeywordToken? AbstractModifier { get; }
    public IFixedList<IGenericParameterSyntax> GenericParameters { get; }
    public IStandardTypeNameSyntax? BaseTypeName { get; }
    public IFixedList<IStandardTypeNameSyntax> SupertypeNames { get; }
    public IFixedList<IClassMemberDefinitionSyntax> Members { get; }
    public IConstKeywordToken? ConstModifier { get; }
    public IMoveKeywordToken? MoveModifier { get; }
    public StandardName Name { get; }
    public IAccessModifierToken? AccessModifier { get; }
    public CodeFile File { get; }
    public TextSpan NameSpan { get; }
    public TextSpan Span { get; }

    public ClassDefinitionSyntax(IAbstractKeywordToken? abstractModifier, IFixedList<IGenericParameterSyntax> genericParameters, IStandardTypeNameSyntax? baseTypeName, IFixedList<IStandardTypeNameSyntax> supertypeNames, IFixedList<IClassMemberDefinitionSyntax> members, IConstKeywordToken? constModifier, IMoveKeywordToken? moveModifier, StandardName name, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span)
    {
        AbstractModifier = abstractModifier;
        GenericParameters = genericParameters;
        BaseTypeName = baseTypeName;
        SupertypeNames = supertypeNames;
        Members = members;
        ConstModifier = constModifier;
        MoveModifier = moveModifier;
        Name = name;
        AccessModifier = accessModifier;
        File = file;
        NameSpan = nameSpan;
        Span = span;
    }
}

file class StructDefinitionSyntax // : IStructDefinitionSyntax
{
    public IFixedList<IGenericParameterSyntax> GenericParameters { get; }
    public IFixedList<IStandardTypeNameSyntax> SupertypeNames { get; }
    public IFixedList<IStructMemberDefinitionSyntax> Members { get; }
    public IConstKeywordToken? ConstModifier { get; }
    public IMoveKeywordToken? MoveModifier { get; }
    public StandardName Name { get; }
    public IAccessModifierToken? AccessModifier { get; }
    public CodeFile File { get; }
    public TextSpan NameSpan { get; }
    public TextSpan Span { get; }

    public StructDefinitionSyntax(IFixedList<IGenericParameterSyntax> genericParameters, IFixedList<IStandardTypeNameSyntax> supertypeNames, IFixedList<IStructMemberDefinitionSyntax> members, IConstKeywordToken? constModifier, IMoveKeywordToken? moveModifier, StandardName name, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span)
    {
        GenericParameters = genericParameters;
        SupertypeNames = supertypeNames;
        Members = members;
        ConstModifier = constModifier;
        MoveModifier = moveModifier;
        Name = name;
        AccessModifier = accessModifier;
        File = file;
        NameSpan = nameSpan;
        Span = span;
    }
}

file class TraitDefinitionSyntax // : ITraitDefinitionSyntax
{
    public IFixedList<IGenericParameterSyntax> GenericParameters { get; }
    public IFixedList<IStandardTypeNameSyntax> SupertypeNames { get; }
    public IFixedList<ITraitMemberDefinitionSyntax> Members { get; }
    public IConstKeywordToken? ConstModifier { get; }
    public IMoveKeywordToken? MoveModifier { get; }
    public StandardName Name { get; }
    public IAccessModifierToken? AccessModifier { get; }
    public CodeFile File { get; }
    public TextSpan NameSpan { get; }
    public TextSpan Span { get; }

    public TraitDefinitionSyntax(IFixedList<IGenericParameterSyntax> genericParameters, IFixedList<IStandardTypeNameSyntax> supertypeNames, IFixedList<ITraitMemberDefinitionSyntax> members, IConstKeywordToken? constModifier, IMoveKeywordToken? moveModifier, StandardName name, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span)
    {
        GenericParameters = genericParameters;
        SupertypeNames = supertypeNames;
        Members = members;
        ConstModifier = constModifier;
        MoveModifier = moveModifier;
        Name = name;
        AccessModifier = accessModifier;
        File = file;
        NameSpan = nameSpan;
        Span = span;
    }
}

file class GenericParameterSyntax // : IGenericParameterSyntax
{
    public ICapabilityConstraintSyntax Constraint { get; }
    public IdentifierName Name { get; }
    public TypeParameterIndependence Independence { get; }
    public TypeParameterVariance Variance { get; }
    public TextSpan Span { get; }

    public GenericParameterSyntax(ICapabilityConstraintSyntax constraint, IdentifierName name, TypeParameterIndependence independence, TypeParameterVariance variance, TextSpan span)
    {
        Constraint = constraint;
        Name = name;
        Independence = independence;
        Variance = variance;
        Span = span;
    }
}

file class AbstractMethodDefinitionSyntax // : IAbstractMethodDefinitionSyntax
{
    public IMethodSelfParameterSyntax SelfParameter { get; }
    public IFixedList<INamedParameterSyntax> Parameters { get; }
    public IReturnSyntax? Return { get; }
    public MethodKind Kind { get; }
    public IdentifierName Name { get; }
    public IAccessModifierToken? AccessModifier { get; }
    public CodeFile File { get; }
    public TextSpan NameSpan { get; }
    public TextSpan Span { get; }

    public AbstractMethodDefinitionSyntax(IMethodSelfParameterSyntax selfParameter, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax? @return, MethodKind kind, IdentifierName name, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span)
    {
        SelfParameter = selfParameter;
        Parameters = parameters;
        Return = @return;
        Kind = kind;
        Name = name;
        AccessModifier = accessModifier;
        File = file;
        NameSpan = nameSpan;
        Span = span;
    }
}

file class StandardMethodDefinitionSyntax // : IStandardMethodDefinitionSyntax
{
    public IMethodSelfParameterSyntax SelfParameter { get; }
    public IFixedList<INamedParameterSyntax> Parameters { get; }
    public IReturnSyntax? Return { get; }
    public IBodySyntax Body { get; }
    public MethodKind Kind { get; }
    public IdentifierName Name { get; }
    public IAccessModifierToken? AccessModifier { get; }
    public CodeFile File { get; }
    public TextSpan NameSpan { get; }
    public TextSpan Span { get; }

    public StandardMethodDefinitionSyntax(IMethodSelfParameterSyntax selfParameter, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax? @return, IBodySyntax body, MethodKind kind, IdentifierName name, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span)
    {
        SelfParameter = selfParameter;
        Parameters = parameters;
        Return = @return;
        Body = body;
        Kind = kind;
        Name = name;
        AccessModifier = accessModifier;
        File = file;
        NameSpan = nameSpan;
        Span = span;
    }
}

file class GetterMethodDefinitionSyntax // : IGetterMethodDefinitionSyntax
{
    public IMethodSelfParameterSyntax SelfParameter { get; }
    public IFixedList<INamedParameterSyntax> Parameters { get; }
    public IReturnSyntax Return { get; }
    public IBodySyntax Body { get; }
    public MethodKind Kind { get; }
    public IdentifierName Name { get; }
    public IAccessModifierToken? AccessModifier { get; }
    public CodeFile File { get; }
    public TextSpan NameSpan { get; }
    public TextSpan Span { get; }

    public GetterMethodDefinitionSyntax(IMethodSelfParameterSyntax selfParameter, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax @return, IBodySyntax body, MethodKind kind, IdentifierName name, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span)
    {
        SelfParameter = selfParameter;
        Parameters = parameters;
        Return = @return;
        Body = body;
        Kind = kind;
        Name = name;
        AccessModifier = accessModifier;
        File = file;
        NameSpan = nameSpan;
        Span = span;
    }
}

file class SetterMethodDefinitionSyntax // : ISetterMethodDefinitionSyntax
{
    public IMethodSelfParameterSyntax SelfParameter { get; }
    public IFixedList<INamedParameterSyntax> Parameters { get; }
    public IReturnSyntax? Return { get; }
    public IBodySyntax Body { get; }
    public MethodKind Kind { get; }
    public IdentifierName Name { get; }
    public IAccessModifierToken? AccessModifier { get; }
    public CodeFile File { get; }
    public TextSpan NameSpan { get; }
    public TextSpan Span { get; }

    public SetterMethodDefinitionSyntax(IMethodSelfParameterSyntax selfParameter, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax? @return, IBodySyntax body, MethodKind kind, IdentifierName name, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span)
    {
        SelfParameter = selfParameter;
        Parameters = parameters;
        Return = @return;
        Body = body;
        Kind = kind;
        Name = name;
        AccessModifier = accessModifier;
        File = file;
        NameSpan = nameSpan;
        Span = span;
    }
}

file class ConstructorDefinitionSyntax // : IConstructorDefinitionSyntax
{
    public IdentifierName? Name { get; }
    public IConstructorSelfParameterSyntax SelfParameter { get; }
    public IFixedList<IConstructorOrInitializerParameterSyntax> Parameters { get; }
    public IBlockBodySyntax Body { get; }
    public IAccessModifierToken? AccessModifier { get; }
    public CodeFile File { get; }
    public TextSpan NameSpan { get; }
    public TextSpan Span { get; }

    public ConstructorDefinitionSyntax(IdentifierName? name, IConstructorSelfParameterSyntax selfParameter, IFixedList<IConstructorOrInitializerParameterSyntax> parameters, IBlockBodySyntax body, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span)
    {
        Name = name;
        SelfParameter = selfParameter;
        Parameters = parameters;
        Body = body;
        AccessModifier = accessModifier;
        File = file;
        NameSpan = nameSpan;
        Span = span;
    }
}

file class InitializerDefinitionSyntax // : IInitializerDefinitionSyntax
{
    public IdentifierName? Name { get; }
    public IInitializerSelfParameterSyntax SelfParameter { get; }
    public IFixedList<IConstructorOrInitializerParameterSyntax> Parameters { get; }
    public IBlockBodySyntax Body { get; }
    public IAccessModifierToken? AccessModifier { get; }
    public CodeFile File { get; }
    public TextSpan NameSpan { get; }
    public TextSpan Span { get; }

    public InitializerDefinitionSyntax(IdentifierName? name, IInitializerSelfParameterSyntax selfParameter, IFixedList<IConstructorOrInitializerParameterSyntax> parameters, IBlockBodySyntax body, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span)
    {
        Name = name;
        SelfParameter = selfParameter;
        Parameters = parameters;
        Body = body;
        AccessModifier = accessModifier;
        File = file;
        NameSpan = nameSpan;
        Span = span;
    }
}

file class FieldDefinitionSyntax // : IFieldDefinitionSyntax
{
    public IdentifierName Name { get; }
    public ITypeSyntax Type { get; }
    public IExpressionSyntax? Initializer { get; }
    public IAccessModifierToken? AccessModifier { get; }
    public CodeFile File { get; }
    public TextSpan NameSpan { get; }
    public TextSpan Span { get; }
    public bool IsMutableBinding { get; }

    public FieldDefinitionSyntax(IdentifierName name, ITypeSyntax type, IExpressionSyntax? initializer, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span, bool isMutableBinding)
    {
        Name = name;
        Type = type;
        Initializer = initializer;
        AccessModifier = accessModifier;
        File = file;
        NameSpan = nameSpan;
        Span = span;
        IsMutableBinding = isMutableBinding;
    }
}

file class AssociatedFunctionDefinitionSyntax // : IAssociatedFunctionDefinitionSyntax
{
    public IdentifierName Name { get; }
    public IFixedList<INamedParameterSyntax> Parameters { get; }
    public IReturnSyntax? Return { get; }
    public IBodySyntax Body { get; }
    public IAccessModifierToken? AccessModifier { get; }
    public CodeFile File { get; }
    public TextSpan NameSpan { get; }
    public TextSpan Span { get; }

    public AssociatedFunctionDefinitionSyntax(IdentifierName name, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax? @return, IBodySyntax body, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span)
    {
        Name = name;
        Parameters = parameters;
        Return = @return;
        Body = body;
        AccessModifier = accessModifier;
        File = file;
        NameSpan = nameSpan;
        Span = span;
    }
}

file class AttributeSyntax // : IAttributeSyntax
{
    public IStandardTypeNameSyntax TypeName { get; }
    public TextSpan Span { get; }

    public AttributeSyntax(IStandardTypeNameSyntax typeName, TextSpan span)
    {
        TypeName = typeName;
        Span = span;
    }
}

file class CapabilitySetSyntax // : ICapabilitySetSyntax
{
    public CapabilitySet Constraint { get; }
    public TextSpan Span { get; }

    public CapabilitySetSyntax(CapabilitySet constraint, TextSpan span)
    {
        Constraint = constraint;
        Span = span;
    }
}

file class CapabilitySyntax // : ICapabilitySyntax
{
    public IFixedList<ICapabilityToken> Tokens { get; }
    public DeclaredCapability Declared { get; }
    public Capability Capability { get; }
    public ICapabilityConstraint Constraint { get; }
    public TextSpan Span { get; }

    public CapabilitySyntax(IFixedList<ICapabilityToken> tokens, DeclaredCapability declared, Capability capability, ICapabilityConstraint constraint, TextSpan span)
    {
        Tokens = tokens;
        Declared = declared;
        Capability = capability;
        Constraint = constraint;
        Span = span;
    }
}

file class NamedParameterSyntax // : INamedParameterSyntax
{
    public bool IsMutableBinding { get; }
    public bool IsLentBinding { get; }
    public IdentifierName Name { get; }
    public ITypeSyntax Type { get; }
    public IExpressionSyntax? DefaultValue { get; }
    public TextSpan Span { get; }
    public TextSpan NameSpan { get; }

    public NamedParameterSyntax(bool isMutableBinding, bool isLentBinding, IdentifierName name, ITypeSyntax type, IExpressionSyntax? defaultValue, TextSpan span, TextSpan nameSpan)
    {
        IsMutableBinding = isMutableBinding;
        IsLentBinding = isLentBinding;
        Name = name;
        Type = type;
        DefaultValue = defaultValue;
        Span = span;
        NameSpan = nameSpan;
    }
}

file class ConstructorSelfParameterSyntax // : IConstructorSelfParameterSyntax
{
    public ICapabilitySyntax Capability { get; }
    public bool IsLentBinding { get; }
    public IdentifierName? Name { get; }
    public TextSpan Span { get; }

    public ConstructorSelfParameterSyntax(ICapabilitySyntax capability, bool isLentBinding, IdentifierName? name, TextSpan span)
    {
        Capability = capability;
        IsLentBinding = isLentBinding;
        Name = name;
        Span = span;
    }
}

file class InitializerSelfParameterSyntax // : IInitializerSelfParameterSyntax
{
    public ICapabilitySyntax Capability { get; }
    public bool IsLentBinding { get; }
    public IdentifierName? Name { get; }
    public TextSpan Span { get; }

    public InitializerSelfParameterSyntax(ICapabilitySyntax capability, bool isLentBinding, IdentifierName? name, TextSpan span)
    {
        Capability = capability;
        IsLentBinding = isLentBinding;
        Name = name;
        Span = span;
    }
}

file class MethodSelfParameterSyntax // : IMethodSelfParameterSyntax
{
    public ICapabilityConstraintSyntax Capability { get; }
    public bool IsLentBinding { get; }
    public IdentifierName? Name { get; }
    public TextSpan Span { get; }

    public MethodSelfParameterSyntax(ICapabilityConstraintSyntax capability, bool isLentBinding, IdentifierName? name, TextSpan span)
    {
        Capability = capability;
        IsLentBinding = isLentBinding;
        Name = name;
        Span = span;
    }
}

file class FieldParameterSyntax // : IFieldParameterSyntax
{
    public IdentifierName Name { get; }
    public IExpressionSyntax? DefaultValue { get; }
    public TextSpan Span { get; }

    public FieldParameterSyntax(IdentifierName name, IExpressionSyntax? defaultValue, TextSpan span)
    {
        Name = name;
        DefaultValue = defaultValue;
        Span = span;
    }
}

file class ReturnSyntax // : IReturnSyntax
{
    public ITypeSyntax Type { get; }
    public TextSpan Span { get; }

    public ReturnSyntax(ITypeSyntax type, TextSpan span)
    {
        Type = type;
        Span = span;
    }
}

file class BlockBodySyntax // : IBlockBodySyntax
{
    public IFixedList<IBodyStatementSyntax> Statements { get; }
    public TextSpan Span { get; }

    public BlockBodySyntax(IFixedList<IBodyStatementSyntax> statements, TextSpan span)
    {
        Statements = statements;
        Span = span;
    }
}

file class ExpressionBodySyntax // : IExpressionBodySyntax
{
    public IResultStatementSyntax ResultStatement { get; }
    public IFixedList<IStatementSyntax> Statements { get; }
    public TextSpan Span { get; }

    public ExpressionBodySyntax(IResultStatementSyntax resultStatement, IFixedList<IStatementSyntax> statements, TextSpan span)
    {
        ResultStatement = resultStatement;
        Statements = statements;
        Span = span;
    }
}

file class IdentifierTypeNameSyntax // : IIdentifierTypeNameSyntax
{
    public IdentifierName Name { get; }
    public TextSpan Span { get; }

    public IdentifierTypeNameSyntax(IdentifierName name, TextSpan span)
    {
        Name = name;
        Span = span;
    }
}

file class SpecialTypeNameSyntax // : ISpecialTypeNameSyntax
{
    public SpecialTypeName Name { get; }
    public TextSpan Span { get; }

    public SpecialTypeNameSyntax(SpecialTypeName name, TextSpan span)
    {
        Name = name;
        Span = span;
    }
}

file class GenericTypeNameSyntax // : IGenericTypeNameSyntax
{
    public GenericName Name { get; }
    public IFixedList<ITypeSyntax> TypeArguments { get; }
    public TextSpan Span { get; }

    public GenericTypeNameSyntax(GenericName name, IFixedList<ITypeSyntax> typeArguments, TextSpan span)
    {
        Name = name;
        TypeArguments = typeArguments;
        Span = span;
    }
}

file class QualifiedTypeNameSyntax // : IQualifiedTypeNameSyntax
{
    public ITypeNameSyntax Context { get; }
    public IStandardTypeNameSyntax QualifiedName { get; }
    public TypeName Name { get; }
    public TextSpan Span { get; }

    public QualifiedTypeNameSyntax(ITypeNameSyntax context, IStandardTypeNameSyntax qualifiedName, TypeName name, TextSpan span)
    {
        Context = context;
        QualifiedName = qualifiedName;
        Name = name;
        Span = span;
    }
}

file class OptionalTypeSyntax // : IOptionalTypeSyntax
{
    public ITypeSyntax Referent { get; }
    public TextSpan Span { get; }

    public OptionalTypeSyntax(ITypeSyntax referent, TextSpan span)
    {
        Referent = referent;
        Span = span;
    }
}

file class CapabilityTypeSyntax // : ICapabilityTypeSyntax
{
    public ICapabilitySyntax Capability { get; }
    public ITypeSyntax Referent { get; }
    public TextSpan Span { get; }

    public CapabilityTypeSyntax(ICapabilitySyntax capability, ITypeSyntax referent, TextSpan span)
    {
        Capability = capability;
        Referent = referent;
        Span = span;
    }
}

file class FunctionTypeSyntax // : IFunctionTypeSyntax
{
    public IFixedList<IParameterTypeSyntax> Parameters { get; }
    public IReturnTypeSyntax Return { get; }
    public TextSpan Span { get; }

    public FunctionTypeSyntax(IFixedList<IParameterTypeSyntax> parameters, IReturnTypeSyntax @return, TextSpan span)
    {
        Parameters = parameters;
        Return = @return;
        Span = span;
    }
}

file class ParameterTypeSyntax // : IParameterTypeSyntax
{
    public bool IsLent { get; }
    public ITypeSyntax Referent { get; }
    public TextSpan Span { get; }

    public ParameterTypeSyntax(bool isLent, ITypeSyntax referent, TextSpan span)
    {
        IsLent = isLent;
        Referent = referent;
        Span = span;
    }
}

file class ReturnTypeSyntax // : IReturnTypeSyntax
{
    public ITypeSyntax Referent { get; }
    public TextSpan Span { get; }

    public ReturnTypeSyntax(ITypeSyntax referent, TextSpan span)
    {
        Referent = referent;
        Span = span;
    }
}

file class CapabilityViewpointTypeSyntax // : ICapabilityViewpointTypeSyntax
{
    public ICapabilitySyntax Capability { get; }
    public ITypeSyntax Referent { get; }
    public TextSpan Span { get; }

    public CapabilityViewpointTypeSyntax(ICapabilitySyntax capability, ITypeSyntax referent, TextSpan span)
    {
        Capability = capability;
        Referent = referent;
        Span = span;
    }
}

file class SelfViewpointTypeSyntax // : ISelfViewpointTypeSyntax
{
    public ITypeSyntax Referent { get; }
    public TextSpan Span { get; }

    public SelfViewpointTypeSyntax(ITypeSyntax referent, TextSpan span)
    {
        Referent = referent;
        Span = span;
    }
}

file class ResultStatementSyntax // : IResultStatementSyntax
{
    public IExpressionSyntax Expression { get; }
    public TextSpan Span { get; }

    public ResultStatementSyntax(IExpressionSyntax expression, TextSpan span)
    {
        Expression = expression;
        Span = span;
    }
}

file class VariableDeclarationStatementSyntax // : IVariableDeclarationStatementSyntax
{
    public TextSpan NameSpan { get; }
    public IdentifierName Name { get; }
    public ICapabilitySyntax? Capability { get; }
    public ITypeSyntax? Type { get; }
    public IExpressionSyntax? Initializer { get; }
    public TextSpan Span { get; }
    public bool IsMutableBinding { get; }

    public VariableDeclarationStatementSyntax(TextSpan nameSpan, IdentifierName name, ICapabilitySyntax? capability, ITypeSyntax? type, IExpressionSyntax? initializer, TextSpan span, bool isMutableBinding)
    {
        NameSpan = nameSpan;
        Name = name;
        Capability = capability;
        Type = type;
        Initializer = initializer;
        Span = span;
        IsMutableBinding = isMutableBinding;
    }
}

file class ExpressionStatementSyntax // : IExpressionStatementSyntax
{
    public IExpressionSyntax Expression { get; }
    public TextSpan Span { get; }

    public ExpressionStatementSyntax(IExpressionSyntax expression, TextSpan span)
    {
        Expression = expression;
        Span = span;
    }
}

file class BindingContextPatternSyntax // : IBindingContextPatternSyntax
{
    public bool IsMutableBinding { get; }
    public IPatternSyntax Pattern { get; }
    public ITypeSyntax? Type { get; }
    public TextSpan Span { get; }

    public BindingContextPatternSyntax(bool isMutableBinding, IPatternSyntax pattern, ITypeSyntax? type, TextSpan span)
    {
        IsMutableBinding = isMutableBinding;
        Pattern = pattern;
        Type = type;
        Span = span;
    }
}

file class BindingPatternSyntax // : IBindingPatternSyntax
{
    public IdentifierName Name { get; }
    public TextSpan Span { get; }
    public TextSpan NameSpan { get; }
    public bool IsMutableBinding { get; }

    public BindingPatternSyntax(IdentifierName name, TextSpan span, TextSpan nameSpan, bool isMutableBinding)
    {
        Name = name;
        Span = span;
        NameSpan = nameSpan;
        IsMutableBinding = isMutableBinding;
    }
}

file class OptionalPatternSyntax // : IOptionalPatternSyntax
{
    public IOptionalOrBindingPatternSyntax Pattern { get; }
    public TextSpan Span { get; }

    public OptionalPatternSyntax(IOptionalOrBindingPatternSyntax pattern, TextSpan span)
    {
        Pattern = pattern;
        Span = span;
    }
}

file class BlockExpressionSyntax // : IBlockExpressionSyntax
{
    public IFixedList<IStatementSyntax> Statements { get; }
    public TextSpan Span { get; }

    public BlockExpressionSyntax(IFixedList<IStatementSyntax> statements, TextSpan span)
    {
        Statements = statements;
        Span = span;
    }
}

file class NewObjectExpressionSyntax // : INewObjectExpressionSyntax
{
    public ITypeNameSyntax Type { get; }
    public IdentifierName? ConstructorName { get; }
    public TextSpan? ConstructorNameSpan { get; }
    public IFixedList<IExpressionSyntax> Arguments { get; }
    public TextSpan Span { get; }

    public NewObjectExpressionSyntax(ITypeNameSyntax type, IdentifierName? constructorName, TextSpan? constructorNameSpan, IFixedList<IExpressionSyntax> arguments, TextSpan span)
    {
        Type = type;
        ConstructorName = constructorName;
        ConstructorNameSpan = constructorNameSpan;
        Arguments = arguments;
        Span = span;
    }
}

file class UnsafeExpressionSyntax // : IUnsafeExpressionSyntax
{
    public IExpressionSyntax Expression { get; }
    public TextSpan Span { get; }

    public UnsafeExpressionSyntax(IExpressionSyntax expression, TextSpan span)
    {
        Expression = expression;
        Span = span;
    }
}

file class BoolLiteralExpressionSyntax // : IBoolLiteralExpressionSyntax
{
    public bool Value { get; }
    public TextSpan Span { get; }

    public BoolLiteralExpressionSyntax(bool value, TextSpan span)
    {
        Value = value;
        Span = span;
    }
}

file class IntegerLiteralExpressionSyntax // : IIntegerLiteralExpressionSyntax
{
    public BigInteger Value { get; }
    public TextSpan Span { get; }

    public IntegerLiteralExpressionSyntax(BigInteger value, TextSpan span)
    {
        Value = value;
        Span = span;
    }
}

file class NoneLiteralExpressionSyntax // : INoneLiteralExpressionSyntax
{
    public TextSpan Span { get; }

    public NoneLiteralExpressionSyntax(TextSpan span)
    {
        Span = span;
    }
}

file class StringLiteralExpressionSyntax // : IStringLiteralExpressionSyntax
{
    public string Value { get; }
    public TextSpan Span { get; }

    public StringLiteralExpressionSyntax(string value, TextSpan span)
    {
        Value = value;
        Span = span;
    }
}

file class AssignmentExpressionSyntax // : IAssignmentExpressionSyntax
{
    public IAssignableExpressionSyntax LeftOperand { get; }
    public AssignmentOperator Operator { get; }
    public IExpressionSyntax RightOperand { get; }
    public TextSpan Span { get; }

    public AssignmentExpressionSyntax(IAssignableExpressionSyntax leftOperand, AssignmentOperator @operator, IExpressionSyntax rightOperand, TextSpan span)
    {
        LeftOperand = leftOperand;
        Operator = @operator;
        RightOperand = rightOperand;
        Span = span;
    }
}

file class BinaryOperatorExpressionSyntax // : IBinaryOperatorExpressionSyntax
{
    public IExpressionSyntax LeftOperand { get; }
    public BinaryOperator Operator { get; }
    public IExpressionSyntax RightOperand { get; }
    public TextSpan Span { get; }

    public BinaryOperatorExpressionSyntax(IExpressionSyntax leftOperand, BinaryOperator @operator, IExpressionSyntax rightOperand, TextSpan span)
    {
        LeftOperand = leftOperand;
        Operator = @operator;
        RightOperand = rightOperand;
        Span = span;
    }
}

file class UnaryOperatorExpressionSyntax // : IUnaryOperatorExpressionSyntax
{
    public UnaryOperatorFixity Fixity { get; }
    public UnaryOperator Operator { get; }
    public IExpressionSyntax Operand { get; }
    public TextSpan Span { get; }

    public UnaryOperatorExpressionSyntax(UnaryOperatorFixity fixity, UnaryOperator @operator, IExpressionSyntax operand, TextSpan span)
    {
        Fixity = fixity;
        Operator = @operator;
        Operand = operand;
        Span = span;
    }
}

file class IdExpressionSyntax // : IIdExpressionSyntax
{
    public IExpressionSyntax Referent { get; }
    public TextSpan Span { get; }

    public IdExpressionSyntax(IExpressionSyntax referent, TextSpan span)
    {
        Referent = referent;
        Span = span;
    }
}

file class ConversionExpressionSyntax // : IConversionExpressionSyntax
{
    public IExpressionSyntax Referent { get; }
    public ConversionOperator Operator { get; }
    public ITypeSyntax ConvertToType { get; }
    public TextSpan Span { get; }

    public ConversionExpressionSyntax(IExpressionSyntax referent, ConversionOperator @operator, ITypeSyntax convertToType, TextSpan span)
    {
        Referent = referent;
        Operator = @operator;
        ConvertToType = convertToType;
        Span = span;
    }
}

file class PatternMatchExpressionSyntax // : IPatternMatchExpressionSyntax
{
    public IExpressionSyntax Referent { get; }
    public IPatternSyntax Pattern { get; }
    public TextSpan Span { get; }

    public PatternMatchExpressionSyntax(IExpressionSyntax referent, IPatternSyntax pattern, TextSpan span)
    {
        Referent = referent;
        Pattern = pattern;
        Span = span;
    }
}

file class IfExpressionSyntax // : IIfExpressionSyntax
{
    public IExpressionSyntax Condition { get; }
    public IBlockOrResultSyntax ThenBlock { get; }
    public IElseClauseSyntax? ElseClause { get; }
    public TextSpan Span { get; }

    public IfExpressionSyntax(IExpressionSyntax condition, IBlockOrResultSyntax thenBlock, IElseClauseSyntax? elseClause, TextSpan span)
    {
        Condition = condition;
        ThenBlock = thenBlock;
        ElseClause = elseClause;
        Span = span;
    }
}

file class LoopExpressionSyntax // : ILoopExpressionSyntax
{
    public IBlockExpressionSyntax Block { get; }
    public TextSpan Span { get; }

    public LoopExpressionSyntax(IBlockExpressionSyntax block, TextSpan span)
    {
        Block = block;
        Span = span;
    }
}

file class WhileExpressionSyntax // : IWhileExpressionSyntax
{
    public IExpressionSyntax Condition { get; }
    public IBlockExpressionSyntax Block { get; }
    public TextSpan Span { get; }

    public WhileExpressionSyntax(IExpressionSyntax condition, IBlockExpressionSyntax block, TextSpan span)
    {
        Condition = condition;
        Block = block;
        Span = span;
    }
}

file class ForeachExpressionSyntax // : IForeachExpressionSyntax
{
    public IdentifierName VariableName { get; }
    public IExpressionSyntax InExpression { get; }
    public ITypeSyntax? Type { get; }
    public IBlockExpressionSyntax Block { get; }
    public TextSpan Span { get; }
    public TextSpan NameSpan { get; }
    public bool IsMutableBinding { get; }

    public ForeachExpressionSyntax(IdentifierName variableName, IExpressionSyntax inExpression, ITypeSyntax? type, IBlockExpressionSyntax block, TextSpan span, TextSpan nameSpan, bool isMutableBinding)
    {
        VariableName = variableName;
        InExpression = inExpression;
        Type = type;
        Block = block;
        Span = span;
        NameSpan = nameSpan;
        IsMutableBinding = isMutableBinding;
    }
}

file class BreakExpressionSyntax // : IBreakExpressionSyntax
{
    public IExpressionSyntax? Value { get; }
    public TextSpan Span { get; }

    public BreakExpressionSyntax(IExpressionSyntax? value, TextSpan span)
    {
        Value = value;
        Span = span;
    }
}

file class NextExpressionSyntax // : INextExpressionSyntax
{
    public TextSpan Span { get; }

    public NextExpressionSyntax(TextSpan span)
    {
        Span = span;
    }
}

file class ReturnExpressionSyntax // : IReturnExpressionSyntax
{
    public IExpressionSyntax? Value { get; }
    public TextSpan Span { get; }

    public ReturnExpressionSyntax(IExpressionSyntax? value, TextSpan span)
    {
        Value = value;
        Span = span;
    }
}

file class InvocationExpressionSyntax // : IInvocationExpressionSyntax
{
    public IExpressionSyntax Expression { get; }
    public IFixedList<IExpressionSyntax> Arguments { get; }
    public TextSpan Span { get; }

    public InvocationExpressionSyntax(IExpressionSyntax expression, IFixedList<IExpressionSyntax> arguments, TextSpan span)
    {
        Expression = expression;
        Arguments = arguments;
        Span = span;
    }
}

file class IdentifierNameExpressionSyntax // : IIdentifierNameExpressionSyntax
{
    public IdentifierName Name { get; }
    public TextSpan Span { get; }

    public IdentifierNameExpressionSyntax(IdentifierName name, TextSpan span)
    {
        Name = name;
        Span = span;
    }
}

file class SpecialTypeNameExpressionSyntax // : ISpecialTypeNameExpressionSyntax
{
    public SpecialTypeName Name { get; }
    public TextSpan Span { get; }

    public SpecialTypeNameExpressionSyntax(SpecialTypeName name, TextSpan span)
    {
        Name = name;
        Span = span;
    }
}

file class GenericNameExpressionSyntax // : IGenericNameExpressionSyntax
{
    public GenericName Name { get; }
    public IFixedList<ITypeSyntax> TypeArguments { get; }
    public TextSpan Span { get; }

    public GenericNameExpressionSyntax(GenericName name, IFixedList<ITypeSyntax> typeArguments, TextSpan span)
    {
        Name = name;
        TypeArguments = typeArguments;
        Span = span;
    }
}

file class SelfExpressionSyntax // : ISelfExpressionSyntax
{
    public bool IsImplicit { get; }
    public TextSpan Span { get; }

    public SelfExpressionSyntax(bool isImplicit, TextSpan span)
    {
        IsImplicit = isImplicit;
        Span = span;
    }
}

file class MemberAccessExpressionSyntax // : IMemberAccessExpressionSyntax
{
    public IExpressionSyntax Context { get; }
    public StandardName MemberName { get; }
    public IFixedList<ITypeSyntax> TypeArguments { get; }
    public TextSpan MemberNameSpan { get; }
    public TextSpan Span { get; }

    public MemberAccessExpressionSyntax(IExpressionSyntax context, StandardName memberName, IFixedList<ITypeSyntax> typeArguments, TextSpan memberNameSpan, TextSpan span)
    {
        Context = context;
        MemberName = memberName;
        TypeArguments = typeArguments;
        MemberNameSpan = memberNameSpan;
        Span = span;
    }
}

file class MissingNameSyntax // : IMissingNameSyntax
{
    public TextSpan Span { get; }

    public MissingNameSyntax(TextSpan span)
    {
        Span = span;
    }
}

file class MoveExpressionSyntax // : IMoveExpressionSyntax
{
    public ISimpleNameSyntax Referent { get; }
    public TextSpan Span { get; }

    public MoveExpressionSyntax(ISimpleNameSyntax referent, TextSpan span)
    {
        Referent = referent;
        Span = span;
    }
}

file class FreezeExpressionSyntax // : IFreezeExpressionSyntax
{
    public ISimpleNameSyntax Referent { get; }
    public TextSpan Span { get; }

    public FreezeExpressionSyntax(ISimpleNameSyntax referent, TextSpan span)
    {
        Referent = referent;
        Span = span;
    }
}

file class AsyncBlockExpressionSyntax // : IAsyncBlockExpressionSyntax
{
    public IBlockExpressionSyntax Block { get; }
    public TextSpan Span { get; }

    public AsyncBlockExpressionSyntax(IBlockExpressionSyntax block, TextSpan span)
    {
        Block = block;
        Span = span;
    }
}

file class AsyncStartExpressionSyntax // : IAsyncStartExpressionSyntax
{
    public bool Scheduled { get; }
    public IExpressionSyntax Expression { get; }
    public TextSpan Span { get; }

    public AsyncStartExpressionSyntax(bool scheduled, IExpressionSyntax expression, TextSpan span)
    {
        Scheduled = scheduled;
        Expression = expression;
        Span = span;
    }
}

file class AwaitExpressionSyntax // : IAwaitExpressionSyntax
{
    public IExpressionSyntax Expression { get; }
    public TextSpan Span { get; }

    public AwaitExpressionSyntax(IExpressionSyntax expression, TextSpan span)
    {
        Expression = expression;
        Span = span;
    }
}

