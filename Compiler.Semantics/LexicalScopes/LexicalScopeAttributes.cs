using System;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

internal static class LexicalScopeAttributes
{
    public static PackageNameScope PackageInheritedMainFacet(IPackageNode node)
        => new PackageNameScope(new[] { node.MainFacet.SymbolNode }, node.References.Select(r => r.SymbolNode.MainFacet));

    public static PackageNameScope PackageInheritedTestingFacet(IPackageNode node)
        => new PackageNameScope(new[] { node.MainFacet.SymbolNode, node.TestingFacet.SymbolNode },
            node.References.Select(r => r.SymbolNode.MainFacet).Concat(node.References.Select(r => r.SymbolNode.TestingFacet)));

    public static NamespaceScope CompilationUnit(CompilationUnitNode node)
    {
        var containingLexicalScope = node.ContainingLexicalScope;
        return BuildNamespaceScope(containingLexicalScope, node.ImplicitNamespaceName, node.UsingDirectives);
    }

    private static NamespaceScope BuildNamespaceScope(
        NamespaceScope containingLexicalScope,
        NamespaceName namespaceName,
        IFixedList<IUsingDirectiveNode> usingDirectives)
    {
        var lexicalScope = containingLexicalScope;
        foreach (var ns in namespaceName.Segments)
            lexicalScope = lexicalScope.GetChildScope(ns)!;

        lexicalScope = BuildUsingDirectivesScope(lexicalScope, usingDirectives);
        return lexicalScope;
    }

    private static NamespaceScope BuildUsingDirectivesScope(
        NamespaceScope containingScope,
        IFixedList<IUsingDirectiveNode> usingDirectives)
    {
        if (!usingDirectives.Any()) return containingScope;

        //var globalScope = containingScope.GlobalScope;
        //foreach (var usingDirective in usingDirectives)
        //{
        //    usingDirective.Name
        //}
        //var importedSymbols = new Dictionary<TypeName, HashSet<IPromise<Symbol>>>();
        //foreach (var usingDirective in usingDirectives)
        //{
        //    if (!namespaces.TryGetValue(usingDirective.Name, out var ns))
        //    {
        //        // TODO diagnostics.Add(NameBindingError.UsingNonExistentNamespace(file, usingDirective.Span, usingDirective.Name));
        //        continue;
        //    }

        //    foreach (var (name, additionalSymbols) in ns.Symbols)
        //    {
        //        if (importedSymbols.TryGetValue(name, out var symbols))
        //            symbols.AddRange(additionalSymbols);
        //        else
        //            importedSymbols.Add(name, additionalSymbols.ToHashSet());
        //    }
        //}

        //var symbolsInScope = importedSymbols.ToFixedDictionary(e => e.Key, e => e.Value.ToFixedSet());
        //return NestedSymbolScope.Create(containingScope, symbolsInScope);
        throw new NotImplementedException();
    }
}
