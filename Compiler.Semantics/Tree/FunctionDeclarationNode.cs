using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class FunctionDeclarationNode : PackageMemberDeclarationNode, IFunctionDeclarationNode
{
    public override IFunctionDeclarationSyntax Syntax { get; }
    public StandardName Name => Syntax.Name;
    public override INamespaceSymbolNode ContainingSymbolNode => (INamespaceSymbolNode)base.ContainingSymbolNode;
    public override NamespaceSymbol ContainingSymbol => (NamespaceSymbol)base.ContainingSymbol;

    public FunctionDeclarationNode(IFunctionDeclarationSyntax syntax)
    {
        Syntax = syntax;
    }
}
