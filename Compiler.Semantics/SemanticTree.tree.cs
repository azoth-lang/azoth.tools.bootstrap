using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

// ReSharper disable PartialTypeWithSinglePart

[Closed(
    typeof(Package),
    typeof(PackageReference),
    typeof(Code),
    typeof(NamespaceMemberDeclaration))]
public partial interface SemanticNode
{
    ISyntax Syntax { get; }
}

public partial interface Package : SemanticNode
{
    new IPackageSyntax Syntax { get; }
    IdentifierName Name { get; }
    PackageSymbol Symbol { get; }
    IFixedSet<PackageReference> References { get; }
    IFixedSet<CompilationUnit> CompilationUnits { get; }
    IFixedSet<CompilationUnit> TestingCompilationUnits { get; }
}

public partial interface PackageReference : SemanticNode
{
    new IPackageReferenceSyntax Syntax { get; }
    IdentifierName AliasOrName { get; }
    IPackageSymbols Package { get; }
    bool IsTrusted { get; }
}

[Closed(
    typeof(CompilationUnit),
    typeof(Declaration),
    typeof(UsingDirective))]
public partial interface Code : SemanticNode
{
    new IConcreteSyntax Syntax { get; }
}

public partial interface CompilationUnit : Code
{
    new ICompilationUnitSyntax Syntax { get; }
    CodeFile File { get; }
    NamespaceName ImplicitNamespaceName { get; }
    IFixedList<UsingDirective> UsingDirectives { get; }
    IFixedList<NamespaceMemberDeclaration> Declarations { get; }
}

[Closed(
    typeof(NamespaceMemberDeclaration))]
public partial interface Declaration : Code
{
    new IDeclarationSyntax Syntax { get; }
}

public partial interface NamespaceDeclaration : NamespaceMemberDeclaration
{
    new INamespaceDeclarationSyntax Syntax { get; }
    bool IsGlobalQualified { get; }
    NamespaceName DeclaredNames { get; }
    IFixedList<UsingDirective> UsingDirectives { get; }
    IFixedList<NamespaceMemberDeclaration> Declarations { get; }
}

[Closed(
    typeof(NamespaceDeclaration))]
public partial interface NamespaceMemberDeclaration : SemanticNode, Declaration
{
}

public partial interface UsingDirective : Code
{
    new IUsingDirectiveSyntax Syntax { get; }
    NamespaceName Name { get; }
}
