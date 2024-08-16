using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public sealed class TreeModel : IHasUsingNamespaces
{
    public TreeSyntax Syntax { get; }

    public string Namespace => Syntax.Namespace;
    public Symbol? Root { get; }
    public string SymbolPrefix => Syntax.SymbolPrefix;
    public string SymbolSuffix => Syntax.SymbolSuffix;
    public bool GenerateClasses => Syntax.GenerateClasses;
    public string ClassPrefix => Syntax.ClassPrefix;
    public string ClassSuffix => Syntax.ClassSuffix;
    public IFixedSet<string> UsingNamespaces { get; }
    public IFixedList<TreeNodeModel> Nodes { get; }
    public IFixedList<AspectModel> Aspects { get; }

    public TreeModel(TreeSyntax syntax, List<AspectSyntax> aspects)
    {
        Syntax = syntax;
        Root = Symbol.CreateFromSyntax(this, syntax.Root);
        UsingNamespaces = syntax.UsingNamespaces
                                .Concat(aspects.SelectMany(a => a.UsingNamespaces.Append(a.Namespace)))
                                .Except(Namespace).ToFixedSet();
        Nodes = syntax.Nodes.Select(r => new TreeNodeModel(this, r)).ToFixedList();
        nodesLookup = Nodes.ToFixedDictionary(r => r.Defines.ShortName);

        // Now that the three is fully created, it is safe to create the aspects
        Aspects = aspects.Select(a => new AspectModel(this, a)).ToFixedList();
    }

    public TreeNodeModel? NodeFor(InternalSymbol symbol) => NodeFor(symbol.ShortName);

    public TreeNodeModel? NodeFor(string shortName) => nodesLookup.GetValueOrDefault(shortName);

    public T? AttributeFor<T>(InternalSymbol nodeSymbol, string name)
        where T : AttributeModel
    {
        var node = NodeFor(nodeSymbol.ShortName)!;
        var attribute = node.DeclaredAttributes.Concat(node.SupertypeNodes.SelectMany(s => s.DeclaredAttributes))
                            .OfType<T>().FirstOrDefault(a => a.Name == name);
        return attribute;
    }

    private readonly FixedDictionary<string, TreeNodeModel> nodesLookup;

    public void Validate()
    {
        var errors = ValidateAmbiguousProperties();
        errors |= ValidateUndefinedAttributes();
        if (errors)
            throw new Exception("Invalid definitions. See console output for errors.");
    }


    /// <summary>
    /// This checks that there are no properties that have conflicting definitions due to the
    /// inheritance of properties from supertypes.
    /// </summary>
    private bool ValidateAmbiguousProperties()
    {
        var errors = false;
        foreach (var node in Nodes)
            if (node.ActualProperties.Duplicates(PropertyModel.NameComparer).Any())
            {
                errors = true;
                var ambiguousNames = node.ActualProperties.Duplicates(PropertyModel.NameComparer).Select(p => p.Name).ToList();
                Console.Error.WriteLine($"ERROR: Node '{node.Defines}' has ambiguous properties.");
                foreach (var ambiguousName in ambiguousNames)
                {
                    Console.Error.WriteLine($"    '{ambiguousName}' defined as:");
                    foreach (var property in ambiguousNames.SelectMany(node.PropertiesNamed))
                    {
                        Console.Error.WriteLine($"        '{property}'");
                    }
                }
            }

        return errors;
    }

    /// <summary>
    /// This checks that each attribute has a definition for each concrete node.
    /// </summary>
    private bool ValidateUndefinedAttributes()
    {
        var errors = false;
        foreach (var node in Nodes.Where(n => !n.IsAbstract))
        {
        }
        return errors;
    }
}
