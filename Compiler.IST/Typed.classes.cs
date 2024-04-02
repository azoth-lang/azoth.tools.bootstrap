using System.CodeDom.Compiler;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart

// ReSharper disable once CheckNamespace
namespace Azoth.Tools.Bootstrap.Compiler.IST;

internal partial class PackageNode : Typed.Package
{
    IPackageSyntax Typed.Package.Syntax => Syntax;
    PackageSymbol Typed.Package.Symbol => Symbol;
    IFixedList<Typed.PackageReference> Typed.Package.References => References;
    IFixedList<Typed.CompilationUnit> Typed.Package.CompilationUnits => CompilationUnits;
    Typed.CompilationUnit Typed.Package.TestingCompilationUnits => TestingCompilationUnits;
}

internal partial class PackageReferenceNode : Typed.PackageReference
{
    IdentifierName Typed.PackageReference.AliasOrName => AliasOrName;
}

internal partial class CompilationUnitNode : Typed.CompilationUnit
{
    ICompilationUnitSyntax Typed.CompilationUnit.Syntax => Syntax;
    ISyntax Typed.IHasSyntax.Syntax => Syntax;
    CodeFile Typed.CompilationUnit.File => File;
    NamespaceName Typed.CompilationUnit.ImplicitNamespaceName => ImplicitNamespaceName;
    IFixedList<Typed.UsingDirective> Typed.CompilationUnit.UsingDirectives => UsingDirectives;
    IFixedList<Typed.NamespaceMemberDeclaration> Typed.CompilationUnit.Declarations => Declarations;
}

internal partial class UsingDirectiveNode : Typed.UsingDirective
{
    IUsingDirectiveSyntax Typed.UsingDirective.Syntax => Syntax;
    ISyntax Typed.IHasSyntax.Syntax => Syntax;
    NamespaceName Typed.UsingDirective.Name => Name;
}

internal partial class DeclarationNode : Typed.Declaration
{
    IDeclarationSyntax Typed.Declaration.Syntax => Syntax;
    ISyntax Typed.IHasSyntax.Syntax => Syntax;
    Symbol Typed.Declaration.Symbol => Symbol;
    IdentifierName Typed.Declaration.Name => Name;
}

internal partial class NamespaceDeclarationNode : Typed.NamespaceDeclaration
{
    INamespaceDeclarationSyntax Typed.NamespaceDeclaration.Syntax => Syntax;
    IFixedList<Typed.UsingDirective> Typed.NamespaceDeclaration.UsingDirectives => UsingDirectives;
    IFixedList<Typed.NamespaceMemberDeclaration> Typed.NamespaceDeclaration.Declarations => Declarations;
}

internal partial class NamespaceMemberDeclarationNode : Typed.NamespaceMemberDeclaration
{
}

internal partial class TypeDeclarationNode : Typed.TypeDeclaration
{
    ITypeDeclarationSyntax Typed.TypeDeclaration.Syntax => Syntax;
}

internal partial class ClassDeclarationNode : Typed.ClassDeclaration
{
    IClassDeclarationSyntax Typed.ClassDeclaration.Syntax => Syntax;
    bool Typed.ClassDeclaration.IsAbstract => IsAbstract;
    IFixedList<Typed.ClassMemberDeclaration> Typed.ClassDeclaration.Members => Members;
}

internal partial class StructDeclarationNode : Typed.StructDeclaration
{
    IStructDeclarationSyntax Typed.StructDeclaration.Syntax => Syntax;
    IFixedList<Typed.StructMemberDeclaration> Typed.StructDeclaration.Members => Members;
}

internal partial class TraitDeclarationNode : Typed.TraitDeclaration
{
    ITraitDeclarationSyntax Typed.TraitDeclaration.Syntax => Syntax;
    IFixedList<Typed.TraitMemberDeclaration> Typed.TraitDeclaration.Members => Members;
}

internal partial class TypeMemberDeclarationNode : Typed.TypeMemberDeclaration
{
}

internal partial class ClassMemberDeclarationNode : Typed.ClassMemberDeclaration
{
}

internal partial class TraitMemberDeclarationNode : Typed.TraitMemberDeclaration
{
}

internal partial class StructMemberDeclarationNode : Typed.StructMemberDeclaration
{
}

internal partial class FunctionDeclarationNode : Typed.FunctionDeclaration
{
    IFunctionDeclarationSyntax Typed.FunctionDeclaration.Syntax => Syntax;
}

