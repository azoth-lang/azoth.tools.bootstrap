using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;
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

    public FixedDictionary<ExternalSymbol, TypeDeclarationModel> TypeDeclarations { get; }

    public IFixedSet<TreeNodeModel> TreeChildNodes => treeChildNodes.Value;
    private readonly Lazy<IFixedSet<TreeNodeModel>> treeChildNodes;

    public IFixedSet<AttributeFamilyModel> DeclaredAttributeFamilies { get; }

    public IFixedSet<ContextAttributeFamilyModel> ImplicitAttributeFamilies => implicitAttributeFamilies.Value;
    private readonly Lazy<IFixedSet<ContextAttributeFamilyModel>> implicitAttributeFamilies;

    public IFixedSet<AttributeFamilyModel> AllAttributeFamilies => allAttributeFamilies.Value;
    private readonly Lazy<IFixedSet<AttributeFamilyModel>> allAttributeFamilies;

    public TreeModel(TreeSyntax syntax, List<AspectSyntax> aspects)
    {
        Syntax = syntax;
        RootSupertype = Symbol.CreateInternalFromSyntax(this, syntax.Root);
        UsingNamespaces = syntax.UsingNamespaces
                                .Concat(aspects.SelectMany(a => a.UsingNamespaces.Append(a.Namespace)))
                                .Except(Namespace).ToFixedSet();
        Nodes = syntax.Nodes.Select(r => new TreeNodeModel(this, r)).ToFixedList();
        nodesLookup = Nodes.ToFixedDictionary(r => r.Defines.ShortName);

        // Now that the tree is fully created, it is safe to create the aspects
        Aspects = aspects.Select(a => new AspectModel(this, a)).ToFixedList();
        TypeDeclarations = Aspects.SelectMany(a => a.TypeDeclarations).ToFixedDictionary(d => d.Name);
        treeChildNodes = new(() => Nodes.SelectMany(n => n.TreeChildNodes)
                                        .SelectMany(n => n.DescendantNodes.Append(n)).ToFixedSet());
        DeclaredAttributeFamilies = Aspects.SelectMany(a => a.DeclaredAttributeKins).ToFixedSet();
        implicitAttributeFamilies = new(ComputeImplicitAttributeFamilies);
        allAttributeFamilies = new(()
            => DeclaredAttributeFamilies.Concat(ImplicitAttributeFamilies).ToFixedSet());
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

    private IFixedSet<ContextAttributeFamilyModel> ComputeImplicitAttributeFamilies()
    {
        var declaredAttributeSupertypes = DeclaredAttributeFamilies.Select(s => s.Name).ToFixedSet();
        var implicitInheritedAttributeSupertypes = ComputeGroupedDeclaredAttributes<InheritedAttributeModel>()
                                                        .Where(g => !declaredAttributeSupertypes.Contains(g.Key))
                                                        .Select(attrs => new InheritedAttributeFamilyModel(this, attrs));
        return implicitInheritedAttributeSupertypes.ToFixedSet();
    }

    private IEnumerable<IGrouping<string, T>> ComputeGroupedDeclaredAttributes<T>()
        where T : AttributeModel
        => Nodes.SelectMany(n => n.DeclaredAttributes.OfType<T>()).GroupBy(a => a.Name);

    public void Validate()
    {
        var errors = ValidateAllInheritRootSupertype();
        errors |= ValidateNoAmbiguousAttributes();
        errors |= ValidateNoAmbiguousEquations();
        errors |= ValidateInheritedEquationsProduceSingleType();
        errors |= ValidateTempNodesHaveFinalNodes();
        errors |= ValidatePlaceholdersAreUniquelyFilled();
        errors |= ValidateAggregateEquationsContribute();
        errors |= ValidateLocalAttributeEquations();
        errors |= ValidateCircularAttributeEquations();
        if (errors)
            throw new ValidationFailedException();
    }

    /// <summary>
    /// Check that all nodes inherit from the root supertype.
    /// </summary>
    private bool ValidateAllInheritRootSupertype()
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
    private bool ValidateNoAmbiguousAttributes()
    {
        var errors = false;
        foreach (var node in Nodes)
            if (DuplicateAttributes(node).Any())
            {
                errors = true;
                var ambiguousNames = DuplicateAttributes(node).Select(p => p.Name).ToList();
                Console.Error.WriteLine($"ERROR: Node '{node.Defines}' has ambiguous attributes {string.Join(", ", ambiguousNames.Select(n => $"'{n}'"))}."
                                        + $" Definitions: {string.Join(", ", ambiguousNames.SelectMany(node.AttributesNamed).Select(n => $"'{n}'"))}");
            }
        return errors;
    }

    private static IEnumerable<AttributeModel> DuplicateAttributes(TreeNodeModel node)
        => node.ActualAttributes.Duplicates<AttributeModel>(IMemberModel.NameIsPlaceholderComparer);

    /// <summary>
    /// This checks that there are no equations that have conflicting definitions due to the
    /// inheritance of equations from supertypes.
    /// </summary>
    private bool ValidateNoAmbiguousEquations()
    {
        var errors = false;
        foreach (var node in Nodes)
            if (DuplicateEquations(node).Any())
            {
                errors = true;
                var ambiguousNames = DuplicateEquations(node).Select(p => p.Name).ToList();
                Console.Error.WriteLine(
                    $"ERROR: Node '{node.Defines}' has ambiguous equations {string.Join(", ", ambiguousNames.Select(n => $"'{n}'"))}."
                    + $" Definitions: {string.Join(", ", ambiguousNames.SelectMany(node.EquationsNamed).Select(n => $"'{n}'"))}");
            }

        return errors;
    }

    private static IEnumerable<SoleEquationModel> DuplicateEquations(TreeNodeModel node)
        => node.ActualEquations.OfType<SoleEquationModel>()
               .Duplicates<SoleEquationModel>(IMemberModel.NameComparer);

    private bool ValidateInheritedEquationsProduceSingleType()
    {
        var errors = false;
        foreach (var equation in Aspects.SelectMany(a => a.DeclaredEquations).OfType<InheritedAttributeEquationModel>())
            if (equation.InheritedToTypes.Count > 1)
            {
                errors = true;
                Console.Error.WriteLine($"ERROR: Equation '{equation}' matches attributes without a most specific type. Types {string.Join(", ", equation.InheritedToTypes)}.");
            }
        return errors;
    }

    private bool ValidateTempNodesHaveFinalNodes()
    {
        var errors = false;
        // Only temp nodes used as child attribute types need a final type
        var tempNodes = Nodes.SelectMany(n => n.ActualProperties).Where(p => p.IsChild)
                             .Select(p => p.Type.ReferencedNode()!).Where(n => n.IsTemp).Distinct();
        foreach (var node in tempNodes)
            if (node.FinalNode is null)
            {
                errors = true;
                Console.Error.WriteLine($"ERROR: Node '{node.Defines}' is a temp node that must have"
                                        + $" a final node and doesn't. Candidates are: {string.Join(", ", node.CandidateFinalNodes)}");
            }
        return errors;
    }

    private bool ValidatePlaceholdersAreUniquelyFilled()
    {
        var errors = false;
        var placeholders = Nodes.SelectMany(n => n.DeclaredTreeAttributes).OfType<PlaceholderModel>();
        foreach (var placeholder in placeholders.Where(p => p is { IsChild: false, Attribute: not PropertyModel }))
        {
            errors = true;
            Console.Error.WriteLine($"ERROR: Placeholder '{placeholder}' refers to non-child, non-property attribute {placeholder.Attribute}.");
        }
        return errors;
    }

    private bool ValidateAggregateEquationsContribute()
    {
        var errors = false;
        var equations = Aspects.SelectMany(a => a.DeclaredEquations).OfType<AggregateAttributeEquationModel>();
        foreach (var equation in equations.Where(eq => eq.Attributes.IsEmpty))
        {
            errors = true;
            Console.Error.WriteLine($"ERROR: Equation '{equation}' does not contribute to any node.");
        }
        return errors;
    }

    private bool ValidateLocalAttributeEquations()
    {
        var errors = false;
        var equations = Aspects.SelectMany(a => a.DeclaredEquations).OfType<LocalAttributeEquationModel>();
        foreach (var equation in equations)
            if (equation.IsMethod != equation.Attribute.IsMethod)
            {
                errors = true;
                Console.Error.WriteLine($"ERROR: Equation '{equation}' method state doesn't match attribute {equation.Attribute}.");
            }

        return errors;
    }

    private bool ValidateCircularAttributeEquations()
    {
        var errors = false;
        var equations = Aspects.SelectMany(a => a.DeclaredEquations)
                               .OfType<LocalAttributeEquationModel>()
                               .Where(a => a.Attribute is CircularAttributeModel);
        foreach (var equation in equations)
            if (equation.Syntax?.Strategy is not null)
            {
                errors = true;
                Console.Error.WriteLine($"ERROR: Cannot specify strategy for '{equation}' because it is for a circular attribute.");
            }
        return errors;
    }
}
