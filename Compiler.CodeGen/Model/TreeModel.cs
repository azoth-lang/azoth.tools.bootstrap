using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
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
    public InternalSymbol? RootSupertype { get; }
    public string SymbolPrefix => Syntax.SymbolPrefix;
    public string SymbolSuffix => Syntax.SymbolSuffix;
    public bool GenerateClasses => Syntax.GenerateClasses;
    public bool SimplifiedTree => Syntax.SimplifiedTree;
    public string ClassPrefix => Syntax.ClassPrefix;
    public string ClassSuffix => Syntax.ClassSuffix;
    public IFixedSet<string> UsingNamespaces { get; }
    public IFixedList<TreeNodeModel> Nodes { get; }
    public IFixedList<AspectModel> Aspects { get; }

    public TreeModel(TreeSyntax syntax, List<AspectSyntax> aspects)
    {
        Syntax = syntax;
        RootSupertype = Symbol.CreateInternalFromSyntax(this, syntax.Root);
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
        var attribute = node.ActualAttributes.OfType<T>().FirstOrDefault(a => a.Name == name);
        return attribute;
    }

    private readonly FixedDictionary<string, TreeNodeModel> nodesLookup;

    public void Validate()
    {
        var errors = ValidateRootSupertype();
        errors |= ValidateAmbiguousAttributes();
        errors |= ValidateUndefinedAttributes();
        if (errors)
            throw new ValidationFailedException();
    }

    /// <summary>
    /// Check that all nodes inherit from the root supertype.
    /// </summary>
    private bool ValidateRootSupertype()
    {
        if (RootSupertype is null)
            // No root supertype, nothing to validate
            return true;
        var errors = false;
        foreach (var node in Nodes)
            if (!node.InheritsFromRootSupertype)
            {
                errors = true;
                Console.Error.WriteLine($"ERROR: Node '{node.Defines}' does not inherit from the root supertype '{RootSupertype}'.");
            }
        return errors;
    }

    /// <summary>
    /// This checks that there are no attributes that have conflicting definitions due to the
    /// inheritance of attributes from supertypes.
    /// </summary>
    private bool ValidateAmbiguousAttributes()
    {
        var errors = false;
        foreach (var node in Nodes)
            if (node.ActualAttributes.Duplicates(AttributeModel.NameComparer).Any())
            {
                errors = true;
                var ambiguousNames = node.ActualAttributes.Duplicates(AttributeModel.NameComparer).Select(p => p.Name).ToList();
                Console.Error.WriteLine($"ERROR: Node '{node.Defines}' has ambiguous attributes {string.Join(", ", ambiguousNames.Select(n => $"'{n}'"))}."
                                        + $" Definitions: {string.Join(", ", ambiguousNames.SelectMany(node.AttributesNamed).Select(n => $"'{n}'"))}");
            }
        return errors;
    }

    /// <summary>
    /// This checks that each attribute has a definition for each concrete node.
    /// </summary>
    // TODO with implicitly declared equations, this check should never fail. However, something similar might be needed for other cases.
    private bool ValidateUndefinedAttributes()
    {
        var errors = false;
        foreach (var node in Nodes.Where(n => !n.IsAbstract))
        {
            var actualEquations = node.ActualEquations.Select(e => e.Name).ToFixedSet();
            foreach (var synthesizedAttribute in AttributesRequiringEquations(node))
                if (!actualEquations.Contains(synthesizedAttribute.Name))
                {
                    errors = true;
                    Console.Error.WriteLine($"ERROR: Node '{node.Defines}' is missing an equation for attribute '{synthesizedAttribute.Name}'.");
                }
        }
        return errors;
    }

    private static IEnumerable<SynthesizedAttributeModel> AttributesRequiringEquations(TreeNodeModel node)
        => node.ActualAttributes.OfType<SynthesizedAttributeModel>().Where(a => a.DefaultExpression is null);
}
