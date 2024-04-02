using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.IST;

// ReSharper disable InconsistentNaming

[GeneratedCode("AzothCompilerCodeGen", null)]
public sealed class Typed
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

        public static Package Modify(Concrete.Package package)
        {
            var node = (PackageNode)package;
            return node;
        }
    }

    public interface PackageReference : IImplementationRestricted
    {
        IdentifierName AliasOrName { get; }
        AST.Package Package { get; }

        public static PackageReference Create(IdentifierName aliasOrName, AST.Package package)
            => new PackageReferenceNode(aliasOrName, package);

        public static PackageReference Modify(Concrete.PackageReference packageReference)
        {
            var node = (PackageReferenceNode)packageReference;
            return node;
        }
    }

    public interface CompilationUnit : Code
    {
        new ICompilationUnitSyntax Syntax { get; }
        CodeFile File { get; }
        NamespaceName ImplicitNamespaceName { get; }
        IFixedList<UsingDirective> UsingDirectives { get; }
        IFixedList<NamespaceMemberDeclaration> Declarations { get; }

        public static CompilationUnit Create(ICompilationUnitSyntax syntax, CodeFile file, NamespaceName implicitNamespaceName, IEnumerable<UsingDirective> usingDirectives, IEnumerable<NamespaceMemberDeclaration> declarations)
            => new CompilationUnitNode(syntax, file, implicitNamespaceName, usingDirectives.CastToFixedList<CommonUsingDirective>(), declarations.CastToFixedList<CommonNamespaceMemberDeclaration>());

        public static CompilationUnit Modify(Concrete.CompilationUnit compilationUnit)
        {
            var node = (CompilationUnitNode)compilationUnit;
            return node;
        }
    }

    public interface UsingDirective : Code
    {
        new IUsingDirectiveSyntax Syntax { get; }
        NamespaceName Name { get; }

        public static UsingDirective Create(IUsingDirectiveSyntax syntax, NamespaceName name)
            => new UsingDirectiveNode(syntax, name);

        public static UsingDirective Modify(Concrete.UsingDirective usingDirective)
        {
            var node = (UsingDirectiveNode)usingDirective;
            return node;
        }
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
        IFixedList<UsingDirective> UsingDirectives { get; }
        IFixedList<NamespaceMemberDeclaration> Declarations { get; }

        public static NamespaceDeclaration Create(INamespaceDeclarationSyntax syntax, IEnumerable<UsingDirective> usingDirectives, IEnumerable<NamespaceMemberDeclaration> declarations)
            => new NamespaceDeclarationNode(syntax, usingDirectives.CastToFixedList<CommonUsingDirective>(), declarations.CastToFixedList<CommonNamespaceMemberDeclaration>());

        public static NamespaceDeclaration Modify(Concrete.NamespaceDeclaration namespaceDeclaration)
        {
            var node = (NamespaceDeclarationNode)namespaceDeclaration;
            return node;
        }
    }

    [Closed(
        typeof(ClassDeclaration),
        typeof(StructDeclaration),
        typeof(TraitDeclaration))]
    public interface TypeDeclaration : Declaration, NamespaceMemberDeclaration, TypeMemberDeclaration
    {
        new ITypeDeclarationSyntax Syntax { get; }
    }

    public interface ClassDeclaration : TypeDeclaration
    {
        new IClassDeclarationSyntax Syntax { get; }
        bool IsAbstract { get; }
        IFixedList<ClassMemberDeclaration> Members { get; }

        public static ClassDeclaration Create(IClassDeclarationSyntax syntax, bool isAbstract, IEnumerable<ClassMemberDeclaration> members)
            => new ClassDeclarationNode(syntax, isAbstract, members.CastToFixedList<CommonClassMemberDeclaration>());

        public static ClassDeclaration Modify(Concrete.ClassDeclaration classDeclaration)
        {
            var node = (ClassDeclarationNode)classDeclaration;
            return node;
        }
    }

    public interface StructDeclaration : TypeDeclaration
    {
        new IStructDeclarationSyntax Syntax { get; }
        IFixedList<StructMemberDeclaration> Members { get; }

        public static StructDeclaration Create(IStructDeclarationSyntax syntax, IEnumerable<StructMemberDeclaration> members)
            => new StructDeclarationNode(syntax, members.CastToFixedList<CommonStructMemberDeclaration>());

        public static StructDeclaration Modify(Concrete.StructDeclaration structDeclaration)
        {
            var node = (StructDeclarationNode)structDeclaration;
            return node;
        }
    }

    public interface TraitDeclaration : TypeDeclaration
    {
        new ITraitDeclarationSyntax Syntax { get; }
        IFixedList<TraitMemberDeclaration> Members { get; }

        public static TraitDeclaration Create(ITraitDeclarationSyntax syntax, IEnumerable<TraitMemberDeclaration> members)
            => new TraitDeclarationNode(syntax, members.CastToFixedList<CommonTraitMemberDeclaration>());

        public static TraitDeclaration Modify(Concrete.TraitDeclaration traitDeclaration)
        {
            var node = (TraitDeclarationNode)traitDeclaration;
            return node;
        }
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

        public static ClassMemberDeclaration Modify(Concrete.ClassMemberDeclaration classMemberDeclaration)
        {
            var node = (ClassMemberDeclarationNode)classMemberDeclaration;
            return node;
        }
    }

    public interface TraitMemberDeclaration : TypeMemberDeclaration
    {

        public static TraitMemberDeclaration Create(IDeclarationSyntax syntax)
            => new TraitMemberDeclarationNode(syntax);

        public static TraitMemberDeclaration Modify(Concrete.TraitMemberDeclaration traitMemberDeclaration)
        {
            var node = (TraitMemberDeclarationNode)traitMemberDeclaration;
            return node;
        }
    }

    public interface StructMemberDeclaration : TypeMemberDeclaration
    {

        public static StructMemberDeclaration Create(IDeclarationSyntax syntax)
            => new StructMemberDeclarationNode(syntax);

        public static StructMemberDeclaration Modify(Concrete.StructMemberDeclaration structMemberDeclaration)
        {
            var node = (StructMemberDeclarationNode)structMemberDeclaration;
            return node;
        }
    }

    public interface FunctionDeclaration : Declaration, NamespaceMemberDeclaration, TypeMemberDeclaration
    {
        new IFunctionDeclarationSyntax Syntax { get; }

        public static FunctionDeclaration Create(IFunctionDeclarationSyntax syntax)
            => new FunctionDeclarationNode(syntax);

        public static FunctionDeclaration Modify(Concrete.FunctionDeclaration functionDeclaration)
        {
            var node = (FunctionDeclarationNode)functionDeclaration;
            return node;
        }
    }

}
