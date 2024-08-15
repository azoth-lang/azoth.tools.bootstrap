using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public sealed class TreeModel
{
    public TreeSyntax Syntax { get; }

    public string Namespace => Syntax.Namespace;
    public Symbol? Root { get; }
    public string SymbolPrefix => Syntax.SymbolPrefix;
    public string SymbolSuffix => Syntax.SymbolSuffix;
    public string ClassPrefix => Syntax.ClassPrefix;
    public string ClassSuffix => Syntax.ClassSuffix;
    public IFixedSet<string> UsingNamespaces => Syntax.UsingNamespaces;
    public IFixedList<TreeNodeModel> Nodes { get; }

    public TreeModel(TreeSyntax syntax)
    {
        Syntax = syntax;
        Root = Symbol.CreateFromSyntax(this, syntax.Root);
        Nodes = syntax.Nodes.Select(r => new TreeNodeModel(this, r)).ToFixedList();
        nodesLookup = Nodes.ToFixedDictionary(r => r.Defines.ShortName);
    }

    public TreeNodeModel? NodeFor(string shortName)
        => nodesLookup.GetValueOrDefault(shortName);

    private readonly FixedDictionary<string, TreeNodeModel> nodesLookup;

    public void ValidateAmbiguousProperties()
    {
        foreach (var node in Nodes)
            if (node.AllProperties.Duplicates(PropertyModel.NameComparer).Any())
            {
                var ambiguousNames = node.AllProperties.Duplicates(PropertyModel.NameComparer).Select(p => p.Name).ToList();
                throw new Exception(
                    $"Node {node.Defines} has ambiguous properties {string.Join(", ", ambiguousNames)}."
                    + $" Duplicate properties are: {string.Join(", ", ambiguousNames.SelectMany(node.PropertiesNamed))}");
            }
    }
}
