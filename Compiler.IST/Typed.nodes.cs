using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.IST;

// ReSharper disable MemberHidesStaticFromOuterClass

public sealed partial class Typed
{
    private sealed class PackageNode : Node, Package
    {
        public IPackageSyntax Syntax { get; }
        public PackageSymbol Symbol { get; }
        public IFixedList<PackageReference> References { get; }
        public IFixedList<CompilationUnit> CompilationUnits { get; }
        public IFixedList<CompilationUnit> TestingCompilationUnits { get; }

        public PackageNode(IPackageSyntax syntax, PackageSymbol symbol, IFixedList<PackageReference> references, IFixedList<CompilationUnit> compilationUnits, IFixedList<CompilationUnit> testingCompilationUnits)
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
        public IFixedList<UsingDirective> UsingDirectives { get; }
        public IFixedList<NamespaceMemberDeclaration> Declarations { get; }
        public LexicalScope ContainingLexicalScope { get; }

        public NamespaceDeclarationNode(INamespaceDeclarationSyntax syntax, IFixedList<UsingDirective> usingDirectives, IFixedList<NamespaceMemberDeclaration> declarations, LexicalScope containingLexicalScope)
        {
            Syntax = syntax;
            UsingDirectives = usingDirectives;
            Declarations = declarations;
            ContainingLexicalScope = containingLexicalScope;
        }
    }

    private sealed class ClassDeclarationNode : Node, ClassDeclaration
    {
        public IClassDeclarationSyntax Syntax { get; }
        public bool IsAbstract { get; }
        public IFixedList<ClassMemberDeclaration> Members { get; }
        public LexicalScope ContainingLexicalScope { get; }

        public ClassDeclarationNode(IClassDeclarationSyntax syntax, bool isAbstract, IFixedList<ClassMemberDeclaration> members, LexicalScope containingLexicalScope)
        {
            Syntax = syntax;
            IsAbstract = isAbstract;
            Members = members;
            ContainingLexicalScope = containingLexicalScope;
        }
    }

    private sealed class StructDeclarationNode : Node, StructDeclaration
    {
        public IStructDeclarationSyntax Syntax { get; }
        public IFixedList<StructMemberDeclaration> Members { get; }
        public LexicalScope ContainingLexicalScope { get; }

        public StructDeclarationNode(IStructDeclarationSyntax syntax, IFixedList<StructMemberDeclaration> members, LexicalScope containingLexicalScope)
        {
            Syntax = syntax;
            Members = members;
            ContainingLexicalScope = containingLexicalScope;
        }
    }

    private sealed class TraitDeclarationNode : Node, TraitDeclaration
    {
        public ITraitDeclarationSyntax Syntax { get; }
        public IFixedList<TraitMemberDeclaration> Members { get; }
        public LexicalScope ContainingLexicalScope { get; }

        public TraitDeclarationNode(ITraitDeclarationSyntax syntax, IFixedList<TraitMemberDeclaration> members, LexicalScope containingLexicalScope)
        {
            Syntax = syntax;
            Members = members;
            ContainingLexicalScope = containingLexicalScope;
        }
    }

    private sealed class ClassMemberDeclarationNode : Node, ClassMemberDeclaration
    {
        public IDeclarationSyntax Syntax { get; }
        public LexicalScope ContainingLexicalScope { get; }

        public ClassMemberDeclarationNode(IDeclarationSyntax syntax, LexicalScope containingLexicalScope)
        {
            Syntax = syntax;
            ContainingLexicalScope = containingLexicalScope;
        }
    }

    private sealed class TraitMemberDeclarationNode : Node, TraitMemberDeclaration
    {
        public IDeclarationSyntax Syntax { get; }
        public LexicalScope ContainingLexicalScope { get; }

        public TraitMemberDeclarationNode(IDeclarationSyntax syntax, LexicalScope containingLexicalScope)
        {
            Syntax = syntax;
            ContainingLexicalScope = containingLexicalScope;
        }
    }

    private sealed class StructMemberDeclarationNode : Node, StructMemberDeclaration
    {
        public IDeclarationSyntax Syntax { get; }
        public LexicalScope ContainingLexicalScope { get; }

        public StructMemberDeclarationNode(IDeclarationSyntax syntax, LexicalScope containingLexicalScope)
        {
            Syntax = syntax;
            ContainingLexicalScope = containingLexicalScope;
        }
    }

    private sealed class FunctionDeclarationNode : Node, FunctionDeclaration
    {
        public IFunctionDeclarationSyntax Syntax { get; }
        public LexicalScope ContainingLexicalScope { get; }

        public FunctionDeclarationNode(IFunctionDeclarationSyntax syntax, LexicalScope containingLexicalScope)
        {
            Syntax = syntax;
            ContainingLexicalScope = containingLexicalScope;
        }
    }

}
