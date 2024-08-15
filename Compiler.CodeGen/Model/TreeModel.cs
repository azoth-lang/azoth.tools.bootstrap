using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public sealed class TreeModel
{
    public TreeSyntax Syntax { get; }

    public string Namespace => Syntax.Namespace;
    public Symbol? Root { get; }
    public string SymbolPrefix => Syntax.SymbolPrefix;
    public string SymbolSuffix => Syntax.SymbolSuffix;
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

    public void ValidateTreeOrdering()
    {
        foreach (var rule in Nodes.Where(r => !r.IsAbstract))
        {
            var baseNonTerminalPropertyNames
                = rule.AncestorNodes
                  .SelectMany(r => r.DeclaredProperties)
                  .Where(p => p.ReferencesNode).Select(p => p.Name);
            var nonTerminalPropertyNames = rule.DeclaredProperties.Where(p => p.ReferencesNode).Select(p => p.Name);
            var missingProperties = baseNonTerminalPropertyNames.Except(nonTerminalPropertyNames).ToList();
            if (missingProperties.Any())
                throw new ValidationException($"Rule for {rule.Defines} is missing inherited properties: {string.Join(", ", missingProperties)}. Can't determine order to visit children.");
        }
    }
}
