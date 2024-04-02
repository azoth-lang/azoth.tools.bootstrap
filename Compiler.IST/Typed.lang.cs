using System.CodeDom.Compiler;
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
        CompilationUnit TestingCompilationUnits { get; }

        // public static Package Create(IPackageSyntax syntax, PackageSymbol symbol, IFixedList<PackageReference> references, IFixedList<CompilationUnit> compilationUnits, CompilationUnit testingCompilationUnits)
        //     => new PackageNode(syntax, symbol, (IFixedList<PackageReferenceNode>)references, (IFixedList<CompilationUnitNode>)compilationUnits, (CompilationUnitNode)testingCompilationUnits);

        // public static Package Create(Typed.Package package, )
        //     => new PackageNode(package.Syntax, package.Symbol, (IFixedList<PackageReferenceNode>)package.References, (IFixedList<CompilationUnitNode>)package.CompilationUnits, (CompilationUnitNode)package.TestingCompilationUnits);
    }

    public interface PackageReference : IImplementationRestricted
    {
        IdentifierName AliasOrName { get; }

        // public static PackageReference Create(IdentifierName aliasOrName)
        //     => new PackageReferenceNode(aliasOrName);

        // public static PackageReference Create(Typed.PackageReference packageReference, )
        //     => new PackageReferenceNode(packageReference.AliasOrName);
    }

    public interface CompilationUnit : IHasSyntax
    {
        new ICompilationUnitSyntax Syntax { get; }
        CodeFile File { get; }
        NamespaceName ImplicitNamespaceName { get; }
        IFixedList<UsingDirective> UsingDirectives { get; }
        IFixedList<NamespaceMemberDeclaration> Declarations { get; }

        // public static CompilationUnit Create(ICompilationUnitSyntax syntax, CodeFile file, NamespaceName implicitNamespaceName, IFixedList<UsingDirective> usingDirectives, IFixedList<NamespaceMemberDeclaration> declarations)
        //     => new CompilationUnitNode(syntax, file, implicitNamespaceName, (IFixedList<UsingDirectiveNode>)usingDirectives, (IFixedList<NamespaceMemberDeclarationNode>)declarations);

        // public static CompilationUnit Create(Typed.CompilationUnit compilationUnit, )
        //     => new CompilationUnitNode(compilationUnit.Syntax, compilationUnit.File, compilationUnit.ImplicitNamespaceName, (IFixedList<UsingDirectiveNode>)compilationUnit.UsingDirectives, (IFixedList<NamespaceMemberDeclarationNode>)compilationUnit.Declarations);
    }

    public interface UsingDirective : IHasSyntax
    {
        new IUsingDirectiveSyntax Syntax { get; }
        NamespaceName Name { get; }

        // public static UsingDirective Create(IUsingDirectiveSyntax syntax, NamespaceName name)
        //     => new UsingDirectiveNode(syntax, name);

        // public static UsingDirective Create(Typed.UsingDirective usingDirective, )
        //     => new UsingDirectiveNode(usingDirective.Syntax, usingDirective.Name);
    }

    [Closed(
        typeof(CompilationUnit),
        typeof(UsingDirective),
        typeof(Declaration))]
    public interface IHasSyntax : IImplementationRestricted
    {
        ISyntax Syntax { get; }
    }

    [Closed(
        typeof(NamespaceDeclaration),
        typeof(NamespaceMemberDeclaration),
        typeof(TypeDeclaration),
        typeof(TypeMemberDeclaration),
        typeof(FunctionDeclaration))]
    public interface Declaration : IHasSyntax
    {
        new IDeclarationSyntax Syntax { get; }
        Symbol Symbol { get; }
        IdentifierName Name { get; }
    }

    public interface NamespaceDeclaration : Declaration
    {
        new INamespaceDeclarationSyntax Syntax { get; }
        IFixedList<UsingDirective> UsingDirectives { get; }
        IFixedList<NamespaceMemberDeclaration> Declarations { get; }

        // public static NamespaceDeclaration Create(INamespaceDeclarationSyntax syntax, IFixedList<UsingDirective> usingDirectives, IFixedList<NamespaceMemberDeclaration> declarations, Symbol symbol, IdentifierName name)
        //     => new NamespaceDeclarationNode(syntax, (IFixedList<UsingDirectiveNode>)usingDirectives, (IFixedList<NamespaceMemberDeclarationNode>)declarations, symbol, name);

        // public static NamespaceDeclaration Create(Typed.NamespaceDeclaration namespaceDeclaration, )
        //     => new NamespaceDeclarationNode(namespaceDeclaration.Syntax, (IFixedList<UsingDirectiveNode>)namespaceDeclaration.UsingDirectives, (IFixedList<NamespaceMemberDeclarationNode>)namespaceDeclaration.Declarations, namespaceDeclaration.Symbol, namespaceDeclaration.Name);
    }

    [Closed(
        typeof(TypeDeclaration),
        typeof(FunctionDeclaration))]
    public interface NamespaceMemberDeclaration : Declaration
    {
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

        // public static ClassDeclaration Create(IClassDeclarationSyntax syntax, bool isAbstract, IFixedList<ClassMemberDeclaration> members, Symbol symbol, IdentifierName name)
        //     => new ClassDeclarationNode(syntax, isAbstract, (IFixedList<ClassMemberDeclarationNode>)members, symbol, name);

        // public static ClassDeclaration Create(Typed.ClassDeclaration classDeclaration, )
        //     => new ClassDeclarationNode(classDeclaration.Syntax, classDeclaration.IsAbstract, (IFixedList<ClassMemberDeclarationNode>)classDeclaration.Members, classDeclaration.Symbol, classDeclaration.Name);
    }

    public interface StructDeclaration : TypeDeclaration
    {
        new IStructDeclarationSyntax Syntax { get; }
        IFixedList<StructMemberDeclaration> Members { get; }

        // public static StructDeclaration Create(IStructDeclarationSyntax syntax, IFixedList<StructMemberDeclaration> members, Symbol symbol, IdentifierName name)
        //     => new StructDeclarationNode(syntax, (IFixedList<StructMemberDeclarationNode>)members, symbol, name);

        // public static StructDeclaration Create(Typed.StructDeclaration structDeclaration, )
        //     => new StructDeclarationNode(structDeclaration.Syntax, (IFixedList<StructMemberDeclarationNode>)structDeclaration.Members, structDeclaration.Symbol, structDeclaration.Name);
    }

    public interface TraitDeclaration : TypeDeclaration
    {
        new ITraitDeclarationSyntax Syntax { get; }
        IFixedList<TraitMemberDeclaration> Members { get; }

        // public static TraitDeclaration Create(ITraitDeclarationSyntax syntax, IFixedList<TraitMemberDeclaration> members, Symbol symbol, IdentifierName name)
        //     => new TraitDeclarationNode(syntax, (IFixedList<TraitMemberDeclarationNode>)members, symbol, name);

        // public static TraitDeclaration Create(Typed.TraitDeclaration traitDeclaration, )
        //     => new TraitDeclarationNode(traitDeclaration.Syntax, (IFixedList<TraitMemberDeclarationNode>)traitDeclaration.Members, traitDeclaration.Symbol, traitDeclaration.Name);
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

        // public static ClassMemberDeclaration Create(IDeclarationSyntax syntax, Symbol symbol, IdentifierName name)
        //     => new ClassMemberDeclarationNode(syntax, symbol, name);

        // public static ClassMemberDeclaration Create(Typed.ClassMemberDeclaration classMemberDeclaration, )
        //     => new ClassMemberDeclarationNode(classMemberDeclaration.Syntax, classMemberDeclaration.Symbol, classMemberDeclaration.Name);
    }

    public interface TraitMemberDeclaration : TypeMemberDeclaration
    {

        // public static TraitMemberDeclaration Create(IDeclarationSyntax syntax, Symbol symbol, IdentifierName name)
        //     => new TraitMemberDeclarationNode(syntax, symbol, name);

        // public static TraitMemberDeclaration Create(Typed.TraitMemberDeclaration traitMemberDeclaration, )
        //     => new TraitMemberDeclarationNode(traitMemberDeclaration.Syntax, traitMemberDeclaration.Symbol, traitMemberDeclaration.Name);
    }

    public interface StructMemberDeclaration : TypeMemberDeclaration
    {

        // public static StructMemberDeclaration Create(IDeclarationSyntax syntax, Symbol symbol, IdentifierName name)
        //     => new StructMemberDeclarationNode(syntax, symbol, name);

        // public static StructMemberDeclaration Create(Typed.StructMemberDeclaration structMemberDeclaration, )
        //     => new StructMemberDeclarationNode(structMemberDeclaration.Syntax, structMemberDeclaration.Symbol, structMemberDeclaration.Name);
    }

    public interface FunctionDeclaration : Declaration, NamespaceMemberDeclaration, TypeMemberDeclaration
    {
        new IFunctionDeclarationSyntax Syntax { get; }

        // public static FunctionDeclaration Create(IFunctionDeclarationSyntax syntax, Symbol symbol, IdentifierName name)
        //     => new FunctionDeclarationNode(syntax, symbol, name);

        // public static FunctionDeclaration Create(Typed.FunctionDeclaration functionDeclaration, )
        //     => new FunctionDeclarationNode(functionDeclaration.Syntax, functionDeclaration.Symbol, functionDeclaration.Name);
    }

}
