using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;

public sealed class TreeSyntax
{
    public string Namespace { get; }
    public SymbolSyntax? Root { get; }
    public string SymbolPrefix { get; }
    public string SymbolSuffix { get; }
    public bool GenerateClasses { get; }
    public bool SimplifiedTree { get; }
    public string ClassPrefix { get; }
    public string ClassSuffix { get; }

    public IFixedSet<string> UsingNamespaces { get; }
    public IFixedList<TreeNodeSyntax> Nodes { get; }

    public TreeSyntax(
        string @namespace,
        SymbolSyntax? root,
        string symbolPrefix,
        string symbolSuffix,
        bool generateClasses,
        bool simplifiedTree,
        string classPrefix,
        string classSuffix,
        IEnumerable<string> usingNamespaces,
        IEnumerable<TreeNodeSyntax> nodes)
    {
        Namespace = @namespace;
        Root = root;
        SymbolPrefix = symbolPrefix;
        SymbolSuffix = symbolSuffix;
        GenerateClasses = generateClasses;
        SimplifiedTree = simplifiedTree;
        ClassPrefix = classPrefix;
        ClassSuffix = classSuffix;
        UsingNamespaces = usingNamespaces.ToFixedSet();
        Nodes = nodes.ToFixedList();
        if (Nodes.Select(r => r.Defines).Duplicates().Any())
            throw new ValidationException("Node names must be unique.");
    }
}
