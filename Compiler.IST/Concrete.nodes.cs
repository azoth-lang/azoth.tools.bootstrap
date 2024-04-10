using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.IST;

// ReSharper disable MemberHidesStaticFromOuterClass

public sealed partial class Concrete
{
    private sealed class PackageNode : Node, Package
    {
        public IPackageSyntax Syntax { get; }
        public PackageSymbol Symbol { get; }
        public IFixedSet<PackageReference> References { get; }
        public IFixedSet<CompilationUnit> CompilationUnits { get; }
        public IFixedSet<CompilationUnit> TestingCompilationUnits { get; }

        public PackageNode(IPackageSyntax syntax, PackageSymbol symbol, IFixedSet<PackageReference> references, IFixedSet<CompilationUnit> compilationUnits, IFixedSet<CompilationUnit> testingCompilationUnits)
        {
            Syntax = syntax;
            Symbol = symbol;
            References = references;
            CompilationUnits = compilationUnits;
            TestingCompilationUnits = testingCompilationUnits;
        }
    }

    private sealed class PackageReferenceNode : Node, PackageReference
    {
        public IPackageReferenceSyntax Syntax { get; }
        public IdentifierName AliasOrName { get; }
        public IPackageSymbols Package { get; }
        public bool IsTrusted { get; }

        public PackageReferenceNode(IPackageReferenceSyntax syntax, IdentifierName aliasOrName, IPackageSymbols package, bool isTrusted)
        {
            Syntax = syntax;
            AliasOrName = aliasOrName;
            Package = package;
            IsTrusted = isTrusted;
        }
    }

    private sealed class CompilationUnitNode : Node, CompilationUnit
    {
        public ICompilationUnitSyntax Syntax { get; }
        public CodeFile File { get; }
        public NamespaceName ImplicitNamespaceName { get; }
        public IFixedList<UsingDirective> UsingDirectives { get; }
        public IFixedList<NamespaceMemberDeclaration> Declarations { get; }

        public CompilationUnitNode(ICompilationUnitSyntax syntax, CodeFile file, NamespaceName implicitNamespaceName, IFixedList<UsingDirective> usingDirectives, IFixedList<NamespaceMemberDeclaration> declarations)
        {
            Syntax = syntax;
            File = file;
            ImplicitNamespaceName = implicitNamespaceName;
            UsingDirectives = usingDirectives;
            Declarations = declarations;
        }
    }

    private sealed class UsingDirectiveNode : Node, UsingDirective
    {
        public IUsingDirectiveSyntax Syntax { get; }
        public NamespaceName Name { get; }

        public UsingDirectiveNode(IUsingDirectiveSyntax syntax, NamespaceName name)
        {
            Syntax = syntax;
            Name = name;
        }
    }

    private sealed class NamespaceDeclarationNode : Node, NamespaceDeclaration
    {
        public INamespaceDeclarationSyntax Syntax { get; }
        public bool IsGlobalQualified { get; }
        public NamespaceName DeclaredNames { get; }
        public IFixedList<UsingDirective> UsingDirectives { get; }
        public IFixedList<NamespaceMemberDeclaration> Declarations { get; }

        public NamespaceDeclarationNode(INamespaceDeclarationSyntax syntax, bool isGlobalQualified, NamespaceName declaredNames, IFixedList<UsingDirective> usingDirectives, IFixedList<NamespaceMemberDeclaration> declarations)
        {
            Syntax = syntax;
            IsGlobalQualified = isGlobalQualified;
            DeclaredNames = declaredNames;
            UsingDirectives = usingDirectives;
            Declarations = declarations;
        }
    }

    private sealed class ClassDeclarationNode : Node, ClassDeclaration
    {
        public IClassDeclarationSyntax Syntax { get; }
        public bool IsAbstract { get; }
        public IFixedList<ClassMemberDeclaration> Members { get; }

        public ClassDeclarationNode(IClassDeclarationSyntax syntax, bool isAbstract, IFixedList<ClassMemberDeclaration> members)
        {
            Syntax = syntax;
            IsAbstract = isAbstract;
            Members = members;
        }
    }

    private sealed class StructDeclarationNode : Node, StructDeclaration
    {
        public IStructDeclarationSyntax Syntax { get; }
        public IFixedList<StructMemberDeclaration> Members { get; }

        public StructDeclarationNode(IStructDeclarationSyntax syntax, IFixedList<StructMemberDeclaration> members)
        {
            Syntax = syntax;
            Members = members;
        }
    }

    private sealed class TraitDeclarationNode : Node, TraitDeclaration
    {
        public ITraitDeclarationSyntax Syntax { get; }
        public IFixedList<TraitMemberDeclaration> Members { get; }

        public TraitDeclarationNode(ITraitDeclarationSyntax syntax, IFixedList<TraitMemberDeclaration> members)
        {
            Syntax = syntax;
            Members = members;
        }
    }

    private sealed class ClassMemberDeclarationNode : Node, ClassMemberDeclaration
    {
        public IDeclarationSyntax Syntax { get; }

        public ClassMemberDeclarationNode(IDeclarationSyntax syntax)
        {
            Syntax = syntax;
        }
    }

    private sealed class TraitMemberDeclarationNode : Node, TraitMemberDeclaration
    {
        public IDeclarationSyntax Syntax { get; }

        public TraitMemberDeclarationNode(IDeclarationSyntax syntax)
        {
            Syntax = syntax;
        }
    }

    private sealed class StructMemberDeclarationNode : Node, StructMemberDeclaration
    {
        public IDeclarationSyntax Syntax { get; }

        public StructMemberDeclarationNode(IDeclarationSyntax syntax)
        {
            Syntax = syntax;
        }
    }

    private sealed class FunctionDeclarationNode : Node, FunctionDeclaration
    {
        public IFunctionDeclarationSyntax Syntax { get; }

        public FunctionDeclarationNode(IFunctionDeclarationSyntax syntax)
        {
            Syntax = syntax;
        }
    }

}
