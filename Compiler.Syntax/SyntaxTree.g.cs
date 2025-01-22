using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Core.Code;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Core.Operators;
using Azoth.Tools.Bootstrap.Compiler.Core.Types;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Tokens;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

#nullable enable
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
    string ToString();
}

[Closed(
    typeof(ICompilationUnitSyntax),
    typeof(IImportDirectiveSyntax),
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
    IFixedList<IImportDirectiveSyntax> ImportDirectives { get; }
    IFixedList<INamespaceBlockMemberDefinitionSyntax> Definitions { get; }

    public static ICompilationUnitSyntax Create(
        TextSpan span,
        CodeFile file,
        NamespaceName implicitNamespaceName,
        DiagnosticCollection diagnostics,
        IEnumerable<IImportDirectiveSyntax> importDirectives,
        IEnumerable<INamespaceBlockMemberDefinitionSyntax> definitions)
        => new CompilationUnitSyntax(span, file, implicitNamespaceName, diagnostics, importDirectives, definitions);
}

// [Closed(typeof(ImportDirectiveSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IImportDirectiveSyntax : ICodeSyntax
{
    NamespaceName Name { get; }

    public static IImportDirectiveSyntax Create(
        TextSpan span,
        NamespaceName name)
        => new ImportDirectiveSyntax(span, name);
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

    public static IPackageSyntax Create(
        IdentifierName name,
        IEnumerable<ICompilationUnitSyntax> compilationUnits,
        IEnumerable<ICompilationUnitSyntax> testingCompilationUnits,
        IEnumerable<IPackageReferenceSyntax> references)
        => new PackageSyntax(name, compilationUnits, testingCompilationUnits, references);
}

// [Closed(typeof(PackageReferenceSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IPackageReferenceSyntax : ISyntax
{
    IdentifierName AliasOrName { get; }
    IPackageSymbols Package { get; }
    bool IsTrusted { get; }

    public static IPackageReferenceSyntax Create(
        IdentifierName aliasOrName,
        IPackageSymbols package,
        bool isTrusted)
        => new PackageReferenceSyntax(aliasOrName, package, isTrusted);
}

[Closed(
    typeof(IEntityDefinitionSyntax),
    typeof(INamespaceBlockMemberDefinitionSyntax))]
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
    typeof(IFunctionDefinitionSyntax),
    typeof(IMethodDefinitionSyntax),
    typeof(IConstructorDefinitionSyntax),
    typeof(IInitializerDefinitionSyntax),
    typeof(IAssociatedFunctionDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInvocableDefinitionSyntax : IEntityDefinitionSyntax
{
    IFixedList<IConstructorOrInitializerParameterSyntax> Parameters { get; }
    IBodySyntax? Body { get; }
}

// [Closed(typeof(NamespaceBlockDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamespaceBlockDefinitionSyntax : INamespaceBlockMemberDefinitionSyntax
{
    bool IsGlobalQualified { get; }
    NamespaceName DeclaredNames { get; }
    IFixedList<IImportDirectiveSyntax> ImportDirectives { get; }
    IFixedList<INamespaceBlockMemberDefinitionSyntax> Definitions { get; }

    public static INamespaceBlockDefinitionSyntax Create(
        TextSpan span,
        CodeFile file,
        TypeName? name,
        TextSpan nameSpan,
        bool isGlobalQualified,
        NamespaceName declaredNames,
        IEnumerable<IImportDirectiveSyntax> importDirectives,
        IEnumerable<INamespaceBlockMemberDefinitionSyntax> definitions)
        => new NamespaceBlockDefinitionSyntax(span, file, name, nameSpan, isGlobalQualified, declaredNames, importDirectives, definitions);
}

[Closed(
    typeof(INamespaceBlockDefinitionSyntax),
    typeof(IFunctionDefinitionSyntax),
    typeof(ITypeDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INamespaceBlockMemberDefinitionSyntax : IDefinitionSyntax
{
}

// [Closed(typeof(FunctionDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionDefinitionSyntax : IInvocableDefinitionSyntax, INamespaceBlockMemberDefinitionSyntax
{
    IFixedList<IAttributeSyntax> Attributes { get; }
    new IdentifierName Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    new IFixedList<INamedParameterSyntax> Parameters { get; }
    IFixedList<IConstructorOrInitializerParameterSyntax> IInvocableDefinitionSyntax.Parameters => Parameters;
    IReturnSyntax? Return { get; }
    new IBodySyntax Body { get; }
    IBodySyntax? IInvocableDefinitionSyntax.Body => Body;

    public static IFunctionDefinitionSyntax Create(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        IEnumerable<IAttributeSyntax> attributes,
        IdentifierName name,
        IEnumerable<INamedParameterSyntax> parameters,
        IReturnSyntax? @return,
        IBodySyntax body)
        => new FunctionDefinitionSyntax(span, file, nameSpan, accessModifier, attributes, name, parameters, @return, body);
}

[Closed(
    typeof(IClassDefinitionSyntax),
    typeof(IStructDefinitionSyntax),
    typeof(ITraitDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITypeDefinitionSyntax : INamespaceBlockMemberDefinitionSyntax, IClassMemberDefinitionSyntax, ITraitMemberDefinitionSyntax, IStructMemberDefinitionSyntax
{
    IConstKeywordToken? ConstModifier { get; }
    IMoveKeywordToken? MoveModifier { get; }
    new OrdinaryName Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    IFixedList<IGenericParameterSyntax> GenericParameters { get; }
    IFixedList<IOrdinaryTypeNameSyntax> SupertypeNames { get; }
    IFixedList<ITypeMemberDefinitionSyntax> Members { get; }
}

// [Closed(typeof(ClassDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IClassDefinitionSyntax : ITypeDefinitionSyntax
{
    IAbstractKeywordToken? AbstractModifier { get; }
    IOrdinaryTypeNameSyntax? BaseTypeName { get; }
    new IFixedList<IClassMemberDefinitionSyntax> Members { get; }
    IFixedList<ITypeMemberDefinitionSyntax> ITypeDefinitionSyntax.Members => Members;

    public static IClassDefinitionSyntax Create(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        IConstKeywordToken? constModifier,
        IMoveKeywordToken? moveModifier,
        OrdinaryName name,
        IAbstractKeywordToken? abstractModifier,
        IEnumerable<IGenericParameterSyntax> genericParameters,
        IOrdinaryTypeNameSyntax? baseTypeName,
        IEnumerable<IOrdinaryTypeNameSyntax> supertypeNames,
        IEnumerable<IClassMemberDefinitionSyntax> members)
        => new ClassDefinitionSyntax(span, file, nameSpan, accessModifier, constModifier, moveModifier, name, abstractModifier, genericParameters, baseTypeName, supertypeNames, members);
}

// [Closed(typeof(StructDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStructDefinitionSyntax : ITypeDefinitionSyntax
{
    new IFixedList<IStructMemberDefinitionSyntax> Members { get; }
    IFixedList<ITypeMemberDefinitionSyntax> ITypeDefinitionSyntax.Members => Members;

    public static IStructDefinitionSyntax Create(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        IConstKeywordToken? constModifier,
        IMoveKeywordToken? moveModifier,
        OrdinaryName name,
        IEnumerable<IGenericParameterSyntax> genericParameters,
        IEnumerable<IOrdinaryTypeNameSyntax> supertypeNames,
        IEnumerable<IStructMemberDefinitionSyntax> members)
        => new StructDefinitionSyntax(span, file, nameSpan, accessModifier, constModifier, moveModifier, name, genericParameters, supertypeNames, members);
}

// [Closed(typeof(TraitDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITraitDefinitionSyntax : ITypeDefinitionSyntax
{
    new IFixedList<ITraitMemberDefinitionSyntax> Members { get; }
    IFixedList<ITypeMemberDefinitionSyntax> ITypeDefinitionSyntax.Members => Members;

    public static ITraitDefinitionSyntax Create(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        IConstKeywordToken? constModifier,
        IMoveKeywordToken? moveModifier,
        OrdinaryName name,
        IEnumerable<IGenericParameterSyntax> genericParameters,
        IEnumerable<IOrdinaryTypeNameSyntax> supertypeNames,
        IEnumerable<ITraitMemberDefinitionSyntax> members)
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

    public static IGenericParameterSyntax Create(
        TextSpan span,
        ICapabilityConstraintSyntax constraint,
        IdentifierName name,
        TypeParameterIndependence independence,
        TypeParameterVariance variance)
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
    typeof(IInitializerDefinitionSyntax),
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
    typeof(IMethodDefinitionSyntax),
    typeof(IInitializerDefinitionSyntax),
    typeof(IFieldDefinitionSyntax),
    typeof(IAssociatedFunctionDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IStructMemberDefinitionSyntax : ITypeMemberDefinitionSyntax
{
}

[Closed(
    typeof(IAbstractMethodDefinitionSyntax),
    typeof(IOrdinaryMethodDefinitionSyntax),
    typeof(IGetterMethodDefinitionSyntax),
    typeof(ISetterMethodDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMethodDefinitionSyntax : IClassMemberDefinitionSyntax, ITraitMemberDefinitionSyntax, IStructMemberDefinitionSyntax, IInvocableDefinitionSyntax
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
    new IBodySyntax? Body
        => null;
    IBodySyntax? IInvocableDefinitionSyntax.Body => Body;

    public static IAbstractMethodDefinitionSyntax Create(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        IdentifierName name,
        IMethodSelfParameterSyntax selfParameter,
        IEnumerable<INamedParameterSyntax> parameters,
        IReturnSyntax? @return)
        => new AbstractMethodDefinitionSyntax(span, file, nameSpan, accessModifier, name, selfParameter, parameters, @return);
}

// [Closed(typeof(OrdinaryMethodDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IOrdinaryMethodDefinitionSyntax : IMethodDefinitionSyntax
{
    new IBodySyntax Body { get; }
    IBodySyntax? IInvocableDefinitionSyntax.Body => Body;

    public static IOrdinaryMethodDefinitionSyntax Create(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        IdentifierName name,
        IMethodSelfParameterSyntax selfParameter,
        IEnumerable<INamedParameterSyntax> parameters,
        IReturnSyntax? @return,
        IBodySyntax body)
        => new OrdinaryMethodDefinitionSyntax(span, file, nameSpan, accessModifier, name, selfParameter, parameters, @return, body);
}

// [Closed(typeof(GetterMethodDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGetterMethodDefinitionSyntax : IMethodDefinitionSyntax
{
    new IReturnSyntax Return { get; }
    IReturnSyntax? IMethodDefinitionSyntax.Return => Return;
    new IFixedList<INamedParameterSyntax> Parameters
        => [];
    IFixedList<INamedParameterSyntax> IMethodDefinitionSyntax.Parameters => Parameters;
    IFixedList<IConstructorOrInitializerParameterSyntax> IInvocableDefinitionSyntax.Parameters => Parameters;

    public static IGetterMethodDefinitionSyntax Create(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        IdentifierName name,
        IMethodSelfParameterSyntax selfParameter,
        IReturnSyntax @return,
        IBodySyntax? body)
        => new GetterMethodDefinitionSyntax(span, file, nameSpan, accessModifier, name, selfParameter, @return, body);
}

// [Closed(typeof(SetterMethodDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISetterMethodDefinitionSyntax : IMethodDefinitionSyntax
{
    new IBodySyntax Body { get; }
    IBodySyntax? IInvocableDefinitionSyntax.Body => Body;
    new IReturnSyntax? Return
        => null;
    IReturnSyntax? IMethodDefinitionSyntax.Return => Return;

    public static ISetterMethodDefinitionSyntax Create(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        IdentifierName name,
        IMethodSelfParameterSyntax selfParameter,
        IEnumerable<INamedParameterSyntax> parameters,
        IBodySyntax body)
        => new SetterMethodDefinitionSyntax(span, file, nameSpan, accessModifier, name, selfParameter, parameters, body);
}

// [Closed(typeof(ConstructorDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IConstructorDefinitionSyntax : IInvocableDefinitionSyntax, IClassMemberDefinitionSyntax
{
    new IdentifierName? Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    IConstructorSelfParameterSyntax SelfParameter { get; }
    new IBlockBodySyntax Body { get; }
    IBodySyntax? IInvocableDefinitionSyntax.Body => Body;

    public static IConstructorDefinitionSyntax Create(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        IdentifierName? name,
        IConstructorSelfParameterSyntax selfParameter,
        IEnumerable<IConstructorOrInitializerParameterSyntax> parameters,
        IBlockBodySyntax body)
        => new ConstructorDefinitionSyntax(span, file, nameSpan, accessModifier, name, selfParameter, parameters, body);
}

// [Closed(typeof(InitializerDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInitializerDefinitionSyntax : IInvocableDefinitionSyntax, IStructMemberDefinitionSyntax, IClassMemberDefinitionSyntax
{
    new IdentifierName? Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    IInitializerSelfParameterSyntax SelfParameter { get; }
    new IBlockBodySyntax Body { get; }
    IBodySyntax? IInvocableDefinitionSyntax.Body => Body;

    public static IInitializerDefinitionSyntax Create(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        IdentifierName? name,
        IInitializerSelfParameterSyntax selfParameter,
        IEnumerable<IConstructorOrInitializerParameterSyntax> parameters,
        IBlockBodySyntax body)
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

    public static IFieldDefinitionSyntax Create(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        bool isMutableBinding,
        IdentifierName name,
        ITypeSyntax type,
        IExpressionSyntax? initializer)
        => new FieldDefinitionSyntax(span, file, nameSpan, accessModifier, isMutableBinding, name, type, initializer);
}

// [Closed(typeof(AssociatedFunctionDefinitionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAssociatedFunctionDefinitionSyntax : IClassMemberDefinitionSyntax, ITraitMemberDefinitionSyntax, IStructMemberDefinitionSyntax, IInvocableDefinitionSyntax
{
    new IdentifierName Name { get; }
    TypeName? IDefinitionSyntax.Name => Name;
    new IFixedList<INamedParameterSyntax> Parameters { get; }
    IFixedList<IConstructorOrInitializerParameterSyntax> IInvocableDefinitionSyntax.Parameters => Parameters;
    IReturnSyntax? Return { get; }
    new IBodySyntax Body { get; }
    IBodySyntax? IInvocableDefinitionSyntax.Body => Body;

    public static IAssociatedFunctionDefinitionSyntax Create(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        IdentifierName name,
        IEnumerable<INamedParameterSyntax> parameters,
        IReturnSyntax? @return,
        IBodySyntax body)
        => new AssociatedFunctionDefinitionSyntax(span, file, nameSpan, accessModifier, name, parameters, @return, body);
}

// [Closed(typeof(AttributeSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAttributeSyntax : ICodeSyntax
{
    IOrdinaryTypeNameSyntax TypeName { get; }

    public static IAttributeSyntax Create(
        TextSpan span,
        IOrdinaryTypeNameSyntax typeName)
        => new AttributeSyntax(span, typeName);
}

[Closed(
    typeof(ICapabilitySetSyntax),
    typeof(ICapabilitySyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICapabilityConstraintSyntax : ICodeSyntax
{
}

// [Closed(typeof(CapabilitySetSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICapabilitySetSyntax : ICapabilityConstraintSyntax
{
    CapabilitySet CapabilitySet { get; }

    public static ICapabilitySetSyntax Create(
        TextSpan span,
        CapabilitySet capabilitySet)
        => new CapabilitySetSyntax(span, capabilitySet);
}

// [Closed(typeof(CapabilitySyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICapabilitySyntax : ICapabilityConstraintSyntax
{
    IFixedList<ICapabilityToken> Tokens { get; }
    DeclaredCapability Capability { get; }

    public static ICapabilitySyntax Create(
        TextSpan span,
        IEnumerable<ICapabilityToken> tokens,
        DeclaredCapability capability)
        => new CapabilitySyntax(span, tokens, capability);
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

    public static INamedParameterSyntax Create(
        TextSpan span,
        TextSpan nameSpan,
        bool isMutableBinding,
        bool isLentBinding,
        IdentifierName name,
        ITypeSyntax type,
        IExpressionSyntax? defaultValue)
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
    ICapabilityConstraintSyntax Constraint { get; }
    new IdentifierName? Name
        => null;
    IdentifierName? IParameterSyntax.Name => Name;
}

// [Closed(typeof(ConstructorSelfParameterSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IConstructorSelfParameterSyntax : ISelfParameterSyntax
{
    new ICapabilitySyntax Constraint { get; }
    ICapabilityConstraintSyntax ISelfParameterSyntax.Constraint => Constraint;

    public static IConstructorSelfParameterSyntax Create(
        TextSpan span,
        bool isLentBinding,
        ICapabilitySyntax constraint)
        => new ConstructorSelfParameterSyntax(span, isLentBinding, constraint);
}

// [Closed(typeof(InitializerSelfParameterSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInitializerSelfParameterSyntax : ISelfParameterSyntax
{
    new ICapabilitySyntax Constraint { get; }
    ICapabilityConstraintSyntax ISelfParameterSyntax.Constraint => Constraint;

    public static IInitializerSelfParameterSyntax Create(
        TextSpan span,
        bool isLentBinding,
        ICapabilitySyntax constraint)
        => new InitializerSelfParameterSyntax(span, isLentBinding, constraint);
}

// [Closed(typeof(MethodSelfParameterSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMethodSelfParameterSyntax : ISelfParameterSyntax
{

    public static IMethodSelfParameterSyntax Create(
        TextSpan span,
        bool isLentBinding,
        ICapabilityConstraintSyntax constraint)
        => new MethodSelfParameterSyntax(span, isLentBinding, constraint);
}

// [Closed(typeof(FieldParameterSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFieldParameterSyntax : IConstructorOrInitializerParameterSyntax
{
    new IdentifierName Name { get; }
    IdentifierName? IParameterSyntax.Name => Name;
    IExpressionSyntax? DefaultValue { get; }

    public static IFieldParameterSyntax Create(
        TextSpan span,
        IdentifierName name,
        IExpressionSyntax? defaultValue)
        => new FieldParameterSyntax(span, name, defaultValue);
}

// [Closed(typeof(ReturnSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IReturnSyntax : ICodeSyntax
{
    ITypeSyntax Type { get; }

    public static IReturnSyntax Create(
        TextSpan span,
        ITypeSyntax type)
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

    public static IBlockBodySyntax Create(
        TextSpan span,
        IEnumerable<IBodyStatementSyntax> statements)
        => new BlockBodySyntax(span, statements);
}

// [Closed(typeof(ExpressionBodySyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IExpressionBodySyntax : IBodySyntax
{
    IResultStatementSyntax ResultStatement { get; }
    new IFixedList<IStatementSyntax> Statements { get; }
    IFixedList<IStatementSyntax> IBodyOrBlockSyntax.Statements => Statements;

    public static IExpressionBodySyntax Create(
        TextSpan span,
        IResultStatementSyntax resultStatement)
        => new ExpressionBodySyntax(span, resultStatement);
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
    typeof(IBuiltInTypeNameSyntax),
    typeof(IOrdinaryTypeNameSyntax),
    typeof(IQualifiedTypeNameSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ITypeNameSyntax : ITypeSyntax
{
    TypeName Name { get; }
}

// [Closed(typeof(BuiltInTypeNameSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBuiltInTypeNameSyntax : ITypeNameSyntax
{
    new BuiltInTypeName Name { get; }
    TypeName ITypeNameSyntax.Name => Name;

    public static IBuiltInTypeNameSyntax Create(
        TextSpan span,
        BuiltInTypeName name)
        => new BuiltInTypeNameSyntax(span, name);
}

[Closed(
    typeof(IIdentifierTypeNameSyntax),
    typeof(IGenericTypeNameSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IOrdinaryTypeNameSyntax : ITypeNameSyntax
{
    new OrdinaryName Name { get; }
    TypeName ITypeNameSyntax.Name => Name;
}

// [Closed(typeof(IdentifierTypeNameSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IIdentifierTypeNameSyntax : IOrdinaryTypeNameSyntax
{
    new IdentifierName Name { get; }
    OrdinaryName IOrdinaryTypeNameSyntax.Name => Name;
    TypeName ITypeNameSyntax.Name => Name;

    public static IIdentifierTypeNameSyntax Create(
        TextSpan span,
        IdentifierName name)
        => new IdentifierTypeNameSyntax(span, name);
}

// [Closed(typeof(GenericTypeNameSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGenericTypeNameSyntax : IOrdinaryTypeNameSyntax
{
    new GenericName Name { get; }
    OrdinaryName IOrdinaryTypeNameSyntax.Name => Name;
    TypeName ITypeNameSyntax.Name => Name;
    IFixedList<ITypeSyntax> GenericArguments { get; }

    public static IGenericTypeNameSyntax Create(
        TextSpan span,
        GenericName name,
        IEnumerable<ITypeSyntax> genericArguments)
        => new GenericTypeNameSyntax(span, name, genericArguments);
}

// [Closed(typeof(QualifiedTypeNameSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IQualifiedTypeNameSyntax : ITypeNameSyntax
{
    ITypeNameSyntax Context { get; }
    IOrdinaryTypeNameSyntax QualifiedName { get; }

    public static IQualifiedTypeNameSyntax Create(
        TextSpan span,
        TypeName name,
        ITypeNameSyntax context,
        IOrdinaryTypeNameSyntax qualifiedName)
        => new QualifiedTypeNameSyntax(span, name, context, qualifiedName);
}

// [Closed(typeof(OptionalTypeSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IOptionalTypeSyntax : ITypeSyntax
{
    ITypeSyntax Referent { get; }

    public static IOptionalTypeSyntax Create(
        TextSpan span,
        ITypeSyntax referent)
        => new OptionalTypeSyntax(span, referent);
}

// [Closed(typeof(CapabilityTypeSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ICapabilityTypeSyntax : ITypeSyntax
{
    ICapabilitySyntax Capability { get; }
    ITypeSyntax Referent { get; }

    public static ICapabilityTypeSyntax Create(
        TextSpan span,
        ICapabilitySyntax capability,
        ITypeSyntax referent)
        => new CapabilityTypeSyntax(span, capability, referent);
}

// [Closed(typeof(FunctionTypeSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFunctionTypeSyntax : ITypeSyntax
{
    IFixedList<IParameterTypeSyntax> Parameters { get; }
    ITypeSyntax Return { get; }

    public static IFunctionTypeSyntax Create(
        TextSpan span,
        IEnumerable<IParameterTypeSyntax> parameters,
        ITypeSyntax @return)
        => new FunctionTypeSyntax(span, parameters, @return);
}

// [Closed(typeof(ParameterTypeSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IParameterTypeSyntax : ICodeSyntax
{
    bool IsLent { get; }
    ITypeSyntax Referent { get; }

    public static IParameterTypeSyntax Create(
        TextSpan span,
        bool isLent,
        ITypeSyntax referent)
        => new ParameterTypeSyntax(span, isLent, referent);
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

    public static ICapabilityViewpointTypeSyntax Create(
        TextSpan span,
        ICapabilitySyntax capability,
        ITypeSyntax referent)
        => new CapabilityViewpointTypeSyntax(span, capability, referent);
}

// [Closed(typeof(SelfViewpointTypeSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISelfViewpointTypeSyntax : IViewpointTypeSyntax
{

    public static ISelfViewpointTypeSyntax Create(
        TextSpan span,
        ITypeSyntax referent)
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

    public static IResultStatementSyntax Create(
        TextSpan span,
        IExpressionSyntax expression)
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

    public static IVariableDeclarationStatementSyntax Create(
        TextSpan span,
        bool isMutableBinding,
        TextSpan nameSpan,
        IdentifierName name,
        ICapabilitySyntax? capability,
        ITypeSyntax? type,
        IExpressionSyntax? initializer)
        => new VariableDeclarationStatementSyntax(span, isMutableBinding, nameSpan, name, capability, type, initializer);
}

// [Closed(typeof(ExpressionStatementSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IExpressionStatementSyntax : IBodyStatementSyntax
{
    IExpressionSyntax Expression { get; }

    public static IExpressionStatementSyntax Create(
        TextSpan span,
        IExpressionSyntax expression)
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

    public static IBindingContextPatternSyntax Create(
        TextSpan span,
        bool isMutableBinding,
        IPatternSyntax pattern,
        ITypeSyntax? type)
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
    new TextSpan Span
        => NameSpan;
    TextSpan ICodeSyntax.Span => Span;

    public static IBindingPatternSyntax Create(
        bool isMutableBinding,
        TextSpan nameSpan,
        IdentifierName name)
        => new BindingPatternSyntax(isMutableBinding, nameSpan, name);
}

// [Closed(typeof(OptionalPatternSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IOptionalPatternSyntax : IOptionalOrBindingPatternSyntax
{
    IOptionalOrBindingPatternSyntax Pattern { get; }

    public static IOptionalPatternSyntax Create(
        TextSpan span,
        IOptionalOrBindingPatternSyntax pattern)
        => new OptionalPatternSyntax(span, pattern);
}

[Closed(
    typeof(IBlockExpressionSyntax),
    typeof(INewObjectExpressionSyntax),
    typeof(IUnsafeExpressionSyntax),
    typeof(ILiteralExpressionSyntax),
    typeof(IAssignmentExpressionSyntax),
    typeof(IBinaryOperatorExpressionSyntax),
    typeof(IUnaryOperatorExpressionSyntax),
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

// [Closed(typeof(BlockExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBlockExpressionSyntax : IExpressionSyntax, IBlockOrResultSyntax, IBodyOrBlockSyntax
{
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Primary;

    public static IBlockExpressionSyntax Create(
        TextSpan span,
        IEnumerable<IStatementSyntax> statements)
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

    public static INewObjectExpressionSyntax Create(
        TextSpan span,
        ITypeNameSyntax type,
        IdentifierName? constructorName,
        TextSpan? constructorNameSpan,
        IEnumerable<IExpressionSyntax> arguments)
        => new NewObjectExpressionSyntax(span, type, constructorName, constructorNameSpan, arguments);
}

// [Closed(typeof(UnsafeExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IUnsafeExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Expression { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Primary;

    public static IUnsafeExpressionSyntax Create(
        TextSpan span,
        IExpressionSyntax expression)
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

    public static IBoolLiteralExpressionSyntax Create(
        TextSpan span,
        bool value)
        => new BoolLiteralExpressionSyntax(span, value);
}

// [Closed(typeof(IntegerLiteralExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IIntegerLiteralExpressionSyntax : ILiteralExpressionSyntax
{
    BigInteger Value { get; }

    public static IIntegerLiteralExpressionSyntax Create(
        TextSpan span,
        BigInteger value)
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

    public static IStringLiteralExpressionSyntax Create(
        TextSpan span,
        string value)
        => new StringLiteralExpressionSyntax(span, value);
}

// [Closed(typeof(AssignmentExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAssignmentExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax LeftOperand { get; }
    AssignmentOperator Operator { get; }
    IExpressionSyntax RightOperand { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Assignment;

    public static IAssignmentExpressionSyntax Create(
        TextSpan span,
        IExpressionSyntax leftOperand,
        AssignmentOperator @operator,
        IExpressionSyntax rightOperand)
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

    public static IBinaryOperatorExpressionSyntax Create(
        TextSpan span,
        IExpressionSyntax leftOperand,
        BinaryOperator @operator,
        IExpressionSyntax rightOperand)
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

    public static IUnaryOperatorExpressionSyntax Create(
        TextSpan span,
        UnaryOperatorFixity fixity,
        UnaryOperator @operator,
        IExpressionSyntax operand)
        => new UnaryOperatorExpressionSyntax(span, fixity, @operator, operand);
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

    public static IConversionExpressionSyntax Create(
        TextSpan span,
        IExpressionSyntax referent,
        ConversionOperator @operator,
        ITypeSyntax convertToType)
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

    public static IPatternMatchExpressionSyntax Create(
        TextSpan span,
        IExpressionSyntax referent,
        IPatternSyntax pattern)
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

    public static IIfExpressionSyntax Create(
        TextSpan span,
        IExpressionSyntax condition,
        IBlockOrResultSyntax thenBlock,
        IElseClauseSyntax? elseClause)
        => new IfExpressionSyntax(span, condition, thenBlock, elseClause);
}

// [Closed(typeof(LoopExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ILoopExpressionSyntax : IExpressionSyntax
{
    IBlockExpressionSyntax Block { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Primary;

    public static ILoopExpressionSyntax Create(
        TextSpan span,
        IBlockExpressionSyntax block)
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

    public static IWhileExpressionSyntax Create(
        TextSpan span,
        IExpressionSyntax condition,
        IBlockExpressionSyntax block)
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

    public static IForeachExpressionSyntax Create(
        TextSpan span,
        bool isMutableBinding,
        TextSpan nameSpan,
        IdentifierName variableName,
        IExpressionSyntax inExpression,
        ITypeSyntax? type,
        IBlockExpressionSyntax block)
        => new ForeachExpressionSyntax(span, isMutableBinding, nameSpan, variableName, inExpression, type, block);
}

// [Closed(typeof(BreakExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBreakExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax? Value { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => Value is not null ? OperatorPrecedence.Min : OperatorPrecedence.Primary;

    public static IBreakExpressionSyntax Create(
        TextSpan span,
        IExpressionSyntax? value)
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

    public static IReturnExpressionSyntax Create(
        TextSpan span,
        IExpressionSyntax? value)
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

    public static IInvocationExpressionSyntax Create(
        TextSpan span,
        IExpressionSyntax expression,
        IEnumerable<IExpressionSyntax> arguments)
        => new InvocationExpressionSyntax(span, expression, arguments);
}

[Closed(
    typeof(IBuiltInTypeNameExpressionSyntax),
    typeof(IOrdinaryNameExpressionSyntax),
    typeof(IInstanceExpressionSyntax),
    typeof(IMemberAccessExpressionSyntax),
    typeof(IMissingNameSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface INameExpressionSyntax : IExpressionSyntax
{
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Primary;
}

// [Closed(typeof(BuiltInTypeNameExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IBuiltInTypeNameExpressionSyntax : INameExpressionSyntax
{
    BuiltInTypeName Name { get; }

    public static IBuiltInTypeNameExpressionSyntax Create(
        TextSpan span,
        BuiltInTypeName name)
        => new BuiltInTypeNameExpressionSyntax(span, name);
}

[Closed(
    typeof(IIdentifierNameExpressionSyntax),
    typeof(IGenericNameExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IOrdinaryNameExpressionSyntax : INameExpressionSyntax
{
    OrdinaryName Name { get; }
    IFixedList<ITypeSyntax> GenericArguments { get; }
}

// [Closed(typeof(IdentifierNameExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IIdentifierNameExpressionSyntax : IOrdinaryNameExpressionSyntax
{
    new IdentifierName Name { get; }
    OrdinaryName IOrdinaryNameExpressionSyntax.Name => Name;
    new IFixedList<ITypeSyntax> GenericArguments
        => [];
    IFixedList<ITypeSyntax> IOrdinaryNameExpressionSyntax.GenericArguments => GenericArguments;

    public static IIdentifierNameExpressionSyntax Create(
        TextSpan span,
        IdentifierName name)
        => new IdentifierNameExpressionSyntax(span, name);
}

// [Closed(typeof(GenericNameExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IGenericNameExpressionSyntax : IOrdinaryNameExpressionSyntax
{
    new GenericName Name { get; }
    OrdinaryName IOrdinaryNameExpressionSyntax.Name => Name;

    public static IGenericNameExpressionSyntax Create(
        TextSpan span,
        GenericName name,
        IEnumerable<ITypeSyntax> genericArguments)
        => new GenericNameExpressionSyntax(span, name, genericArguments);
}

[Closed(
    typeof(ISelfExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IInstanceExpressionSyntax : INameExpressionSyntax
{
}

// [Closed(typeof(SelfExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface ISelfExpressionSyntax : IInstanceExpressionSyntax
{
    bool IsImplicit { get; }

    public static ISelfExpressionSyntax Create(
        TextSpan span,
        bool isImplicit)
        => new SelfExpressionSyntax(span, isImplicit);
}

// [Closed(typeof(MemberAccessExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMemberAccessExpressionSyntax : INameExpressionSyntax
{
    IExpressionSyntax Context { get; }
    OrdinaryName MemberName { get; }
    IFixedList<ITypeSyntax> GenericArguments { get; }
    TextSpan MemberNameSpan { get; }

    public static IMemberAccessExpressionSyntax Create(
        TextSpan span,
        IExpressionSyntax context,
        OrdinaryName memberName,
        IEnumerable<ITypeSyntax> genericArguments,
        TextSpan memberNameSpan)
        => new MemberAccessExpressionSyntax(span, context, memberName, genericArguments, memberNameSpan);
}

// [Closed(typeof(MissingNameSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMissingNameSyntax : INameExpressionSyntax
{

    public static IMissingNameSyntax Create(TextSpan span)
        => new MissingNameSyntax(span);
}

// [Closed(typeof(MoveExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IMoveExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Referent { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Min;

    public static IMoveExpressionSyntax Create(
        TextSpan span,
        IExpressionSyntax referent)
        => new MoveExpressionSyntax(span, referent);
}

// [Closed(typeof(FreezeExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IFreezeExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Referent { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Min;

    public static IFreezeExpressionSyntax Create(
        TextSpan span,
        IExpressionSyntax referent)
        => new FreezeExpressionSyntax(span, referent);
}

// [Closed(typeof(AsyncBlockExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAsyncBlockExpressionSyntax : IExpressionSyntax
{
    IBlockExpressionSyntax Block { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Primary;

    public static IAsyncBlockExpressionSyntax Create(
        TextSpan span,
        IBlockExpressionSyntax block)
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

    public static IAsyncStartExpressionSyntax Create(
        TextSpan span,
        bool scheduled,
        IExpressionSyntax expression)
        => new AsyncStartExpressionSyntax(span, scheduled, expression);
}

// [Closed(typeof(AwaitExpressionSyntax))]
[GeneratedCode("AzothCompilerCodeGen", null)]
public partial interface IAwaitExpressionSyntax : IExpressionSyntax
{
    IExpressionSyntax Expression { get; }
    OperatorPrecedence IExpressionSyntax.ExpressionPrecedence
        => OperatorPrecedence.Unary;

    public static IAwaitExpressionSyntax Create(
        TextSpan span,
        IExpressionSyntax expression)
        => new AwaitExpressionSyntax(span, expression);
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class CompilationUnitSyntax : ICompilationUnitSyntax
{
    private ICompilationUnitSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public NamespaceName ImplicitNamespaceName { [DebuggerStepThrough] get; }
    public DiagnosticCollection Diagnostics { [DebuggerStepThrough] get; }
    public IFixedList<IImportDirectiveSyntax> ImportDirectives { [DebuggerStepThrough] get; }
    public IFixedList<INamespaceBlockMemberDefinitionSyntax> Definitions { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.CompilationUnit_ToString(this);

    public CompilationUnitSyntax(
        TextSpan span,
        CodeFile file,
        NamespaceName implicitNamespaceName,
        DiagnosticCollection diagnostics,
        IEnumerable<IImportDirectiveSyntax> importDirectives,
        IEnumerable<INamespaceBlockMemberDefinitionSyntax> definitions)
    {
        Span = span;
        File = file;
        ImplicitNamespaceName = implicitNamespaceName;
        Diagnostics = diagnostics;
        ImportDirectives = importDirectives.ToFixedList();
        Definitions = definitions.ToFixedList();
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ImportDirectiveSyntax : IImportDirectiveSyntax
{
    private IImportDirectiveSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public NamespaceName Name { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.ImportDirective_ToString(this);

    public ImportDirectiveSyntax(
        TextSpan span,
        NamespaceName name)
    {
        Span = span;
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class PackageSyntax : IPackageSyntax
{
    private IPackageSyntax Self { [Inline] get => this; }

    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IFixedSet<ICompilationUnitSyntax> CompilationUnits { [DebuggerStepThrough] get; }
    public IFixedSet<ICompilationUnitSyntax> TestingCompilationUnits { [DebuggerStepThrough] get; }
    public IFixedSet<IPackageReferenceSyntax> References { [DebuggerStepThrough] get; }
    public DiagnosticCollection Diagnostics { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.Package_ToString(this);

    public PackageSyntax(
        IdentifierName name,
        IEnumerable<ICompilationUnitSyntax> compilationUnits,
        IEnumerable<ICompilationUnitSyntax> testingCompilationUnits,
        IEnumerable<IPackageReferenceSyntax> references)
    {
        Name = name;
        CompilationUnits = compilationUnits.ToFixedSet();
        TestingCompilationUnits = testingCompilationUnits.ToFixedSet();
        References = references.ToFixedSet();
        Diagnostics = ComputedAspect.Package_Diagnostics(this);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class PackageReferenceSyntax : IPackageReferenceSyntax
{
    private IPackageReferenceSyntax Self { [Inline] get => this; }

    public IdentifierName AliasOrName { [DebuggerStepThrough] get; }
    public IPackageSymbols Package { [DebuggerStepThrough] get; }
    public bool IsTrusted { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.PackageReference_ToString(this);

    public PackageReferenceSyntax(
        IdentifierName aliasOrName,
        IPackageSymbols package,
        bool isTrusted)
    {
        AliasOrName = aliasOrName;
        Package = package;
        IsTrusted = isTrusted;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class NamespaceBlockDefinitionSyntax : INamespaceBlockDefinitionSyntax
{
    private INamespaceBlockDefinitionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TypeName? Name { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public bool IsGlobalQualified { [DebuggerStepThrough] get; }
    public NamespaceName DeclaredNames { [DebuggerStepThrough] get; }
    public IFixedList<IImportDirectiveSyntax> ImportDirectives { [DebuggerStepThrough] get; }
    public IFixedList<INamespaceBlockMemberDefinitionSyntax> Definitions { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.NamespaceBlockDefinition_ToString(this);

    public NamespaceBlockDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        TypeName? name,
        TextSpan nameSpan,
        bool isGlobalQualified,
        NamespaceName declaredNames,
        IEnumerable<IImportDirectiveSyntax> importDirectives,
        IEnumerable<INamespaceBlockMemberDefinitionSyntax> definitions)
    {
        Span = span;
        File = file;
        Name = name;
        NameSpan = nameSpan;
        IsGlobalQualified = isGlobalQualified;
        DeclaredNames = declaredNames;
        ImportDirectives = importDirectives.ToFixedList();
        Definitions = definitions.ToFixedList();
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FunctionDefinitionSyntax : IFunctionDefinitionSyntax
{
    private IFunctionDefinitionSyntax Self { [Inline] get => this; }

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

    public FunctionDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        IEnumerable<IAttributeSyntax> attributes,
        IdentifierName name,
        IEnumerable<INamedParameterSyntax> parameters,
        IReturnSyntax? @return,
        IBodySyntax body)
    {
        Span = span;
        File = file;
        NameSpan = nameSpan;
        AccessModifier = accessModifier;
        Attributes = attributes.ToFixedList();
        Name = name;
        Parameters = parameters.ToFixedList();
        Return = @return;
        Body = body;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ClassDefinitionSyntax : IClassDefinitionSyntax
{
    private IClassDefinitionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public IConstKeywordToken? ConstModifier { [DebuggerStepThrough] get; }
    public IMoveKeywordToken? MoveModifier { [DebuggerStepThrough] get; }
    public OrdinaryName Name { [DebuggerStepThrough] get; }
    public IAbstractKeywordToken? AbstractModifier { [DebuggerStepThrough] get; }
    public IFixedList<IGenericParameterSyntax> GenericParameters { [DebuggerStepThrough] get; }
    public IOrdinaryTypeNameSyntax? BaseTypeName { [DebuggerStepThrough] get; }
    public IFixedList<IOrdinaryTypeNameSyntax> SupertypeNames { [DebuggerStepThrough] get; }
    public IFixedList<IClassMemberDefinitionSyntax> Members { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.ClassDefinition_ToString(this);

    public ClassDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        IConstKeywordToken? constModifier,
        IMoveKeywordToken? moveModifier,
        OrdinaryName name,
        IAbstractKeywordToken? abstractModifier,
        IEnumerable<IGenericParameterSyntax> genericParameters,
        IOrdinaryTypeNameSyntax? baseTypeName,
        IEnumerable<IOrdinaryTypeNameSyntax> supertypeNames,
        IEnumerable<IClassMemberDefinitionSyntax> members)
    {
        Span = span;
        File = file;
        NameSpan = nameSpan;
        AccessModifier = accessModifier;
        ConstModifier = constModifier;
        MoveModifier = moveModifier;
        Name = name;
        AbstractModifier = abstractModifier;
        GenericParameters = genericParameters.ToFixedList();
        BaseTypeName = baseTypeName;
        SupertypeNames = supertypeNames.ToFixedList();
        Members = members.ToFixedList();
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class StructDefinitionSyntax : IStructDefinitionSyntax
{
    private IStructDefinitionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public IConstKeywordToken? ConstModifier { [DebuggerStepThrough] get; }
    public IMoveKeywordToken? MoveModifier { [DebuggerStepThrough] get; }
    public OrdinaryName Name { [DebuggerStepThrough] get; }
    public IFixedList<IGenericParameterSyntax> GenericParameters { [DebuggerStepThrough] get; }
    public IFixedList<IOrdinaryTypeNameSyntax> SupertypeNames { [DebuggerStepThrough] get; }
    public IFixedList<IStructMemberDefinitionSyntax> Members { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.StructDefinition_ToString(this);

    public StructDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        IConstKeywordToken? constModifier,
        IMoveKeywordToken? moveModifier,
        OrdinaryName name,
        IEnumerable<IGenericParameterSyntax> genericParameters,
        IEnumerable<IOrdinaryTypeNameSyntax> supertypeNames,
        IEnumerable<IStructMemberDefinitionSyntax> members)
    {
        Span = span;
        File = file;
        NameSpan = nameSpan;
        AccessModifier = accessModifier;
        ConstModifier = constModifier;
        MoveModifier = moveModifier;
        Name = name;
        GenericParameters = genericParameters.ToFixedList();
        SupertypeNames = supertypeNames.ToFixedList();
        Members = members.ToFixedList();
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class TraitDefinitionSyntax : ITraitDefinitionSyntax
{
    private ITraitDefinitionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public IConstKeywordToken? ConstModifier { [DebuggerStepThrough] get; }
    public IMoveKeywordToken? MoveModifier { [DebuggerStepThrough] get; }
    public OrdinaryName Name { [DebuggerStepThrough] get; }
    public IFixedList<IGenericParameterSyntax> GenericParameters { [DebuggerStepThrough] get; }
    public IFixedList<IOrdinaryTypeNameSyntax> SupertypeNames { [DebuggerStepThrough] get; }
    public IFixedList<ITraitMemberDefinitionSyntax> Members { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.TraitDefinition_ToString(this);

    public TraitDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        IConstKeywordToken? constModifier,
        IMoveKeywordToken? moveModifier,
        OrdinaryName name,
        IEnumerable<IGenericParameterSyntax> genericParameters,
        IEnumerable<IOrdinaryTypeNameSyntax> supertypeNames,
        IEnumerable<ITraitMemberDefinitionSyntax> members)
    {
        Span = span;
        File = file;
        NameSpan = nameSpan;
        AccessModifier = accessModifier;
        ConstModifier = constModifier;
        MoveModifier = moveModifier;
        Name = name;
        GenericParameters = genericParameters.ToFixedList();
        SupertypeNames = supertypeNames.ToFixedList();
        Members = members.ToFixedList();
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class GenericParameterSyntax : IGenericParameterSyntax
{
    private IGenericParameterSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public ICapabilityConstraintSyntax Constraint { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public TypeParameterIndependence Independence { [DebuggerStepThrough] get; }
    public TypeParameterVariance Variance { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.GenericParameter_ToString(this);

    public GenericParameterSyntax(
        TextSpan span,
        ICapabilityConstraintSyntax constraint,
        IdentifierName name,
        TypeParameterIndependence independence,
        TypeParameterVariance variance)
    {
        Span = span;
        Constraint = constraint;
        Name = name;
        Independence = independence;
        Variance = variance;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AbstractMethodDefinitionSyntax : IAbstractMethodDefinitionSyntax
{
    private IAbstractMethodDefinitionSyntax Self { [Inline] get => this; }

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

    public AbstractMethodDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        IdentifierName name,
        IMethodSelfParameterSyntax selfParameter,
        IEnumerable<INamedParameterSyntax> parameters,
        IReturnSyntax? @return)
    {
        Span = span;
        File = file;
        NameSpan = nameSpan;
        AccessModifier = accessModifier;
        Name = name;
        SelfParameter = selfParameter;
        Parameters = parameters.ToFixedList();
        Return = @return;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class OrdinaryMethodDefinitionSyntax : IOrdinaryMethodDefinitionSyntax
{
    private IOrdinaryMethodDefinitionSyntax Self { [Inline] get => this; }

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
        => FormattingAspect.OrdinaryMethodDefinition_ToString(this);

    public OrdinaryMethodDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        IdentifierName name,
        IMethodSelfParameterSyntax selfParameter,
        IEnumerable<INamedParameterSyntax> parameters,
        IReturnSyntax? @return,
        IBodySyntax body)
    {
        Span = span;
        File = file;
        NameSpan = nameSpan;
        AccessModifier = accessModifier;
        Name = name;
        SelfParameter = selfParameter;
        Parameters = parameters.ToFixedList();
        Return = @return;
        Body = body;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class GetterMethodDefinitionSyntax : IGetterMethodDefinitionSyntax
{
    private IGetterMethodDefinitionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IMethodSelfParameterSyntax SelfParameter { [DebuggerStepThrough] get; }
    public IReturnSyntax Return { [DebuggerStepThrough] get; }
    public IBodySyntax? Body { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.GetterMethodDefinition_ToString(this);

    public GetterMethodDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        IdentifierName name,
        IMethodSelfParameterSyntax selfParameter,
        IReturnSyntax @return,
        IBodySyntax? body)
    {
        Span = span;
        File = file;
        NameSpan = nameSpan;
        AccessModifier = accessModifier;
        Name = name;
        SelfParameter = selfParameter;
        Return = @return;
        Body = body;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SetterMethodDefinitionSyntax : ISetterMethodDefinitionSyntax
{
    private ISetterMethodDefinitionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public CodeFile File { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public IAccessModifierToken? AccessModifier { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IMethodSelfParameterSyntax SelfParameter { [DebuggerStepThrough] get; }
    public IFixedList<INamedParameterSyntax> Parameters { [DebuggerStepThrough] get; }
    public IBodySyntax Body { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.SetterMethodDefinition_ToString(this);

    public SetterMethodDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        IdentifierName name,
        IMethodSelfParameterSyntax selfParameter,
        IEnumerable<INamedParameterSyntax> parameters,
        IBodySyntax body)
    {
        Span = span;
        File = file;
        NameSpan = nameSpan;
        AccessModifier = accessModifier;
        Name = name;
        SelfParameter = selfParameter;
        Parameters = parameters.ToFixedList();
        Body = body;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ConstructorDefinitionSyntax : IConstructorDefinitionSyntax
{
    private IConstructorDefinitionSyntax Self { [Inline] get => this; }

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

    public ConstructorDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        IdentifierName? name,
        IConstructorSelfParameterSyntax selfParameter,
        IEnumerable<IConstructorOrInitializerParameterSyntax> parameters,
        IBlockBodySyntax body)
    {
        Span = span;
        File = file;
        NameSpan = nameSpan;
        AccessModifier = accessModifier;
        Name = name;
        SelfParameter = selfParameter;
        Parameters = parameters.ToFixedList();
        Body = body;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class InitializerDefinitionSyntax : IInitializerDefinitionSyntax
{
    private IInitializerDefinitionSyntax Self { [Inline] get => this; }

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

    public InitializerDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        IdentifierName? name,
        IInitializerSelfParameterSyntax selfParameter,
        IEnumerable<IConstructorOrInitializerParameterSyntax> parameters,
        IBlockBodySyntax body)
    {
        Span = span;
        File = file;
        NameSpan = nameSpan;
        AccessModifier = accessModifier;
        Name = name;
        SelfParameter = selfParameter;
        Parameters = parameters.ToFixedList();
        Body = body;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FieldDefinitionSyntax : IFieldDefinitionSyntax
{
    private IFieldDefinitionSyntax Self { [Inline] get => this; }

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

    public FieldDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        bool isMutableBinding,
        IdentifierName name,
        ITypeSyntax type,
        IExpressionSyntax? initializer)
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
file class AssociatedFunctionDefinitionSyntax : IAssociatedFunctionDefinitionSyntax
{
    private IAssociatedFunctionDefinitionSyntax Self { [Inline] get => this; }

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

    public AssociatedFunctionDefinitionSyntax(
        TextSpan span,
        CodeFile file,
        TextSpan nameSpan,
        IAccessModifierToken? accessModifier,
        IdentifierName name,
        IEnumerable<INamedParameterSyntax> parameters,
        IReturnSyntax? @return,
        IBodySyntax body)
    {
        Span = span;
        File = file;
        NameSpan = nameSpan;
        AccessModifier = accessModifier;
        Name = name;
        Parameters = parameters.ToFixedList();
        Return = @return;
        Body = body;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AttributeSyntax : IAttributeSyntax
{
    private IAttributeSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IOrdinaryTypeNameSyntax TypeName { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.Attribute_ToString(this);

    public AttributeSyntax(
        TextSpan span,
        IOrdinaryTypeNameSyntax typeName)
    {
        Span = span;
        TypeName = typeName;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class CapabilitySetSyntax : ICapabilitySetSyntax
{
    private ICapabilitySetSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public CapabilitySet CapabilitySet { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.CapabilitySet_ToString(this);

    public CapabilitySetSyntax(
        TextSpan span,
        CapabilitySet capabilitySet)
    {
        Span = span;
        CapabilitySet = capabilitySet;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class CapabilitySyntax : ICapabilitySyntax
{
    private ICapabilitySyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IFixedList<ICapabilityToken> Tokens { [DebuggerStepThrough] get; }
    public DeclaredCapability Capability { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.Capability_ToString(this);

    public CapabilitySyntax(
        TextSpan span,
        IEnumerable<ICapabilityToken> tokens,
        DeclaredCapability capability)
    {
        Span = span;
        Tokens = tokens.ToFixedList();
        Capability = capability;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class NamedParameterSyntax : INamedParameterSyntax
{
    private INamedParameterSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public bool IsLentBinding { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public ITypeSyntax Type { [DebuggerStepThrough] get; }
    public IExpressionSyntax? DefaultValue { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.NamedParameter_ToString(this);

    public NamedParameterSyntax(
        TextSpan span,
        TextSpan nameSpan,
        bool isMutableBinding,
        bool isLentBinding,
        IdentifierName name,
        ITypeSyntax type,
        IExpressionSyntax? defaultValue)
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
file class ConstructorSelfParameterSyntax : IConstructorSelfParameterSyntax
{
    private IConstructorSelfParameterSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public bool IsLentBinding { [DebuggerStepThrough] get; }
    public ICapabilitySyntax Constraint { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.SelfParameter_ToString(this);

    public ConstructorSelfParameterSyntax(
        TextSpan span,
        bool isLentBinding,
        ICapabilitySyntax constraint)
    {
        Span = span;
        IsLentBinding = isLentBinding;
        Constraint = constraint;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class InitializerSelfParameterSyntax : IInitializerSelfParameterSyntax
{
    private IInitializerSelfParameterSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public bool IsLentBinding { [DebuggerStepThrough] get; }
    public ICapabilitySyntax Constraint { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.SelfParameter_ToString(this);

    public InitializerSelfParameterSyntax(
        TextSpan span,
        bool isLentBinding,
        ICapabilitySyntax constraint)
    {
        Span = span;
        IsLentBinding = isLentBinding;
        Constraint = constraint;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class MethodSelfParameterSyntax : IMethodSelfParameterSyntax
{
    private IMethodSelfParameterSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public bool IsLentBinding { [DebuggerStepThrough] get; }
    public ICapabilityConstraintSyntax Constraint { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.SelfParameter_ToString(this);

    public MethodSelfParameterSyntax(
        TextSpan span,
        bool isLentBinding,
        ICapabilityConstraintSyntax constraint)
    {
        Span = span;
        IsLentBinding = isLentBinding;
        Constraint = constraint;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FieldParameterSyntax : IFieldParameterSyntax
{
    private IFieldParameterSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public IExpressionSyntax? DefaultValue { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.FieldParameter_ToString(this);

    public FieldParameterSyntax(
        TextSpan span,
        IdentifierName name,
        IExpressionSyntax? defaultValue)
    {
        Span = span;
        Name = name;
        DefaultValue = defaultValue;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ReturnSyntax : IReturnSyntax
{
    private IReturnSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public ITypeSyntax Type { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.Return_ToString(this);

    public ReturnSyntax(
        TextSpan span,
        ITypeSyntax type)
    {
        Span = span;
        Type = type;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BlockBodySyntax : IBlockBodySyntax
{
    private IBlockBodySyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IFixedList<IBodyStatementSyntax> Statements { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.BlockBody_ToString(this);

    public BlockBodySyntax(
        TextSpan span,
        IEnumerable<IBodyStatementSyntax> statements)
    {
        Span = span;
        Statements = statements.ToFixedList();
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ExpressionBodySyntax : IExpressionBodySyntax
{
    private IExpressionBodySyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IResultStatementSyntax ResultStatement { [DebuggerStepThrough] get; }
    public IFixedList<IStatementSyntax> Statements { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.ExpressionBody_ToString(this);

    public ExpressionBodySyntax(
        TextSpan span,
        IResultStatementSyntax resultStatement)
    {
        Span = span;
        ResultStatement = resultStatement;
        Statements = ComputedAspect.ExpressionBody_Statements(this);
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BuiltInTypeNameSyntax : IBuiltInTypeNameSyntax
{
    private IBuiltInTypeNameSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public BuiltInTypeName Name { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.BuiltInTypeName_ToString(this);

    public BuiltInTypeNameSyntax(
        TextSpan span,
        BuiltInTypeName name)
    {
        Span = span;
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class IdentifierTypeNameSyntax : IIdentifierTypeNameSyntax
{
    private IIdentifierTypeNameSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.IdentifierTypeName_ToString(this);

    public IdentifierTypeNameSyntax(
        TextSpan span,
        IdentifierName name)
    {
        Span = span;
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class GenericTypeNameSyntax : IGenericTypeNameSyntax
{
    private IGenericTypeNameSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public GenericName Name { [DebuggerStepThrough] get; }
    public IFixedList<ITypeSyntax> GenericArguments { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.GenericTypeName_ToString(this);

    public GenericTypeNameSyntax(
        TextSpan span,
        GenericName name,
        IEnumerable<ITypeSyntax> genericArguments)
    {
        Span = span;
        Name = name;
        GenericArguments = genericArguments.ToFixedList();
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class QualifiedTypeNameSyntax : IQualifiedTypeNameSyntax
{
    private IQualifiedTypeNameSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public TypeName Name { [DebuggerStepThrough] get; }
    public ITypeNameSyntax Context { [DebuggerStepThrough] get; }
    public IOrdinaryTypeNameSyntax QualifiedName { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.QualifiedTypeName_ToString(this);

    public QualifiedTypeNameSyntax(
        TextSpan span,
        TypeName name,
        ITypeNameSyntax context,
        IOrdinaryTypeNameSyntax qualifiedName)
    {
        Span = span;
        Name = name;
        Context = context;
        QualifiedName = qualifiedName;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class OptionalTypeSyntax : IOptionalTypeSyntax
{
    private IOptionalTypeSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public ITypeSyntax Referent { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.OptionalType_ToString(this);

    public OptionalTypeSyntax(
        TextSpan span,
        ITypeSyntax referent)
    {
        Span = span;
        Referent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class CapabilityTypeSyntax : ICapabilityTypeSyntax
{
    private ICapabilityTypeSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public ICapabilitySyntax Capability { [DebuggerStepThrough] get; }
    public ITypeSyntax Referent { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.CapabilityType_ToString(this);

    public CapabilityTypeSyntax(
        TextSpan span,
        ICapabilitySyntax capability,
        ITypeSyntax referent)
    {
        Span = span;
        Capability = capability;
        Referent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FunctionTypeSyntax : IFunctionTypeSyntax
{
    private IFunctionTypeSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IFixedList<IParameterTypeSyntax> Parameters { [DebuggerStepThrough] get; }
    public ITypeSyntax Return { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.FunctionType_ToString(this);

    public FunctionTypeSyntax(
        TextSpan span,
        IEnumerable<IParameterTypeSyntax> parameters,
        ITypeSyntax @return)
    {
        Span = span;
        Parameters = parameters.ToFixedList();
        Return = @return;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ParameterTypeSyntax : IParameterTypeSyntax
{
    private IParameterTypeSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public bool IsLent { [DebuggerStepThrough] get; }
    public ITypeSyntax Referent { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.ParameterType_ToString(this);

    public ParameterTypeSyntax(
        TextSpan span,
        bool isLent,
        ITypeSyntax referent)
    {
        Span = span;
        IsLent = isLent;
        Referent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class CapabilityViewpointTypeSyntax : ICapabilityViewpointTypeSyntax
{
    private ICapabilityViewpointTypeSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public ICapabilitySyntax Capability { [DebuggerStepThrough] get; }
    public ITypeSyntax Referent { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.CapabilityViewpointType_ToString(this);

    public CapabilityViewpointTypeSyntax(
        TextSpan span,
        ICapabilitySyntax capability,
        ITypeSyntax referent)
    {
        Span = span;
        Capability = capability;
        Referent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SelfViewpointTypeSyntax : ISelfViewpointTypeSyntax
{
    private ISelfViewpointTypeSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public ITypeSyntax Referent { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.SelfViewpointType_ToString(this);

    public SelfViewpointTypeSyntax(
        TextSpan span,
        ITypeSyntax referent)
    {
        Span = span;
        Referent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ResultStatementSyntax : IResultStatementSyntax
{
    private IResultStatementSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax Expression { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.ResultStatement_ToString(this);

    public ResultStatementSyntax(
        TextSpan span,
        IExpressionSyntax expression)
    {
        Span = span;
        Expression = expression;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class VariableDeclarationStatementSyntax : IVariableDeclarationStatementSyntax
{
    private IVariableDeclarationStatementSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public ICapabilitySyntax? Capability { [DebuggerStepThrough] get; }
    public ITypeSyntax? Type { [DebuggerStepThrough] get; }
    public IExpressionSyntax? Initializer { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.VariableDeclarationStatement_ToString(this);

    public VariableDeclarationStatementSyntax(
        TextSpan span,
        bool isMutableBinding,
        TextSpan nameSpan,
        IdentifierName name,
        ICapabilitySyntax? capability,
        ITypeSyntax? type,
        IExpressionSyntax? initializer)
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
file class ExpressionStatementSyntax : IExpressionStatementSyntax
{
    private IExpressionStatementSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax Expression { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.ExpressionStatement_ToString(this);

    public ExpressionStatementSyntax(
        TextSpan span,
        IExpressionSyntax expression)
    {
        Span = span;
        Expression = expression;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BindingContextPatternSyntax : IBindingContextPatternSyntax
{
    private IBindingContextPatternSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public IPatternSyntax Pattern { [DebuggerStepThrough] get; }
    public ITypeSyntax? Type { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.BindingContextPattern_ToString(this);

    public BindingContextPatternSyntax(
        TextSpan span,
        bool isMutableBinding,
        IPatternSyntax pattern,
        ITypeSyntax? type)
    {
        Span = span;
        IsMutableBinding = isMutableBinding;
        Pattern = pattern;
        Type = type;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BindingPatternSyntax : IBindingPatternSyntax
{
    private IBindingPatternSyntax Self { [Inline] get => this; }

    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.BindingPattern_ToString(this);

    public BindingPatternSyntax(
        bool isMutableBinding,
        TextSpan nameSpan,
        IdentifierName name)
    {
        IsMutableBinding = isMutableBinding;
        NameSpan = nameSpan;
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class OptionalPatternSyntax : IOptionalPatternSyntax
{
    private IOptionalPatternSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IOptionalOrBindingPatternSyntax Pattern { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.OptionalPattern_ToString(this);

    public OptionalPatternSyntax(
        TextSpan span,
        IOptionalOrBindingPatternSyntax pattern)
    {
        Span = span;
        Pattern = pattern;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BlockExpressionSyntax : IBlockExpressionSyntax
{
    private IBlockExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IFixedList<IStatementSyntax> Statements { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.BlockExpression_ToString(this);

    public BlockExpressionSyntax(
        TextSpan span,
        IEnumerable<IStatementSyntax> statements)
    {
        Span = span;
        Statements = statements.ToFixedList();
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class NewObjectExpressionSyntax : INewObjectExpressionSyntax
{
    private INewObjectExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public ITypeNameSyntax Type { [DebuggerStepThrough] get; }
    public IdentifierName? ConstructorName { [DebuggerStepThrough] get; }
    public TextSpan? ConstructorNameSpan { [DebuggerStepThrough] get; }
    public IFixedList<IExpressionSyntax> Arguments { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.NewObjectExpression_ToString(this);

    public NewObjectExpressionSyntax(
        TextSpan span,
        ITypeNameSyntax type,
        IdentifierName? constructorName,
        TextSpan? constructorNameSpan,
        IEnumerable<IExpressionSyntax> arguments)
    {
        Span = span;
        Type = type;
        ConstructorName = constructorName;
        ConstructorNameSpan = constructorNameSpan;
        Arguments = arguments.ToFixedList();
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UnsafeExpressionSyntax : IUnsafeExpressionSyntax
{
    private IUnsafeExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax Expression { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.UnsafeExpression_ToString(this);

    public UnsafeExpressionSyntax(
        TextSpan span,
        IExpressionSyntax expression)
    {
        Span = span;
        Expression = expression;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BoolLiteralExpressionSyntax : IBoolLiteralExpressionSyntax
{
    private IBoolLiteralExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public bool Value { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.BoolLiteralExpression_ToString(this);

    public BoolLiteralExpressionSyntax(
        TextSpan span,
        bool value)
    {
        Span = span;
        Value = value;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class IntegerLiteralExpressionSyntax : IIntegerLiteralExpressionSyntax
{
    private IIntegerLiteralExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public BigInteger Value { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.IntegerLiteralExpression_ToString(this);

    public IntegerLiteralExpressionSyntax(
        TextSpan span,
        BigInteger value)
    {
        Span = span;
        Value = value;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class NoneLiteralExpressionSyntax : INoneLiteralExpressionSyntax
{
    private INoneLiteralExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.NoneLiteralExpression_ToString(this);

    public NoneLiteralExpressionSyntax(TextSpan span)
    {
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class StringLiteralExpressionSyntax : IStringLiteralExpressionSyntax
{
    private IStringLiteralExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public string Value { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.StringLiteralExpression_ToString(this);

    public StringLiteralExpressionSyntax(
        TextSpan span,
        string value)
    {
        Span = span;
        Value = value;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AssignmentExpressionSyntax : IAssignmentExpressionSyntax
{
    private IAssignmentExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax LeftOperand { [DebuggerStepThrough] get; }
    public AssignmentOperator Operator { [DebuggerStepThrough] get; }
    public IExpressionSyntax RightOperand { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.AssignmentExpression_ToString(this);

    public AssignmentExpressionSyntax(
        TextSpan span,
        IExpressionSyntax leftOperand,
        AssignmentOperator @operator,
        IExpressionSyntax rightOperand)
    {
        Span = span;
        LeftOperand = leftOperand;
        Operator = @operator;
        RightOperand = rightOperand;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BinaryOperatorExpressionSyntax : IBinaryOperatorExpressionSyntax
{
    private IBinaryOperatorExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax LeftOperand { [DebuggerStepThrough] get; }
    public BinaryOperator Operator { [DebuggerStepThrough] get; }
    public IExpressionSyntax RightOperand { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.BinaryOperatorExpression_ToString(this);

    public BinaryOperatorExpressionSyntax(
        TextSpan span,
        IExpressionSyntax leftOperand,
        BinaryOperator @operator,
        IExpressionSyntax rightOperand)
    {
        Span = span;
        LeftOperand = leftOperand;
        Operator = @operator;
        RightOperand = rightOperand;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class UnaryOperatorExpressionSyntax : IUnaryOperatorExpressionSyntax
{
    private IUnaryOperatorExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public UnaryOperatorFixity Fixity { [DebuggerStepThrough] get; }
    public UnaryOperator Operator { [DebuggerStepThrough] get; }
    public IExpressionSyntax Operand { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.UnaryOperatorExpression_ToString(this);

    public UnaryOperatorExpressionSyntax(
        TextSpan span,
        UnaryOperatorFixity fixity,
        UnaryOperator @operator,
        IExpressionSyntax operand)
    {
        Span = span;
        Fixity = fixity;
        Operator = @operator;
        Operand = operand;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ConversionExpressionSyntax : IConversionExpressionSyntax
{
    private IConversionExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax Referent { [DebuggerStepThrough] get; }
    public ConversionOperator Operator { [DebuggerStepThrough] get; }
    public ITypeSyntax ConvertToType { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.ConversionExpression_ToString(this);

    public ConversionExpressionSyntax(
        TextSpan span,
        IExpressionSyntax referent,
        ConversionOperator @operator,
        ITypeSyntax convertToType)
    {
        Span = span;
        Referent = referent;
        Operator = @operator;
        ConvertToType = convertToType;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class PatternMatchExpressionSyntax : IPatternMatchExpressionSyntax
{
    private IPatternMatchExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax Referent { [DebuggerStepThrough] get; }
    public IPatternSyntax Pattern { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.PatternMatchExpression_ToString(this);

    public PatternMatchExpressionSyntax(
        TextSpan span,
        IExpressionSyntax referent,
        IPatternSyntax pattern)
    {
        Span = span;
        Referent = referent;
        Pattern = pattern;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class IfExpressionSyntax : IIfExpressionSyntax
{
    private IIfExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax Condition { [DebuggerStepThrough] get; }
    public IBlockOrResultSyntax ThenBlock { [DebuggerStepThrough] get; }
    public IElseClauseSyntax? ElseClause { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.IfExpression_ToString(this);

    public IfExpressionSyntax(
        TextSpan span,
        IExpressionSyntax condition,
        IBlockOrResultSyntax thenBlock,
        IElseClauseSyntax? elseClause)
    {
        Span = span;
        Condition = condition;
        ThenBlock = thenBlock;
        ElseClause = elseClause;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class LoopExpressionSyntax : ILoopExpressionSyntax
{
    private ILoopExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IBlockExpressionSyntax Block { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.LoopExpression_ToString(this);

    public LoopExpressionSyntax(
        TextSpan span,
        IBlockExpressionSyntax block)
    {
        Span = span;
        Block = block;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class WhileExpressionSyntax : IWhileExpressionSyntax
{
    private IWhileExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax Condition { [DebuggerStepThrough] get; }
    public IBlockExpressionSyntax Block { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.WhileExpression_ToString(this);

    public WhileExpressionSyntax(
        TextSpan span,
        IExpressionSyntax condition,
        IBlockExpressionSyntax block)
    {
        Span = span;
        Condition = condition;
        Block = block;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ForeachExpressionSyntax : IForeachExpressionSyntax
{
    private IForeachExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public bool IsMutableBinding { [DebuggerStepThrough] get; }
    public TextSpan NameSpan { [DebuggerStepThrough] get; }
    public IdentifierName VariableName { [DebuggerStepThrough] get; }
    public IExpressionSyntax InExpression { [DebuggerStepThrough] get; }
    public ITypeSyntax? Type { [DebuggerStepThrough] get; }
    public IBlockExpressionSyntax Block { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.ForeachExpression_ToString(this);

    public ForeachExpressionSyntax(
        TextSpan span,
        bool isMutableBinding,
        TextSpan nameSpan,
        IdentifierName variableName,
        IExpressionSyntax inExpression,
        ITypeSyntax? type,
        IBlockExpressionSyntax block)
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
file class BreakExpressionSyntax : IBreakExpressionSyntax
{
    private IBreakExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax? Value { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.BreakExpression_ToString(this);

    public BreakExpressionSyntax(
        TextSpan span,
        IExpressionSyntax? value)
    {
        Span = span;
        Value = value;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class NextExpressionSyntax : INextExpressionSyntax
{
    private INextExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.NextExpression_ToString(this);

    public NextExpressionSyntax(TextSpan span)
    {
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class ReturnExpressionSyntax : IReturnExpressionSyntax
{
    private IReturnExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax? Value { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.ReturnExpression_ToString(this);

    public ReturnExpressionSyntax(
        TextSpan span,
        IExpressionSyntax? value)
    {
        Span = span;
        Value = value;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class InvocationExpressionSyntax : IInvocationExpressionSyntax
{
    private IInvocationExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax Expression { [DebuggerStepThrough] get; }
    public IFixedList<IExpressionSyntax> Arguments { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.InvocationExpression_ToString(this);

    public InvocationExpressionSyntax(
        TextSpan span,
        IExpressionSyntax expression,
        IEnumerable<IExpressionSyntax> arguments)
    {
        Span = span;
        Expression = expression;
        Arguments = arguments.ToFixedList();
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class BuiltInTypeNameExpressionSyntax : IBuiltInTypeNameExpressionSyntax
{
    private IBuiltInTypeNameExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public BuiltInTypeName Name { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.BuiltInTypeNameExpression_ToString(this);

    public BuiltInTypeNameExpressionSyntax(
        TextSpan span,
        BuiltInTypeName name)
    {
        Span = span;
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class IdentifierNameExpressionSyntax : IIdentifierNameExpressionSyntax
{
    private IIdentifierNameExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IdentifierName Name { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.IdentifierNameExpression_ToString(this);

    public IdentifierNameExpressionSyntax(
        TextSpan span,
        IdentifierName name)
    {
        Span = span;
        Name = name;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class GenericNameExpressionSyntax : IGenericNameExpressionSyntax
{
    private IGenericNameExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public GenericName Name { [DebuggerStepThrough] get; }
    public IFixedList<ITypeSyntax> GenericArguments { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.GenericNameExpression_ToString(this);

    public GenericNameExpressionSyntax(
        TextSpan span,
        GenericName name,
        IEnumerable<ITypeSyntax> genericArguments)
    {
        Span = span;
        Name = name;
        GenericArguments = genericArguments.ToFixedList();
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class SelfExpressionSyntax : ISelfExpressionSyntax
{
    private ISelfExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public bool IsImplicit { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.SelfExpression_ToString(this);

    public SelfExpressionSyntax(
        TextSpan span,
        bool isImplicit)
    {
        Span = span;
        IsImplicit = isImplicit;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class MemberAccessExpressionSyntax : IMemberAccessExpressionSyntax
{
    private IMemberAccessExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax Context { [DebuggerStepThrough] get; }
    public OrdinaryName MemberName { [DebuggerStepThrough] get; }
    public IFixedList<ITypeSyntax> GenericArguments { [DebuggerStepThrough] get; }
    public TextSpan MemberNameSpan { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.MemberAccessExpression_ToString(this);

    public MemberAccessExpressionSyntax(
        TextSpan span,
        IExpressionSyntax context,
        OrdinaryName memberName,
        IEnumerable<ITypeSyntax> genericArguments,
        TextSpan memberNameSpan)
    {
        Span = span;
        Context = context;
        MemberName = memberName;
        GenericArguments = genericArguments.ToFixedList();
        MemberNameSpan = memberNameSpan;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class MissingNameSyntax : IMissingNameSyntax
{
    private IMissingNameSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.MissingName_ToString(this);

    public MissingNameSyntax(TextSpan span)
    {
        Span = span;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class MoveExpressionSyntax : IMoveExpressionSyntax
{
    private IMoveExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax Referent { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.MoveExpression_ToString(this);

    public MoveExpressionSyntax(
        TextSpan span,
        IExpressionSyntax referent)
    {
        Span = span;
        Referent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class FreezeExpressionSyntax : IFreezeExpressionSyntax
{
    private IFreezeExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax Referent { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.FreezeExpression_ToString(this);

    public FreezeExpressionSyntax(
        TextSpan span,
        IExpressionSyntax referent)
    {
        Span = span;
        Referent = referent;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AsyncBlockExpressionSyntax : IAsyncBlockExpressionSyntax
{
    private IAsyncBlockExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IBlockExpressionSyntax Block { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.AsyncBlockExpression_ToString(this);

    public AsyncBlockExpressionSyntax(
        TextSpan span,
        IBlockExpressionSyntax block)
    {
        Span = span;
        Block = block;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AsyncStartExpressionSyntax : IAsyncStartExpressionSyntax
{
    private IAsyncStartExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public bool Scheduled { [DebuggerStepThrough] get; }
    public IExpressionSyntax Expression { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.AsyncStartExpression_ToString(this);

    public AsyncStartExpressionSyntax(
        TextSpan span,
        bool scheduled,
        IExpressionSyntax expression)
    {
        Span = span;
        Scheduled = scheduled;
        Expression = expression;
    }
}

[GeneratedCode("AzothCompilerCodeGen", null)]
file class AwaitExpressionSyntax : IAwaitExpressionSyntax
{
    private IAwaitExpressionSyntax Self { [Inline] get => this; }

    public TextSpan Span { [DebuggerStepThrough] get; }
    public IExpressionSyntax Expression { [DebuggerStepThrough] get; }
    public override string ToString()
        => FormattingAspect.AwaitExpression_ToString(this);

    public AwaitExpressionSyntax(
        TextSpan span,
        IExpressionSyntax expression)
    {
        Span = span;
        Expression = expression;
    }
}

