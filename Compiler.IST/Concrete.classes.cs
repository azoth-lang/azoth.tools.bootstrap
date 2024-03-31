using System.CodeDom.Compiler;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

// ReSharper disable InconsistentNaming
// ReSharper disable PartialTypeWithSinglePart

// ReSharper disable once CheckNamespace
namespace Azoth.Tools.Bootstrap.Compiler.IST;

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class PackageNode : Node, Concrete.Package
{
    public override IPackageSyntax Syntax { get; }
    public override PackageSymbol Symbol { get; }
    public override IFixedList<PackageReferenceNode> References { get; }
    IFixedList<Concrete.PackageReference> Concrete.Package.References => References;
    public override IFixedList<CompilationUnitNode> CompilationUnits { get; }
    IFixedList<Concrete.CompilationUnit> Concrete.Package.CompilationUnits => CompilationUnits;
    public override CompilationUnitNode TestingCompilationUnits { get; }
    Concrete.CompilationUnit Concrete.Package.TestingCompilationUnits => TestingCompilationUnits;

    public PackageNode(IPackageSyntax syntax, PackageSymbol symbol, IFixedList<PackageReferenceNode> references, IFixedList<CompilationUnitNode> compilationUnits, CompilationUnitNode testingCompilationUnits)
    {
        Syntax = syntax;
        Symbol = symbol;
        References = references;
        CompilationUnits = compilationUnits;
        TestingCompilationUnits = testingCompilationUnits;
    }
 }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class PackageReferenceNode : Node, Concrete.PackageReference
{
    public override IdentifierName AliasOrName { get; }

    public PackageReferenceNode(IdentifierName aliasOrName)
    {
        AliasOrName = aliasOrName;
    }
 }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class CompilationUnitNode : Node, Concrete.CompilationUnit
{
    public override ICompilationUnitSyntax Syntax { get; }
    public override CodeFile File { get; }
    public override NamespaceName ImplicitNamespaceName { get; }
    public override IFixedList<UsingDirectiveNode> UsingDirectives { get; }
    IFixedList<Concrete.UsingDirective> Concrete.CompilationUnit.UsingDirectives => UsingDirectives;
    public override IFixedList<NamespaceMemberDeclarationNode> Declarations { get; }
    IFixedList<Concrete.NamespaceMemberDeclaration> Concrete.CompilationUnit.Declarations => Declarations;

    public CompilationUnitNode(ICompilationUnitSyntax syntax, CodeFile file, NamespaceName implicitNamespaceName, IFixedList<UsingDirectiveNode> usingDirectives, IFixedList<NamespaceMemberDeclarationNode> declarations)
    {
        Syntax = syntax;
        File = file;
        ImplicitNamespaceName = implicitNamespaceName;
        UsingDirectives = usingDirectives;
        Declarations = declarations;
    }
 }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class UsingDirectiveNode : Node, Concrete.UsingDirective
{
    public override IUsingDirectiveSyntax Syntax { get; }
    public override NamespaceName Name { get; }

    public UsingDirectiveNode(IUsingDirectiveSyntax syntax, NamespaceName name)
    {
        Syntax = syntax;
        Name = name;
    }
 }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal abstract partial class DeclarationNode : Node, Concrete.Declaration
{
    public abstract IDeclarationSyntax Syntax { get; }
    public abstract Symbol Symbol { get; }
    public abstract IdentifierName Name { get; }

 }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class NamespaceDeclarationNode : DeclarationNode, Concrete.NamespaceDeclaration
{
    public override INamespaceDeclarationSyntax Syntax { get; }
    public override IFixedList<UsingDirectiveNode> UsingDirectives { get; }
    IFixedList<Concrete.UsingDirective> Concrete.NamespaceDeclaration.UsingDirectives => UsingDirectives;
    public override IFixedList<NamespaceMemberDeclarationNode> Declarations { get; }
    IFixedList<Concrete.NamespaceMemberDeclaration> Concrete.NamespaceDeclaration.Declarations => Declarations;
    public override Symbol Symbol { get; }
    public override IdentifierName Name { get; }

    public NamespaceDeclarationNode(INamespaceDeclarationSyntax syntax, IFixedList<UsingDirectiveNode> usingDirectives, IFixedList<NamespaceMemberDeclarationNode> declarations, Symbol symbol, IdentifierName name)
    {
        Syntax = syntax;
        UsingDirectives = usingDirectives;
        Declarations = declarations;
        Symbol = symbol;
        Name = name;
    }
 }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal abstract partial class NamespaceMemberDeclarationNode : DeclarationNode, Concrete.NamespaceMemberDeclaration
{
    public abstract IDeclarationSyntax Syntax { get; }
    public abstract Symbol Symbol { get; }
    public abstract IdentifierName Name { get; }

 }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal abstract partial class TypeDeclarationNode : DeclarationNode, Concrete.TypeDeclaration
{
    public abstract ITypeDeclarationSyntax Syntax { get; }
    public abstract Symbol Symbol { get; }
    public abstract IdentifierName Name { get; }
    public abstract Symbol Symbol { get; }
    public abstract IdentifierName Name { get; }
    public abstract Symbol Symbol { get; }
    public abstract IdentifierName Name { get; }

 }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class ClassDeclarationNode : TypeDeclarationNode, Concrete.ClassDeclaration
{
    public override IClassDeclarationSyntax Syntax { get; }
    public override bool IsAbstract { get; }
    public override IFixedList<ClassMemberDeclarationNode> Members { get; }
    IFixedList<Concrete.ClassMemberDeclaration> Concrete.ClassDeclaration.Members => Members;
    public override Symbol Symbol { get; }
    public override IdentifierName Name { get; }
    public override Symbol Symbol { get; }
    public override IdentifierName Name { get; }
    public override Symbol Symbol { get; }
    public override IdentifierName Name { get; }

    public ClassDeclarationNode(IClassDeclarationSyntax syntax, bool isAbstract, IFixedList<ClassMemberDeclarationNode> members, Symbol symbol, IdentifierName name, Symbol symbol, IdentifierName name, Symbol symbol, IdentifierName name)
    {
        Syntax = syntax;
        IsAbstract = isAbstract;
        Members = members;
        Symbol = symbol;
        Name = name;
        Symbol = symbol;
        Name = name;
        Symbol = symbol;
        Name = name;
    }
 }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class StructDeclarationNode : TypeDeclarationNode, Concrete.StructDeclaration
{
    public override IStructDeclarationSyntax Syntax { get; }
    public override IFixedList<StructMemberDeclarationNode> Members { get; }
    IFixedList<Concrete.StructMemberDeclaration> Concrete.StructDeclaration.Members => Members;
    public override Symbol Symbol { get; }
    public override IdentifierName Name { get; }
    public override Symbol Symbol { get; }
    public override IdentifierName Name { get; }
    public override Symbol Symbol { get; }
    public override IdentifierName Name { get; }

    public StructDeclarationNode(IStructDeclarationSyntax syntax, IFixedList<StructMemberDeclarationNode> members, Symbol symbol, IdentifierName name, Symbol symbol, IdentifierName name, Symbol symbol, IdentifierName name)
    {
        Syntax = syntax;
        Members = members;
        Symbol = symbol;
        Name = name;
        Symbol = symbol;
        Name = name;
        Symbol = symbol;
        Name = name;
    }
 }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class TraitDeclarationNode : TypeDeclarationNode, Concrete.TraitDeclaration
{
    public override ITraitDeclarationSyntax Syntax { get; }
    public override IFixedList<TraitMemberDeclarationNode> Members { get; }
    IFixedList<Concrete.TraitMemberDeclaration> Concrete.TraitDeclaration.Members => Members;
    public override Symbol Symbol { get; }
    public override IdentifierName Name { get; }
    public override Symbol Symbol { get; }
    public override IdentifierName Name { get; }
    public override Symbol Symbol { get; }
    public override IdentifierName Name { get; }

    public TraitDeclarationNode(ITraitDeclarationSyntax syntax, IFixedList<TraitMemberDeclarationNode> members, Symbol symbol, IdentifierName name, Symbol symbol, IdentifierName name, Symbol symbol, IdentifierName name)
    {
        Syntax = syntax;
        Members = members;
        Symbol = symbol;
        Name = name;
        Symbol = symbol;
        Name = name;
        Symbol = symbol;
        Name = name;
    }
 }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal abstract partial class TypeMemberDeclarationNode : DeclarationNode, Concrete.TypeMemberDeclaration
{
    public abstract IDeclarationSyntax Syntax { get; }
    public abstract Symbol Symbol { get; }
    public abstract IdentifierName Name { get; }

 }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class ClassMemberDeclarationNode : TypeMemberDeclarationNode, Concrete.ClassMemberDeclaration
{
    public override IDeclarationSyntax Syntax { get; }
    public override Symbol Symbol { get; }
    public override IdentifierName Name { get; }

    public ClassMemberDeclarationNode(IDeclarationSyntax syntax, Symbol symbol, IdentifierName name)
    {
        Syntax = syntax;
        Symbol = symbol;
        Name = name;
    }
 }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class TraitMemberDeclarationNode : TypeMemberDeclarationNode, Concrete.TraitMemberDeclaration
{
    public override IDeclarationSyntax Syntax { get; }
    public override Symbol Symbol { get; }
    public override IdentifierName Name { get; }

    public TraitMemberDeclarationNode(IDeclarationSyntax syntax, Symbol symbol, IdentifierName name)
    {
        Syntax = syntax;
        Symbol = symbol;
        Name = name;
    }
 }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class StructMemberDeclarationNode : TypeMemberDeclarationNode, Concrete.StructMemberDeclaration
{
    public override IDeclarationSyntax Syntax { get; }
    public override Symbol Symbol { get; }
    public override IdentifierName Name { get; }

    public StructMemberDeclarationNode(IDeclarationSyntax syntax, Symbol symbol, IdentifierName name)
    {
        Syntax = syntax;
        Symbol = symbol;
        Name = name;
    }
 }

[GeneratedCode("AzothCompilerCodeGen", null)]
internal sealed partial class FunctionDeclarationNode : NamespaceMemberDeclaration, TypeMemberDeclarationNode, Concrete.FunctionDeclaration
{
    public override IFunctionDeclarationSyntax Syntax { get; }

    public FunctionDeclarationNode(IFunctionDeclarationSyntax syntax)
    {
        Syntax = syntax;
    }
 }


