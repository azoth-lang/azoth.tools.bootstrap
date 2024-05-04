using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

internal static class LexicalScopeAttributes
{
    public static PackageNameScope Package(IPackageNode node)
        => new PackageNameScope(node.Symbol, new[] { node.MainFacet.SymbolNode, node.TestingFacet.SymbolNode });

    public static LexicalScope PackageInheritedMainFacet(IPackageNode node)
        => new NestedLexicalScope(node.LexicalScope, node.MainFacet.SymbolNode.GlobalNamespace);

    public static LexicalScope PackageInheritedTestingFacet(IPackageNode node)
        => new NestedLexicalScope(node.LexicalScope,
            node.MainFacet.SymbolNode.GlobalNamespace, node.TestingFacet.SymbolNode.GlobalNamespace);
}
