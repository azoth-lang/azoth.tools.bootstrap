using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Parsing.Tree;

internal class NamespaceDeclarationSyntax : NonMemberDeclarationSyntax, INamespaceDeclarationSyntax
{
    /// <summary>
    /// Whether this namespace declaration is in the global namespace, the
    /// implicit file namespace is in the global namespace. As are namespaces
    /// declared using the package qualifier `namespace ::example { }`.
    /// </summary>
    public bool IsGlobalQualified { get; }
    public NamespaceName DeclaredNames { get; }
    public new IdentifierName Name { get; }
    public NamespaceName FullName { get; }
    public new Promise<NamespaceOrPackageSymbol> Symbol { get; }
    public IFixedList<IUsingDirectiveSyntax> UsingDirectives { get; }
    public IFixedList<INonMemberDeclarationSyntax> Declarations { get; }

    public NamespaceDeclarationSyntax(
        NamespaceName containingNamespaceName,
        TextSpan span,
        CodeFile file,
        bool isGlobalQualified,
        NamespaceName declaredNames,
        TextSpan nameSpan,
        IFixedList<IUsingDirectiveSyntax> usingDirectives,
        IFixedList<INonMemberDeclarationSyntax> declarations)
        : base(containingNamespaceName, span, file, declaredNames.Segments[^1], nameSpan, new Promise<NamespaceOrPackageSymbol>())
    {
        DeclaredNames = declaredNames;
        FullName = containingNamespaceName.Qualify(declaredNames);
        Name = declaredNames.Segments[^1];
        UsingDirectives = usingDirectives;
        Declarations = declarations;
        IsGlobalQualified = isGlobalQualified;
        Symbol = (Promise<NamespaceOrPackageSymbol>)base.Symbol;
    }

    public override string ToString() => $"namespace ::{FullName} {{ … }}";
}
