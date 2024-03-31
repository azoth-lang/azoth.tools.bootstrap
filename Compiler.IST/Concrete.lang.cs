using System.CodeDom.Compiler;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.IST;

// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart

[GeneratedCode("AzothCompilerCodeGen", null)]
public sealed class Concrete
{
    public interface Package : IImplementationRestricted
    {
        IPackageSyntax Syntax { get; }
        PackageSymbol Symbol { get; }
        IFixedList<PackageReferenece> References { get; }
        IFixedList<CompilationUnit> CompilationUnits { get; }
        CompilationUnit TestingCompilationUnits { get; }

        // public static Package Create(IPackageSyntax syntax, PackageSymbol symbol, IFixedList<PackageReferenece> references, IFixedList<CompilationUnit> compilationUnits, CompilationUnit testingCompilationUnits)
        //     => new Package_Concrete(syntax, symbol, (IFixedList<PackageReferenece_Concrete>)references, (IFixedList<CompilationUnit_Concrete>)compilationUnits, (CompilationUnit_Concrete)testingCompilationUnits);
    }

    public interface PackageReferenece : IImplementationRestricted
    {
        IdentifierName AliasOrName { get; }

        // public static PackageReferenece Create(IdentifierName aliasOrName)
        //     => new PackageReferenece_Concrete(aliasOrName);
    }

    public interface CompilationUnit : IHasSyntax
    {
        new ICompilationUnitSyntax Syntax { get; }
        CodeFile File { get; }
        NamespaceName ImplicitNamespaceName { get; }
        IFixedList<UsingDirective> UsingDirectives { get; }
        IFixedList<NamespaceMemberDeclaration> Declarations { get; }

        // public static CompilationUnit Create(ICompilationUnitSyntax syntax, CodeFile file, NamespaceName implicitNamespaceName, IFixedList<UsingDirective> usingDirectives, IFixedList<NamespaceMemberDeclaration> declarations)
        //     => new CompilationUnit_Concrete(syntax, file, implicitNamespaceName, (IFixedList<UsingDirective_Concrete>)usingDirectives, (IFixedList<NamespaceMemberDeclaration_Concrete>)declarations);
    }

    public interface UsingDirective : IHasSyntax
    {
        new IUsingDirectiveSyntax Syntax { get; }
        NamespaceName Name { get; }

        // public static UsingDirective Create(IUsingDirectiveSyntax syntax, NamespaceName name)
        //     => new UsingDirective_Concrete(syntax, name);
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
        typeof(TypeMemberDeclaration))]
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
        //     => new NamespaceDeclaration_Concrete(syntax, (IFixedList<UsingDirective_Concrete>)usingDirectives, (IFixedList<NamespaceMemberDeclaration_Concrete>)declarations, symbol, name);
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
    public interface TypeDeclaration : NamespaceMemberDeclaration, TypeMemberDeclaration
    {
        new ITypeDeclarationSyntax Syntax { get; }
    }

    public interface ClassDeclaration : TypeDeclaration
    {
        new IClassDeclarationSyntax Syntax { get; }
        bool IsAbstract { get; }
        IFixedList<ClassMemberDeclaration> Members { get; }

        // public static ClassDeclaration Create(IClassDeclarationSyntax syntax, bool isAbstract, IFixedList<ClassMemberDeclaration> members, Symbol symbol, IdentifierName name, Symbol symbol, IdentifierName name)
        //     => new ClassDeclaration_Concrete(syntax, isAbstract, (IFixedList<ClassMemberDeclaration_Concrete>)members, symbol, name, symbol, name);
    }

    public interface StructDeclaration : TypeDeclaration
    {
        new IStructDeclarationSyntax Syntax { get; }
        IFixedList<StructMemberDeclaration> Members { get; }

        // public static StructDeclaration Create(IStructDeclarationSyntax syntax, IFixedList<StructMemberDeclaration> members, Symbol symbol, IdentifierName name, Symbol symbol, IdentifierName name)
        //     => new StructDeclaration_Concrete(syntax, (IFixedList<StructMemberDeclaration_Concrete>)members, symbol, name, symbol, name);
    }

    public interface TraitDeclaration : TypeDeclaration
    {
        new ITraitDeclarationSyntax Syntax { get; }
        IFixedList<TraitMemberDeclaration> Members { get; }

        // public static TraitDeclaration Create(ITraitDeclarationSyntax syntax, IFixedList<TraitMemberDeclaration> members, Symbol symbol, IdentifierName name, Symbol symbol, IdentifierName name)
        //     => new TraitDeclaration_Concrete(syntax, (IFixedList<TraitMemberDeclaration_Concrete>)members, symbol, name, symbol, name);
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
        //     => new ClassMemberDeclaration_Concrete(syntax, symbol, name);
    }

    public interface TraitMemberDeclaration : TypeMemberDeclaration
    {

        // public static TraitMemberDeclaration Create(IDeclarationSyntax syntax, Symbol symbol, IdentifierName name)
        //     => new TraitMemberDeclaration_Concrete(syntax, symbol, name);
    }

    public interface StructMemberDeclaration : TypeMemberDeclaration
    {

        // public static StructMemberDeclaration Create(IDeclarationSyntax syntax, Symbol symbol, IdentifierName name)
        //     => new StructMemberDeclaration_Concrete(syntax, symbol, name);
    }

    public interface FunctionDeclaration : NamespaceMemberDeclaration, TypeMemberDeclaration
    {
        new IFunctionDeclarationSyntax Syntax { get; }

        // public static FunctionDeclaration Create(IFunctionDeclarationSyntax syntax, Symbol symbol, IdentifierName name, Symbol symbol, IdentifierName name)
        //     => new FunctionDeclaration_Concrete(syntax, symbol, name, symbol, name);
    }

}
