using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.IST;

// ReSharper disable MemberHidesStaticFromOuterClass

public sealed partial class WithNamespaceSymbols
{
    private sealed class NamespaceDeclarationNode : Node, NamespaceDeclaration
    {
        public NamespaceOrPackageSymbol ContainingSymbol { get; }
        public NamespaceOrPackageSymbol Symbol { get; }
        public INamespaceDeclarationSyntax Syntax { get; }
        public bool IsGlobalQualified { get; }
        public NamespaceName DeclaredNames { get; }
        public IFixedList<UsingDirective> UsingDirectives { get; }
        public IFixedList<NamespaceMemberDeclaration> Declarations { get; }

        public NamespaceDeclarationNode(NamespaceOrPackageSymbol containingSymbol, NamespaceOrPackageSymbol symbol, INamespaceDeclarationSyntax syntax, bool isGlobalQualified, NamespaceName declaredNames, IFixedList<UsingDirective> usingDirectives, IFixedList<NamespaceMemberDeclaration> declarations)
        {
            ContainingSymbol = containingSymbol;
            Symbol = symbol;
            Syntax = syntax;
            IsGlobalQualified = isGlobalQualified;
            DeclaredNames = declaredNames;
            UsingDirectives = usingDirectives;
            Declarations = declarations;
        }
    }

    private sealed class FunctionDeclarationNode : Node, FunctionDeclaration
    {
        public NamespaceOrPackageSymbol ContainingSymbol { get; }
        public IFunctionDeclarationSyntax Syntax { get; }

        public FunctionDeclarationNode(NamespaceOrPackageSymbol containingSymbol, IFunctionDeclarationSyntax syntax)
        {
            ContainingSymbol = containingSymbol;
            Syntax = syntax;
        }
    }

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

    private sealed class ClassDeclarationNode : Node, ClassDeclaration
    {
        public IClassDeclarationSyntax Syntax { get; }
        public bool IsAbstract { get; }
        public IFixedList<ClassMemberDeclaration> Members { get; }
        public NamespaceOrPackageSymbol? ContainingSymbol { get; }

        public ClassDeclarationNode(IClassDeclarationSyntax syntax, bool isAbstract, IFixedList<ClassMemberDeclaration> members, NamespaceOrPackageSymbol? containingSymbol)
        {
            Syntax = syntax;
            IsAbstract = isAbstract;
            Members = members;
            ContainingSymbol = containingSymbol;
        }
    }

    private sealed class StructDeclarationNode : Node, StructDeclaration
    {
        public IStructDeclarationSyntax Syntax { get; }
        public IFixedList<StructMemberDeclaration> Members { get; }
        public NamespaceOrPackageSymbol? ContainingSymbol { get; }

        public StructDeclarationNode(IStructDeclarationSyntax syntax, IFixedList<StructMemberDeclaration> members, NamespaceOrPackageSymbol? containingSymbol)
        {
            Syntax = syntax;
            Members = members;
            ContainingSymbol = containingSymbol;
        }
    }

    private sealed class TraitDeclarationNode : Node, TraitDeclaration
    {
        public ITraitDeclarationSyntax Syntax { get; }
        public IFixedList<TraitMemberDeclaration> Members { get; }
        public NamespaceOrPackageSymbol? ContainingSymbol { get; }

        public TraitDeclarationNode(ITraitDeclarationSyntax syntax, IFixedList<TraitMemberDeclaration> members, NamespaceOrPackageSymbol? containingSymbol)
        {
            Syntax = syntax;
            Members = members;
            ContainingSymbol = containingSymbol;
        }
    }

    private sealed class ClassMemberDeclarationNode : Node, ClassMemberDeclaration
    {
        public IDeclarationSyntax Syntax { get; }
        public NamespaceOrPackageSymbol? ContainingSymbol { get; }

        public ClassMemberDeclarationNode(IDeclarationSyntax syntax, NamespaceOrPackageSymbol? containingSymbol)
        {
            Syntax = syntax;
            ContainingSymbol = containingSymbol;
        }
    }

    private sealed class TraitMemberDeclarationNode : Node, TraitMemberDeclaration
    {
        public IDeclarationSyntax Syntax { get; }
        public NamespaceOrPackageSymbol? ContainingSymbol { get; }

        public TraitMemberDeclarationNode(IDeclarationSyntax syntax, NamespaceOrPackageSymbol? containingSymbol)
        {
            Syntax = syntax;
            ContainingSymbol = containingSymbol;
        }
    }

    private sealed class StructMemberDeclarationNode : Node, StructMemberDeclaration
    {
        public IDeclarationSyntax Syntax { get; }
        public NamespaceOrPackageSymbol? ContainingSymbol { get; }

        public StructMemberDeclarationNode(IDeclarationSyntax syntax, NamespaceOrPackageSymbol? containingSymbol)
        {
            Syntax = syntax;
            ContainingSymbol = containingSymbol;
        }
    }

}
