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

internal class NamespaceBlockDefinitionNode : DefinitionNode, INamespaceBlockDefinitionNode
{
    public override INamespaceDefinitionSyntax Syntax { get; }
    public bool IsGlobalQualified => Syntax.IsGlobalQualified;
    public NamespaceName DeclaredNames => Syntax.DeclaredNames;

    private ValueAttribute<INamespaceDeclarationNode> containingSymbolNode;
    public override INamespaceDeclarationNode ContainingDeclaration
        => containingSymbolNode.TryGetValue(out var value) ? value
            : containingSymbolNode.GetValue(this, node
                => SymbolNodeAttributes.NamespaceBlockDefinition_ContainingDeclaration(node,
                    (INamespaceDeclarationNode)Parent.InheritedContainingDeclaration(this, this)));
    public override NamespaceSymbol ContainingSymbol => ContainingDeclaration.Symbol;

    private ValueAttribute<INamespaceDeclarationNode> declaration;
    public INamespaceDeclarationNode Declaration
        => declaration.TryGetValue(out var value) ? value
            : declaration.GetValue(this, SymbolNodeAttributes.NamespaceBlockDefinition_Declaration);
    public NamespaceSymbol Symbol => Declaration.Symbol;

    public IFixedList<IUsingDirectiveNode> UsingDirectives { get; }
    public IFixedList<INamespaceBlockMemberDefinitionNode> Members { get; }
    private ValueAttribute<LexicalScope> lexicalScope;
    public override LexicalScope LexicalScope
        => lexicalScope.TryGetValue(out var value) ? value
            : lexicalScope.GetValue(this, LexicalScopeAttributes.NamespaceBlockDefinition_LexicalScope);

    public NamespaceBlockDefinitionNode(
        INamespaceDefinitionSyntax syntax,
        IEnumerable<IUsingDirectiveNode> usingDirectives,
        IEnumerable<INamespaceBlockMemberDefinitionNode> members)
    {
        Syntax = syntax;
        UsingDirectives = ChildList.Attach(this, usingDirectives);
        Members = ChildList.Attach(this, members);
    }

    internal override IDeclarationNode InheritedContainingDeclaration(IChildNode caller, IChildNode child)
        => SymbolNodeAttributes.NamespaceBlockDefinition_InheritedContainingDeclaration(this);

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode caller, IChildNode child)
        => LexicalScope;
}
