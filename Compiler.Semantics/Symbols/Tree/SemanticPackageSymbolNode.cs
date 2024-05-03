using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal sealed class SemanticPackageSymbolNode : SemanticSymbolNode, IPackageSymbolNode
{
    private IPackageNode Node { get; }
    // TODO this should be the special name "package"
    public IdentifierName AliasOrName => Symbol.Name;
    public override PackageSymbol Symbol => Node.Symbol;
    public IdentifierName Name => Symbol.Name;

    public INamespaceSymbolNode GlobalNamespace { get; }
    public INamespaceSymbolNode TestingGlobalNamespace { get; }

    public SemanticPackageSymbolNode(
        IPackageNode node,
        INamespaceSymbolNode globalNamespace,
        INamespaceSymbolNode testingGlobalNamespace)
    {
        Node = node;
        GlobalNamespace = globalNamespace;
        TestingGlobalNamespace = testingGlobalNamespace;
    }
}
