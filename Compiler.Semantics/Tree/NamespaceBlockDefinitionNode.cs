using System.Collections.Generic;
using System.Linq;
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
    public override IdentifierName? Name => DeclaredNames.Segments.LastOrDefault();
    public bool IsGlobalQualified => Syntax.IsGlobalQualified;
    public NamespaceName DeclaredNames => Syntax.DeclaredNames;
    public override INamespaceDefinitionNode ContainingDeclaration => (INamespaceDefinitionNode)base.ContainingDeclaration;
    private ValueAttribute<INamespaceDefinitionNode> containingNamespace;
    public INamespaceDefinitionNode ContainingNamespace
        => containingNamespace.TryGetValue(out var value) ? value
            : containingNamespace.GetValue(this, SymbolNodeAttributes.NamespaceBlockDefinition_ContainingDeclaration);
    public override NamespaceSymbol ContainingSymbol => ContainingDeclaration.Symbol;
    public override NamespaceSearchScope ContainingLexicalScope
        => (NamespaceSearchScope)base.ContainingLexicalScope;
    private ValueAttribute<INamespaceDefinitionNode> definition;
    public INamespaceDefinitionNode Definition
        => definition.TryGetValue(out var value) ? value
            : definition.GetValue(this, SymbolNodeAttributes.NamespaceBlockDefinition_Declaration);
    public override NamespaceSymbol Symbol => Definition.Symbol;

    public IFixedList<IUsingDirectiveNode> UsingDirectives { get; }
    public IFixedList<INamespaceBlockMemberDefinitionNode> Members { get; }
    private ValueAttribute<NamespaceSearchScope> lexicalScope;
    public override NamespaceSearchScope LexicalScope
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

    internal override ISymbolDeclarationNode InheritedContainingDeclaration(IChildNode caller, IChildNode child)
        => SymbolNodeAttributes.NamespaceBlockDefinition_InheritedContainingDeclaration(this);

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode caller, IChildNode child)
        => LexicalScope;
}
