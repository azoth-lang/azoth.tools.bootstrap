using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal class NamespaceDeclarationNode : CodeNode, INamespaceDeclarationNode
{
    public override INamespaceDeclarationSyntax Syntax { get; }
    public bool IsGlobalQualified => Syntax.IsGlobalQualified;
    public NamespaceName DeclaredNames => Syntax.DeclaredNames;

    private ValueAttribute<NamespaceSymbol> inheritedContainingNamespace;

    public override Symbol InheritedContainingSymbol(IChildNode caller, IChildNode child)
        => inheritedContainingNamespace.TryGetValue(out var value)
            ? value
            : inheritedContainingNamespace.GetValue(this, ContainingSymbolAttribute.NamespaceDeclarationInherited);

    public NamespaceSymbol ContainingSymbol => (NamespaceSymbol)Parent.InheritedContainingSymbol(this, this);

    private ValueAttribute<NamespaceSymbol> symbol;
    public NamespaceSymbol Symbol
        => symbol.TryGetValue(out var value) ? value : symbol.GetValue(this, SymbolAttribute.NamespaceDeclaration);

    public IFixedList<IUsingDirectiveNode> UsingDirectives { get; }
    public IFixedList<INamespaceMemberDeclarationNode> Declarations { get; }

    public NamespaceDeclarationNode(
        INamespaceDeclarationSyntax syntax,
        IEnumerable<IUsingDirectiveNode> usingDirectives,
        IEnumerable<INamespaceMemberDeclarationNode> declarations)
    {
        Syntax = syntax;
        UsingDirectives = ChildList.CreateFixed(this, usingDirectives);
        Declarations = ChildList.CreateFixed(this, declarations);
    }
}
