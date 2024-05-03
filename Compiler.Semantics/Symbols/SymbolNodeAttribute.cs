using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;

internal static class SymbolNodeAttribute
{
    public static IPackageSymbolNode PackageSymbolNode(IPackageNode node)
        => new SemanticPackageSymbolNode(node, CompilationUnitsSymbolNode(node.CompilationUnits), CompilationUnitsSymbolNode(node.TestingCompilationUnits));

    private static INamespaceSymbolNode CompilationUnitsSymbolNode(IEnumerable<ICompilationUnitNode> nodes)
        => throw new NotImplementedException();

    public static IPackageSymbolNode PackageReferenceSymbolNode(IPackageReferenceNode node)
        => new ReferencedPackageSymbolNode(node);

    public static FixedDictionary<IdentifierName, IPackageSymbolNode> PackageSymbolNodes(IPackageNode node)
        => node.References.Select(r => r.SymbolNode).Append(node.SymbolNode).ToFixedDictionary(n => n.AliasOrName);
}
