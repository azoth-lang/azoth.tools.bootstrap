using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal class NamespaceDeclarationNode : DeclarationNode, INamespaceDeclarationNode
{
    public override INamespaceDeclarationSyntax Syntax { get; }
    public bool IsGlobalQualified => Syntax.IsGlobalQualified;
    public NamespaceName DeclaredNames => Syntax.DeclaredNames;

    private ValueAttribute<INamespaceSymbolNode> containingSymbolNode;
    public override INamespaceSymbolNode ContainingSymbolNode
        => containingSymbolNode.TryGetValue(out var value) ? value
            : containingSymbolNode.GetValue(this, node
                => SymbolNodeAttribute.NamespaceDeclarationContainingSymbolNode(node,
                    (INamespaceSymbolNode)Parent.InheritedContainingSymbolNode(this, this)));
    public override NamespaceSymbol ContainingSymbol => ContainingSymbolNode.Symbol;
    private ValueAttribute<INamespaceSymbolNode> inheritedContainingSymbolNode;

    private ValueAttribute<INamespaceSymbolNode> symbolNode;
    public INamespaceSymbolNode SymbolNode
        => symbolNode.TryGetValue(out var value) ? value
            : symbolNode.GetValue(this, SymbolNodeAttribute.NamespaceDeclaration);

    public NamespaceSymbol Symbol => SymbolNode.Symbol;

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

    internal override ISymbolNode InheritedContainingSymbolNode(IChildNode caller, IChildNode child)
        => inheritedContainingSymbolNode.TryGetValue(out var value) ? value
            : inheritedContainingSymbolNode.GetValue(this, SymbolNodeAttribute.NamespaceDeclarationInherited);

}
