using System.CodeDom.Compiler;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.IST;

// ReSharper disable InconsistentNaming

[GeneratedCode("AzothCompilerCodeGen", null)]
public sealed partial class WithNamespaceSymbols
{
    [Closed(
        typeof(NamespaceMemberDeclaration),
        typeof(TypeMemberDeclaration))]
    public partial interface Declaration : Code
    {
        NamespaceOrPackageSymbol? ContainingSymbol { get; }
        new IDeclarationSyntax Syntax { get; }
        ISyntax Code.Syntax => Syntax;
    }

    public partial interface NamespaceDeclaration : NamespaceMemberDeclaration
    {
        new NamespaceOrPackageSymbol ContainingSymbol { get; }
        NamespaceOrPackageSymbol? Declaration.ContainingSymbol => ContainingSymbol;
        NamespaceOrPackageSymbol Symbol { get; }
        new INamespaceDeclarationSyntax Syntax { get; }
        IDeclarationSyntax Declaration.Syntax => Syntax;
        bool IsGlobalQualified { get; }
        NamespaceName DeclaredNames { get; }
        IFixedList<UsingDirective> UsingDirectives { get; }
        IFixedList<NamespaceMemberDeclaration> Declarations { get; }

        public static NamespaceDeclaration Create(NamespaceOrPackageSymbol containingSymbol, NamespaceOrPackageSymbol symbol, INamespaceDeclarationSyntax syntax, bool isGlobalQualified, NamespaceName declaredNames, IEnumerable<UsingDirective> usingDirectives, IEnumerable<NamespaceMemberDeclaration> declarations)
            => new NamespaceDeclarationNode(containingSymbol, symbol, syntax, isGlobalQualified, declaredNames, usingDirectives.ToFixedList(), declarations.ToFixedList());
    }

    public partial interface FunctionDeclaration : NamespaceMemberDeclaration, TypeMemberDeclaration
    {
        new NamespaceOrPackageSymbol ContainingSymbol { get; }
        NamespaceOrPackageSymbol? Declaration.ContainingSymbol => ContainingSymbol;
        new IFunctionDeclarationSyntax Syntax { get; }
        IDeclarationSyntax Declaration.Syntax => Syntax;

        public static FunctionDeclaration Create(NamespaceOrPackageSymbol containingSymbol, IFunctionDeclarationSyntax syntax)
            => new FunctionDeclarationNode(containingSymbol, syntax);
    }

    public partial interface Package : IImplementationRestricted
    {
        IPackageSyntax Syntax { get; }
        PackageSymbol Symbol { get; }
        IFixedSet<PackageReference> References { get; }
        IFixedSet<CompilationUnit> CompilationUnits { get; }
        IFixedSet<CompilationUnit> TestingCompilationUnits { get; }

        public static Package Create(IPackageSyntax syntax, PackageSymbol symbol, IEnumerable<PackageReference> references, IEnumerable<CompilationUnit> compilationUnits, IEnumerable<CompilationUnit> testingCompilationUnits)
            => new PackageNode(syntax, symbol, references.ToFixedSet(), compilationUnits.ToFixedSet(), testingCompilationUnits.ToFixedSet());
    }

    public partial interface PackageReference : IImplementationRestricted
    {
        IPackageReferenceSyntax Syntax { get; }
        IdentifierName AliasOrName { get; }
        IPackageSymbols Package { get; }
        bool IsTrusted { get; }

        public static PackageReference Create(IPackageReferenceSyntax syntax, IdentifierName aliasOrName, IPackageSymbols package, bool isTrusted)
            => new PackageReferenceNode(syntax, aliasOrName, package, isTrusted);
    }

    public partial interface CompilationUnit : Code
    {
        new ICompilationUnitSyntax Syntax { get; }
        ISyntax Code.Syntax => Syntax;
        CodeFile File { get; }
        NamespaceName ImplicitNamespaceName { get; }
        IFixedList<UsingDirective> UsingDirectives { get; }
        IFixedList<NamespaceMemberDeclaration> Declarations { get; }

        public static CompilationUnit Create(ICompilationUnitSyntax syntax, CodeFile file, NamespaceName implicitNamespaceName, IEnumerable<UsingDirective> usingDirectives, IEnumerable<NamespaceMemberDeclaration> declarations)
            => new CompilationUnitNode(syntax, file, implicitNamespaceName, usingDirectives.ToFixedList(), declarations.ToFixedList());
    }

