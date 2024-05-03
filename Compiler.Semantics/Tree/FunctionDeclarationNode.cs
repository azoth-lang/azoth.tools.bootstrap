using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FunctionDeclarationNode : CodeNode, IFunctionDeclarationNode
{
    public override IFunctionDeclarationSyntax Syntax { get; }

    public NamespaceSymbol ContainingSymbol => (NamespaceSymbol)Parent.InheritedContainingSymbol(this, this)!;

    public FunctionDeclarationNode(IFunctionDeclarationSyntax syntax)
    {
        Syntax = syntax;
    }
}
