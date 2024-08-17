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
// ReSharper disable ConvertToPrimaryConstructor

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
    string ToString();
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

    public static ICompilationUnitSyntax Create(TextSpan span, CodeFile file, NamespaceName implicitNamespaceName, DiagnosticCollection diagnostics, IFixedList<IUsingDirectiveSyntax> usingDirectives, IFixedList<INonMemberDefinitionSyntax> definitions)
        => new CompilationUnitSyntax(span, file, implicitNamespaceName, diagnostics, usingDirectives, definitions);
}

// [Closed(typeof(UsingDirectiveSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUsingDirectiveSyntax : ICodeSyntax
{
    NamespaceName Name { get; }

    public static IUsingDirectiveSyntax Create(TextSpan span, NamespaceName name)
        => new UsingDirectiveSyntax(span, name);
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

    public static INamespaceDefinitionSyntax Create(TextSpan span, CodeFile file, TypeName? name, TextSpan nameSpan, bool isGlobalQualified, NamespaceName declaredNames, IFixedList<IUsingDirectiveSyntax> usingDirectives, IFixedList<INonMemberDefinitionSyntax> definitions)
        => new NamespaceDefinitionSyntax(span, file, name, nameSpan, isGlobalQualified, declaredNames, usingDirectives, definitions);
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

    public static IFunctionDefinitionSyntax Create(TextSpan span, CodeFile file, TextSpan nameSpan, IAccessModifierToken? accessModifier, IFixedList<IAttributeSyntax> attributes, IdentifierName name, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax? @return, IBodySyntax body)
        => new FunctionDefinitionSyntax(span, file, nameSpan, accessModifier, attributes, name, parameters, @return, body);
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

    public static IClassDefinitionSyntax Create(TextSpan span, CodeFile file, TextSpan nameSpan, IAccessModifierToken? accessModifier, IConstKeywordToken? constModifier, IMoveKeywordToken? moveModifier, StandardName name, IAbstractKeywordToken? abstractModifier, IFixedList<IGenericParameterSyntax> genericParameters, IStandardTypeNameSyntax? baseTypeName, IFixedList<IStandardTypeNameSyntax> supertypeNames, IFixedList<IClassMemberDefinitionSyntax> members)
        => new ClassDefinitionSyntax(span, file, nameSpan, accessModifier, constModifier, moveModifier, name, abstractModifier, genericParameters, baseTypeName, supertypeNames, members);
}

// [Closed(typeof(StructDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStructDefinitionSyntax : ITypeDefinitionSyntax
{
    new IFixedList<IStructMemberDefinitionSyntax> Members { get; }
    IFixedList<ITypeMemberDefinitionSyntax> ITypeDefinitionSyntax.Members => Members;

    public static IStructDefinitionSyntax Create(TextSpan span, CodeFile file, TextSpan nameSpan, IAccessModifierToken? accessModifier, IConstKeywordToken? constModifier, IMoveKeywordToken? moveModifier, StandardName name, IFixedList<IGenericParameterSyntax> genericParameters, IFixedList<IStandardTypeNameSyntax> supertypeNames, IFixedList<IStructMemberDefinitionSyntax> members)
        => new StructDefinitionSyntax(span, file, nameSpan, accessModifier, constModifier, moveModifier, name, genericParameters, supertypeNames, members);
}

// [Closed(typeof(TraitDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITraitDefinitionSyntax : ITypeDefinitionSyntax
{
    new IFixedList<ITraitMemberDefinitionSyntax> Members { get; }
    IFixedList<ITypeMemberDefinitionSyntax> ITypeDefinitionSyntax.Members => Members;

    public static ITraitDefinitionSyntax Create(TextSpan span, CodeFile file, TextSpan nameSpan, IAccessModifierToken? accessModifier, IConstKeywordToken? constModifier, IMoveKeywordToken? moveModifier, StandardName name, IFixedList<IGenericParameterSyntax> genericParameters, IFixedList<IStandardTypeNameSyntax> supertypeNames, IFixedList<ITraitMemberDefinitionSyntax> members)
        => new TraitDefinitionSyntax(span, file, nameSpan, accessModifier, constModifier, moveModifier, name, genericParameters, supertypeNames, members);
}

// [Closed(typeof(GenericParameterSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGenericParameterSyntax : ICodeSyntax
{
    ICapabilityConstraintSyntax Constraint { get; }
    IdentifierName Name { get; }
    TypeParameterIndependence Independence { get; }
    TypeParameterVariance Variance { get; }

    public static IGenericParameterSyntax Create(TextSpan span, ICapabilityConstraintSyntax constraint, IdentifierName name, TypeParameterIndependence independence, TypeParameterVariance variance)
        => new GenericParameterSyntax(span, constraint, name, independence, variance);
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

    public static IAbstractMethodDefinitionSyntax Create(TextSpan span, CodeFile file, TextSpan nameSpan, IAccessModifierToken? accessModifier, IdentifierName name, IMethodSelfParameterSyntax selfParameter, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax? @return)
        => new AbstractMethodDefinitionSyntax(span, file, nameSpan, accessModifier, name, selfParameter, parameters, @return);
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

    public static IStandardMethodDefinitionSyntax Create(TextSpan span, CodeFile file, TextSpan nameSpan, IAccessModifierToken? accessModifier, IdentifierName name, IMethodSelfParameterSyntax selfParameter, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax? @return, IBodySyntax body)
        => new StandardMethodDefinitionSyntax(span, file, nameSpan, accessModifier, name, selfParameter, parameters, @return, body);
}

// [Closed(typeof(GetterMethodDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGetterMethodDefinitionSyntax : IConcreteMethodDefinitionSyntax
{
    new IReturnSyntax Return { get; }
    IReturnSyntax? IMethodDefinitionSyntax.Return => Return;

    public static IGetterMethodDefinitionSyntax Create(TextSpan span, CodeFile file, TextSpan nameSpan, IAccessModifierToken? accessModifier, IdentifierName name, IMethodSelfParameterSyntax selfParameter, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax @return, IBodySyntax body)
        => new GetterMethodDefinitionSyntax(span, file, nameSpan, accessModifier, name, selfParameter, parameters, @return, body);
}

// [Closed(typeof(SetterMethodDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISetterMethodDefinitionSyntax : IConcreteMethodDefinitionSyntax
{

    public static ISetterMethodDefinitionSyntax Create(TextSpan span, CodeFile file, TextSpan nameSpan, IAccessModifierToken? accessModifier, IdentifierName name, IMethodSelfParameterSyntax selfParameter, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax? @return, IBodySyntax body)
        => new SetterMethodDefinitionSyntax(span, file, nameSpan, accessModifier, name, selfParameter, parameters, @return, body);
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

    public static IConstructorDefinitionSyntax Create(TextSpan span, CodeFile file, TextSpan nameSpan, IAccessModifierToken? accessModifier, IdentifierName? name, IConstructorSelfParameterSyntax selfParameter, IFixedList<IConstructorOrInitializerParameterSyntax> parameters, IBlockBodySyntax body)
        => new ConstructorDefinitionSyntax(span, file, nameSpan, accessModifier, name, selfParameter, parameters, body);
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

    public static IInitializerDefinitionSyntax Create(TextSpan span, CodeFile file, TextSpan nameSpan, IAccessModifierToken? accessModifier, IdentifierName? name, IInitializerSelfParameterSyntax selfParameter, IFixedList<IConstructorOrInitializerParameterSyntax> parameters, IBlockBodySyntax body)
        => new InitializerDefinitionSyntax(span, file, nameSpan, accessModifier, name, selfParameter, parameters, body);
}

// [Closed(typeof(FieldDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFieldDefinitionSyntax : IClassMemberDefinitionSyntax, IStructMemberDefinitionSyntax, IBindingSyntax
{
    new IdentifierName Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    ITypeSyntax Type { get; }
    IExpressionSyntax? Initializer { get; }

    public static IFieldDefinitionSyntax Create(TextSpan span, CodeFile file, TextSpan nameSpan, IAccessModifierToken? accessModifier, bool isMutableBinding, IdentifierName name, ITypeSyntax type, IExpressionSyntax? initializer)
        => new FieldDefinitionSyntax(span, file, nameSpan, accessModifier, isMutableBinding, name, type, initializer);
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

    public static IAssociatedFunctionDefinitionSyntax Create(TextSpan span, CodeFile file, TextSpan nameSpan, IAccessModifierToken? accessModifier, IdentifierName name, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax? @return, IBodySyntax body)
        => new AssociatedFunctionDefinitionSyntax(span, file, nameSpan, accessModifier, name, parameters, @return, body);
}

