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
public sealed class Concrete
{
    public interface Package : IImplementationRestricted
    {
        IPackageSyntax Syntax { get; }
        PackageSymbol Symbol { get; }
        IFixedList<PackageReference> References { get; }
        IFixedList<CompilationUnit> CompilationUnits { get; }
        IFixedList<CompilationUnit> TestingCompilationUnits { get; }

        public static Package Create(IPackageSyntax syntax, PackageSymbol symbol, IEnumerable<PackageReference> references, IEnumerable<CompilationUnit> compilationUnits, IEnumerable<CompilationUnit> testingCompilationUnits)
            => new PackageNode(syntax, symbol, references.CastToFixedList<CommonPackageReference>(), compilationUnits.CastToFixedList<CommonCompilationUnit>(), testingCompilationUnits.CastToFixedList<CommonCompilationUnit>());
    }

    public interface PackageReference : IImplementationRestricted
    {
        IdentifierName AliasOrName { get; }
        AST.Package Package { get; }

        public static PackageReference Create(IdentifierName aliasOrName, AST.Package package)
            => new PackageReferenceNode(aliasOrName, package);
    }

    public interface CompilationUnit : Code
    {
        new ICompilationUnitSyntax Syntax { get; }
        ISyntax Code.Syntax => Syntax;
        CodeFile File { get; }
        NamespaceName ImplicitNamespaceName { get; }
        IFixedList<UsingDirective> UsingDirectives { get; }
        IFixedList<NamespaceMemberDeclaration> Declarations { get; }

        public static CompilationUnit Create(ICompilationUnitSyntax syntax, CodeFile file, NamespaceName implicitNamespaceName, IEnumerable<UsingDirective> usingDirectives, IEnumerable<NamespaceMemberDeclaration> declarations)
            => new CompilationUnitNode(syntax, file, implicitNamespaceName, usingDirectives.CastToFixedList<CommonUsingDirective>(), declarations.CastToFixedList<CommonNamespaceMemberDeclaration>());
    }

    public interface UsingDirective : Code
    {
        new IUsingDirectiveSyntax Syntax { get; }
        ISyntax Code.Syntax => Syntax;
        NamespaceName Name { get; }

        public static UsingDirective Create(IUsingDirectiveSyntax syntax, NamespaceName name)
            => new UsingDirectiveNode(syntax, name);
    }

    [Closed(
        typeof(CompilationUnit),
        typeof(UsingDirective),
        typeof(Declaration))]
    public interface Code : IImplementationRestricted
    {
        ISyntax Syntax { get; }
    }

    [Closed(
        typeof(NamespaceMemberDeclaration),
        typeof(NamespaceDeclaration),
        typeof(TypeDeclaration),
        typeof(TypeMemberDeclaration),
        typeof(FunctionDeclaration))]
    public interface Declaration : Code
    {
        new IDeclarationSyntax Syntax { get; }
        ISyntax Code.Syntax => Syntax;
    }

    [Closed(
        typeof(NamespaceDeclaration),
        typeof(TypeDeclaration),
        typeof(FunctionDeclaration))]
    public interface NamespaceMemberDeclaration : Declaration
    {
    }

    public interface NamespaceDeclaration : Declaration, NamespaceMemberDeclaration
    {
        new INamespaceDeclarationSyntax Syntax { get; }
        IDeclarationSyntax Declaration.Syntax => Syntax;
        IFixedList<UsingDirective> UsingDirectives { get; }
        IFixedList<NamespaceMemberDeclaration> Declarations { get; }

        public static NamespaceDeclaration Create(INamespaceDeclarationSyntax syntax, IEnumerable<UsingDirective> usingDirectives, IEnumerable<NamespaceMemberDeclaration> declarations)
            => new NamespaceDeclarationNode(syntax, usingDirectives.CastToFixedList<CommonUsingDirective>(), declarations.CastToFixedList<CommonNamespaceMemberDeclaration>());
    }

    [Closed(
        typeof(ClassDeclaration),
        typeof(StructDeclaration),
        typeof(TraitDeclaration))]
    public interface TypeDeclaration : Declaration, NamespaceMemberDeclaration, TypeMemberDeclaration
    {
        new ITypeDeclarationSyntax Syntax { get; }
        IDeclarationSyntax Declaration.Syntax => Syntax;
    }

    public interface ClassDeclaration : TypeDeclaration
    {
        new IClassDeclarationSyntax Syntax { get; }
        ITypeDeclarationSyntax TypeDeclaration.Syntax => Syntax;
        bool IsAbstract { get; }
        IFixedList<ClassMemberDeclaration> Members { get; }

        public static ClassDeclaration Create(IClassDeclarationSyntax syntax, bool isAbstract, IEnumerable<ClassMemberDeclaration> members)
            => new ClassDeclarationNode(syntax, isAbstract, members.CastToFixedList<CommonClassMemberDeclaration>());
    }

    public interface StructDeclaration : TypeDeclaration
    {
        new IStructDeclarationSyntax Syntax { get; }
        ITypeDeclarationSyntax TypeDeclaration.Syntax => Syntax;
        IFixedList<StructMemberDeclaration> Members { get; }

        public static StructDeclaration Create(IStructDeclarationSyntax syntax, IEnumerable<StructMemberDeclaration> members)
            => new StructDeclarationNode(syntax, members.CastToFixedList<CommonStructMemberDeclaration>());
    }

    public interface TraitDeclaration : TypeDeclaration
    {
        new ITraitDeclarationSyntax Syntax { get; }
        ITypeDeclarationSyntax TypeDeclaration.Syntax => Syntax;
        IFixedList<TraitMemberDeclaration> Members { get; }

        public static TraitDeclaration Create(ITraitDeclarationSyntax syntax, IEnumerable<TraitMemberDeclaration> members)
            => new TraitDeclarationNode(syntax, members.CastToFixedList<CommonTraitMemberDeclaration>());
    }

    [Closed(
        typeof(TypeDeclaration),
        typeof(ClassMemberDeclaration),
        typeof(TraitMemberDeclaration),
        typeof(StructMemberDeclaration),
        typeof(FunctionDeclaration))]
    public interface TypeMemberDeclaration : Declaration
    {
    }

    public interface ClassMemberDeclaration : TypeMemberDeclaration
    {

        public static ClassMemberDeclaration Create(IDeclarationSyntax syntax)
            => new ClassMemberDeclarationNode(syntax);
    }

    public interface TraitMemberDeclaration : TypeMemberDeclaration
    {

        public static TraitMemberDeclaration Create(IDeclarationSyntax syntax)
            => new TraitMemberDeclarationNode(syntax);
    }

    public interface StructMemberDeclaration : TypeMemberDeclaration
    {

        public static StructMemberDeclaration Create(IDeclarationSyntax syntax)
            => new StructMemberDeclarationNode(syntax);
    }

    public interface FunctionDeclaration : Declaration, NamespaceMemberDeclaration, TypeMemberDeclaration
    {
        new IFunctionDeclarationSyntax Syntax { get; }
        IDeclarationSyntax Declaration.Syntax => Syntax;

        public static FunctionDeclaration Create(IFunctionDeclarationSyntax syntax)
            => new FunctionDeclarationNode(syntax);
    }

}
