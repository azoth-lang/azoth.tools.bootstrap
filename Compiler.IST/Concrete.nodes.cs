using System.CodeDom.Compiler;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart

// ReSharper disable once CheckNamespace
namespace Azoth.Tools.Bootstrap.Compiler.IST;

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonPackage : Concrete.Package { }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class PackageNode : Node, CommonPackage
{
    public IPackageSyntax Syntax { get; }
    public PackageSymbol Symbol { get; }
    public IFixedList<CommonPackageReference> References { get; }
    IFixedList<Concrete.PackageReference> Concrete.Package.References => References;
    public IFixedList<CommonCompilationUnit> CompilationUnits { get; }
    IFixedList<Concrete.CompilationUnit> Concrete.Package.CompilationUnits => CompilationUnits;
    public IFixedList<CommonCompilationUnit> TestingCompilationUnits { get; }
    IFixedList<Concrete.CompilationUnit> Concrete.Package.TestingCompilationUnits => TestingCompilationUnits;

    public PackageNode(IPackageSyntax syntax, PackageSymbol symbol, IFixedList<CommonPackageReference> references, IFixedList<CommonCompilationUnit> compilationUnits, IFixedList<CommonCompilationUnit> testingCompilationUnits)
    {
        Syntax = syntax;
        Symbol = symbol;
        References = references;
        CompilationUnits = compilationUnits;
        TestingCompilationUnits = testingCompilationUnits;
    }
 }

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonPackageReference : Concrete.PackageReference { }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class PackageReferenceNode : Node, CommonPackageReference
{
    public IdentifierName AliasOrName { get; }
    public AST.Package Package { get; }

    public PackageReferenceNode(IdentifierName aliasOrName, AST.Package package)
    {
        AliasOrName = aliasOrName;
        Package = package;
    }
 }

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonCompilationUnit : Concrete.CompilationUnit, CommonCode { }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class CompilationUnitNode : Node, CommonCompilationUnit
{
    public ICompilationUnitSyntax Syntax { get; }
    ISyntax Concrete.Code.Syntax => Syntax;
    public CodeFile File { get; }
    public NamespaceName ImplicitNamespaceName { get; }
    public IFixedList<CommonUsingDirective> UsingDirectives { get; }
    IFixedList<Concrete.UsingDirective> Concrete.CompilationUnit.UsingDirectives => UsingDirectives;
    public IFixedList<CommonNamespaceMemberDeclaration> Declarations { get; }
    IFixedList<Concrete.NamespaceMemberDeclaration> Concrete.CompilationUnit.Declarations => Declarations;

    public CompilationUnitNode(ICompilationUnitSyntax syntax, CodeFile file, NamespaceName implicitNamespaceName, IFixedList<CommonUsingDirective> usingDirectives, IFixedList<CommonNamespaceMemberDeclaration> declarations)
    {
        Syntax = syntax;
        File = file;
        ImplicitNamespaceName = implicitNamespaceName;
        UsingDirectives = usingDirectives;
        Declarations = declarations;
    }
 }

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonUsingDirective : Concrete.UsingDirective, CommonCode { }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class UsingDirectiveNode : Node, CommonUsingDirective
{
    public IUsingDirectiveSyntax Syntax { get; }
    ISyntax Concrete.Code.Syntax => Syntax;
    public NamespaceName Name { get; }

    public UsingDirectiveNode(IUsingDirectiveSyntax syntax, NamespaceName name)
    {
        Syntax = syntax;
        Name = name;
    }
 }

// ReSharper disable once PossibleInterfaceMemberAmbiguity
[Closed(
    typeof(CommonCompilationUnit),
    typeof(CommonUsingDirective),
    typeof(CommonDeclaration))]
internal partial interface CommonCode : Concrete.Code { }

// ReSharper disable once PossibleInterfaceMemberAmbiguity
[Closed(
    typeof(CommonNamespaceMemberDeclaration),
    typeof(CommonNamespaceDeclaration),
    typeof(CommonTypeDeclaration),
    typeof(CommonTypeMemberDeclaration),
    typeof(CommonFunctionDeclaration))]
internal partial interface CommonDeclaration : Concrete.Declaration, CommonCode { }

// ReSharper disable once PossibleInterfaceMemberAmbiguity
[Closed(
    typeof(CommonNamespaceDeclaration),
    typeof(CommonTypeDeclaration),
    typeof(CommonFunctionDeclaration))]
internal partial interface CommonNamespaceMemberDeclaration : Concrete.NamespaceMemberDeclaration, CommonDeclaration { }

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonNamespaceDeclaration : Concrete.NamespaceDeclaration, CommonDeclaration, CommonNamespaceMemberDeclaration { }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class NamespaceDeclarationNode : Node, CommonNamespaceDeclaration
{
    public INamespaceDeclarationSyntax Syntax { get; }
    IDeclarationSyntax Concrete.Declaration.Syntax => Syntax;
    ISyntax Concrete.Code.Syntax => Syntax;
    public IFixedList<CommonUsingDirective> UsingDirectives { get; }
    IFixedList<Concrete.UsingDirective> Concrete.NamespaceDeclaration.UsingDirectives => UsingDirectives;
    public IFixedList<CommonNamespaceMemberDeclaration> Declarations { get; }
    IFixedList<Concrete.NamespaceMemberDeclaration> Concrete.NamespaceDeclaration.Declarations => Declarations;

    public NamespaceDeclarationNode(INamespaceDeclarationSyntax syntax, IFixedList<CommonUsingDirective> usingDirectives, IFixedList<CommonNamespaceMemberDeclaration> declarations)
    {
        Syntax = syntax;
        UsingDirectives = usingDirectives;
        Declarations = declarations;
    }
 }