// [Closed(typeof(AttributeSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAttributeSyntax : ICodeSyntax
{
    IStandardTypeNameSyntax TypeName { get; }

    public static IAttributeSyntax Create(TextSpan span, IStandardTypeNameSyntax typeName)
        => new AttributeSyntax(span, typeName);
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

    public static ICapabilitySetSyntax Create(TextSpan span, CapabilitySet constraint)
        => new CapabilitySetSyntax(span, constraint);
}

// [Closed(typeof(CapabilitySyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICapabilitySyntax : ICapabilityConstraintSyntax
{
    IFixedList<ICapabilityToken> Tokens { get; }
    DeclaredCapability Declared { get; }
    Capability Capability { get; }

    public static ICapabilitySyntax Create(TextSpan span, ICapabilityConstraint constraint, IFixedList<ICapabilityToken> tokens, DeclaredCapability declared, Capability capability)
        => new CapabilitySyntax(span, constraint, tokens, declared, capability);
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

    public static INamedParameterSyntax Create(TextSpan span, TextSpan nameSpan, bool isMutableBinding, bool isLentBinding, IdentifierName name, ITypeSyntax type, IExpressionSyntax? defaultValue)
        => new NamedParameterSyntax(span, nameSpan, isMutableBinding, isLentBinding, name, type, defaultValue);
}

