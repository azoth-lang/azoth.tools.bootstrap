using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

public class TreeNodeModel
{
    public TreeModel Tree { get; }
    public TreeNodeSyntax Syntax { get; }

    public InternalSymbol Defines { get; }
    public SymbolType DefinesType { get; }
    /// <summary>
    /// The directly declared supertypes of this node.
    /// </summary>
    public IFixedSet<Symbol> Supertypes { get; }
    private readonly Lazy<IFixedSet<TreeNodeModel>> supertypeNodes;
    /// <summary>
    /// The tree nodes corresponding to the directly declared supertypes of this node.
    /// </summary>
    public IFixedSet<TreeNodeModel> SupertypeNodes => supertypeNodes.Value;
    public IFixedSet<TreeNodeModel> AncestorNodes => ancestorNodes.Value;
    private readonly Lazy<IFixedSet<TreeNodeModel>> ancestorNodes;
    public IFixedSet<TreeNodeModel> ChildNodes => childNodes.Value;
    private readonly Lazy<IFixedSet<TreeNodeModel>> childNodes;

    /// <summary>
    /// Whether this tree node is abstract meaning that it cannot be instantiated directly.
    /// </summary>
    /// <remarks>Right now, this is determined solely by whether this node has child nodes. If
    /// needed, keywords <c>abstract</c> and <c>concrete</c> could be added to the definition file
    /// to allow overriding this.</remarks>
    public bool IsAbstract => !ChildNodes.IsEmpty;

    public IFixedSet<TreeNodeModel> DescendantNodes => descendantNodes.Value;
    private readonly Lazy<IFixedSet<TreeNodeModel>> descendantNodes;

    /// <summary>
    /// The properties declared for the node in the definition file.
    /// </summary>
    public IFixedList<PropertyModel> DeclaredProperties { get; }

    /// <summary>
    /// Properties that are implicitly declared on the node because multiple supertypes define the
    /// same property with the same type.
    /// </summary>
    public IFixedList<PropertyModel> ImplicitlyDeclaredProperties => implicitlyDeclaredProperties.Value;
    private readonly Lazy<IFixedList<PropertyModel>> implicitlyDeclaredProperties;

    /// <summary>
    /// The combination of declared and implicitly declared properties.
    /// </summary>
    public IEnumerable<PropertyModel> AllDeclaredProperties
        => DeclaredProperties.Concat(ImplicitlyDeclaredProperties);

    /// <summary>
    /// Properties that must be declared in the node interface.
    /// </summary>
    public IFixedList<PropertyModel> PropertiesRequiringDeclaration => propertiesRequiringDeclaration.Value;
    private readonly Lazy<IFixedList<PropertyModel>> propertiesRequiringDeclaration;

    /// <summary>
    /// Properties inherited from the supertypes of a rule. If the same property is defined on
    /// multiple supertypes, it will be listed multiple times. However, if a property is
    /// inherited from a common supertype through multiple paths it will be listed once.
    /// </summary>
    /// <remarks>This is regardless of whether they are overriden on this node.</remarks>
    public IFixedList<PropertyModel> InheritedProperties => inheritedProperties.Value;
    private readonly Lazy<IFixedList<PropertyModel>> inheritedProperties;

    /// <summary>
    /// Get the actual properties for a node. This will use inherited properties if a property
    /// declared on this node does not require a declaration. Properties will be ordered by
    /// declaration and then by supertype order.
    /// </summary>
    /// <remarks>This will not return duplicate property names unless two supertypes declare
    /// conflicting properties.</remarks>
    public IFixedList<PropertyModel> ActualProperties => allProperties.Value;
    private readonly Lazy<IFixedList<PropertyModel>> allProperties;

    /// <summary>
    /// Attributes declared against this specific node in the definition files.
    /// </summary>
    public IFixedList<AspectAttributeModel> DeclaredAttributes => declaredAttributes.Value;
    private readonly Lazy<IFixedList<AspectAttributeModel>> declaredAttributes;

    /// <summary>
    /// Equations declared against this specific node in the definition files.
    /// </summary>
    public IFixedList<EquationModel> DeclaredEquations => declaredEquations.Value;
    private readonly Lazy<IFixedList<EquationModel>> declaredEquations;

