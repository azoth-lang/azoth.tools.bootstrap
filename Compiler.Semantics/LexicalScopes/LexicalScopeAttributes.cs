using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

internal static class LexicalScopeAttributes
{
    public static PackageNameScope Package(IPackageNode node)
        => new PackageNameScope(node.Symbol, new[] { node.MainFacet.SymbolNode, node.TestingFacet.SymbolNode });

    public static LexicalScope PackageInheritedMainFacet(IPackageNode node)
        => new LexicalScope(node.PackageNameScope, node.MainFacet.SymbolNode.GlobalNamespace);

    public static LexicalScope PackageInheritedTestingFacet(IPackageNode node)
        => new LexicalScope(node.PackageNameScope,
            node.MainFacet.SymbolNode.GlobalNamespace, node.TestingFacet.SymbolNode.GlobalNamespace);

    public static LexicalScope CompilationUnit(CompilationUnitNode node)
    {
        var lexicalScope = node.ContainingLexicalScope;
        foreach (var ns in node.ImplicitNamespaceName.Segments)
            lexicalScope = lexicalScope.GetChildScope(ns)!;

        return lexicalScope;
    }
}
