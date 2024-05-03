using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FunctionDeclarationNode : CodeNode, IFunctionDeclarationNode
{
    public override IFunctionDeclarationSyntax Syntax { get; }
    public StandardName Name => Syntax.Name;
    public INamespaceSymbolNode ContainingSymbolNode => (INamespaceSymbolNode)Parent.InheritedContainingSymbolNode(this, this);
    public NamespaceSymbol ContainingSymbol => ContainingSymbolNode.Symbol;

    public FunctionDeclarationNode(IFunctionDeclarationSyntax syntax)
    {
        Syntax = syntax;
    }
}
