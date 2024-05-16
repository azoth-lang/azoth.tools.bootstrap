using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal class NamespaceDefinitionNode : DefinitionNode, INamespaceDefinitionNode
{
    public override INamespaceDefinitionSyntax Syntax { get; }
    public bool IsGlobalQualified => Syntax.IsGlobalQualified;
    public NamespaceName DeclaredNames => Syntax.DeclaredNames;

    private ValueAttribute<INamespaceDeclarationNode> containingSymbolNode;
    public override INamespaceDeclarationNode ContainingDeclarationNode
        => containingSymbolNode.TryGetValue(out var value) ? value
            : containingSymbolNode.GetValue(this, node
                => SymbolNodeAttributes.NamespaceDeclaration_ContainingSymbolNode(node,
                    (INamespaceDeclarationNode)Parent.InheritedContainingDeclarationNode(this, this)));
    public override NamespaceSymbol ContainingSymbol => ContainingDeclarationNode.Symbol;

    private ValueAttribute<INamespaceDeclarationNode> symbolNode;
    public override INamespaceDeclarationNode SymbolNode
        => symbolNode.TryGetValue(out var value) ? value
            : symbolNode.GetValue(this, SymbolNodeAttributes.NamespaceDeclaration_SymbolNode);

    public NamespaceSymbol Symbol => SymbolNode.Symbol;

    public IFixedList<IUsingDirectiveNode> UsingDirectives { get; }
    public IFixedList<INamespaceMemberDefinitionNode> Definitions { get; }
    private ValueAttribute<LexicalScope> lexicalScope;
    public override LexicalScope LexicalScope
        => lexicalScope.TryGetValue(out var value) ? value
            : lexicalScope.GetValue(this, LexicalScopeAttributes.NamespaceDeclaration);

    public NamespaceDefinitionNode(
        INamespaceDefinitionSyntax syntax,
        IEnumerable<IUsingDirectiveNode> usingDirectives,
        IEnumerable<INamespaceMemberDefinitionNode> definitions)
    {
        Syntax = syntax;
        UsingDirectives = ChildList.Attach(this, usingDirectives);
        Definitions = ChildList.Attach(this, definitions);
    }

    internal override IDeclarationNode InheritedContainingDeclarationNode(IChildNode caller, IChildNode child)
        => SymbolNodeAttributes.NamespaceDeclaration_InheritedContainingSymbolNode(this);

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode caller, IChildNode child)
        => LexicalScope;
}
