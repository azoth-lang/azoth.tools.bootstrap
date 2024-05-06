using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FunctionDeclarationNode : PackageMemberDeclarationNode, IFunctionDeclarationNode
{
    public override IFunctionDeclarationSyntax Syntax { get; }
    public StandardName Name => Syntax.Name;
    public override INamespaceSymbolNode ContainingSymbolNode => (INamespaceSymbolNode)base.ContainingSymbolNode;
    public override NamespaceSymbol ContainingSymbol => (NamespaceSymbol)base.ContainingSymbol;
    private ValueAttribute<LexicalScope> lexicalScope;
    public override LexicalScope LexicalScope
        => lexicalScope.TryGetValue(out var value) ? value
            : lexicalScope.GetValue(this, LexicalScopeAttributes.FunctionDeclaration);

    public FunctionDeclarationNode(IFunctionDeclarationSyntax syntax)
    {
        Syntax = syntax;
    }
}
