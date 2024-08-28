using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeKins;
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

    public IFixedSet<AttributeKinModel> DeclaredAttributeKins { get; }

    public IFixedSet<ContextAttributeKinModel> ImplicitAttributeSupertypes => implicitAttributeSupertypes.Value;
    private readonly Lazy<IFixedSet<ContextAttributeKinModel>> implicitAttributeSupertypes;

    public IFixedSet<AttributeKinModel> AllAttributeFamilies => allAttributeSupertypes.Value;
    private readonly Lazy<IFixedSet<AttributeKinModel>> allAttributeSupertypes;

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
        DeclaredAttributeKins = Aspects.SelectMany(a => a.DeclaredAttributeKins).ToFixedSet();
        implicitAttributeSupertypes = new(ComputeImplicitAttributeSupertypes);
        allAttributeSupertypes = new(()
            => DeclaredAttributeKins.Concat(ImplicitAttributeSupertypes).ToFixedSet());
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

    private IFixedSet<ContextAttributeKinModel> ComputeImplicitAttributeSupertypes()
    {
        var declaredAttributeSupertypes = DeclaredAttributeKins.Select(s => s.Name).ToFixedSet();
        var implicitInheritedAttributeSupertypes = ComputeGroupedDeclaredAttributes<InheritedAttributeModel>()
                                                        .Where(g => !declaredAttributeSupertypes.Contains(g.Key))
                                                        .Select(attrs => new InheritedAttributeKinModel(this, attrs));
        var implicitPreviousAttributeSupertypes = ComputeGroupedDeclaredAttributes<PreviousAttributeModel>()
                                                        .Select(attrs => new PreviousAttributeKinModel(this, attrs));
        return implicitInheritedAttributeSupertypes
               .Concat<ContextAttributeKinModel>(implicitPreviousAttributeSupertypes).ToFixedSet();
    }

    private IEnumerable<IGrouping<string, T>> ComputeGroupedDeclaredAttributes<T>()
        where T : AttributeModel
        => Nodes.SelectMany(n => n.DeclaredAttributes.OfType<T>()).GroupBy(a => a.Name);

    public void Validate()
    {
        var errors = ValidateRootSupertype();
        errors |= ValidateAmbiguousAttributes();
        errors |= ValidateUndefinedAttributes();
        errors |= ValidateInheritedEquationsProduceSingleType();
        errors |= ValidateFinalNodes();
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
        => node.ActualAttributes.Duplicates(AttributeModel.NameComparer);

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

    private bool ValidateFinalNodes()
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
}
