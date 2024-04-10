using System.CodeDom.Compiler;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.IST;

// ReSharper disable InconsistentNaming

[GeneratedCode("AzothCompilerCodeGen", null)]
public sealed partial class Typed
{
    [Closed(
        typeof(Declaration))]
    public partial interface HasLexicalScope : IImplementationRestricted
    {
        LexicalScope ContainingLexicalScope { get; }
    }

    [Closed(
        typeof(NamespaceMemberDeclaration),
        typeof(TypeMemberDeclaration))]
    public partial interface Declaration : HasLexicalScope, Code
    {
        new IDeclarationSyntax Syntax { get; }
        ISyntax Code.Syntax => Syntax;
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
        typeof(TypeDeclaration),
        typeof(FunctionDeclaration))]
    public partial interface NamespaceMemberDeclaration : Declaration
    {
    }

    public partial interface NamespaceDeclaration : NamespaceMemberDeclaration
    {
        new INamespaceDeclarationSyntax Syntax { get; }
        IDeclarationSyntax Declaration.Syntax => Syntax;
        bool IsGlobalQualified { get; }
        NamespaceName DeclaredNames { get; }
        IFixedList<UsingDirective> UsingDirectives { get; }
        IFixedList<NamespaceMemberDeclaration> Declarations { get; }

        public static NamespaceDeclaration Create(INamespaceDeclarationSyntax syntax, bool isGlobalQualified, NamespaceName declaredNames, IEnumerable<UsingDirective> usingDirectives, IEnumerable<NamespaceMemberDeclaration> declarations, LexicalScope containingLexicalScope)
            => new NamespaceDeclarationNode(syntax, isGlobalQualified, declaredNames, usingDirectives.ToFixedList(), declarations.ToFixedList(), containingLexicalScope);
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

        public static ClassDeclaration Create(IClassDeclarationSyntax syntax, bool isAbstract, IEnumerable<ClassMemberDeclaration> members, LexicalScope containingLexicalScope)
            => new ClassDeclarationNode(syntax, isAbstract, members.ToFixedList(), containingLexicalScope);
    }

    public partial interface StructDeclaration : TypeDeclaration
    {
        new IStructDeclarationSyntax Syntax { get; }
        ITypeDeclarationSyntax TypeDeclaration.Syntax => Syntax;
        IFixedList<StructMemberDeclaration> Members { get; }

        public static StructDeclaration Create(IStructDeclarationSyntax syntax, IEnumerable<StructMemberDeclaration> members, LexicalScope containingLexicalScope)
            => new StructDeclarationNode(syntax, members.ToFixedList(), containingLexicalScope);
    }

    public partial interface TraitDeclaration : TypeDeclaration
    {
        new ITraitDeclarationSyntax Syntax { get; }
        ITypeDeclarationSyntax TypeDeclaration.Syntax => Syntax;
        IFixedList<TraitMemberDeclaration> Members { get; }

        public static TraitDeclaration Create(ITraitDeclarationSyntax syntax, IEnumerable<TraitMemberDeclaration> members, LexicalScope containingLexicalScope)
            => new TraitDeclarationNode(syntax, members.ToFixedList(), containingLexicalScope);
    }

    [Closed(
        typeof(TypeDeclaration),
        typeof(ClassMemberDeclaration),
        typeof(TraitMemberDeclaration),
        typeof(StructMemberDeclaration),
        typeof(FunctionDeclaration))]
    public partial interface TypeMemberDeclaration : Declaration
    {
    }

    public partial interface ClassMemberDeclaration : TypeMemberDeclaration
    {

        public static ClassMemberDeclaration Create(IDeclarationSyntax syntax, LexicalScope containingLexicalScope)
            => new ClassMemberDeclarationNode(syntax, containingLexicalScope);
    }

    public partial interface TraitMemberDeclaration : TypeMemberDeclaration
    {

        public static TraitMemberDeclaration Create(IDeclarationSyntax syntax, LexicalScope containingLexicalScope)
            => new TraitMemberDeclarationNode(syntax, containingLexicalScope);
    }

    public partial interface StructMemberDeclaration : TypeMemberDeclaration
    {

        public static StructMemberDeclaration Create(IDeclarationSyntax syntax, LexicalScope containingLexicalScope)
            => new StructMemberDeclarationNode(syntax, containingLexicalScope);
    }

    public partial interface FunctionDeclaration : NamespaceMemberDeclaration, TypeMemberDeclaration
    {
        new IFunctionDeclarationSyntax Syntax { get; }
        IDeclarationSyntax Declaration.Syntax => Syntax;

        public static FunctionDeclaration Create(IFunctionDeclarationSyntax syntax, LexicalScope containingLexicalScope)
            => new FunctionDeclarationNode(syntax, containingLexicalScope);
    }

}

public sealed partial class Scoped
{
    public partial interface HasLexicalScope : Typed.HasLexicalScope
    {
    }

    public partial interface Declaration : Typed.Declaration
    {
    }

    public partial interface Package : Typed.Package
    {
        IFixedSet<Typed.PackageReference> Typed.Package.References => References;
        IFixedSet<Typed.CompilationUnit> Typed.Package.CompilationUnits => CompilationUnits;
        IFixedSet<Typed.CompilationUnit> Typed.Package.TestingCompilationUnits => TestingCompilationUnits;
    }

    public partial interface PackageReference : Typed.PackageReference
    {
    }

    public partial interface CompilationUnit : Typed.CompilationUnit
    {
        IFixedList<Typed.UsingDirective> Typed.CompilationUnit.UsingDirectives => UsingDirectives;
        IFixedList<Typed.NamespaceMemberDeclaration> Typed.CompilationUnit.Declarations => Declarations;
    }

    public partial interface UsingDirective : Typed.UsingDirective
    {
    }

    public partial interface Code : Typed.Code
    {
    }

    public partial interface NamespaceMemberDeclaration : Typed.NamespaceMemberDeclaration
    {
    }

    public partial interface NamespaceDeclaration : Typed.NamespaceDeclaration
    {
        IFixedList<Typed.UsingDirective> Typed.NamespaceDeclaration.UsingDirectives => UsingDirectives;
        IFixedList<Typed.NamespaceMemberDeclaration> Typed.NamespaceDeclaration.Declarations => Declarations;
    }

    public partial interface TypeDeclaration : Typed.TypeDeclaration
    {
    }

    public partial interface ClassDeclaration : Typed.ClassDeclaration
    {
        IFixedList<Typed.ClassMemberDeclaration> Typed.ClassDeclaration.Members => Members;
    }

    public partial interface StructDeclaration : Typed.StructDeclaration
    {
        IFixedList<Typed.StructMemberDeclaration> Typed.StructDeclaration.Members => Members;
    }

    public partial interface TraitDeclaration : Typed.TraitDeclaration
    {
        IFixedList<Typed.TraitMemberDeclaration> Typed.TraitDeclaration.Members => Members;
    }

    public partial interface TypeMemberDeclaration : Typed.TypeMemberDeclaration
    {
    }

    public partial interface ClassMemberDeclaration : Typed.ClassMemberDeclaration
    {
    }

    public partial interface TraitMemberDeclaration : Typed.TraitMemberDeclaration
    {
    }

    public partial interface StructMemberDeclaration : Typed.StructMemberDeclaration
    {
    }

    public partial interface FunctionDeclaration : Typed.FunctionDeclaration
    {
    }

}

