using System.CodeDom.Compiler;
using System.Diagnostics;
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
// ReSharper disable RedundantTypeDeclarationBody
// ReSharper disable ReturnTypeCanBeNotNullable

[Closed(
    typeof(ICodeSyntax),
    typeof(IPackageSyntax),
    typeof(IPackageReferenceSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
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
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICodeSyntax : ISyntax
{
    TextSpan Span { get; }
}

// [Closed(typeof(CompilationUnitSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICompilationUnitSyntax : ICodeSyntax
{
    CodeFile File { get; }
    NamespaceName ImplicitNamespaceName { get; }
    DiagnosticCollection Diagnostics { get; }
    IFixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
    IFixedList<INonMemberDefinitionSyntax> Definitions { get; }

    public static ICompilationUnitSyntax Create(CodeFile file, NamespaceName implicitNamespaceName, DiagnosticCollection diagnostics, IFixedList<IUsingDirectiveSyntax> usingDirectives, IFixedList<INonMemberDefinitionSyntax> definitions, TextSpan span)
        => new CompilationUnitSyntax(file, implicitNamespaceName, diagnostics, usingDirectives, definitions, span);
}

// [Closed(typeof(UsingDirectiveSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUsingDirectiveSyntax : ICodeSyntax
{
    NamespaceName Name { get; }

    public static IUsingDirectiveSyntax Create(NamespaceName name, TextSpan span)
        => new UsingDirectiveSyntax(name, span);
}

[Closed(
    typeof(IBodySyntax),
    typeof(IBlockExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBodyOrBlockSyntax : ICodeSyntax
{
    IFixedList<IStatementSyntax> Statements { get; }
}

[Closed(
    typeof(IBlockOrResultSyntax),
    typeof(IIfExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IElseClauseSyntax : ICodeSyntax
{
}

[Closed(
    typeof(IResultStatementSyntax),
    typeof(IBlockExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBlockOrResultSyntax : IElseClauseSyntax
{
}

[Closed(
    typeof(ILocalBindingSyntax),
    typeof(IFieldDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBindingSyntax : ICodeSyntax
{
    bool IsMutableBinding { get; }
}

[Closed(
    typeof(INamedParameterSyntax),
    typeof(IVariableDeclarationStatementSyntax),
    typeof(IBindingPatternSyntax),
    typeof(IForeachExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ILocalBindingSyntax : IBindingSyntax
{
    TextSpan NameSpan { get; }
}

// [Closed(typeof(PackageSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPackageSyntax : ISyntax
{
    IdentifierName Name { get; }
    IFixedSet<ICompilationUnitSyntax> CompilationUnits { get; }
    IFixedSet<ICompilationUnitSyntax> TestingCompilationUnits { get; }
    IFixedSet<IPackageReferenceSyntax> References { get; }
    DiagnosticCollection Diagnostics { get; }

    public static IPackageSyntax Create(IdentifierName name, IFixedSet<ICompilationUnitSyntax> compilationUnits, IFixedSet<ICompilationUnitSyntax> testingCompilationUnits, IFixedSet<IPackageReferenceSyntax> references, DiagnosticCollection diagnostics)
        => new PackageSyntax(name, compilationUnits, testingCompilationUnits, references, diagnostics);
}

// [Closed(typeof(PackageReferenceSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPackageReferenceSyntax : ISyntax
{
    IdentifierName AliasOrName { get; }
    IPackageSymbols Package { get; }
    bool IsTrusted { get; }

    public static IPackageReferenceSyntax Create(IdentifierName aliasOrName, IPackageSymbols package, bool isTrusted)
        => new PackageReferenceSyntax(aliasOrName, package, isTrusted);
}

[Closed(
    typeof(IEntityDefinitionSyntax),
    typeof(INonMemberDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IDefinitionSyntax : ICodeSyntax
{
    CodeFile File { get; }
    TypeName? Name { get; }
    TextSpan NameSpan { get; }
}

[Closed(
    typeof(IInvocableDefinitionSyntax),
    typeof(ITypeMemberDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IEntityDefinitionSyntax : IDefinitionSyntax
{
    IAccessModifierToken? AccessModifier { get; }
}

[Closed(
    typeof(IConcreteInvocableDefinitionSyntax),
    typeof(IMethodDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
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
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IConcreteInvocableDefinitionSyntax : IInvocableDefinitionSyntax
{
    IBodySyntax Body { get; }
}

[Closed(
    typeof(INamespaceDefinitionSyntax),
    typeof(IFunctionDefinitionSyntax),
    typeof(ITypeDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INonMemberDefinitionSyntax : IDefinitionSyntax
{
}

// [Closed(typeof(NamespaceDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamespaceDefinitionSyntax : INonMemberDefinitionSyntax
{
    bool IsGlobalQualified { get; }
    NamespaceName DeclaredNames { get; }
    IFixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
    IFixedList<INonMemberDefinitionSyntax> Definitions { get; }

    public static INamespaceDefinitionSyntax Create(bool isGlobalQualified, NamespaceName declaredNames, IFixedList<IUsingDirectiveSyntax> usingDirectives, IFixedList<INonMemberDefinitionSyntax> definitions, CodeFile file, TypeName? name, TextSpan nameSpan, TextSpan span)
        => new NamespaceDefinitionSyntax(isGlobalQualified, declaredNames, usingDirectives, definitions, file, name, nameSpan, span);
}

// [Closed(typeof(FunctionDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionDefinitionSyntax : IConcreteInvocableDefinitionSyntax, INonMemberDefinitionSyntax
{
    IFixedList<IAttributeSyntax> Attributes { get; }
    new IdentifierName Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    new IFixedList<INamedParameterSyntax> Parameters { get; }
    IFixedList<IConstructorOrInitializerParameterSyntax> IInvocableDefinitionSyntax.Parameters => Parameters;
    IReturnSyntax? Return { get; }

    public static IFunctionDefinitionSyntax Create(IFixedList<IAttributeSyntax> attributes, IdentifierName name, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax? @return, IBodySyntax body, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span)
        => new FunctionDefinitionSyntax(attributes, name, parameters, @return, body, accessModifier, file, nameSpan, span);
}

[Closed(
    typeof(IClassDefinitionSyntax),
    typeof(IStructDefinitionSyntax),
    typeof(ITraitDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITypeDefinitionSyntax : INonMemberDefinitionSyntax, IClassMemberDefinitionSyntax, ITraitMemberDefinitionSyntax, IStructMemberDefinitionSyntax
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
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IClassDefinitionSyntax : ITypeDefinitionSyntax
{
    IAbstractKeywordToken? AbstractModifier { get; }
    IStandardTypeNameSyntax? BaseTypeName { get; }
    new IFixedList<IClassMemberDefinitionSyntax> Members { get; }
    IFixedList<ITypeMemberDefinitionSyntax> ITypeDefinitionSyntax.Members => Members;

    public static IClassDefinitionSyntax Create(IAbstractKeywordToken? abstractModifier, IFixedList<IGenericParameterSyntax> genericParameters, IStandardTypeNameSyntax? baseTypeName, IFixedList<IStandardTypeNameSyntax> supertypeNames, IFixedList<IClassMemberDefinitionSyntax> members, IConstKeywordToken? constModifier, IMoveKeywordToken? moveModifier, StandardName name, CodeFile file, TextSpan nameSpan, TextSpan span, IAccessModifierToken? accessModifier)
        => new ClassDefinitionSyntax(abstractModifier, genericParameters, baseTypeName, supertypeNames, members, constModifier, moveModifier, name, file, nameSpan, span, accessModifier);
}

// [Closed(typeof(StructDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStructDefinitionSyntax : ITypeDefinitionSyntax
{
    new IFixedList<IStructMemberDefinitionSyntax> Members { get; }
    IFixedList<ITypeMemberDefinitionSyntax> ITypeDefinitionSyntax.Members => Members;

    public static IStructDefinitionSyntax Create(IFixedList<IGenericParameterSyntax> genericParameters, IFixedList<IStandardTypeNameSyntax> supertypeNames, IFixedList<IStructMemberDefinitionSyntax> members, IConstKeywordToken? constModifier, IMoveKeywordToken? moveModifier, StandardName name, CodeFile file, TextSpan nameSpan, TextSpan span, IAccessModifierToken? accessModifier)
        => new StructDefinitionSyntax(genericParameters, supertypeNames, members, constModifier, moveModifier, name, file, nameSpan, span, accessModifier);
}

// [Closed(typeof(TraitDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITraitDefinitionSyntax : ITypeDefinitionSyntax
{
    new IFixedList<ITraitMemberDefinitionSyntax> Members { get; }
    IFixedList<ITypeMemberDefinitionSyntax> ITypeDefinitionSyntax.Members => Members;

    public static ITraitDefinitionSyntax Create(IFixedList<IGenericParameterSyntax> genericParameters, IFixedList<IStandardTypeNameSyntax> supertypeNames, IFixedList<ITraitMemberDefinitionSyntax> members, IConstKeywordToken? constModifier, IMoveKeywordToken? moveModifier, StandardName name, CodeFile file, TextSpan nameSpan, TextSpan span, IAccessModifierToken? accessModifier)
        => new TraitDefinitionSyntax(genericParameters, supertypeNames, members, constModifier, moveModifier, name, file, nameSpan, span, accessModifier);
}

// [Closed(typeof(GenericParameterSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGenericParameterSyntax : ICodeSyntax
{
    ICapabilityConstraintSyntax Constraint { get; }
    IdentifierName Name { get; }
    TypeParameterIndependence Independence { get; }
    TypeParameterVariance Variance { get; }

    public static IGenericParameterSyntax Create(ICapabilityConstraintSyntax constraint, IdentifierName name, TypeParameterIndependence independence, TypeParameterVariance variance, TextSpan span)
        => new GenericParameterSyntax(constraint, name, independence, variance, span);
}

[Closed(
    typeof(IClassMemberDefinitionSyntax),
    typeof(ITraitMemberDefinitionSyntax),
    typeof(IStructMemberDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITypeMemberDefinitionSyntax : IEntityDefinitionSyntax
{
}

[Closed(
    typeof(ITypeDefinitionSyntax),
    typeof(IMethodDefinitionSyntax),
    typeof(IConstructorDefinitionSyntax),
    typeof(IFieldDefinitionSyntax),
    typeof(IAssociatedFunctionDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IClassMemberDefinitionSyntax : ITypeMemberDefinitionSyntax
{
}

[Closed(
    typeof(ITypeDefinitionSyntax),
    typeof(IMethodDefinitionSyntax),
    typeof(IAssociatedFunctionDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITraitMemberDefinitionSyntax : ITypeMemberDefinitionSyntax
{
}

[Closed(
    typeof(ITypeDefinitionSyntax),
    typeof(IConcreteMethodDefinitionSyntax),
    typeof(IInitializerDefinitionSyntax),
    typeof(IFieldDefinitionSyntax),
    typeof(IAssociatedFunctionDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStructMemberDefinitionSyntax : ITypeMemberDefinitionSyntax
{
}

[Closed(
    typeof(IAbstractMethodDefinitionSyntax),
    typeof(IConcreteMethodDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMethodDefinitionSyntax : IClassMemberDefinitionSyntax, ITraitMemberDefinitionSyntax, IInvocableDefinitionSyntax
{
    new IdentifierName Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    IMethodSelfParameterSyntax SelfParameter { get; }
    new IFixedList<INamedParameterSyntax> Parameters { get; }
    IFixedList<IConstructorOrInitializerParameterSyntax> IInvocableDefinitionSyntax.Parameters => Parameters;
    IReturnSyntax? Return { get; }
}

// [Closed(typeof(AbstractMethodDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAbstractMethodDefinitionSyntax : IMethodDefinitionSyntax
{

    public static IAbstractMethodDefinitionSyntax Create(IMethodSelfParameterSyntax selfParameter, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax? @return, IdentifierName name, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span)
        => new AbstractMethodDefinitionSyntax(selfParameter, parameters, @return, name, accessModifier, file, nameSpan, span);
}

[Closed(
    typeof(IStandardMethodDefinitionSyntax),
    typeof(IGetterMethodDefinitionSyntax),
    typeof(ISetterMethodDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IConcreteMethodDefinitionSyntax : IMethodDefinitionSyntax, IStructMemberDefinitionSyntax, IConcreteInvocableDefinitionSyntax
{
}

// [Closed(typeof(StandardMethodDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStandardMethodDefinitionSyntax : IConcreteMethodDefinitionSyntax
{

    public static IStandardMethodDefinitionSyntax Create(IMethodSelfParameterSyntax selfParameter, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax? @return, IBodySyntax body, IdentifierName name, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span)
        => new StandardMethodDefinitionSyntax(selfParameter, parameters, @return, body, name, accessModifier, file, nameSpan, span);
}

// [Closed(typeof(GetterMethodDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGetterMethodDefinitionSyntax : IConcreteMethodDefinitionSyntax
{
    new IReturnSyntax Return { get; }
    IReturnSyntax? IMethodDefinitionSyntax.Return => Return;

    public static IGetterMethodDefinitionSyntax Create(IMethodSelfParameterSyntax selfParameter, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax @return, IBodySyntax body, IdentifierName name, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span)
        => new GetterMethodDefinitionSyntax(selfParameter, parameters, @return, body, name, accessModifier, file, nameSpan, span);
}

// [Closed(typeof(SetterMethodDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISetterMethodDefinitionSyntax : IConcreteMethodDefinitionSyntax
{

    public static ISetterMethodDefinitionSyntax Create(IMethodSelfParameterSyntax selfParameter, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax? @return, IBodySyntax body, IdentifierName name, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span)
        => new SetterMethodDefinitionSyntax(selfParameter, parameters, @return, body, name, accessModifier, file, nameSpan, span);
}

// [Closed(typeof(ConstructorDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IConstructorDefinitionSyntax : IConcreteInvocableDefinitionSyntax, IClassMemberDefinitionSyntax
{
    new IdentifierName? Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    IConstructorSelfParameterSyntax SelfParameter { get; }
    new IBlockBodySyntax Body { get; }
    IBodySyntax IConcreteInvocableDefinitionSyntax.Body => Body;

    public static IConstructorDefinitionSyntax Create(IdentifierName? name, IConstructorSelfParameterSyntax selfParameter, IFixedList<IConstructorOrInitializerParameterSyntax> parameters, IBlockBodySyntax body, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span)
        => new ConstructorDefinitionSyntax(name, selfParameter, parameters, body, accessModifier, file, nameSpan, span);
}

// [Closed(typeof(InitializerDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInitializerDefinitionSyntax : IConcreteInvocableDefinitionSyntax, IStructMemberDefinitionSyntax
{
    new IdentifierName? Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    IInitializerSelfParameterSyntax SelfParameter { get; }
    new IBlockBodySyntax Body { get; }
    IBodySyntax IConcreteInvocableDefinitionSyntax.Body => Body;

    public static IInitializerDefinitionSyntax Create(IdentifierName? name, IInitializerSelfParameterSyntax selfParameter, IFixedList<IConstructorOrInitializerParameterSyntax> parameters, IBlockBodySyntax body, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span)
        => new InitializerDefinitionSyntax(name, selfParameter, parameters, body, accessModifier, file, nameSpan, span);
}

// [Closed(typeof(FieldDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFieldDefinitionSyntax : IClassMemberDefinitionSyntax, IStructMemberDefinitionSyntax, IBindingSyntax
{
    new IdentifierName Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    ITypeSyntax Type { get; }
    IExpressionSyntax? Initializer { get; }

    public static IFieldDefinitionSyntax Create(IdentifierName name, ITypeSyntax type, IExpressionSyntax? initializer, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span, bool isMutableBinding)
        => new FieldDefinitionSyntax(name, type, initializer, accessModifier, file, nameSpan, span, isMutableBinding);
}

// [Closed(typeof(AssociatedFunctionDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAssociatedFunctionDefinitionSyntax : IClassMemberDefinitionSyntax, ITraitMemberDefinitionSyntax, IStructMemberDefinitionSyntax, IConcreteInvocableDefinitionSyntax
{
    new IdentifierName Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    new IFixedList<INamedParameterSyntax> Parameters { get; }
    IFixedList<IConstructorOrInitializerParameterSyntax> IInvocableDefinitionSyntax.Parameters => Parameters;
    IReturnSyntax? Return { get; }

    public static IAssociatedFunctionDefinitionSyntax Create(IdentifierName name, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax? @return, IBodySyntax body, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span)
        => new AssociatedFunctionDefinitionSyntax(name, parameters, @return, body, accessModifier, file, nameSpan, span);
}

// [Closed(typeof(AttributeSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAttributeSyntax : ICodeSyntax
{
    IStandardTypeNameSyntax TypeName { get; }

    public static IAttributeSyntax Create(IStandardTypeNameSyntax typeName, TextSpan span)
        => new AttributeSyntax(typeName, span);
}

[Closed(
    typeof(ICapabilitySetSyntax),
    typeof(ICapabilitySyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICapabilityConstraintSyntax : ICodeSyntax
{
    ICapabilityConstraint Constraint { get; }
}

// [Closed(typeof(CapabilitySetSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICapabilitySetSyntax : ICapabilityConstraintSyntax
{
    new CapabilitySet Constraint { get; }
    ICapabilityConstraint ICapabilityConstraintSyntax.Constraint => Constraint;

    public static ICapabilitySetSyntax Create(CapabilitySet constraint, TextSpan span)
        => new CapabilitySetSyntax(constraint, span);
}

// [Closed(typeof(CapabilitySyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICapabilitySyntax : ICapabilityConstraintSyntax
{
    IFixedList<ICapabilityToken> Tokens { get; }
    DeclaredCapability Declared { get; }
    Capability Capability { get; }

    public static ICapabilitySyntax Create(IFixedList<ICapabilityToken> tokens, DeclaredCapability declared, Capability capability, ICapabilityConstraint constraint, TextSpan span)
        => new CapabilitySyntax(tokens, declared, capability, constraint, span);
}

[Closed(
    typeof(IConstructorOrInitializerParameterSyntax),
    typeof(ISelfParameterSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IParameterSyntax : ICodeSyntax
{
    IdentifierName? Name { get; }
}

[Closed(
    typeof(INamedParameterSyntax),
    typeof(IFieldParameterSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IConstructorOrInitializerParameterSyntax : IParameterSyntax
{
}

// [Closed(typeof(NamedParameterSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamedParameterSyntax : IConstructorOrInitializerParameterSyntax, ILocalBindingSyntax
{
    bool IsLentBinding { get; }
    new IdentifierName Name { get; }
    IdentifierName? IParameterSyntax.Name => Name;
    ITypeSyntax Type { get; }
    IExpressionSyntax? DefaultValue { get; }

    public static INamedParameterSyntax Create(bool isMutableBinding, bool isLentBinding, IdentifierName name, ITypeSyntax type, IExpressionSyntax? defaultValue, TextSpan span, TextSpan nameSpan)
        => new NamedParameterSyntax(isMutableBinding, isLentBinding, name, type, defaultValue, span, nameSpan);
}

[Closed(
    typeof(IConstructorSelfParameterSyntax),
    typeof(IInitializerSelfParameterSyntax),
    typeof(IMethodSelfParameterSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISelfParameterSyntax : IParameterSyntax
{
    bool IsLentBinding { get; }
}

// [Closed(typeof(ConstructorSelfParameterSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IConstructorSelfParameterSyntax : ISelfParameterSyntax
{
    ICapabilitySyntax Capability { get; }

    public static IConstructorSelfParameterSyntax Create(ICapabilitySyntax capability, bool isLentBinding, IdentifierName? name, TextSpan span)
        => new ConstructorSelfParameterSyntax(capability, isLentBinding, name, span);
}

// [Closed(typeof(InitializerSelfParameterSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInitializerSelfParameterSyntax : ISelfParameterSyntax
{
    ICapabilitySyntax Capability { get; }

    public static IInitializerSelfParameterSyntax Create(ICapabilitySyntax capability, bool isLentBinding, IdentifierName? name, TextSpan span)
        => new InitializerSelfParameterSyntax(capability, isLentBinding, name, span);
}

// [Closed(typeof(MethodSelfParameterSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMethodSelfParameterSyntax : ISelfParameterSyntax
{
    ICapabilityConstraintSyntax Capability { get; }

    public static IMethodSelfParameterSyntax Create(ICapabilityConstraintSyntax capability, bool isLentBinding, IdentifierName? name, TextSpan span)
        => new MethodSelfParameterSyntax(capability, isLentBinding, name, span);
}

// [Closed(typeof(FieldParameterSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFieldParameterSyntax : IConstructorOrInitializerParameterSyntax
{
    new IdentifierName Name { get; }
    IdentifierName? IParameterSyntax.Name => Name;
    IExpressionSyntax? DefaultValue { get; }

    public static IFieldParameterSyntax Create(IdentifierName name, IExpressionSyntax? defaultValue, TextSpan span)
        => new FieldParameterSyntax(name, defaultValue, span);
}

// [Closed(typeof(ReturnSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IReturnSyntax : ICodeSyntax
{
    ITypeSyntax Type { get; }

    public static IReturnSyntax Create(ITypeSyntax type, TextSpan span)
        => new ReturnSyntax(type, span);
}

[Closed(
    typeof(IBlockBodySyntax),
    typeof(IExpressionBodySyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBodySyntax : IBodyOrBlockSyntax
{
}

// [Closed(typeof(BlockBodySyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBlockBodySyntax : IBodySyntax
{
    new IFixedList<IBodyStatementSyntax> Statements { get; }
    IFixedList<IStatementSyntax> IBodyOrBlockSyntax.Statements => Statements;

    public static IBlockBodySyntax Create(IFixedList<IBodyStatementSyntax> statements, TextSpan span)
        => new BlockBodySyntax(statements, span);
}

// [Closed(typeof(ExpressionBodySyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IExpressionBodySyntax : IBodySyntax
{
    IResultStatementSyntax ResultStatement { get; }

    public static IExpressionBodySyntax Create(IResultStatementSyntax resultStatement, IFixedList<IStatementSyntax> statements, TextSpan span)
        => new ExpressionBodySyntax(resultStatement, statements, span);
}

[Closed(
    typeof(ITypeNameSyntax),
    typeof(IOptionalTypeSyntax),
    typeof(ICapabilityTypeSyntax),
    typeof(IFunctionTypeSyntax),
    typeof(IViewpointTypeSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITypeSyntax : ICodeSyntax
{
}

[Closed(
    typeof(IStandardTypeNameSyntax),
    typeof(ISimpleTypeNameSyntax),
    typeof(IQualifiedTypeNameSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITypeNameSyntax : ITypeSyntax
{
    TypeName Name { get; }
}

[Closed(
    typeof(IIdentifierTypeNameSyntax),
    typeof(IGenericTypeNameSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStandardTypeNameSyntax : ITypeNameSyntax
{
    new StandardName Name { get; }
    TypeName ITypeNameSyntax.Name => Name;
}

[Closed(
    typeof(IIdentifierTypeNameSyntax),
    typeof(ISpecialTypeNameSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISimpleTypeNameSyntax : ITypeNameSyntax
{
}

// [Closed(typeof(IdentifierTypeNameSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IIdentifierTypeNameSyntax : IStandardTypeNameSyntax, ISimpleTypeNameSyntax
{
    new IdentifierName Name { get; }
    StandardName IStandardTypeNameSyntax.Name => Name;

    public static IIdentifierTypeNameSyntax Create(IdentifierName name, TextSpan span)
        => new IdentifierTypeNameSyntax(name, span);
}

// [Closed(typeof(SpecialTypeNameSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISpecialTypeNameSyntax : ISimpleTypeNameSyntax
{
    new SpecialTypeName Name { get; }
    TypeName ITypeNameSyntax.Name => Name;

    public static ISpecialTypeNameSyntax Create(SpecialTypeName name, TextSpan span)
        => new SpecialTypeNameSyntax(name, span);
}

// [Closed(typeof(GenericTypeNameSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGenericTypeNameSyntax : IStandardTypeNameSyntax
{
    new GenericName Name { get; }
    StandardName IStandardTypeNameSyntax.Name => Name;
    IFixedList<ITypeSyntax> TypeArguments { get; }

    public static IGenericTypeNameSyntax Create(GenericName name, IFixedList<ITypeSyntax> typeArguments, TextSpan span)
        => new GenericTypeNameSyntax(name, typeArguments, span);
}

// [Closed(typeof(QualifiedTypeNameSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IQualifiedTypeNameSyntax : ITypeNameSyntax
{
    ITypeNameSyntax Context { get; }
    IStandardTypeNameSyntax QualifiedName { get; }

    public static IQualifiedTypeNameSyntax Create(ITypeNameSyntax context, IStandardTypeNameSyntax qualifiedName, TypeName name, TextSpan span)
        => new QualifiedTypeNameSyntax(context, qualifiedName, name, span);
}

// [Closed(typeof(OptionalTypeSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IOptionalTypeSyntax : ITypeSyntax
{
    ITypeSyntax Referent { get; }

    public static IOptionalTypeSyntax Create(ITypeSyntax referent, TextSpan span)
        => new OptionalTypeSyntax(referent, span);
}

// [Closed(typeof(CapabilityTypeSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICapabilityTypeSyntax : ITypeSyntax
{
    ICapabilitySyntax Capability { get; }
    ITypeSyntax Referent { get; }

    public static ICapabilityTypeSyntax Create(ICapabilitySyntax capability, ITypeSyntax referent, TextSpan span)
        => new CapabilityTypeSyntax(capability, referent, span);
}

// [Closed(typeof(FunctionTypeSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionTypeSyntax : ITypeSyntax
{
    IFixedList<IParameterTypeSyntax> Parameters { get; }
    IReturnTypeSyntax Return { get; }

    public static IFunctionTypeSyntax Create(IFixedList<IParameterTypeSyntax> parameters, IReturnTypeSyntax @return, TextSpan span)
        => new FunctionTypeSyntax(parameters, @return, span);
}

// [Closed(typeof(ParameterTypeSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IParameterTypeSyntax : ICodeSyntax
{
    bool IsLent { get; }
    ITypeSyntax Referent { get; }

    public static IParameterTypeSyntax Create(bool isLent, ITypeSyntax referent, TextSpan span)
        => new ParameterTypeSyntax(isLent, referent, span);
}

// [Closed(typeof(ReturnTypeSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IReturnTypeSyntax : ICodeSyntax
{
    ITypeSyntax Referent { get; }

    public static IReturnTypeSyntax Create(ITypeSyntax referent, TextSpan span)
        => new ReturnTypeSyntax(referent, span);
}

[Closed(
    typeof(ICapabilityViewpointTypeSyntax),
    typeof(ISelfViewpointTypeSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IViewpointTypeSyntax : ITypeSyntax
{
    ITypeSyntax Referent { get; }
}

// [Closed(typeof(CapabilityViewpointTypeSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICapabilityViewpointTypeSyntax : IViewpointTypeSyntax
{
    ICapabilitySyntax Capability { get; }

    public static ICapabilityViewpointTypeSyntax Create(ICapabilitySyntax capability, ITypeSyntax referent, TextSpan span)
        => new CapabilityViewpointTypeSyntax(capability, referent, span);
}

// [Closed(typeof(SelfViewpointTypeSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISelfViewpointTypeSyntax : IViewpointTypeSyntax
{

    public static ISelfViewpointTypeSyntax Create(ITypeSyntax referent, TextSpan span)
        => new SelfViewpointTypeSyntax(referent, span);
}

[Closed(
    typeof(IResultStatementSyntax),
    typeof(IBodyStatementSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStatementSyntax : ICodeSyntax
{
}

// [Closed(typeof(ResultStatementSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IResultStatementSyntax : IStatementSyntax, IBlockOrResultSyntax
{
    IExpressionSyntax Expression { get; }

    public static IResultStatementSyntax Create(IExpressionSyntax expression, TextSpan span)
        => new ResultStatementSyntax(expression, span);
}

[Closed(
    typeof(IVariableDeclarationStatementSyntax),
    typeof(IExpressionStatementSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBodyStatementSyntax : IStatementSyntax
{
}

// [Closed(typeof(VariableDeclarationStatementSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IVariableDeclarationStatementSyntax : IBodyStatementSyntax, ILocalBindingSyntax
{
    IdentifierName Name { get; }
    ICapabilitySyntax? Capability { get; }
    ITypeSyntax? Type { get; }
    IExpressionSyntax? Initializer { get; }

    public static IVariableDeclarationStatementSyntax Create(TextSpan nameSpan, IdentifierName name, ICapabilitySyntax? capability, ITypeSyntax? type, IExpressionSyntax? initializer, TextSpan span, bool isMutableBinding)
        => new VariableDeclarationStatementSyntax(nameSpan, name, capability, type, initializer, span, isMutableBinding);
}

// [Closed(typeof(ExpressionStatementSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IExpressionStatementSyntax : IBodyStatementSyntax
{
    IExpressionSyntax Expression { get; }

    public static IExpressionStatementSyntax Create(IExpressionSyntax expression, TextSpan span)
        => new ExpressionStatementSyntax(expression, span);
}

[Closed(
    typeof(IBindingContextPatternSyntax),
    typeof(IOptionalOrBindingPatternSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPatternSyntax : ICodeSyntax
{
}

// [Closed(typeof(BindingContextPatternSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBindingContextPatternSyntax : IPatternSyntax
{
    bool IsMutableBinding { get; }
    IPatternSyntax Pattern { get; }
    ITypeSyntax? Type { get; }

    public static IBindingContextPatternSyntax Create(bool isMutableBinding, IPatternSyntax pattern, ITypeSyntax? type, TextSpan span)
        => new BindingContextPatternSyntax(isMutableBinding, pattern, type, span);
}

[Closed(
    typeof(IBindingPatternSyntax),
    typeof(IOptionalPatternSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IOptionalOrBindingPatternSyntax : IPatternSyntax
{
}

// [Closed(typeof(BindingPatternSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBindingPatternSyntax : IOptionalOrBindingPatternSyntax, ILocalBindingSyntax
{
    IdentifierName Name { get; }

    public static IBindingPatternSyntax Create(IdentifierName name, TextSpan span, TextSpan nameSpan, bool isMutableBinding)
        => new BindingPatternSyntax(name, span, nameSpan, isMutableBinding);
}

// [Closed(typeof(OptionalPatternSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IOptionalPatternSyntax : IOptionalOrBindingPatternSyntax
{
    IOptionalOrBindingPatternSyntax Pattern { get; }

    public static IOptionalPatternSyntax Create(IOptionalOrBindingPatternSyntax pattern, TextSpan span)
        => new OptionalPatternSyntax(pattern, span);
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
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IExpressionSyntax : ICodeSyntax
{
    string ToGroupedString(OperatorPrecedence surroundingPrecedence)
        => surroundingPrecedence > ExpressionPrecedence ? $"({this})" : ToString();
    OperatorPrecedence ExpressionPrecedence { get; }
}

[Closed(
    typeof(IIdentifierNameExpressionSyntax),
    typeof(IMemberAccessExpressionSyntax),
    typeof(IMissingNameSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAssignableExpressionSyntax : IExpressionSyntax
{
}

// [Closed(typeof(BlockExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBlockExpressionSyntax : IExpressionSyntax, IBlockOrResultSyntax, IBodyOrBlockSyntax
{
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Primary;

    public static IBlockExpressionSyntax Create(IFixedList<IStatementSyntax> statements, TextSpan span)
        => new BlockExpressionSyntax(statements, span);
}

// [Closed(typeof(NewObjectExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INewObjectExpressionSyntax : IExpressionSyntax
{
    ITypeNameSyntax Type { get; }
    IdentifierName? ConstructorName { get; }
    TextSpan? ConstructorNameSpan { get; }
    IFixedList<IExpressionSyntax> Arguments { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Min;

    public static INewObjectExpressionSyntax Create(ITypeNameSyntax type, IdentifierName? constructorName, TextSpan? constructorNameSpan, IFixedList<IExpressionSyntax> arguments, TextSpan span)
        => new NewObjectExpressionSyntax(type, constructorName, constructorNameSpan, arguments, span);
}

// [Closed(typeof(UnsafeExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUnsafeExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Expression { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Primary;

    public static IUnsafeExpressionSyntax Create(IExpressionSyntax expression, TextSpan span)
        => new UnsafeExpressionSyntax(expression, span);
}

[Closed(
    typeof(IBoolLiteralExpressionSyntax),
    typeof(IIntegerLiteralExpressionSyntax),
    typeof(INoneLiteralExpressionSyntax),
    typeof(IStringLiteralExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ILiteralExpressionSyntax : IExpressionSyntax
{
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Primary;
}

// [Closed(typeof(BoolLiteralExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBoolLiteralExpressionSyntax : ILiteralExpressionSyntax
{
    bool Value { get; }

    public static IBoolLiteralExpressionSyntax Create(bool value, TextSpan span)
        => new BoolLiteralExpressionSyntax(value, span);
}

// [Closed(typeof(IntegerLiteralExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IIntegerLiteralExpressionSyntax : ILiteralExpressionSyntax
{
    BigInteger Value { get; }

    public static IIntegerLiteralExpressionSyntax Create(BigInteger value, TextSpan span)
        => new IntegerLiteralExpressionSyntax(value, span);
}

// [Closed(typeof(NoneLiteralExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INoneLiteralExpressionSyntax : ILiteralExpressionSyntax
{

    public static INoneLiteralExpressionSyntax Create(TextSpan span)
        => new NoneLiteralExpressionSyntax(span);
}

// [Closed(typeof(StringLiteralExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStringLiteralExpressionSyntax : ILiteralExpressionSyntax
{
    string Value { get; }

    public static IStringLiteralExpressionSyntax Create(string value, TextSpan span)
        => new StringLiteralExpressionSyntax(value, span);
}

// [Closed(typeof(AssignmentExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAssignmentExpressionSyntax : IExpressionSyntax
{
    IAssignableExpressionSyntax LeftOperand { get; }
    AssignmentOperator Operator { get; }
    IExpressionSyntax RightOperand { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Assignment;

    public static IAssignmentExpressionSyntax Create(IAssignableExpressionSyntax leftOperand, AssignmentOperator @operator, IExpressionSyntax rightOperand, TextSpan span)
        => new AssignmentExpressionSyntax(leftOperand, @operator, rightOperand, span);
}

// [Closed(typeof(BinaryOperatorExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBinaryOperatorExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax LeftOperand { get; }
    BinaryOperator Operator { get; }
    IExpressionSyntax RightOperand { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => Operator.Precedence();

    public static IBinaryOperatorExpressionSyntax Create(IExpressionSyntax leftOperand, BinaryOperator @operator, IExpressionSyntax rightOperand, TextSpan span)
        => new BinaryOperatorExpressionSyntax(leftOperand, @operator, rightOperand, span);
}

// [Closed(typeof(UnaryOperatorExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUnaryOperatorExpressionSyntax : IExpressionSyntax
{
    UnaryOperatorFixity Fixity { get; }
    UnaryOperator Operator { get; }
    IExpressionSyntax Operand { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Unary;

    public static IUnaryOperatorExpressionSyntax Create(UnaryOperatorFixity fixity, UnaryOperator @operator, IExpressionSyntax operand, TextSpan span)
        => new UnaryOperatorExpressionSyntax(fixity, @operator, operand, span);
}

// [Closed(typeof(IdExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IIdExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Referent { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Min;

    public static IIdExpressionSyntax Create(IExpressionSyntax referent, TextSpan span)
        => new IdExpressionSyntax(referent, span);
}

// [Closed(typeof(ConversionExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IConversionExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Referent { get; }
    ConversionOperator Operator { get; }
    ITypeSyntax ConvertToType { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Conversion;

    public static IConversionExpressionSyntax Create(IExpressionSyntax referent, ConversionOperator @operator, ITypeSyntax convertToType, TextSpan span)
        => new ConversionExpressionSyntax(referent, @operator, convertToType, span);
}

// [Closed(typeof(PatternMatchExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPatternMatchExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Referent { get; }
    IPatternSyntax Pattern { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Conversion;

    public static IPatternMatchExpressionSyntax Create(IExpressionSyntax referent, IPatternSyntax pattern, TextSpan span)
        => new PatternMatchExpressionSyntax(referent, pattern, span);
}

// [Closed(typeof(IfExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IIfExpressionSyntax : IExpressionSyntax, IElseClauseSyntax
{
    IExpressionSyntax Condition { get; }
    IBlockOrResultSyntax ThenBlock { get; }
    IElseClauseSyntax? ElseClause { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Min;

    public static IIfExpressionSyntax Create(IExpressionSyntax condition, IBlockOrResultSyntax thenBlock, IElseClauseSyntax? elseClause, TextSpan span)
        => new IfExpressionSyntax(condition, thenBlock, elseClause, span);
}

// [Closed(typeof(LoopExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ILoopExpressionSyntax : IExpressionSyntax
{
    IBlockExpressionSyntax Block { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Primary;

    public static ILoopExpressionSyntax Create(IBlockExpressionSyntax block, TextSpan span)
        => new LoopExpressionSyntax(block, span);
}

// [Closed(typeof(WhileExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IWhileExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Condition { get; }
    IBlockExpressionSyntax Block { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Min;

    public static IWhileExpressionSyntax Create(IExpressionSyntax condition, IBlockExpressionSyntax block, TextSpan span)
        => new WhileExpressionSyntax(condition, block, span);
}

// [Closed(typeof(ForeachExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IForeachExpressionSyntax : IExpressionSyntax, ILocalBindingSyntax
{
    IdentifierName VariableName { get; }
    IExpressionSyntax InExpression { get; }
    ITypeSyntax? Type { get; }
    IBlockExpressionSyntax Block { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Min;

    public static IForeachExpressionSyntax Create(IdentifierName variableName, IExpressionSyntax inExpression, ITypeSyntax? type, IBlockExpressionSyntax block, TextSpan span, TextSpan nameSpan, bool isMutableBinding)
        => new ForeachExpressionSyntax(variableName, inExpression, type, block, span, nameSpan, isMutableBinding);
}

// [Closed(typeof(BreakExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBreakExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax? Value { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => Value is not null ? OperatorPrecedence.Min : OperatorPrecedence.Primary;

    public static IBreakExpressionSyntax Create(IExpressionSyntax? value, TextSpan span)
        => new BreakExpressionSyntax(value, span);
}

// [Closed(typeof(NextExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INextExpressionSyntax : IExpressionSyntax
{
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Primary;

    public static INextExpressionSyntax Create(TextSpan span)
        => new NextExpressionSyntax(span);
}

// [Closed(typeof(ReturnExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IReturnExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax? Value { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Min;

    public static IReturnExpressionSyntax Create(IExpressionSyntax? value, TextSpan span)
        => new ReturnExpressionSyntax(value, span);
}

// [Closed(typeof(InvocationExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInvocationExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Expression { get; }
    IFixedList<IExpressionSyntax> Arguments { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Primary;

    public static IInvocationExpressionSyntax Create(IExpressionSyntax expression, IFixedList<IExpressionSyntax> arguments, TextSpan span)
        => new InvocationExpressionSyntax(expression, arguments, span);
}

[Closed(
    typeof(ISimpleNameSyntax),
    typeof(IStandardNameExpressionSyntax),
    typeof(ISpecialTypeNameExpressionSyntax),
    typeof(ISelfExpressionSyntax),
    typeof(IMemberAccessExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INameExpressionSyntax : IExpressionSyntax
{
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Primary;
}

[Closed(
    typeof(IIdentifierNameExpressionSyntax),
    typeof(IInstanceExpressionSyntax),
    typeof(IMissingNameSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISimpleNameSyntax : INameExpressionSyntax
{
}

[Closed(
    typeof(IIdentifierNameExpressionSyntax),
    typeof(IGenericNameExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStandardNameExpressionSyntax : INameExpressionSyntax
{
    StandardName Name { get; }
}

// [Closed(typeof(IdentifierNameExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IIdentifierNameExpressionSyntax : IStandardNameExpressionSyntax, ISimpleNameSyntax, IAssignableExpressionSyntax
{
    new IdentifierName Name { get; }
    StandardName IStandardNameExpressionSyntax.Name => Name;

    public static IIdentifierNameExpressionSyntax Create(IdentifierName name, TextSpan span)
        => new IdentifierNameExpressionSyntax(name, span);
}

// [Closed(typeof(SpecialTypeNameExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISpecialTypeNameExpressionSyntax : INameExpressionSyntax
{
    SpecialTypeName Name { get; }

    public static ISpecialTypeNameExpressionSyntax Create(SpecialTypeName name, TextSpan span)
        => new SpecialTypeNameExpressionSyntax(name, span);
}

// [Closed(typeof(GenericNameExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGenericNameExpressionSyntax : IStandardNameExpressionSyntax
{
    new GenericName Name { get; }
    StandardName IStandardNameExpressionSyntax.Name => Name;
    IFixedList<ITypeSyntax> TypeArguments { get; }

    public static IGenericNameExpressionSyntax Create(GenericName name, IFixedList<ITypeSyntax> typeArguments, TextSpan span)
        => new GenericNameExpressionSyntax(name, typeArguments, span);
}

[Closed(
    typeof(ISelfExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInstanceExpressionSyntax : ISimpleNameSyntax
{
}

// [Closed(typeof(SelfExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISelfExpressionSyntax : INameExpressionSyntax, IInstanceExpressionSyntax
{
    bool IsImplicit { get; }

    public static ISelfExpressionSyntax Create(bool isImplicit, TextSpan span)
        => new SelfExpressionSyntax(isImplicit, span);
}

// [Closed(typeof(MemberAccessExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMemberAccessExpressionSyntax : INameExpressionSyntax, IAssignableExpressionSyntax
{
    IExpressionSyntax Context { get; }
    StandardName MemberName { get; }
    IFixedList<ITypeSyntax> TypeArguments { get; }
    TextSpan MemberNameSpan { get; }

    public static IMemberAccessExpressionSyntax Create(IExpressionSyntax context, StandardName memberName, IFixedList<ITypeSyntax> typeArguments, TextSpan memberNameSpan, TextSpan span)
        => new MemberAccessExpressionSyntax(context, memberName, typeArguments, memberNameSpan, span);
}

// [Closed(typeof(MissingNameSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMissingNameSyntax : ISimpleNameSyntax, IAssignableExpressionSyntax
{

    public static IMissingNameSyntax Create(TextSpan span)
        => new MissingNameSyntax(span);
}

// [Closed(typeof(MoveExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMoveExpressionSyntax : IExpressionSyntax
{
    ISimpleNameSyntax Referent { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Min;

    public static IMoveExpressionSyntax Create(ISimpleNameSyntax referent, TextSpan span)
        => new MoveExpressionSyntax(referent, span);
}

// [Closed(typeof(FreezeExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFreezeExpressionSyntax : IExpressionSyntax
{
    ISimpleNameSyntax Referent { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Min;

    public static IFreezeExpressionSyntax Create(ISimpleNameSyntax referent, TextSpan span)
        => new FreezeExpressionSyntax(referent, span);
}

// [Closed(typeof(AsyncBlockExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAsyncBlockExpressionSyntax : IExpressionSyntax
{
    IBlockExpressionSyntax Block { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Primary;

    public static IAsyncBlockExpressionSyntax Create(IBlockExpressionSyntax block, TextSpan span)
        => new AsyncBlockExpressionSyntax(block, span);
}

// [Closed(typeof(AsyncStartExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAsyncStartExpressionSyntax : IExpressionSyntax
{
    bool Scheduled { get; }
    IExpressionSyntax Expression { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Min;

    public static IAsyncStartExpressionSyntax Create(bool scheduled, IExpressionSyntax expression, TextSpan span)
        => new AsyncStartExpressionSyntax(scheduled, expression, span);
}

// [Closed(typeof(AwaitExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAwaitExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Expression { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Unary;

    public static IAwaitExpressionSyntax Create(IExpressionSyntax expression, TextSpan span)
        => new AwaitExpressionSyntax(expression, span);
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class CompilationUnitSyntax : ICompilationUnitSyntax
{
    public CodeFile File { [DebuggerStepThrough] get; }
    public NamespaceName ImplicitNamespaceName { [DebuggerStepThrough] get; }
    public DiagnosticCollection Diagnostics { [DebuggerStepThrough] get; }
    public IFixedList<IUsingDirectiveSyntax> UsingDirectives { [DebuggerStepThrough] get; }
    public IFixedList<INonMemberDefinitionSyntax> Definitions { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

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

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class UsingDirectiveSyntax : IUsingDirectiveSyntax
{
    public NamespaceName Name { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public UsingDirectiveSyntax(NamespaceName name, TextSpan span)
    {
        Name = name;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class PackageSyntax : IPackageSyntax
{
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IFixedSet<ICompilationUnitSyntax> CompilationUnits { [DebuggerStepThrough] get; }
    public IFixedSet<ICompilationUnitSyntax> TestingCompilationUnits { [DebuggerStepThrough] get; }
    public IFixedSet<IPackageReferenceSyntax> References { [DebuggerStepThrough] get; }
    public DiagnosticCollection Diagnostics { [DebuggerStepThrough] get; }

    public PackageSyntax(IdentifierName name, IFixedSet<ICompilationUnitSyntax> compilationUnits, IFixedSet<ICompilationUnitSyntax> testingCompilationUnits, IFixedSet<IPackageReferenceSyntax> references, DiagnosticCollection diagnostics)
    {
        Name = name;
        CompilationUnits = compilationUnits;
        TestingCompilationUnits = testingCompilationUnits;
        References = references;
        Diagnostics = diagnostics;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class PackageReferenceSyntax : IPackageReferenceSyntax
{
    public IdentifierName AliasOrName { [DebuggerStepThrough] get; }
    public IPackageSymbols Package { [DebuggerStepThrough] get; }
    public bool IsTrusted { [DebuggerStepThrough] get; }

    public PackageReferenceSyntax(IdentifierName aliasOrName, IPackageSymbols package, bool isTrusted)
    {
        AliasOrName = aliasOrName;
        Package = package;
        IsTrusted = isTrusted;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class NamespaceDefinitionSyntax : INamespaceDefinitionSyntax
{
    public bool IsGlobalQualified { [DebuggerStepThrough] get; }
    public NamespaceName DeclaredNames { [DebuggerStepThrough] get; }
    public IFixedList<IUsingDirectiveSyntax> UsingDirectives { [DebuggerStepThrough] get; }
    public IFixedList<INonMemberDefinitionSyntax> Definitions { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TypeName? Name { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

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

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class FunctionDefinitionSyntax : IFunctionDefinitionSyntax
{
    public IFixedList<IAttributeSyntax> Attributes { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterSyntax> Parameters { [DebuggerStepThrough] get; }
    public IReturnSyntax? Return { [DebuggerStepThrough] get; }
    public IBodySyntax Body { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

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

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class ClassDefinitionSyntax : IClassDefinitionSyntax
{
    public IAbstractKeywordToken? AbstractModifier { [DebuggerStepThrough] get; }
    public IFixedList<IGenericParameterSyntax> GenericParameters { [DebuggerStepThrough] get; }
    public IStandardTypeNameSyntax? BaseTypeName { [DebuggerStepThrough] get; }
    public IFixedList<IStandardTypeNameSyntax> SupertypeNames { [DebuggerStepThrough] get; }
    public IFixedList<IClassMemberDefinitionSyntax> Members { [DebuggerStepThrough] get; }
    public IConstKeywordToken? ConstModifier { [DebuggerStepThrough] get; }
    public IMoveKeywordToken? MoveModifier { [DebuggerStepThrough] get; }
    public StandardName Name { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }

    public ClassDefinitionSyntax(IAbstractKeywordToken? abstractModifier, IFixedList<IGenericParameterSyntax> genericParameters, IStandardTypeNameSyntax? baseTypeName, IFixedList<IStandardTypeNameSyntax> supertypeNames, IFixedList<IClassMemberDefinitionSyntax> members, IConstKeywordToken? constModifier, IMoveKeywordToken? moveModifier, StandardName name, CodeFile file, TextSpan nameSpan, TextSpan span, IAccessModifierToken? accessModifier)
    {
        AbstractModifier = abstractModifier;
        GenericParameters = genericParameters;
        BaseTypeName = baseTypeName;
        SupertypeNames = supertypeNames;
        Members = members;
        ConstModifier = constModifier;
        MoveModifier = moveModifier;
        Name = name;
        File = file;
        NameSpan = nameSpan;
        Span = span;
        AccessModifier = accessModifier;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class StructDefinitionSyntax : IStructDefinitionSyntax
{
    public IFixedList<IGenericParameterSyntax> GenericParameters { [DebuggerStepThrough] get; }
    public IFixedList<IStandardTypeNameSyntax> SupertypeNames { [DebuggerStepThrough] get; }
    public IFixedList<IStructMemberDefinitionSyntax> Members { [DebuggerStepThrough] get; }
    public IConstKeywordToken? ConstModifier { [DebuggerStepThrough] get; }
    public IMoveKeywordToken? MoveModifier { [DebuggerStepThrough] get; }
    public StandardName Name { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }

    public StructDefinitionSyntax(IFixedList<IGenericParameterSyntax> genericParameters, IFixedList<IStandardTypeNameSyntax> supertypeNames, IFixedList<IStructMemberDefinitionSyntax> members, IConstKeywordToken? constModifier, IMoveKeywordToken? moveModifier, StandardName name, CodeFile file, TextSpan nameSpan, TextSpan span, IAccessModifierToken? accessModifier)
    {
        GenericParameters = genericParameters;
        SupertypeNames = supertypeNames;
        Members = members;
        ConstModifier = constModifier;
        MoveModifier = moveModifier;
        Name = name;
        File = file;
        NameSpan = nameSpan;
        Span = span;
        AccessModifier = accessModifier;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class TraitDefinitionSyntax : ITraitDefinitionSyntax
{
    public IFixedList<IGenericParameterSyntax> GenericParameters { [DebuggerStepThrough] get; }
    public IFixedList<IStandardTypeNameSyntax> SupertypeNames { [DebuggerStepThrough] get; }
    public IFixedList<ITraitMemberDefinitionSyntax> Members { [DebuggerStepThrough] get; }
    public IConstKeywordToken? ConstModifier { [DebuggerStepThrough] get; }
    public IMoveKeywordToken? MoveModifier { [DebuggerStepThrough] get; }
    public StandardName Name { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }

    public TraitDefinitionSyntax(IFixedList<IGenericParameterSyntax> genericParameters, IFixedList<IStandardTypeNameSyntax> supertypeNames, IFixedList<ITraitMemberDefinitionSyntax> members, IConstKeywordToken? constModifier, IMoveKeywordToken? moveModifier, StandardName name, CodeFile file, TextSpan nameSpan, TextSpan span, IAccessModifierToken? accessModifier)
    {
        GenericParameters = genericParameters;
        SupertypeNames = supertypeNames;
        Members = members;
        ConstModifier = constModifier;
        MoveModifier = moveModifier;
        Name = name;
        File = file;
        NameSpan = nameSpan;
        Span = span;
        AccessModifier = accessModifier;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class GenericParameterSyntax : IGenericParameterSyntax
{
    public ICapabilityConstraintSyntax Constraint { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public TypeParameterIndependence Independence { [DebuggerStepThrough] get; }
    public TypeParameterVariance Variance { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public GenericParameterSyntax(ICapabilityConstraintSyntax constraint, IdentifierName name, TypeParameterIndependence independence, TypeParameterVariance variance, TextSpan span)
    {
        Constraint = constraint;
        Name = name;
        Independence = independence;
        Variance = variance;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class AbstractMethodDefinitionSyntax : IAbstractMethodDefinitionSyntax
{
    public IMethodSelfParameterSyntax SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterSyntax> Parameters { [DebuggerStepThrough] get; }
    public IReturnSyntax? Return { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public AbstractMethodDefinitionSyntax(IMethodSelfParameterSyntax selfParameter, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax? @return, IdentifierName name, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span)
    {
        SelfParameter = selfParameter;
        Parameters = parameters;
        Return = @return;
        Name = name;
        AccessModifier = accessModifier;
        File = file;
        NameSpan = nameSpan;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class StandardMethodDefinitionSyntax : IStandardMethodDefinitionSyntax
{
    public IMethodSelfParameterSyntax SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterSyntax> Parameters { [DebuggerStepThrough] get; }
    public IReturnSyntax? Return { [DebuggerStepThrough] get; }
    public IBodySyntax Body { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public StandardMethodDefinitionSyntax(IMethodSelfParameterSyntax selfParameter, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax? @return, IBodySyntax body, IdentifierName name, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span)
    {
        SelfParameter = selfParameter;
        Parameters = parameters;
        Return = @return;
        Body = body;
        Name = name;
        AccessModifier = accessModifier;
        File = file;
        NameSpan = nameSpan;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class GetterMethodDefinitionSyntax : IGetterMethodDefinitionSyntax
{
    public IMethodSelfParameterSyntax SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterSyntax> Parameters { [DebuggerStepThrough] get; }
    public IReturnSyntax Return { [DebuggerStepThrough] get; }
    public IBodySyntax Body { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public GetterMethodDefinitionSyntax(IMethodSelfParameterSyntax selfParameter, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax @return, IBodySyntax body, IdentifierName name, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span)
    {
        SelfParameter = selfParameter;
        Parameters = parameters;
        Return = @return;
        Body = body;
        Name = name;
        AccessModifier = accessModifier;
        File = file;
        NameSpan = nameSpan;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class SetterMethodDefinitionSyntax : ISetterMethodDefinitionSyntax
{
    public IMethodSelfParameterSyntax SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterSyntax> Parameters { [DebuggerStepThrough] get; }
    public IReturnSyntax? Return { [DebuggerStepThrough] get; }
    public IBodySyntax Body { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public SetterMethodDefinitionSyntax(IMethodSelfParameterSyntax selfParameter, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax? @return, IBodySyntax body, IdentifierName name, IAccessModifierToken? accessModifier, CodeFile file, TextSpan nameSpan, TextSpan span)
    {
        SelfParameter = selfParameter;
        Parameters = parameters;
        Return = @return;
        Body = body;
        Name = name;
        AccessModifier = accessModifier;
        File = file;
        NameSpan = nameSpan;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class ConstructorDefinitionSyntax : IConstructorDefinitionSyntax
{
    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public IConstructorSelfParameterSyntax SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<IConstructorOrInitializerParameterSyntax> Parameters { [DebuggerStepThrough] get; }
    public IBlockBodySyntax Body { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

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

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class InitializerDefinitionSyntax : IInitializerDefinitionSyntax
{
    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public IInitializerSelfParameterSyntax SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<IConstructorOrInitializerParameterSyntax> Parameters { [DebuggerStepThrough] get; }
    public IBlockBodySyntax Body { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

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

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class FieldDefinitionSyntax : IFieldDefinitionSyntax
{
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public ITypeSyntax Type { [DebuggerStepThrough] get; }
    public IExpressionSyntax? Initializer { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }
    public bool IsMutableBinding { [DebuggerStepThrough] get; }

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

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class AssociatedFunctionDefinitionSyntax : IAssociatedFunctionDefinitionSyntax
{
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterSyntax> Parameters { [DebuggerStepThrough] get; }
    public IReturnSyntax? Return { [DebuggerStepThrough] get; }
    public IBodySyntax Body { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

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

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class AttributeSyntax : IAttributeSyntax
{
    public IStandardTypeNameSyntax TypeName { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public AttributeSyntax(IStandardTypeNameSyntax typeName, TextSpan span)
    {
        TypeName = typeName;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class CapabilitySetSyntax : ICapabilitySetSyntax
{
    public CapabilitySet Constraint { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public CapabilitySetSyntax(CapabilitySet constraint, TextSpan span)
    {
        Constraint = constraint;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class CapabilitySyntax : ICapabilitySyntax
{
    public IFixedList<ICapabilityToken> Tokens { [DebuggerStepThrough] get; }
    public DeclaredCapability Declared { [DebuggerStepThrough] get; }
    public Capability Capability { [DebuggerStepThrough] get; }
    public ICapabilityConstraint Constraint { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public CapabilitySyntax(IFixedList<ICapabilityToken> tokens, DeclaredCapability declared, Capability capability, ICapabilityConstraint constraint, TextSpan span)
    {
        Tokens = tokens;
        Declared = declared;
        Capability = capability;
        Constraint = constraint;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class NamedParameterSyntax : INamedParameterSyntax
{
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public bool IsLentBinding { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public ITypeSyntax Type { [DebuggerStepThrough] get; }
    public IExpressionSyntax? DefaultValue { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }

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

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class ConstructorSelfParameterSyntax : IConstructorSelfParameterSyntax
{
    public ICapabilitySyntax Capability { [DebuggerStepThrough] get; }
    public bool IsLentBinding { [DebuggerStepThrough] get; }
    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public ConstructorSelfParameterSyntax(ICapabilitySyntax capability, bool isLentBinding, IdentifierName? name, TextSpan span)
    {
        Capability = capability;
        IsLentBinding = isLentBinding;
        Name = name;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class InitializerSelfParameterSyntax : IInitializerSelfParameterSyntax
{
    public ICapabilitySyntax Capability { [DebuggerStepThrough] get; }
    public bool IsLentBinding { [DebuggerStepThrough] get; }
    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public InitializerSelfParameterSyntax(ICapabilitySyntax capability, bool isLentBinding, IdentifierName? name, TextSpan span)
    {
        Capability = capability;
        IsLentBinding = isLentBinding;
        Name = name;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class MethodSelfParameterSyntax : IMethodSelfParameterSyntax
{
    public ICapabilityConstraintSyntax Capability { [DebuggerStepThrough] get; }
    public bool IsLentBinding { [DebuggerStepThrough] get; }
    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public MethodSelfParameterSyntax(ICapabilityConstraintSyntax capability, bool isLentBinding, IdentifierName? name, TextSpan span)
    {
        Capability = capability;
        IsLentBinding = isLentBinding;
        Name = name;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class FieldParameterSyntax : IFieldParameterSyntax
{
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IExpressionSyntax? DefaultValue { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public FieldParameterSyntax(IdentifierName name, IExpressionSyntax? defaultValue, TextSpan span)
    {
        Name = name;
        DefaultValue = defaultValue;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class ReturnSyntax : IReturnSyntax
{
    public ITypeSyntax Type { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public ReturnSyntax(ITypeSyntax type, TextSpan span)
    {
        Type = type;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class BlockBodySyntax : IBlockBodySyntax
{
    public IFixedList<IBodyStatementSyntax> Statements { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public BlockBodySyntax(IFixedList<IBodyStatementSyntax> statements, TextSpan span)
    {
        Statements = statements;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class ExpressionBodySyntax : IExpressionBodySyntax
{
    public IResultStatementSyntax ResultStatement { [DebuggerStepThrough] get; }
    public IFixedList<IStatementSyntax> Statements { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public ExpressionBodySyntax(IResultStatementSyntax resultStatement, IFixedList<IStatementSyntax> statements, TextSpan span)
    {
        ResultStatement = resultStatement;
        Statements = statements;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class IdentifierTypeNameSyntax : IIdentifierTypeNameSyntax
{
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public IdentifierTypeNameSyntax(IdentifierName name, TextSpan span)
    {
        Name = name;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class SpecialTypeNameSyntax : ISpecialTypeNameSyntax
{
    public SpecialTypeName Name { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public SpecialTypeNameSyntax(SpecialTypeName name, TextSpan span)
    {
        Name = name;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class GenericTypeNameSyntax : IGenericTypeNameSyntax
{
    public GenericName Name { [DebuggerStepThrough] get; }
    public IFixedList<ITypeSyntax> TypeArguments { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public GenericTypeNameSyntax(GenericName name, IFixedList<ITypeSyntax> typeArguments, TextSpan span)
    {
        Name = name;
        TypeArguments = typeArguments;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class QualifiedTypeNameSyntax : IQualifiedTypeNameSyntax
{
    public ITypeNameSyntax Context { [DebuggerStepThrough] get; }
    public IStandardTypeNameSyntax QualifiedName { [DebuggerStepThrough] get; }
    public TypeName Name { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public QualifiedTypeNameSyntax(ITypeNameSyntax context, IStandardTypeNameSyntax qualifiedName, TypeName name, TextSpan span)
    {
        Context = context;
        QualifiedName = qualifiedName;
        Name = name;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class OptionalTypeSyntax : IOptionalTypeSyntax
{
    public ITypeSyntax Referent { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public OptionalTypeSyntax(ITypeSyntax referent, TextSpan span)
    {
        Referent = referent;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class CapabilityTypeSyntax : ICapabilityTypeSyntax
{
    public ICapabilitySyntax Capability { [DebuggerStepThrough] get; }
    public ITypeSyntax Referent { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public CapabilityTypeSyntax(ICapabilitySyntax capability, ITypeSyntax referent, TextSpan span)
    {
        Capability = capability;
        Referent = referent;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class FunctionTypeSyntax : IFunctionTypeSyntax
{
    public IFixedList<IParameterTypeSyntax> Parameters { [DebuggerStepThrough] get; }
    public IReturnTypeSyntax Return { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public FunctionTypeSyntax(IFixedList<IParameterTypeSyntax> parameters, IReturnTypeSyntax @return, TextSpan span)
    {
        Parameters = parameters;
        Return = @return;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class ParameterTypeSyntax : IParameterTypeSyntax
{
    public bool IsLent { [DebuggerStepThrough] get; }
    public ITypeSyntax Referent { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public ParameterTypeSyntax(bool isLent, ITypeSyntax referent, TextSpan span)
    {
        IsLent = isLent;
        Referent = referent;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class ReturnTypeSyntax : IReturnTypeSyntax
{
    public ITypeSyntax Referent { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public ReturnTypeSyntax(ITypeSyntax referent, TextSpan span)
    {
        Referent = referent;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class CapabilityViewpointTypeSyntax : ICapabilityViewpointTypeSyntax
{
    public ICapabilitySyntax Capability { [DebuggerStepThrough] get; }
    public ITypeSyntax Referent { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public CapabilityViewpointTypeSyntax(ICapabilitySyntax capability, ITypeSyntax referent, TextSpan span)
    {
        Capability = capability;
        Referent = referent;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class SelfViewpointTypeSyntax : ISelfViewpointTypeSyntax
{
    public ITypeSyntax Referent { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public SelfViewpointTypeSyntax(ITypeSyntax referent, TextSpan span)
    {
        Referent = referent;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class ResultStatementSyntax : IResultStatementSyntax
{
    public IExpressionSyntax Expression { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public ResultStatementSyntax(IExpressionSyntax expression, TextSpan span)
    {
        Expression = expression;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class VariableDeclarationStatementSyntax : IVariableDeclarationStatementSyntax
{
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public ICapabilitySyntax? Capability { [DebuggerStepThrough] get; }
    public ITypeSyntax? Type { [DebuggerStepThrough] get; }
    public IExpressionSyntax? Initializer { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }
    public bool IsMutableBinding { [DebuggerStepThrough] get; }

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

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class ExpressionStatementSyntax : IExpressionStatementSyntax
{
    public IExpressionSyntax Expression { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public ExpressionStatementSyntax(IExpressionSyntax expression, TextSpan span)
    {
        Expression = expression;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class BindingContextPatternSyntax : IBindingContextPatternSyntax
{
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public IPatternSyntax Pattern { [DebuggerStepThrough] get; }
    public ITypeSyntax? Type { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public BindingContextPatternSyntax(bool isMutableBinding, IPatternSyntax pattern, ITypeSyntax? type, TextSpan span)
    {
        IsMutableBinding = isMutableBinding;
        Pattern = pattern;
        Type = type;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class BindingPatternSyntax : IBindingPatternSyntax
{
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public bool IsMutableBinding { [DebuggerStepThrough] get; }

    public BindingPatternSyntax(IdentifierName name, TextSpan span, TextSpan nameSpan, bool isMutableBinding)
    {
        Name = name;
        Span = span;
        NameSpan = nameSpan;
        IsMutableBinding = isMutableBinding;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class OptionalPatternSyntax : IOptionalPatternSyntax
{
    public IOptionalOrBindingPatternSyntax Pattern { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public OptionalPatternSyntax(IOptionalOrBindingPatternSyntax pattern, TextSpan span)
    {
        Pattern = pattern;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class BlockExpressionSyntax : IBlockExpressionSyntax
{
    public IFixedList<IStatementSyntax> Statements { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public BlockExpressionSyntax(IFixedList<IStatementSyntax> statements, TextSpan span)
    {
        Statements = statements;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class NewObjectExpressionSyntax : INewObjectExpressionSyntax
{
    public ITypeNameSyntax Type { [DebuggerStepThrough] get; }
    public IdentifierName? ConstructorName { [DebuggerStepThrough] get; }
    public TextSpan? ConstructorNameSpan { [DebuggerStepThrough] get; }
    public IFixedList<IExpressionSyntax> Arguments { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public NewObjectExpressionSyntax(ITypeNameSyntax type, IdentifierName? constructorName, TextSpan? constructorNameSpan, IFixedList<IExpressionSyntax> arguments, TextSpan span)
    {
        Type = type;
        ConstructorName = constructorName;
        ConstructorNameSpan = constructorNameSpan;
        Arguments = arguments;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class UnsafeExpressionSyntax : IUnsafeExpressionSyntax
{
    public IExpressionSyntax Expression { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public UnsafeExpressionSyntax(IExpressionSyntax expression, TextSpan span)
    {
        Expression = expression;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class BoolLiteralExpressionSyntax : IBoolLiteralExpressionSyntax
{
    public bool Value { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public BoolLiteralExpressionSyntax(bool value, TextSpan span)
    {
        Value = value;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class IntegerLiteralExpressionSyntax : IIntegerLiteralExpressionSyntax
{
    public BigInteger Value { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public IntegerLiteralExpressionSyntax(BigInteger value, TextSpan span)
    {
        Value = value;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class NoneLiteralExpressionSyntax : INoneLiteralExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }

    public NoneLiteralExpressionSyntax(TextSpan span)
    {
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class StringLiteralExpressionSyntax : IStringLiteralExpressionSyntax
{
    public string Value { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public StringLiteralExpressionSyntax(string value, TextSpan span)
    {
        Value = value;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class AssignmentExpressionSyntax : IAssignmentExpressionSyntax
{
    public IAssignableExpressionSyntax LeftOperand { [DebuggerStepThrough] get; }
    public AssignmentOperator Operator { [DebuggerStepThrough] get; }
    public IExpressionSyntax RightOperand { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public AssignmentExpressionSyntax(IAssignableExpressionSyntax leftOperand, AssignmentOperator @operator, IExpressionSyntax rightOperand, TextSpan span)
    {
        LeftOperand = leftOperand;
        Operator = @operator;
        RightOperand = rightOperand;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class BinaryOperatorExpressionSyntax : IBinaryOperatorExpressionSyntax
{
    public IExpressionSyntax LeftOperand { [DebuggerStepThrough] get; }
    public BinaryOperator Operator { [DebuggerStepThrough] get; }
    public IExpressionSyntax RightOperand { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public BinaryOperatorExpressionSyntax(IExpressionSyntax leftOperand, BinaryOperator @operator, IExpressionSyntax rightOperand, TextSpan span)
    {
        LeftOperand = leftOperand;
        Operator = @operator;
        RightOperand = rightOperand;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class UnaryOperatorExpressionSyntax : IUnaryOperatorExpressionSyntax
{
    public UnaryOperatorFixity Fixity { [DebuggerStepThrough] get; }
    public UnaryOperator Operator { [DebuggerStepThrough] get; }
    public IExpressionSyntax Operand { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public UnaryOperatorExpressionSyntax(UnaryOperatorFixity fixity, UnaryOperator @operator, IExpressionSyntax operand, TextSpan span)
    {
        Fixity = fixity;
        Operator = @operator;
        Operand = operand;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class IdExpressionSyntax : IIdExpressionSyntax
{
    public IExpressionSyntax Referent { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public IdExpressionSyntax(IExpressionSyntax referent, TextSpan span)
    {
        Referent = referent;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class ConversionExpressionSyntax : IConversionExpressionSyntax
{
    public IExpressionSyntax Referent { [DebuggerStepThrough] get; }
    public ConversionOperator Operator { [DebuggerStepThrough] get; }
    public ITypeSyntax ConvertToType { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public ConversionExpressionSyntax(IExpressionSyntax referent, ConversionOperator @operator, ITypeSyntax convertToType, TextSpan span)
    {
        Referent = referent;
        Operator = @operator;
        ConvertToType = convertToType;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class PatternMatchExpressionSyntax : IPatternMatchExpressionSyntax
{
    public IExpressionSyntax Referent { [DebuggerStepThrough] get; }
    public IPatternSyntax Pattern { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public PatternMatchExpressionSyntax(IExpressionSyntax referent, IPatternSyntax pattern, TextSpan span)
    {
        Referent = referent;
        Pattern = pattern;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class IfExpressionSyntax : IIfExpressionSyntax
{
    public IExpressionSyntax Condition { [DebuggerStepThrough] get; }
    public IBlockOrResultSyntax ThenBlock { [DebuggerStepThrough] get; }
    public IElseClauseSyntax? ElseClause { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public IfExpressionSyntax(IExpressionSyntax condition, IBlockOrResultSyntax thenBlock, IElseClauseSyntax? elseClause, TextSpan span)
    {
        Condition = condition;
        ThenBlock = thenBlock;
        ElseClause = elseClause;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class LoopExpressionSyntax : ILoopExpressionSyntax
{
    public IBlockExpressionSyntax Block { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public LoopExpressionSyntax(IBlockExpressionSyntax block, TextSpan span)
    {
        Block = block;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class WhileExpressionSyntax : IWhileExpressionSyntax
{
    public IExpressionSyntax Condition { [DebuggerStepThrough] get; }
    public IBlockExpressionSyntax Block { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public WhileExpressionSyntax(IExpressionSyntax condition, IBlockExpressionSyntax block, TextSpan span)
    {
        Condition = condition;
        Block = block;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class ForeachExpressionSyntax : IForeachExpressionSyntax
{
    public IdentifierName VariableName { [DebuggerStepThrough] get; }
    public IExpressionSyntax InExpression { [DebuggerStepThrough] get; }
    public ITypeSyntax? Type { [DebuggerStepThrough] get; }
    public IBlockExpressionSyntax Block { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public bool IsMutableBinding { [DebuggerStepThrough] get; }

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

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class BreakExpressionSyntax : IBreakExpressionSyntax
{
    public IExpressionSyntax? Value { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public BreakExpressionSyntax(IExpressionSyntax? value, TextSpan span)
    {
        Value = value;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class NextExpressionSyntax : INextExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }

    public NextExpressionSyntax(TextSpan span)
    {
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class ReturnExpressionSyntax : IReturnExpressionSyntax
{
    public IExpressionSyntax? Value { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public ReturnExpressionSyntax(IExpressionSyntax? value, TextSpan span)
    {
        Value = value;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class InvocationExpressionSyntax : IInvocationExpressionSyntax
{
    public IExpressionSyntax Expression { [DebuggerStepThrough] get; }
    public IFixedList<IExpressionSyntax> Arguments { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public InvocationExpressionSyntax(IExpressionSyntax expression, IFixedList<IExpressionSyntax> arguments, TextSpan span)
    {
        Expression = expression;
        Arguments = arguments;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class IdentifierNameExpressionSyntax : IIdentifierNameExpressionSyntax
{
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public IdentifierNameExpressionSyntax(IdentifierName name, TextSpan span)
    {
        Name = name;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class SpecialTypeNameExpressionSyntax : ISpecialTypeNameExpressionSyntax
{
    public SpecialTypeName Name { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public SpecialTypeNameExpressionSyntax(SpecialTypeName name, TextSpan span)
    {
        Name = name;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class GenericNameExpressionSyntax : IGenericNameExpressionSyntax
{
    public GenericName Name { [DebuggerStepThrough] get; }
    public IFixedList<ITypeSyntax> TypeArguments { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public GenericNameExpressionSyntax(GenericName name, IFixedList<ITypeSyntax> typeArguments, TextSpan span)
    {
        Name = name;
        TypeArguments = typeArguments;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class SelfExpressionSyntax : ISelfExpressionSyntax
{
    public bool IsImplicit { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public SelfExpressionSyntax(bool isImplicit, TextSpan span)
    {
        IsImplicit = isImplicit;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class MemberAccessExpressionSyntax : IMemberAccessExpressionSyntax
{
    public IExpressionSyntax Context { [DebuggerStepThrough] get; }
    public StandardName MemberName { [DebuggerStepThrough] get; }
    public IFixedList<ITypeSyntax> TypeArguments { [DebuggerStepThrough] get; }
    public TextSpan MemberNameSpan { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public MemberAccessExpressionSyntax(IExpressionSyntax context, StandardName memberName, IFixedList<ITypeSyntax> typeArguments, TextSpan memberNameSpan, TextSpan span)
    {
        Context = context;
        MemberName = memberName;
        TypeArguments = typeArguments;
        MemberNameSpan = memberNameSpan;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class MissingNameSyntax : IMissingNameSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }

    public MissingNameSyntax(TextSpan span)
    {
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class MoveExpressionSyntax : IMoveExpressionSyntax
{
    public ISimpleNameSyntax Referent { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public MoveExpressionSyntax(ISimpleNameSyntax referent, TextSpan span)
    {
        Referent = referent;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class FreezeExpressionSyntax : IFreezeExpressionSyntax
{
    public ISimpleNameSyntax Referent { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public FreezeExpressionSyntax(ISimpleNameSyntax referent, TextSpan span)
    {
        Referent = referent;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class AsyncBlockExpressionSyntax : IAsyncBlockExpressionSyntax
{
    public IBlockExpressionSyntax Block { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public AsyncBlockExpressionSyntax(IBlockExpressionSyntax block, TextSpan span)
    {
        Block = block;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class AsyncStartExpressionSyntax : IAsyncStartExpressionSyntax
{
    public bool Scheduled { [DebuggerStepThrough] get; }
    public IExpressionSyntax Expression { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public AsyncStartExpressionSyntax(bool scheduled, IExpressionSyntax expression, TextSpan span)
    {
        Scheduled = scheduled;
        Expression = expression;
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class AwaitExpressionSyntax : IAwaitExpressionSyntax
{
    public IExpressionSyntax Expression { [DebuggerStepThrough] get; }
    public TextSpan Span { [DebuggerStepThrough] get; }

    public AwaitExpressionSyntax(IExpressionSyntax expression, TextSpan span)
    {
        Expression = expression;
        Span = span;
    }
}