    public partial interface UsingDirective : Code
    {
        new IUsingDirectiveSyntax Syntax { get; }
        ISyntax Code.Syntax => Syntax;
        NamespaceName Name { get; }

        public static UsingDirective Create(IUsingDirectiveSyntax syntax, NamespaceName name)
            => new UsingDirectiveNode(syntax, name);
    }

    [Closed(
        typeof(Declaration),
        typeof(CompilationUnit),
        typeof(UsingDirective))]
    public partial interface Code : IImplementationRestricted
    {
        ISyntax Syntax { get; }
    }

    [Closed(
        typeof(NamespaceDeclaration),
        typeof(FunctionDeclaration),
        typeof(TypeDeclaration))]
    public partial interface NamespaceMemberDeclaration : Declaration
    {
    }

    [Closed(
        typeof(ClassDeclaration),
        typeof(StructDeclaration),
        typeof(TraitDeclaration))]
    public partial interface TypeDeclaration : NamespaceMemberDeclaration, TypeMemberDeclaration
    {
        new ITypeDeclarationSyntax Syntax { get; }
        IDeclarationSyntax Declaration.Syntax => Syntax;
    }

    public partial interface ClassDeclaration : TypeDeclaration
    {
        new IClassDeclarationSyntax Syntax { get; }
        ITypeDeclarationSyntax TypeDeclaration.Syntax => Syntax;
        bool IsAbstract { get; }
        IFixedList<ClassMemberDeclaration> Members { get; }

        public static ClassDeclaration Create(IClassDeclarationSyntax syntax, bool isAbstract, IEnumerable<ClassMemberDeclaration> members, NamespaceOrPackageSymbol? containingSymbol)
            => new ClassDeclarationNode(syntax, isAbstract, members.ToFixedList(), containingSymbol);
    }

    public partial interface StructDeclaration : TypeDeclaration
    {
        new IStructDeclarationSyntax Syntax { get; }
        ITypeDeclarationSyntax TypeDeclaration.Syntax => Syntax;
        IFixedList<StructMemberDeclaration> Members { get; }

        public static StructDeclaration Create(IStructDeclarationSyntax syntax, IEnumerable<StructMemberDeclaration> members, NamespaceOrPackageSymbol? containingSymbol)
            => new StructDeclarationNode(syntax, members.ToFixedList(), containingSymbol);
    }

    public partial interface TraitDeclaration : TypeDeclaration
    {
        new ITraitDeclarationSyntax Syntax { get; }
        ITypeDeclarationSyntax TypeDeclaration.Syntax => Syntax;
        IFixedList<TraitMemberDeclaration> Members { get; }

        public static TraitDeclaration Create(ITraitDeclarationSyntax syntax, IEnumerable<TraitMemberDeclaration> members, NamespaceOrPackageSymbol? containingSymbol)
            => new TraitDeclarationNode(syntax, members.ToFixedList(), containingSymbol);
    }

    [Closed(
        typeof(FunctionDeclaration),
        typeof(TypeDeclaration),
        typeof(ClassMemberDeclaration),
        typeof(TraitMemberDeclaration),
        typeof(StructMemberDeclaration))]
    public partial interface TypeMemberDeclaration : Declaration
    {
    }

    public partial interface ClassMemberDeclaration : TypeMemberDeclaration
    {

        public static ClassMemberDeclaration Create(IDeclarationSyntax syntax, NamespaceOrPackageSymbol? containingSymbol)
            => new ClassMemberDeclarationNode(syntax, containingSymbol);
    }

    public partial interface TraitMemberDeclaration : TypeMemberDeclaration
    {

        public static TraitMemberDeclaration Create(IDeclarationSyntax syntax, NamespaceOrPackageSymbol? containingSymbol)
            => new TraitMemberDeclarationNode(syntax, containingSymbol);
    }

    public partial interface StructMemberDeclaration : TypeMemberDeclaration
    {

        public static StructMemberDeclaration Create(IDeclarationSyntax syntax, NamespaceOrPackageSymbol? containingSymbol)
            => new StructMemberDeclarationNode(syntax, containingSymbol);
    }

}

public sealed partial class Concrete
{
    public partial interface PackageReference : WithNamespaceSymbols.PackageReference
    {
    }

    public partial interface UsingDirective : WithNamespaceSymbols.UsingDirective
    {
    }

}

