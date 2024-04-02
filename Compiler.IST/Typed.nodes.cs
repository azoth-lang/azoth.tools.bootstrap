using System.CodeDom.Compiler;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart

// ReSharper disable once CheckNamespace
namespace Azoth.Tools.Bootstrap.Compiler.IST;

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonPackage : Typed.Package { }

internal partial class PackageNode
{
    IPackageSyntax Typed.Package.Syntax => Syntax;
    PackageSymbol Typed.Package.Symbol => Symbol;
    IFixedList<Typed.PackageReference> Typed.Package.References => References;
    IFixedList<Typed.CompilationUnit> Typed.Package.CompilationUnits => CompilationUnits;
    IFixedList<Typed.CompilationUnit> Typed.Package.TestingCompilationUnits => TestingCompilationUnits;
}

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonPackageReference : Typed.PackageReference { }

internal partial class PackageReferenceNode
{
    IdentifierName Typed.PackageReference.AliasOrName => AliasOrName;
    AST.Package Typed.PackageReference.Package => Package;
}

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonCompilationUnit : Typed.CompilationUnit { }

internal partial class CompilationUnitNode
{
    ICompilationUnitSyntax Typed.CompilationUnit.Syntax => Syntax;
    ISyntax Typed.Code.Syntax => Syntax;
    CodeFile Typed.CompilationUnit.File => File;
    NamespaceName Typed.CompilationUnit.ImplicitNamespaceName => ImplicitNamespaceName;
    IFixedList<Typed.UsingDirective> Typed.CompilationUnit.UsingDirectives => UsingDirectives;
    IFixedList<Typed.NamespaceMemberDeclaration> Typed.CompilationUnit.Declarations => Declarations;
}

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonUsingDirective : Typed.UsingDirective { }

internal partial class UsingDirectiveNode
{
    IUsingDirectiveSyntax Typed.UsingDirective.Syntax => Syntax;
    ISyntax Typed.Code.Syntax => Syntax;
    NamespaceName Typed.UsingDirective.Name => Name;
}

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonCode : Typed.Code { }

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonDeclaration : Typed.Declaration { }

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonNamespaceMemberDeclaration : Typed.NamespaceMemberDeclaration { }

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonNamespaceDeclaration : Typed.NamespaceDeclaration { }

internal partial class NamespaceDeclarationNode
{
    INamespaceDeclarationSyntax Typed.NamespaceDeclaration.Syntax => Syntax;
    IDeclarationSyntax Typed.Declaration.Syntax => Syntax;
    ISyntax Typed.Code.Syntax => Syntax;
    IFixedList<Typed.UsingDirective> Typed.NamespaceDeclaration.UsingDirectives => UsingDirectives;
    IFixedList<Typed.NamespaceMemberDeclaration> Typed.NamespaceDeclaration.Declarations => Declarations;
}

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonTypeDeclaration : Typed.TypeDeclaration { }

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonClassDeclaration : Typed.ClassDeclaration { }

internal partial class ClassDeclarationNode
{
    IClassDeclarationSyntax Typed.ClassDeclaration.Syntax => Syntax;
    ITypeDeclarationSyntax Typed.TypeDeclaration.Syntax => Syntax;
    IDeclarationSyntax Typed.Declaration.Syntax => Syntax;
    ISyntax Typed.Code.Syntax => Syntax;
    bool Typed.ClassDeclaration.IsAbstract => IsAbstract;
    IFixedList<Typed.ClassMemberDeclaration> Typed.ClassDeclaration.Members => Members;
}

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonStructDeclaration : Typed.StructDeclaration { }

internal partial class StructDeclarationNode
{
    IStructDeclarationSyntax Typed.StructDeclaration.Syntax => Syntax;
    ITypeDeclarationSyntax Typed.TypeDeclaration.Syntax => Syntax;
    IDeclarationSyntax Typed.Declaration.Syntax => Syntax;
    ISyntax Typed.Code.Syntax => Syntax;
    IFixedList<Typed.StructMemberDeclaration> Typed.StructDeclaration.Members => Members;
}

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonTraitDeclaration : Typed.TraitDeclaration { }

internal partial class TraitDeclarationNode
{
    ITraitDeclarationSyntax Typed.TraitDeclaration.Syntax => Syntax;
    ITypeDeclarationSyntax Typed.TypeDeclaration.Syntax => Syntax;
    IDeclarationSyntax Typed.Declaration.Syntax => Syntax;
    ISyntax Typed.Code.Syntax => Syntax;
    IFixedList<Typed.TraitMemberDeclaration> Typed.TraitDeclaration.Members => Members;
}

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonTypeMemberDeclaration : Typed.TypeMemberDeclaration { }

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonClassMemberDeclaration : Typed.ClassMemberDeclaration { }

internal partial class ClassMemberDeclarationNode
{
    IDeclarationSyntax Typed.Declaration.Syntax => Syntax;
    ISyntax Typed.Code.Syntax => Syntax;
}

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonTraitMemberDeclaration : Typed.TraitMemberDeclaration { }

internal partial class TraitMemberDeclarationNode
{
    IDeclarationSyntax Typed.Declaration.Syntax => Syntax;
    ISyntax Typed.Code.Syntax => Syntax;
}

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonStructMemberDeclaration : Typed.StructMemberDeclaration { }

internal partial class StructMemberDeclarationNode
{
    IDeclarationSyntax Typed.Declaration.Syntax => Syntax;
    ISyntax Typed.Code.Syntax => Syntax;
}

// ReSharper disable once PossibleInterfaceMemberAmbiguity
internal partial interface CommonFunctionDeclaration : Typed.FunctionDeclaration { }

internal partial class FunctionDeclarationNode
{
    IFunctionDeclarationSyntax Typed.FunctionDeclaration.Syntax => Syntax;
    IDeclarationSyntax Typed.Declaration.Syntax => Syntax;
    ISyntax Typed.Code.Syntax => Syntax;
}