    public TreeNodeModel(TreeModel tree, TreeNodeSyntax syntax)
    {
        Tree = tree;
        Syntax = syntax;
        Defines = Symbol.CreateInternalFromSyntax(tree, syntax.Defines);
        DefinesType = new SymbolType(Defines);
        Supertypes = syntax.Supertypes.Select(s => Symbol.CreateFromSyntax(tree, s)).ToFixedSet();
        // Add root supertype if no supertypes are declared
        if (Tree.Root is { } root && root != Defines && !Supertypes.Any(s => s is InternalSymbol))
            Supertypes = Supertypes.Append(root).ToFixedSet();

        // Inheritance relationships
        supertypeNodes = new(() => Supertypes.OfType<InternalSymbol>().Select(s => s.ReferencedNode)
                                             .EliminateRedundantRules().ToFixedSet());
        ancestorNodes = new(() => SupertypeNodes.Concat(SupertypeNodes.SelectMany(p => p.AncestorNodes)).ToFixedSet());
        childNodes = new(() => Tree.Nodes.Where(r => r.SupertypeNodes.Contains(this)).ToFixedSet());
        descendantNodes = new(() => ChildNodes.Concat(ChildNodes.SelectMany(r => r.DescendantNodes)).ToFixedSet());

        // Properties
        DeclaredProperties = syntax.DeclaredProperties.Select(p => new PropertyModel(this, p)).ToFixedList();
        inheritedProperties = new(()
            => MostSpecificProperties(SupertypeNodes.SelectMany(r => r.ActualProperties).Distinct()).ToFixedList());
        implicitlyDeclaredProperties = new(()
            => InheritedProperties.AllExcept<PropertyModel>(DeclaredProperties, AttributeModel.NameComparer)
                                  .GroupBy(p => p.Name)
                                  .Select(ImplicitlyDeclaredProperty).WhereNotNull()
                                  .Assert(p => p.IsDeclarationRequired, p => new($"Implicit property {p} no declared."))
                                  .ToFixedList());
        propertiesRequiringDeclaration = new(()
            => AllDeclaredProperties.Where(p => p.IsDeclarationRequired).ToFixedList());
        allProperties = new(() =>
        {
            var propertyLookup = PropertiesRequiringDeclaration
                                 .Concat(InheritedProperties.AllExcept<PropertyModel>(PropertiesRequiringDeclaration, AttributeModel.NameComparer))
                                 .ToLookup(p => p.Name);
            var propertyOrder = AllDeclaredProperties.Concat(SupertypeNodes.SelectMany(s => s.ActualProperties))
                                                     .DistinctBy(p => p.Name);
            return propertyOrder.SelectMany(p => propertyLookup[p.Name]).ToFixedList();
        });

        // Attributes
        declaredAttributes = new(() => Tree.Aspects.SelectMany(a => a.Attributes).Where(a => a.NodeSymbol == Defines).ToFixedList());
        declaredEquations = new(() => Tree.Aspects.SelectMany(a => a.Equations).Where(e => e.Node == Defines).ToFixedList());
    }

    /// <summary>
    /// The distinct properties with the same name that are inherited from supertypes.
    /// </summary>
    public IEnumerable<PropertyModel> InheritedPropertiesNamedSameAs(PropertyModel property)
        => InheritedPropertiesNamed(property.Name);
    private IEnumerable<PropertyModel> InheritedPropertiesNamed(string propertyName)
        => InheritedProperties.Where(p => p.Name == propertyName).Distinct();

    public IEnumerable<PropertyModel> PropertiesNamed(string propertyName)
        => ActualProperties.Where(p => p.Name == propertyName);

    private static IEnumerable<PropertyModel> MostSpecificProperties(IEnumerable<PropertyModel> propertyModels)
        => propertyModels.GroupBy(p => p.Name).SelectMany(MostSpecificProperties);

    private static IEnumerable<PropertyModel> MostSpecificProperties(IGrouping<string, PropertyModel> properties)
    {
        var mostSpecific = new List<PropertyModel>();
        foreach (var property in properties)
        {
            for (var i = mostSpecific.Count - 1; i >= 0; i--)
            {
                var mostSpecificProperty = mostSpecific[i];
                if (IsMoreSpecific(mostSpecificProperty, property))
                    goto nextProperty;
                if (IsMoreSpecific(property, mostSpecificProperty))
                    mostSpecific.RemoveAt(i);
            }
            mostSpecific.Add(property);

        nextProperty:;
        }
        return mostSpecific;
    }

    private PropertyModel? ImplicitlyDeclaredProperty(IGrouping<string, PropertyModel> properties)
    {
        var name = properties.Key;
        var (type, count) = properties.CountBy(p => p.Type).TrySingle();
        return count switch
        {
            0 => null, // Multiple types
            1 => null, // Single property that doesn't need to be redeclared
            _ => new PropertyModel(this, name, type),
        };
    }

    private static bool IsMoreSpecific(PropertyModel property, PropertyModel other)
        => property.Node.AncestorNodes.Contains(other.Node);
}