[Closed(
    typeof(IConstructorSelfParameterSyntax),
    typeof(IInitializerSelfParameterSyntax),
    typeof(IMethodSelfParameterSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISelfParameterSyntax : IParameterSyntax
{
    bool IsLentBinding { get; }
    ICapabilityConstraintSyntax Capability { get; }
}

// [Closed(typeof(ConstructorSelfParameterSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IConstructorSelfParameterSyntax : ISelfParameterSyntax
{
    new ICapabilitySyntax Capability { get; }
    ICapabilityConstraintSyntax ISelfParameterSyntax.Capability => Capability;

    public static IConstructorSelfParameterSyntax Create(TextSpan span, IdentifierName? name, bool isLentBinding, ICapabilitySyntax capability)
        => new ConstructorSelfParameterSyntax(span, name, isLentBinding, capability);
}

// [Closed(typeof(InitializerSelfParameterSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInitializerSelfParameterSyntax : ISelfParameterSyntax
{
    new ICapabilitySyntax Capability { get; }
    ICapabilityConstraintSyntax ISelfParameterSyntax.Capability => Capability;

    public static IInitializerSelfParameterSyntax Create(TextSpan span, IdentifierName? name, bool isLentBinding, ICapabilitySyntax capability)
        => new InitializerSelfParameterSyntax(span, name, isLentBinding, capability);
}

// [Closed(typeof(MethodSelfParameterSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMethodSelfParameterSyntax : ISelfParameterSyntax
{

    public static IMethodSelfParameterSyntax Create(TextSpan span, IdentifierName? name, bool isLentBinding, ICapabilityConstraintSyntax capability)
        => new MethodSelfParameterSyntax(span, name, isLentBinding, capability);
}

// [Closed(typeof(FieldParameterSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFieldParameterSyntax : IConstructorOrInitializerParameterSyntax
{
    new IdentifierName Name { get; }
    IdentifierName? IParameterSyntax.Name => Name;
    IExpressionSyntax? DefaultValue { get; }

    public static IFieldParameterSyntax Create(TextSpan span, IdentifierName name, IExpressionSyntax? defaultValue)
        => new FieldParameterSyntax(span, name, defaultValue);
}

// [Closed(typeof(ReturnSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IReturnSyntax : ICodeSyntax
{
    ITypeSyntax Type { get; }

    public static IReturnSyntax Create(TextSpan span, ITypeSyntax type)
        => new ReturnSyntax(span, type);
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

    public static IBlockBodySyntax Create(TextSpan span, IFixedList<IBodyStatementSyntax> statements)
        => new BlockBodySyntax(span, statements);
}

// [Closed(typeof(ExpressionBodySyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IExpressionBodySyntax : IBodySyntax
{
    IResultStatementSyntax ResultStatement { get; }

    public static IExpressionBodySyntax Create(TextSpan span, IResultStatementSyntax resultStatement, IFixedList<IStatementSyntax> statements)
        => new ExpressionBodySyntax(span, resultStatement, statements);
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

    public static IIdentifierTypeNameSyntax Create(TextSpan span, IdentifierName name)
        => new IdentifierTypeNameSyntax(span, name);
}

// [Closed(typeof(SpecialTypeNameSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISpecialTypeNameSyntax : ISimpleTypeNameSyntax
{
    new SpecialTypeName Name { get; }
    TypeName ITypeNameSyntax.Name => Name;

    public static ISpecialTypeNameSyntax Create(TextSpan span, SpecialTypeName name)
        => new SpecialTypeNameSyntax(span, name);
}

// [Closed(typeof(GenericTypeNameSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGenericTypeNameSyntax : IStandardTypeNameSyntax
{
    new GenericName Name { get; }
    StandardName IStandardTypeNameSyntax.Name => Name;
    IFixedList<ITypeSyntax> TypeArguments { get; }

    public static IGenericTypeNameSyntax Create(TextSpan span, GenericName name, IFixedList<ITypeSyntax> typeArguments)
        => new GenericTypeNameSyntax(span, name, typeArguments);
}

// [Closed(typeof(QualifiedTypeNameSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IQualifiedTypeNameSyntax : ITypeNameSyntax
{
    ITypeNameSyntax Context { get; }
    IStandardTypeNameSyntax QualifiedName { get; }

    public static IQualifiedTypeNameSyntax Create(TextSpan span, TypeName name, ITypeNameSyntax context, IStandardTypeNameSyntax qualifiedName)
        => new QualifiedTypeNameSyntax(span, name, context, qualifiedName);
}

// [Closed(typeof(OptionalTypeSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IOptionalTypeSyntax : ITypeSyntax
{
    ITypeSyntax Referent { get; }

    public static IOptionalTypeSyntax Create(TextSpan span, ITypeSyntax referent)
        => new OptionalTypeSyntax(span, referent);
}

// [Closed(typeof(CapabilityTypeSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICapabilityTypeSyntax : ITypeSyntax
{
    ICapabilitySyntax Capability { get; }
    ITypeSyntax Referent { get; }

    public static ICapabilityTypeSyntax Create(TextSpan span, ICapabilitySyntax capability, ITypeSyntax referent)
        => new CapabilityTypeSyntax(span, capability, referent);
}

// [Closed(typeof(FunctionTypeSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionTypeSyntax : ITypeSyntax
{
    IFixedList<IParameterTypeSyntax> Parameters { get; }
    IReturnTypeSyntax Return { get; }

    public static IFunctionTypeSyntax Create(TextSpan span, IFixedList<IParameterTypeSyntax> parameters, IReturnTypeSyntax @return)
        => new FunctionTypeSyntax(span, parameters, @return);
}

// [Closed(typeof(ParameterTypeSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IParameterTypeSyntax : ICodeSyntax
{
    bool IsLent { get; }
    ITypeSyntax Referent { get; }

    public static IParameterTypeSyntax Create(TextSpan span, bool isLent, ITypeSyntax referent)
        => new ParameterTypeSyntax(span, isLent, referent);
}

// [Closed(typeof(ReturnTypeSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IReturnTypeSyntax : ICodeSyntax
{
    ITypeSyntax Referent { get; }

    public static IReturnTypeSyntax Create(TextSpan span, ITypeSyntax referent)
        => new ReturnTypeSyntax(span, referent);
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

    public static ICapabilityViewpointTypeSyntax Create(TextSpan span, ICapabilitySyntax capability, ITypeSyntax referent)
        => new CapabilityViewpointTypeSyntax(span, capability, referent);
}

// [Closed(typeof(SelfViewpointTypeSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISelfViewpointTypeSyntax : IViewpointTypeSyntax
{

    public static ISelfViewpointTypeSyntax Create(TextSpan span, ITypeSyntax referent)
        => new SelfViewpointTypeSyntax(span, referent);
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

    public static IResultStatementSyntax Create(TextSpan span, IExpressionSyntax expression)
        => new ResultStatementSyntax(span, expression);
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

    public static IVariableDeclarationStatementSyntax Create(TextSpan span, bool isMutableBinding, TextSpan nameSpan, IdentifierName name, ICapabilitySyntax? capability, ITypeSyntax? type, IExpressionSyntax? initializer)
        => new VariableDeclarationStatementSyntax(span, isMutableBinding, nameSpan, name, capability, type, initializer);
}

// [Closed(typeof(ExpressionStatementSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IExpressionStatementSyntax : IBodyStatementSyntax
{
    IExpressionSyntax Expression { get; }

    public static IExpressionStatementSyntax Create(TextSpan span, IExpressionSyntax expression)
        => new ExpressionStatementSyntax(span, expression);
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

    public static IBindingContextPatternSyntax Create(TextSpan span, bool isMutableBinding, IPatternSyntax pattern, ITypeSyntax? type)
        => new BindingContextPatternSyntax(span, isMutableBinding, pattern, type);
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

    public static IBindingPatternSyntax Create(TextSpan span, bool isMutableBinding, TextSpan nameSpan, IdentifierName name)
        => new BindingPatternSyntax(span, isMutableBinding, nameSpan, name);
}

// [Closed(typeof(OptionalPatternSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IOptionalPatternSyntax : IOptionalOrBindingPatternSyntax
{
    IOptionalOrBindingPatternSyntax Pattern { get; }

    public static IOptionalPatternSyntax Create(TextSpan span, IOptionalOrBindingPatternSyntax pattern)
        => new OptionalPatternSyntax(span, pattern);
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

    public static IBlockExpressionSyntax Create(TextSpan span, IFixedList<IStatementSyntax> statements)
        => new BlockExpressionSyntax(span, statements);
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

    public static INewObjectExpressionSyntax Create(TextSpan span, ITypeNameSyntax type, IdentifierName? constructorName, TextSpan? constructorNameSpan, IFixedList<IExpressionSyntax> arguments)
        => new NewObjectExpressionSyntax(span, type, constructorName, constructorNameSpan, arguments);
}

// [Closed(typeof(UnsafeExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUnsafeExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Expression { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Primary;

    public static IUnsafeExpressionSyntax Create(TextSpan span, IExpressionSyntax expression)
        => new UnsafeExpressionSyntax(span, expression);
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

    public static IBoolLiteralExpressionSyntax Create(TextSpan span, bool value)
        => new BoolLiteralExpressionSyntax(span, value);
}

// [Closed(typeof(IntegerLiteralExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IIntegerLiteralExpressionSyntax : ILiteralExpressionSyntax
{
    BigInteger Value { get; }

    public static IIntegerLiteralExpressionSyntax Create(TextSpan span, BigInteger value)
        => new IntegerLiteralExpressionSyntax(span, value);
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

    public static IStringLiteralExpressionSyntax Create(TextSpan span, string value)
        => new StringLiteralExpressionSyntax(span, value);
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

    public static IAssignmentExpressionSyntax Create(TextSpan span, IAssignableExpressionSyntax leftOperand, AssignmentOperator @operator, IExpressionSyntax rightOperand)
        => new AssignmentExpressionSyntax(span, leftOperand, @operator, rightOperand);
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

    public static IBinaryOperatorExpressionSyntax Create(TextSpan span, IExpressionSyntax leftOperand, BinaryOperator @operator, IExpressionSyntax rightOperand)
        => new BinaryOperatorExpressionSyntax(span, leftOperand, @operator, rightOperand);
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

    public static IUnaryOperatorExpressionSyntax Create(TextSpan span, UnaryOperatorFixity fixity, UnaryOperator @operator, IExpressionSyntax operand)
        => new UnaryOperatorExpressionSyntax(span, fixity, @operator, operand);
}

// [Closed(typeof(IdExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IIdExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Referent { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Min;

    public static IIdExpressionSyntax Create(TextSpan span, IExpressionSyntax referent)
        => new IdExpressionSyntax(span, referent);
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

    public static IConversionExpressionSyntax Create(TextSpan span, IExpressionSyntax referent, ConversionOperator @operator, ITypeSyntax convertToType)
        => new ConversionExpressionSyntax(span, referent, @operator, convertToType);
}

// [Closed(typeof(PatternMatchExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPatternMatchExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Referent { get; }
    IPatternSyntax Pattern { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Conversion;

    public static IPatternMatchExpressionSyntax Create(TextSpan span, IExpressionSyntax referent, IPatternSyntax pattern)
        => new PatternMatchExpressionSyntax(span, referent, pattern);
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

    public static IIfExpressionSyntax Create(TextSpan span, IExpressionSyntax condition, IBlockOrResultSyntax thenBlock, IElseClauseSyntax? elseClause)
        => new IfExpressionSyntax(span, condition, thenBlock, elseClause);
}

// [Closed(typeof(LoopExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ILoopExpressionSyntax : IExpressionSyntax
{
    IBlockExpressionSyntax Block { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Primary;

    public static ILoopExpressionSyntax Create(TextSpan span, IBlockExpressionSyntax block)
        => new LoopExpressionSyntax(span, block);
}

// [Closed(typeof(WhileExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IWhileExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Condition { get; }
    IBlockExpressionSyntax Block { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Min;

    public static IWhileExpressionSyntax Create(TextSpan span, IExpressionSyntax condition, IBlockExpressionSyntax block)
        => new WhileExpressionSyntax(span, condition, block);
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

    public static IForeachExpressionSyntax Create(TextSpan span, bool isMutableBinding, TextSpan nameSpan, IdentifierName variableName, IExpressionSyntax inExpression, ITypeSyntax? type, IBlockExpressionSyntax block)
        => new ForeachExpressionSyntax(span, isMutableBinding, nameSpan, variableName, inExpression, type, block);
}

// [Closed(typeof(BreakExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBreakExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax? Value { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => Value is not null ? OperatorPrecedence.Min : OperatorPrecedence.Primary;

    public static IBreakExpressionSyntax Create(TextSpan span, IExpressionSyntax? value)
        => new BreakExpressionSyntax(span, value);
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

    public static IReturnExpressionSyntax Create(TextSpan span, IExpressionSyntax? value)
        => new ReturnExpressionSyntax(span, value);
}

// [Closed(typeof(InvocationExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInvocationExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Expression { get; }
    IFixedList<IExpressionSyntax> Arguments { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Primary;

    public static IInvocationExpressionSyntax Create(TextSpan span, IExpressionSyntax expression, IFixedList<IExpressionSyntax> arguments)
        => new InvocationExpressionSyntax(span, expression, arguments);
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

    public static IIdentifierNameExpressionSyntax Create(TextSpan span, IdentifierName name)
        => new IdentifierNameExpressionSyntax(span, name);
}

// [Closed(typeof(SpecialTypeNameExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISpecialTypeNameExpressionSyntax : INameExpressionSyntax
{
    SpecialTypeName Name { get; }

    public static ISpecialTypeNameExpressionSyntax Create(TextSpan span, SpecialTypeName name)
        => new SpecialTypeNameExpressionSyntax(span, name);
}

// [Closed(typeof(GenericNameExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGenericNameExpressionSyntax : IStandardNameExpressionSyntax
{
    new GenericName Name { get; }
    StandardName IStandardNameExpressionSyntax.Name => Name;
    IFixedList<ITypeSyntax> TypeArguments { get; }

    public static IGenericNameExpressionSyntax Create(TextSpan span, GenericName name, IFixedList<ITypeSyntax> typeArguments)
        => new GenericNameExpressionSyntax(span, name, typeArguments);
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

    public static ISelfExpressionSyntax Create(TextSpan span, bool isImplicit)
        => new SelfExpressionSyntax(span, isImplicit);
}

// [Closed(typeof(MemberAccessExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMemberAccessExpressionSyntax : INameExpressionSyntax, IAssignableExpressionSyntax
{
    IExpressionSyntax Context { get; }
    StandardName MemberName { get; }
    IFixedList<ITypeSyntax> TypeArguments { get; }
    TextSpan MemberNameSpan { get; }

    public static IMemberAccessExpressionSyntax Create(TextSpan span, IExpressionSyntax context, StandardName memberName, IFixedList<ITypeSyntax> typeArguments, TextSpan memberNameSpan)
        => new MemberAccessExpressionSyntax(span, context, memberName, typeArguments, memberNameSpan);
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

    public static IMoveExpressionSyntax Create(TextSpan span, ISimpleNameSyntax referent)
        => new MoveExpressionSyntax(span, referent);
}

// [Closed(typeof(FreezeExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFreezeExpressionSyntax : IExpressionSyntax
{
    ISimpleNameSyntax Referent { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Min;

    public static IFreezeExpressionSyntax Create(TextSpan span, ISimpleNameSyntax referent)
        => new FreezeExpressionSyntax(span, referent);
}

// [Closed(typeof(AsyncBlockExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAsyncBlockExpressionSyntax : IExpressionSyntax
{
    IBlockExpressionSyntax Block { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Primary;

    public static IAsyncBlockExpressionSyntax Create(TextSpan span, IBlockExpressionSyntax block)
        => new AsyncBlockExpressionSyntax(span, block);
}

// [Closed(typeof(AsyncStartExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAsyncStartExpressionSyntax : IExpressionSyntax
{
    bool Scheduled { get; }
    IExpressionSyntax Expression { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Min;

    public static IAsyncStartExpressionSyntax Create(TextSpan span, bool scheduled, IExpressionSyntax expression)
        => new AsyncStartExpressionSyntax(span, scheduled, expression);
}

// [Closed(typeof(AwaitExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAwaitExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Expression { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Unary;

    public static IAwaitExpressionSyntax Create(TextSpan span, IExpressionSyntax expression)
        => new AwaitExpressionSyntax(span, expression);
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class CompilationUnitSyntax : ICompilationUnitSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public NamespaceName ImplicitNamespaceName { [DebuggerStepThrough] get; }
    public DiagnosticCollection Diagnostics { [DebuggerStepThrough] get; }
    public IFixedList<IUsingDirectiveSyntax> UsingDirectives { [DebuggerStepThrough] get; }
    public IFixedList<INonMemberDefinitionSyntax> Definitions { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.CompilationUnit_ToString(this);

    public CompilationUnitSyntax(TextSpan span, CodeFile file, NamespaceName implicitNamespaceName, DiagnosticCollection diagnostics, IFixedList<IUsingDirectiveSyntax> usingDirectives, IFixedList<INonMemberDefinitionSyntax> definitions)
    {
        Span = span;
        File = file;
        ImplicitNamespaceName = implicitNamespaceName;
        Diagnostics = diagnostics;
        UsingDirectives = usingDirectives;
        Definitions = definitions;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class UsingDirectiveSyntax : IUsingDirectiveSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public NamespaceName Name { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.UsingDirective_ToString(this);

    public UsingDirectiveSyntax(TextSpan span, NamespaceName name)
    {
        Span = span;
        Name = name;
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
    public TextSpan Span { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TypeName? Name { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public bool IsGlobalQualified { [DebuggerStepThrough] get; }
    public NamespaceName DeclaredNames { [DebuggerStepThrough] get; }
    public IFixedList<IUsingDirectiveSyntax> UsingDirectives { [DebuggerStepThrough] get; }
    public IFixedList<INonMemberDefinitionSyntax> Definitions { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.NamespaceDefinition_ToString(this);

    public NamespaceDefinitionSyntax(TextSpan span, CodeFile file, TypeName? name, TextSpan nameSpan, bool isGlobalQualified, NamespaceName declaredNames, IFixedList<IUsingDirectiveSyntax> usingDirectives, IFixedList<INonMemberDefinitionSyntax> definitions)
    {
        Span = span;
        File = file;
        Name = name;
        NameSpan = nameSpan;
        IsGlobalQualified = isGlobalQualified;
        DeclaredNames = declaredNames;
        UsingDirectives = usingDirectives;
        Definitions = definitions;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class FunctionDefinitionSyntax : IFunctionDefinitionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public IFixedList<IAttributeSyntax> Attributes { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterSyntax> Parameters { [DebuggerStepThrough] get; }
    public IReturnSyntax? Return { [DebuggerStepThrough] get; }
    public IBodySyntax Body { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.FunctionDefinition_ToString(this);

    public FunctionDefinitionSyntax(TextSpan span, CodeFile file, TextSpan nameSpan, IAccessModifierToken? accessModifier, IFixedList<IAttributeSyntax> attributes, IdentifierName name, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax? @return, IBodySyntax body)
    {
        Span = span;
        File = file;
        NameSpan = nameSpan;
        AccessModifier = accessModifier;
        Attributes = attributes;
        Name = name;
        Parameters = parameters;
        Return = @return;
        Body = body;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class ClassDefinitionSyntax : IClassDefinitionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public IConstKeywordToken? ConstModifier { [DebuggerStepThrough] get; }
    public IMoveKeywordToken? MoveModifier { [DebuggerStepThrough] get; }
    public StandardName Name { [DebuggerStepThrough] get; }
    public IAbstractKeywordToken? AbstractModifier { [DebuggerStepThrough] get; }
    public IFixedList<IGenericParameterSyntax> GenericParameters { [DebuggerStepThrough] get; }
    public IStandardTypeNameSyntax? BaseTypeName { [DebuggerStepThrough] get; }
    public IFixedList<IStandardTypeNameSyntax> SupertypeNames { [DebuggerStepThrough] get; }
    public IFixedList<IClassMemberDefinitionSyntax> Members { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.ClassDefinition_ToString(this);

    public ClassDefinitionSyntax(TextSpan span, CodeFile file, TextSpan nameSpan, IAccessModifierToken? accessModifier, IConstKeywordToken? constModifier, IMoveKeywordToken? moveModifier, StandardName name, IAbstractKeywordToken? abstractModifier, IFixedList<IGenericParameterSyntax> genericParameters, IStandardTypeNameSyntax? baseTypeName, IFixedList<IStandardTypeNameSyntax> supertypeNames, IFixedList<IClassMemberDefinitionSyntax> members)
    {
        Span = span;
        File = file;
        NameSpan = nameSpan;
        AccessModifier = accessModifier;
        ConstModifier = constModifier;
        MoveModifier = moveModifier;
        Name = name;
        AbstractModifier = abstractModifier;
        GenericParameters = genericParameters;
        BaseTypeName = baseTypeName;
        SupertypeNames = supertypeNames;
        Members = members;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class StructDefinitionSyntax : IStructDefinitionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public IConstKeywordToken? ConstModifier { [DebuggerStepThrough] get; }
    public IMoveKeywordToken? MoveModifier { [DebuggerStepThrough] get; }
    public StandardName Name { [DebuggerStepThrough] get; }
    public IFixedList<IGenericParameterSyntax> GenericParameters { [DebuggerStepThrough] get; }
    public IFixedList<IStandardTypeNameSyntax> SupertypeNames { [DebuggerStepThrough] get; }
    public IFixedList<IStructMemberDefinitionSyntax> Members { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.StructDefinition_ToString(this);

    public StructDefinitionSyntax(TextSpan span, CodeFile file, TextSpan nameSpan, IAccessModifierToken? accessModifier, IConstKeywordToken? constModifier, IMoveKeywordToken? moveModifier, StandardName name, IFixedList<IGenericParameterSyntax> genericParameters, IFixedList<IStandardTypeNameSyntax> supertypeNames, IFixedList<IStructMemberDefinitionSyntax> members)
    {
        Span = span;
        File = file;
        NameSpan = nameSpan;
        AccessModifier = accessModifier;
        ConstModifier = constModifier;
        MoveModifier = moveModifier;
        Name = name;
        GenericParameters = genericParameters;
        SupertypeNames = supertypeNames;
        Members = members;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class TraitDefinitionSyntax : ITraitDefinitionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public IConstKeywordToken? ConstModifier { [DebuggerStepThrough] get; }
    public IMoveKeywordToken? MoveModifier { [DebuggerStepThrough] get; }
    public StandardName Name { [DebuggerStepThrough] get; }
    public IFixedList<IGenericParameterSyntax> GenericParameters { [DebuggerStepThrough] get; }
    public IFixedList<IStandardTypeNameSyntax> SupertypeNames { [DebuggerStepThrough] get; }
    public IFixedList<ITraitMemberDefinitionSyntax> Members { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.TraitDefinition_ToString(this);

    public TraitDefinitionSyntax(TextSpan span, CodeFile file, TextSpan nameSpan, IAccessModifierToken? accessModifier, IConstKeywordToken? constModifier, IMoveKeywordToken? moveModifier, StandardName name, IFixedList<IGenericParameterSyntax> genericParameters, IFixedList<IStandardTypeNameSyntax> supertypeNames, IFixedList<ITraitMemberDefinitionSyntax> members)
    {
        Span = span;
        File = file;
        NameSpan = nameSpan;
        AccessModifier = accessModifier;
        ConstModifier = constModifier;
        MoveModifier = moveModifier;
        Name = name;
        GenericParameters = genericParameters;
        SupertypeNames = supertypeNames;
        Members = members;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class GenericParameterSyntax : IGenericParameterSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public ICapabilityConstraintSyntax Constraint { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public TypeParameterIndependence Independence { [DebuggerStepThrough] get; }
    public TypeParameterVariance Variance { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.GenericParameter_ToString(this);

    public GenericParameterSyntax(TextSpan span, ICapabilityConstraintSyntax constraint, IdentifierName name, TypeParameterIndependence independence, TypeParameterVariance variance)
    {
        Span = span;
        Constraint = constraint;
        Name = name;
        Independence = independence;
        Variance = variance;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class AbstractMethodDefinitionSyntax : IAbstractMethodDefinitionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IMethodSelfParameterSyntax SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterSyntax> Parameters { [DebuggerStepThrough] get; }
    public IReturnSyntax? Return { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.AbstractMethodDefinition_ToString(this);

    public AbstractMethodDefinitionSyntax(TextSpan span, CodeFile file, TextSpan nameSpan, IAccessModifierToken? accessModifier, IdentifierName name, IMethodSelfParameterSyntax selfParameter, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax? @return)
    {
        Span = span;
        File = file;
        NameSpan = nameSpan;
        AccessModifier = accessModifier;
        Name = name;
        SelfParameter = selfParameter;
        Parameters = parameters;
        Return = @return;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class StandardMethodDefinitionSyntax : IStandardMethodDefinitionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IMethodSelfParameterSyntax SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterSyntax> Parameters { [DebuggerStepThrough] get; }
    public IReturnSyntax? Return { [DebuggerStepThrough] get; }
    public IBodySyntax Body { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.StandardMethodDefinition_ToString(this);

    public StandardMethodDefinitionSyntax(TextSpan span, CodeFile file, TextSpan nameSpan, IAccessModifierToken? accessModifier, IdentifierName name, IMethodSelfParameterSyntax selfParameter, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax? @return, IBodySyntax body)
    {
        Span = span;
        File = file;
        NameSpan = nameSpan;
        AccessModifier = accessModifier;
        Name = name;
        SelfParameter = selfParameter;
        Parameters = parameters;
        Return = @return;
        Body = body;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class GetterMethodDefinitionSyntax : IGetterMethodDefinitionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IMethodSelfParameterSyntax SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterSyntax> Parameters { [DebuggerStepThrough] get; }
    public IReturnSyntax Return { [DebuggerStepThrough] get; }
    public IBodySyntax Body { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.GetterMethodDefinition_ToString(this);

    public GetterMethodDefinitionSyntax(TextSpan span, CodeFile file, TextSpan nameSpan, IAccessModifierToken? accessModifier, IdentifierName name, IMethodSelfParameterSyntax selfParameter, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax @return, IBodySyntax body)
    {
        Span = span;
        File = file;
        NameSpan = nameSpan;
        AccessModifier = accessModifier;
        Name = name;
        SelfParameter = selfParameter;
        Parameters = parameters;
        Return = @return;
        Body = body;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class SetterMethodDefinitionSyntax : ISetterMethodDefinitionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IMethodSelfParameterSyntax SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterSyntax> Parameters { [DebuggerStepThrough] get; }
    public IReturnSyntax? Return { [DebuggerStepThrough] get; }
    public IBodySyntax Body { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.SetterMethodDefinition_ToString(this);

    public SetterMethodDefinitionSyntax(TextSpan span, CodeFile file, TextSpan nameSpan, IAccessModifierToken? accessModifier, IdentifierName name, IMethodSelfParameterSyntax selfParameter, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax? @return, IBodySyntax body)
    {
        Span = span;
        File = file;
        NameSpan = nameSpan;
        AccessModifier = accessModifier;
        Name = name;
        SelfParameter = selfParameter;
        Parameters = parameters;
        Return = @return;
        Body = body;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class ConstructorDefinitionSyntax : IConstructorDefinitionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public IConstructorSelfParameterSyntax SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<IConstructorOrInitializerParameterSyntax> Parameters { [DebuggerStepThrough] get; }
    public IBlockBodySyntax Body { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.ConstructorDefinition_ToString(this);

    public ConstructorDefinitionSyntax(TextSpan span, CodeFile file, TextSpan nameSpan, IAccessModifierToken? accessModifier, IdentifierName? name, IConstructorSelfParameterSyntax selfParameter, IFixedList<IConstructorOrInitializerParameterSyntax> parameters, IBlockBodySyntax body)
    {
        Span = span;
        File = file;
        NameSpan = nameSpan;
        AccessModifier = accessModifier;
        Name = name;
        SelfParameter = selfParameter;
        Parameters = parameters;
        Body = body;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class InitializerDefinitionSyntax : IInitializerDefinitionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public IInitializerSelfParameterSyntax SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<IConstructorOrInitializerParameterSyntax> Parameters { [DebuggerStepThrough] get; }
    public IBlockBodySyntax Body { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.InitializerDefinition_ToString(this);

    public InitializerDefinitionSyntax(TextSpan span, CodeFile file, TextSpan nameSpan, IAccessModifierToken? accessModifier, IdentifierName? name, IInitializerSelfParameterSyntax selfParameter, IFixedList<IConstructorOrInitializerParameterSyntax> parameters, IBlockBodySyntax body)
    {
        Span = span;
        File = file;
        NameSpan = nameSpan;
        AccessModifier = accessModifier;
        Name = name;
        SelfParameter = selfParameter;
        Parameters = parameters;
        Body = body;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class FieldDefinitionSyntax : IFieldDefinitionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public ITypeSyntax Type { [DebuggerStepThrough] get; }
    public IExpressionSyntax? Initializer { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.FieldDefinition_ToString(this);

    public FieldDefinitionSyntax(TextSpan span, CodeFile file, TextSpan nameSpan, IAccessModifierToken? accessModifier, bool isMutableBinding, IdentifierName name, ITypeSyntax type, IExpressionSyntax? initializer)
    {
        Span = span;
        File = file;
        NameSpan = nameSpan;
        AccessModifier = accessModifier;
        IsMutableBinding = isMutableBinding;
        Name = name;
        Type = type;
        Initializer = initializer;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class AssociatedFunctionDefinitionSyntax : IAssociatedFunctionDefinitionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterSyntax> Parameters { [DebuggerStepThrough] get; }
    public IReturnSyntax? Return { [DebuggerStepThrough] get; }
    public IBodySyntax Body { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.AssociatedFunctionDefinition_ToString(this);

    public AssociatedFunctionDefinitionSyntax(TextSpan span, CodeFile file, TextSpan nameSpan, IAccessModifierToken? accessModifier, IdentifierName name, IFixedList<INamedParameterSyntax> parameters, IReturnSyntax? @return, IBodySyntax body)
    {
        Span = span;
        File = file;
        NameSpan = nameSpan;
        AccessModifier = accessModifier;
        Name = name;
        Parameters = parameters;
        Return = @return;
        Body = body;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class AttributeSyntax : IAttributeSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IStandardTypeNameSyntax TypeName { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.Attribute_ToString(this);

    public AttributeSyntax(TextSpan span, IStandardTypeNameSyntax typeName)
    {
        Span = span;
        TypeName = typeName;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class CapabilitySetSyntax : ICapabilitySetSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public CapabilitySet Constraint { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.CapabilitySet_ToString(this);

    public CapabilitySetSyntax(TextSpan span, CapabilitySet constraint)
    {
        Span = span;
        Constraint = constraint;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class CapabilitySyntax : ICapabilitySyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public ICapabilityConstraint Constraint { [DebuggerStepThrough] get; }
    public IFixedList<ICapabilityToken> Tokens { [DebuggerStepThrough] get; }
    public DeclaredCapability Declared { [DebuggerStepThrough] get; }
    public Capability Capability { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.Capability_ToString(this);

    public CapabilitySyntax(TextSpan span, ICapabilityConstraint constraint, IFixedList<ICapabilityToken> tokens, DeclaredCapability declared, Capability capability)
    {
        Span = span;
        Constraint = constraint;
        Tokens = tokens;
        Declared = declared;
        Capability = capability;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class NamedParameterSyntax : INamedParameterSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public bool IsLentBinding { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public ITypeSyntax Type { [DebuggerStepThrough] get; }
    public IExpressionSyntax? DefaultValue { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.NamedParameter_ToString(this);

    public NamedParameterSyntax(TextSpan span, TextSpan nameSpan, bool isMutableBinding, bool isLentBinding, IdentifierName name, ITypeSyntax type, IExpressionSyntax? defaultValue)
    {
        Span = span;
        NameSpan = nameSpan;
        IsMutableBinding = isMutableBinding;
        IsLentBinding = isLentBinding;
        Name = name;
        Type = type;
        DefaultValue = defaultValue;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class ConstructorSelfParameterSyntax : IConstructorSelfParameterSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public bool IsLentBinding { [DebuggerStepThrough] get; }
    public ICapabilitySyntax Capability { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.SelfParameter_ToString(this);

    public ConstructorSelfParameterSyntax(TextSpan span, IdentifierName? name, bool isLentBinding, ICapabilitySyntax capability)
    {
        Span = span;
        Name = name;
        IsLentBinding = isLentBinding;
        Capability = capability;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class InitializerSelfParameterSyntax : IInitializerSelfParameterSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public bool IsLentBinding { [DebuggerStepThrough] get; }
    public ICapabilitySyntax Capability { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.SelfParameter_ToString(this);

    public InitializerSelfParameterSyntax(TextSpan span, IdentifierName? name, bool isLentBinding, ICapabilitySyntax capability)
    {
        Span = span;
        Name = name;
        IsLentBinding = isLentBinding;
        Capability = capability;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class MethodSelfParameterSyntax : IMethodSelfParameterSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IdentifierName? Name { [DebuggerStepThrough] get; }
    public bool IsLentBinding { [DebuggerStepThrough] get; }
    public ICapabilityConstraintSyntax Capability { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.SelfParameter_ToString(this);

    public MethodSelfParameterSyntax(TextSpan span, IdentifierName? name, bool isLentBinding, ICapabilityConstraintSyntax capability)
    {
        Span = span;
        Name = name;
        IsLentBinding = isLentBinding;
        Capability = capability;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class FieldParameterSyntax : IFieldParameterSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IExpressionSyntax? DefaultValue { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.FieldParameter_ToString(this);

    public FieldParameterSyntax(TextSpan span, IdentifierName name, IExpressionSyntax? defaultValue)
    {
        Span = span;
        Name = name;
        DefaultValue = defaultValue;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class ReturnSyntax : IReturnSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public ITypeSyntax Type { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.Return_ToString(this);

    public ReturnSyntax(TextSpan span, ITypeSyntax type)
    {
        Span = span;
        Type = type;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class BlockBodySyntax : IBlockBodySyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IFixedList<IBodyStatementSyntax> Statements { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.BlockBody_ToString(this);

    public BlockBodySyntax(TextSpan span, IFixedList<IBodyStatementSyntax> statements)
    {
        Span = span;
        Statements = statements;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class ExpressionBodySyntax : IExpressionBodySyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IResultStatementSyntax ResultStatement { [DebuggerStepThrough] get; }
    public IFixedList<IStatementSyntax> Statements { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.ExpressionBody_ToString(this);

    public ExpressionBodySyntax(TextSpan span, IResultStatementSyntax resultStatement, IFixedList<IStatementSyntax> statements)
    {
        Span = span;
        ResultStatement = resultStatement;
        Statements = statements;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class IdentifierTypeNameSyntax : IIdentifierTypeNameSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.IdentifierTypeName_ToString(this);

    public IdentifierTypeNameSyntax(TextSpan span, IdentifierName name)
    {
        Span = span;
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class SpecialTypeNameSyntax : ISpecialTypeNameSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public SpecialTypeName Name { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.SpecialTypeName_ToString(this);

    public SpecialTypeNameSyntax(TextSpan span, SpecialTypeName name)
    {
        Span = span;
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class GenericTypeNameSyntax : IGenericTypeNameSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public GenericName Name { [DebuggerStepThrough] get; }
    public IFixedList<ITypeSyntax> TypeArguments { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.GenericTypeName_ToString(this);

    public GenericTypeNameSyntax(TextSpan span, GenericName name, IFixedList<ITypeSyntax> typeArguments)
    {
        Span = span;
        Name = name;
        TypeArguments = typeArguments;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class QualifiedTypeNameSyntax : IQualifiedTypeNameSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public TypeName Name { [DebuggerStepThrough] get; }
    public ITypeNameSyntax Context { [DebuggerStepThrough] get; }
    public IStandardTypeNameSyntax QualifiedName { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.QualifiedTypeName_ToString(this);

    public QualifiedTypeNameSyntax(TextSpan span, TypeName name, ITypeNameSyntax context, IStandardTypeNameSyntax qualifiedName)
    {
        Span = span;
        Name = name;
        Context = context;
        QualifiedName = qualifiedName;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class OptionalTypeSyntax : IOptionalTypeSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public ITypeSyntax Referent { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.OptionalType_ToString(this);

    public OptionalTypeSyntax(TextSpan span, ITypeSyntax referent)
    {
        Span = span;
        Referent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class CapabilityTypeSyntax : ICapabilityTypeSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public ICapabilitySyntax Capability { [DebuggerStepThrough] get; }
    public ITypeSyntax Referent { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.CapabilityType_ToString(this);

    public CapabilityTypeSyntax(TextSpan span, ICapabilitySyntax capability, ITypeSyntax referent)
    {
        Span = span;
        Capability = capability;
        Referent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class FunctionTypeSyntax : IFunctionTypeSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IFixedList<IParameterTypeSyntax> Parameters { [DebuggerStepThrough] get; }
    public IReturnTypeSyntax Return { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.FunctionType_ToString(this);

    public FunctionTypeSyntax(TextSpan span, IFixedList<IParameterTypeSyntax> parameters, IReturnTypeSyntax @return)
    {
        Span = span;
        Parameters = parameters;
        Return = @return;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class ParameterTypeSyntax : IParameterTypeSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public bool IsLent { [DebuggerStepThrough] get; }
    public ITypeSyntax Referent { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.ParameterType_ToString(this);

    public ParameterTypeSyntax(TextSpan span, bool isLent, ITypeSyntax referent)
    {
        Span = span;
        IsLent = isLent;
        Referent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class ReturnTypeSyntax : IReturnTypeSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public ITypeSyntax Referent { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.ReturnType_ToString(this);

    public ReturnTypeSyntax(TextSpan span, ITypeSyntax referent)
    {
        Span = span;
        Referent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class CapabilityViewpointTypeSyntax : ICapabilityViewpointTypeSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public ICapabilitySyntax Capability { [DebuggerStepThrough] get; }
    public ITypeSyntax Referent { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.CapabilityViewpointType_ToString(this);

    public CapabilityViewpointTypeSyntax(TextSpan span, ICapabilitySyntax capability, ITypeSyntax referent)
    {
        Span = span;
        Capability = capability;
        Referent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class SelfViewpointTypeSyntax : ISelfViewpointTypeSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public ITypeSyntax Referent { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.SelfViewpointType_ToString(this);

    public SelfViewpointTypeSyntax(TextSpan span, ITypeSyntax referent)
    {
        Span = span;
        Referent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class ResultStatementSyntax : IResultStatementSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax Expression { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.ResultStatement_ToString(this);

    public ResultStatementSyntax(TextSpan span, IExpressionSyntax expression)
    {
        Span = span;
        Expression = expression;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class VariableDeclarationStatementSyntax : IVariableDeclarationStatementSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public ICapabilitySyntax? Capability { [DebuggerStepThrough] get; }
    public ITypeSyntax? Type { [DebuggerStepThrough] get; }
    public IExpressionSyntax? Initializer { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.VariableDeclarationStatement_ToString(this);

    public VariableDeclarationStatementSyntax(TextSpan span, bool isMutableBinding, TextSpan nameSpan, IdentifierName name, ICapabilitySyntax? capability, ITypeSyntax? type, IExpressionSyntax? initializer)
    {
        Span = span;
        IsMutableBinding = isMutableBinding;
        NameSpan = nameSpan;
        Name = name;
        Capability = capability;
        Type = type;
        Initializer = initializer;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class ExpressionStatementSyntax : IExpressionStatementSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax Expression { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.ExpressionStatement_ToString(this);

    public ExpressionStatementSyntax(TextSpan span, IExpressionSyntax expression)
    {
        Span = span;
        Expression = expression;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class BindingContextPatternSyntax : IBindingContextPatternSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public IPatternSyntax Pattern { [DebuggerStepThrough] get; }
    public ITypeSyntax? Type { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.BindingContextPattern_ToString(this);

    public BindingContextPatternSyntax(TextSpan span, bool isMutableBinding, IPatternSyntax pattern, ITypeSyntax? type)
    {
        Span = span;
        IsMutableBinding = isMutableBinding;
        Pattern = pattern;
        Type = type;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class BindingPatternSyntax : IBindingPatternSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.BindingPattern_ToString(this);

    public BindingPatternSyntax(TextSpan span, bool isMutableBinding, TextSpan nameSpan, IdentifierName name)
    {
        Span = span;
        IsMutableBinding = isMutableBinding;
        NameSpan = nameSpan;
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class OptionalPatternSyntax : IOptionalPatternSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IOptionalOrBindingPatternSyntax Pattern { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.OptionalPattern_ToString(this);

    public OptionalPatternSyntax(TextSpan span, IOptionalOrBindingPatternSyntax pattern)
    {
        Span = span;
        Pattern = pattern;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class BlockExpressionSyntax : IBlockExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IFixedList<IStatementSyntax> Statements { [DebuggerStepThrough] get; }
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Primary;
    public override string ToString()
        => FormattingAspect.BlockExpression_ToString(this);

    public BlockExpressionSyntax(TextSpan span, IFixedList<IStatementSyntax> statements)
    {
        Span = span;
        Statements = statements;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class NewObjectExpressionSyntax : INewObjectExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public ITypeNameSyntax Type { [DebuggerStepThrough] get; }
    public IdentifierName? ConstructorName { [DebuggerStepThrough] get; }
    public TextSpan? ConstructorNameSpan { [DebuggerStepThrough] get; }
    public IFixedList<IExpressionSyntax> Arguments { [DebuggerStepThrough] get; }
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Min;
    public override string ToString()
        => FormattingAspect.NewObjectExpression_ToString(this);

    public NewObjectExpressionSyntax(TextSpan span, ITypeNameSyntax type, IdentifierName? constructorName, TextSpan? constructorNameSpan, IFixedList<IExpressionSyntax> arguments)
    {
        Span = span;
        Type = type;
        ConstructorName = constructorName;
        ConstructorNameSpan = constructorNameSpan;
        Arguments = arguments;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class UnsafeExpressionSyntax : IUnsafeExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax Expression { [DebuggerStepThrough] get; }
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Primary;
    public override string ToString()
        => FormattingAspect.UnsafeExpression_ToString(this);

    public UnsafeExpressionSyntax(TextSpan span, IExpressionSyntax expression)
    {
        Span = span;
        Expression = expression;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class BoolLiteralExpressionSyntax : IBoolLiteralExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public bool Value { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.BoolLiteralExpression_ToString(this);
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Primary;

    public BoolLiteralExpressionSyntax(TextSpan span, bool value)
    {
        Span = span;
        Value = value;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class IntegerLiteralExpressionSyntax : IIntegerLiteralExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public BigInteger Value { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.IntegerLiteralExpression_ToString(this);
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Primary;

    public IntegerLiteralExpressionSyntax(TextSpan span, BigInteger value)
    {
        Span = span;
        Value = value;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class NoneLiteralExpressionSyntax : INoneLiteralExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.NoneLiteralExpression_ToString(this);
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Primary;

    public NoneLiteralExpressionSyntax(TextSpan span)
    {
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class StringLiteralExpressionSyntax : IStringLiteralExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public string Value { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.StringLiteralExpression_ToString(this);
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Primary;

    public StringLiteralExpressionSyntax(TextSpan span, string value)
    {
        Span = span;
        Value = value;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class AssignmentExpressionSyntax : IAssignmentExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IAssignableExpressionSyntax LeftOperand { [DebuggerStepThrough] get; }
    public AssignmentOperator Operator { [DebuggerStepThrough] get; }
    public IExpressionSyntax RightOperand { [DebuggerStepThrough] get; }
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Assignment;
    public override string ToString()
        => FormattingAspect.AssignmentExpression_ToString(this);

    public AssignmentExpressionSyntax(TextSpan span, IAssignableExpressionSyntax leftOperand, AssignmentOperator @operator, IExpressionSyntax rightOperand)
    {
        Span = span;
        LeftOperand = leftOperand;
        Operator = @operator;
        RightOperand = rightOperand;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class BinaryOperatorExpressionSyntax : IBinaryOperatorExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax LeftOperand { [DebuggerStepThrough] get; }
    public BinaryOperator Operator { [DebuggerStepThrough] get; }
    public IExpressionSyntax RightOperand { [DebuggerStepThrough] get; }
    public OperatorPrecedence ExpressionPrecedence
        => Operator.Precedence();
    public override string ToString()
        => FormattingAspect.BinaryOperatorExpression_ToString(this);

    public BinaryOperatorExpressionSyntax(TextSpan span, IExpressionSyntax leftOperand, BinaryOperator @operator, IExpressionSyntax rightOperand)
    {
        Span = span;
        LeftOperand = leftOperand;
        Operator = @operator;
        RightOperand = rightOperand;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class UnaryOperatorExpressionSyntax : IUnaryOperatorExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public UnaryOperatorFixity Fixity { [DebuggerStepThrough] get; }
    public UnaryOperator Operator { [DebuggerStepThrough] get; }
    public IExpressionSyntax Operand { [DebuggerStepThrough] get; }
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Unary;
    public override string ToString()
        => FormattingAspect.UnaryOperatorExpression_ToString(this);

    public UnaryOperatorExpressionSyntax(TextSpan span, UnaryOperatorFixity fixity, UnaryOperator @operator, IExpressionSyntax operand)
    {
        Span = span;
        Fixity = fixity;
        Operator = @operator;
        Operand = operand;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class IdExpressionSyntax : IIdExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax Referent { [DebuggerStepThrough] get; }
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Min;
    public override string ToString()
        => FormattingAspect.IdExpression_ToString(this);

    public IdExpressionSyntax(TextSpan span, IExpressionSyntax referent)
    {
        Span = span;
        Referent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class ConversionExpressionSyntax : IConversionExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax Referent { [DebuggerStepThrough] get; }
    public ConversionOperator Operator { [DebuggerStepThrough] get; }
    public ITypeSyntax ConvertToType { [DebuggerStepThrough] get; }
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Conversion;
    public override string ToString()
        => FormattingAspect.ConversionExpression_ToString(this);

    public ConversionExpressionSyntax(TextSpan span, IExpressionSyntax referent, ConversionOperator @operator, ITypeSyntax convertToType)
    {
        Span = span;
        Referent = referent;
        Operator = @operator;
        ConvertToType = convertToType;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class PatternMatchExpressionSyntax : IPatternMatchExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax Referent { [DebuggerStepThrough] get; }
    public IPatternSyntax Pattern { [DebuggerStepThrough] get; }
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Conversion;
    public override string ToString()
        => FormattingAspect.PatternMatchExpression_ToString(this);

    public PatternMatchExpressionSyntax(TextSpan span, IExpressionSyntax referent, IPatternSyntax pattern)
    {
        Span = span;
        Referent = referent;
        Pattern = pattern;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class IfExpressionSyntax : IIfExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax Condition { [DebuggerStepThrough] get; }
    public IBlockOrResultSyntax ThenBlock { [DebuggerStepThrough] get; }
    public IElseClauseSyntax? ElseClause { [DebuggerStepThrough] get; }
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Min;
    public override string ToString()
        => FormattingAspect.IfExpression_ToString(this);

    public IfExpressionSyntax(TextSpan span, IExpressionSyntax condition, IBlockOrResultSyntax thenBlock, IElseClauseSyntax? elseClause)
    {
        Span = span;
        Condition = condition;
        ThenBlock = thenBlock;
        ElseClause = elseClause;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class LoopExpressionSyntax : ILoopExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IBlockExpressionSyntax Block { [DebuggerStepThrough] get; }
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Primary;
    public override string ToString()
        => FormattingAspect.LoopExpression_ToString(this);

    public LoopExpressionSyntax(TextSpan span, IBlockExpressionSyntax block)
    {
        Span = span;
        Block = block;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class WhileExpressionSyntax : IWhileExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax Condition { [DebuggerStepThrough] get; }
    public IBlockExpressionSyntax Block { [DebuggerStepThrough] get; }
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Min;
    public override string ToString()
        => FormattingAspect.WhileExpression_ToString(this);

    public WhileExpressionSyntax(TextSpan span, IExpressionSyntax condition, IBlockExpressionSyntax block)
    {
        Span = span;
        Condition = condition;
        Block = block;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class ForeachExpressionSyntax : IForeachExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public IdentifierName VariableName { [DebuggerStepThrough] get; }
    public IExpressionSyntax InExpression { [DebuggerStepThrough] get; }
    public ITypeSyntax? Type { [DebuggerStepThrough] get; }
    public IBlockExpressionSyntax Block { [DebuggerStepThrough] get; }
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Min;
    public override string ToString()
        => FormattingAspect.ForeachExpression_ToString(this);

    public ForeachExpressionSyntax(TextSpan span, bool isMutableBinding, TextSpan nameSpan, IdentifierName variableName, IExpressionSyntax inExpression, ITypeSyntax? type, IBlockExpressionSyntax block)
    {
        Span = span;
        IsMutableBinding = isMutableBinding;
        NameSpan = nameSpan;
        VariableName = variableName;
        InExpression = inExpression;
        Type = type;
        Block = block;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class BreakExpressionSyntax : IBreakExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax? Value { [DebuggerStepThrough] get; }
    public OperatorPrecedence ExpressionPrecedence
        => Value is not null ? OperatorPrecedence.Min : OperatorPrecedence.Primary;
    public override string ToString()
        => FormattingAspect.BreakExpression_ToString(this);

    public BreakExpressionSyntax(TextSpan span, IExpressionSyntax? value)
    {
        Span = span;
        Value = value;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class NextExpressionSyntax : INextExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Primary;
    public override string ToString()
        => FormattingAspect.NextExpression_ToString(this);

    public NextExpressionSyntax(TextSpan span)
    {
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class ReturnExpressionSyntax : IReturnExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax? Value { [DebuggerStepThrough] get; }
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Min;
    public override string ToString()
        => FormattingAspect.ReturnExpression_ToString(this);

    public ReturnExpressionSyntax(TextSpan span, IExpressionSyntax? value)
    {
        Span = span;
        Value = value;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class InvocationExpressionSyntax : IInvocationExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax Expression { [DebuggerStepThrough] get; }
    public IFixedList<IExpressionSyntax> Arguments { [DebuggerStepThrough] get; }
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Primary;
    public override string ToString()
        => FormattingAspect.InvocationExpression_ToString(this);

    public InvocationExpressionSyntax(TextSpan span, IExpressionSyntax expression, IFixedList<IExpressionSyntax> arguments)
    {
        Span = span;
        Expression = expression;
        Arguments = arguments;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class IdentifierNameExpressionSyntax : IIdentifierNameExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.IdentifierNameExpression_ToString(this);
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Primary;

    public IdentifierNameExpressionSyntax(TextSpan span, IdentifierName name)
    {
        Span = span;
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class SpecialTypeNameExpressionSyntax : ISpecialTypeNameExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public SpecialTypeName Name { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.SpecialTypeNameExpression_ToString(this);
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Primary;

    public SpecialTypeNameExpressionSyntax(TextSpan span, SpecialTypeName name)
    {
        Span = span;
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class GenericNameExpressionSyntax : IGenericNameExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public GenericName Name { [DebuggerStepThrough] get; }
    public IFixedList<ITypeSyntax> TypeArguments { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.GenericNameExpression_ToString(this);
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Primary;

    public GenericNameExpressionSyntax(TextSpan span, GenericName name, IFixedList<ITypeSyntax> typeArguments)
    {
        Span = span;
        Name = name;
        TypeArguments = typeArguments;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class SelfExpressionSyntax : ISelfExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public bool IsImplicit { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.SelfExpression_ToString(this);
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Primary;

    public SelfExpressionSyntax(TextSpan span, bool isImplicit)
    {
        Span = span;
        IsImplicit = isImplicit;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class MemberAccessExpressionSyntax : IMemberAccessExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax Context { [DebuggerStepThrough] get; }
    public StandardName MemberName { [DebuggerStepThrough] get; }
    public IFixedList<ITypeSyntax> TypeArguments { [DebuggerStepThrough] get; }
    public TextSpan MemberNameSpan { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.MemberAccessExpression_ToString(this);
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Primary;

    public MemberAccessExpressionSyntax(TextSpan span, IExpressionSyntax context, StandardName memberName, IFixedList<ITypeSyntax> typeArguments, TextSpan memberNameSpan)
    {
        Span = span;
        Context = context;
        MemberName = memberName;
        TypeArguments = typeArguments;
        MemberNameSpan = memberNameSpan;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class MissingNameSyntax : IMissingNameSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.MissingName_ToString(this);
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Primary;

    public MissingNameSyntax(TextSpan span)
    {
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class MoveExpressionSyntax : IMoveExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public ISimpleNameSyntax Referent { [DebuggerStepThrough] get; }
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Min;
    public override string ToString()
        => FormattingAspect.MoveExpression_ToString(this);

    public MoveExpressionSyntax(TextSpan span, ISimpleNameSyntax referent)
    {
        Span = span;
        Referent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class FreezeExpressionSyntax : IFreezeExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public ISimpleNameSyntax Referent { [DebuggerStepThrough] get; }
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Min;
    public override string ToString()
        => FormattingAspect.FreezeExpression_ToString(this);

    public FreezeExpressionSyntax(TextSpan span, ISimpleNameSyntax referent)
    {
        Span = span;
        Referent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class AsyncBlockExpressionSyntax : IAsyncBlockExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IBlockExpressionSyntax Block { [DebuggerStepThrough] get; }
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Primary;
    public override string ToString()
        => FormattingAspect.AsyncBlockExpression_ToString(this);

    public AsyncBlockExpressionSyntax(TextSpan span, IBlockExpressionSyntax block)
    {
        Span = span;
        Block = block;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class AsyncStartExpressionSyntax : IAsyncStartExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public bool Scheduled { [DebuggerStepThrough] get; }
    public IExpressionSyntax Expression { [DebuggerStepThrough] get; }
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Min;
    public override string ToString()
        => FormattingAspect.AsyncStartExpression_ToString(this);

    public AsyncStartExpressionSyntax(TextSpan span, bool scheduled, IExpressionSyntax expression)
    {
        Span = span;
        Scheduled = scheduled;
        Expression = expression;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file sealed class AwaitExpressionSyntax : IAwaitExpressionSyntax
{
    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax Expression { [DebuggerStepThrough] get; }
    public OperatorPrecedence ExpressionPrecedence
        => OperatorPrecedence.Unary;
    public override string ToString()
        => FormattingAspect.AwaitExpression_ToString(this);

    public AwaitExpressionSyntax(TextSpan span, IExpressionSyntax expression)
    {
        Span = span;
        Expression = expression;
    }
}