// ReSharper disable once PossibleInterfaceMemberAmbiguity
[Closed(
    typeof(CommonClassDeclaration),
    typeof(CommonStructDeclaration),
    typeof(CommonTraitDeclaration))]
internal partial interface CommonTypeDeclaration : Concrete.TypeDeclaration, CommonDeclaration, CommonNamespaceMemberDeclaration, CommonTypeMemberDeclaration { }

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonClassDeclaration : Concrete.ClassDeclaration, CommonTypeDeclaration { }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class ClassDeclarationNode : Node, CommonClassDeclaration
{
    public IClassDeclarationSyntax Syntax { get; }
    ITypeDeclarationSyntax Concrete.TypeDeclaration.Syntax => Syntax;
    IDeclarationSyntax Concrete.Declaration.Syntax => Syntax;
    ISyntax Concrete.Code.Syntax => Syntax;
    public bool IsAbstract { get; }
    public IFixedList<CommonClassMemberDeclaration> Members { get; }
    IFixedList<Concrete.ClassMemberDeclaration> Concrete.ClassDeclaration.Members => Members;

    public ClassDeclarationNode(IClassDeclarationSyntax syntax, bool isAbstract, IFixedList<CommonClassMemberDeclaration> members)
    {
        Syntax = syntax;
        IsAbstract = isAbstract;
        Members = members;
    }
 }

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonStructDeclaration : Concrete.StructDeclaration, CommonTypeDeclaration { }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class StructDeclarationNode : Node, CommonStructDeclaration
{
    public IStructDeclarationSyntax Syntax { get; }
    ITypeDeclarationSyntax Concrete.TypeDeclaration.Syntax => Syntax;
    IDeclarationSyntax Concrete.Declaration.Syntax => Syntax;
    ISyntax Concrete.Code.Syntax => Syntax;
    public IFixedList<CommonStructMemberDeclaration> Members { get; }
    IFixedList<Concrete.StructMemberDeclaration> Concrete.StructDeclaration.Members => Members;

    public StructDeclarationNode(IStructDeclarationSyntax syntax, IFixedList<CommonStructMemberDeclaration> members)
    {
        Syntax = syntax;
        Members = members;
    }
 }

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonTraitDeclaration : Concrete.TraitDeclaration, CommonTypeDeclaration { }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class TraitDeclarationNode : Node, CommonTraitDeclaration
{
    public ITraitDeclarationSyntax Syntax { get; }
    ITypeDeclarationSyntax Concrete.TypeDeclaration.Syntax => Syntax;
    IDeclarationSyntax Concrete.Declaration.Syntax => Syntax;
    ISyntax Concrete.Code.Syntax => Syntax;
    public IFixedList<CommonTraitMemberDeclaration> Members { get; }
    IFixedList<Concrete.TraitMemberDeclaration> Concrete.TraitDeclaration.Members => Members;

    public TraitDeclarationNode(ITraitDeclarationSyntax syntax, IFixedList<CommonTraitMemberDeclaration> members)
    {
        Syntax = syntax;
        Members = members;
    }
 }

// ReSharper disable once PossibleInterfaceMemberAmbiguity
[Closed(
    typeof(CommonTypeDeclaration),
    typeof(CommonClassMemberDeclaration),
    typeof(CommonTraitMemberDeclaration),
    typeof(CommonStructMemberDeclaration),
    typeof(CommonFunctionDeclaration))]
internal partial interface CommonTypeMemberDeclaration : Concrete.TypeMemberDeclaration, CommonDeclaration { }

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonClassMemberDeclaration : Concrete.ClassMemberDeclaration, CommonTypeMemberDeclaration { }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class ClassMemberDeclarationNode : Node, CommonClassMemberDeclaration
{
    public IDeclarationSyntax Syntax { get; }
    IDeclarationSyntax Concrete.Declaration.Syntax => Syntax;
    ISyntax Concrete.Code.Syntax => Syntax;

    public ClassMemberDeclarationNode(IDeclarationSyntax syntax)
    {
        Syntax = syntax;
    }
 }

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonTraitMemberDeclaration : Concrete.TraitMemberDeclaration, CommonTypeMemberDeclaration { }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class TraitMemberDeclarationNode : Node, CommonTraitMemberDeclaration
{
    public IDeclarationSyntax Syntax { get; }
    IDeclarationSyntax Concrete.Declaration.Syntax => Syntax;
    ISyntax Concrete.Code.Syntax => Syntax;

    public TraitMemberDeclarationNode(IDeclarationSyntax syntax)
    {
        Syntax = syntax;
    }
 }

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonStructMemberDeclaration : Concrete.StructMemberDeclaration, CommonTypeMemberDeclaration { }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class StructMemberDeclarationNode : Node, CommonStructMemberDeclaration
{
    public IDeclarationSyntax Syntax { get; }
    IDeclarationSyntax Concrete.Declaration.Syntax => Syntax;
    ISyntax Concrete.Code.Syntax => Syntax;

    public StructMemberDeclarationNode(IDeclarationSyntax syntax)
    {
        Syntax = syntax;
    }
 }

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonFunctionDeclaration : Concrete.FunctionDeclaration, CommonDeclaration, CommonNamespaceMemberDeclaration, CommonTypeMemberDeclaration { }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class FunctionDeclarationNode : Node, CommonFunctionDeclaration
{
    public IFunctionDeclarationSyntax Syntax { get; }
    IDeclarationSyntax Concrete.Declaration.Syntax => Syntax;
    ISyntax Concrete.Code.Syntax => Syntax;

    public FunctionDeclarationNode(IFunctionDeclarationSyntax syntax)
    {
        Syntax = syntax;
    }
 }

