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

    public bool InheritsFromRootSupertype => inheritsFromRootSupertype.Value;
    private readonly Lazy<bool> inheritsFromRootSupertype;

    /// <summary>
    /// The properties declared for the node in the definition file.
    /// </summary>
    public IFixedList<PropertyModel> DeclaredProperties { get; }

    /// <summary>
    /// Attributes (including properties) declared against this node in both the tree and aspect
    /// definition files.
    /// </summary>
    public IEnumerable<AttributeModel> DeclaredAttributes => declaredAttributes.Value;
    private readonly Lazy<IFixedList<AttributeModel>> declaredAttributes;

    /// <summary>
    /// Properties that are implicitly declared on the node because multiple supertypes define the
    /// same property with the same type.
    /// </summary>
    public IFixedList<PropertyModel> ImplicitlyDeclaredProperties => implicitlyDeclaredProperties.Value;
    private readonly Lazy<IFixedList<PropertyModel>> implicitlyDeclaredProperties;

    /// <summary>
    /// The combination of declared and implicitly declared attributes.
    /// </summary>
    public IEnumerable<AttributeModel> AllDeclaredAttributes
        => DeclaredAttributes.Concat(ImplicitlyDeclaredProperties);

    /// <summary>
    /// Attributes that must be declared in the node interface.
    /// </summary>
    public IFixedList<AttributeModel> AttributesRequiringDeclaration => attributesRequiringDeclaration.Value;
    private readonly Lazy<IFixedList<AttributeModel>> attributesRequiringDeclaration;

    /// <summary>
    /// Attributes inherited from the supertypes of a node. If the same attribute is defined on
    /// multiple supertypes, it will be listed multiple times. However, if a is
    /// inherited from a common supertype through multiple paths it will be listed once.
    /// </summary>
    /// <remarks>This is regardless of whether they are overriden on this node.</remarks>
    public IFixedList<AttributeModel> InheritedAttributes => inheritedAttributes.Value;
    private readonly Lazy<IFixedList<AttributeModel>> inheritedAttributes;

    /// <summary>
    /// Get the actual attributes for a node. This will use inherited attributes if an attribute
    /// declared on this node does not require a declaration. Attributes will be ordered by
    /// supertype and declaration order.
    /// </summary>
    /// <remarks>This will not return duplicate attribute names unless two supertypes declare
    /// conflicting attributes.</remarks>
    public IFixedList<AttributeModel> ActualAttributes => actualAttributes.Value;
    private readonly Lazy<IFixedList<AttributeModel>> actualAttributes;

    public IEnumerable<PropertyModel> ActualProperties => ActualAttributes.OfType<PropertyModel>();

    /// <summary>
    /// Equations declared against this specific node in the definition files.
    /// </summary>
    public IFixedList<EquationModel> DeclaredEquations => declaredEquations.Value;
    private readonly Lazy<IFixedList<EquationModel>> declaredEquations;

    public IFixedList<SynthesizedAttributeEquationModel> ImplicitlyDeclaredEquations => implicitlyDeclaredEquations.Value;
    private readonly Lazy<IFixedList<SynthesizedAttributeEquationModel>> implicitlyDeclaredEquations;

    public IEnumerable<EquationModel> AllDeclaredEquations
        => DeclaredEquations.Concat(ImplicitlyDeclaredEquations);

    public IFixedList<SynthesizedAttributeEquationModel> InheritedEquations => inheritedEquations.Value;
    private readonly Lazy<IFixedList<SynthesizedAttributeEquationModel>> inheritedEquations;

    public IFixedList<SynthesizedAttributeEquationModel> ActualEquations => actualEquations.Value;
    private readonly Lazy<IFixedList<SynthesizedAttributeEquationModel>> actualEquations;

    public IFixedSet<InheritedAttributeEquationGroupModel> DeclaredInheritedAttributeEquationGroups
        => declaredInheritedAttributeEquationGroups.Value;
    private readonly Lazy<IFixedSet<InheritedAttributeEquationGroupModel>> declaredInheritedAttributeEquationGroups;

    public TreeNodeModel(TreeModel tree, TreeNodeSyntax syntax)
    {
        Tree = tree;
        Syntax = syntax;
        Defines = Symbol.CreateInternalFromSyntax(tree, syntax.Defines);
        DefinesType = new SymbolType(Defines);
        Supertypes = syntax.Supertypes.Select(s => Symbol.CreateFromSyntax(tree, s)).ToFixedSet();
        // Add root supertype if no supertypes are declared
        if (Tree.RootSupertype is { } root && root != Defines && !Supertypes.Any(s => s is InternalSymbol))
            Supertypes = Supertypes.Append(root).ToFixedSet();

        // Inheritance relationships
        supertypeNodes = new(() => Supertypes.OfType<InternalSymbol>().Select(s => s.ReferencedNode)
                                             .EliminateRedundantRules().ToFixedSet());
        ancestorNodes = new(() => SupertypeNodes.Concat(SupertypeNodes.SelectMany(p => p.AncestorNodes)).ToFixedSet());
        childNodes = new(() => Tree.Nodes.Where(r => r.SupertypeNodes.Contains(this)).ToFixedSet());
        descendantNodes = new(() => ChildNodes.Concat(ChildNodes.SelectMany(r => r.DescendantNodes)).ToFixedSet());
        inheritsFromRootSupertype = new(() => Tree.RootSupertype is not null
            && (Defines == Tree.RootSupertype || SupertypeNodes.Any(s => s.InheritsFromRootSupertype)));

        // Attributes
        DeclaredProperties = syntax.DeclaredProperties.Select(p => new PropertyModel(this, p)).ToFixedList();
        declaredAttributes = new(() => DeclaredProperties.Concat<AttributeModel>(Tree.Aspects.SelectMany(a => a.Attributes).Where(a => a.NodeSymbol == Defines)).ToFixedList());
        inheritedAttributes = new(()
            => MostSpecificMembers(SupertypeNodes.SelectMany(r => r.ActualAttributes).Distinct()).ToFixedList());
        implicitlyDeclaredProperties = new(()
            => InheritedAttributes.OfType<PropertyModel>()
                                  .AllExcept<PropertyModel>(DeclaredProperties, AttributeModel.NameComparer)
                                  .GroupBy(p => p.Name)
                                  .Select(ImplicitlyDeclaredProperty).WhereNotNull()
                                  .Assert(p => p.IsDeclarationRequired, p => new($"Implicit property {p} no declared."))
                                  .ToFixedList());
        attributesRequiringDeclaration = new(()
            => AllDeclaredAttributes.Where(p => p.IsDeclarationRequired).ToFixedList());
        actualAttributes = new(() =>
        {
            var attributeLookup = AttributesRequiringDeclaration
                                 .Concat(InheritedAttributes.AllExcept(AttributesRequiringDeclaration, AttributeModel.NameComparer))
                                 .ToLookup(p => p.Name);
            var attributeOrder = SupertypeNodes.SelectMany(s => s.ActualAttributes)
                                               .Except(AllDeclaredAttributes, AttributeModel.NameComparer)
                                               .Concat(AllDeclaredAttributes);
            return attributeOrder.SelectMany(p => attributeLookup[p.Name]).ToFixedList();
        });

        // Equations
        declaredEquations = new(() => Tree.Aspects.SelectMany(a => a.DeclaredEquations).Where(e => e.NodeSymbol == Defines).ToFixedList());
        inheritedEquations = new(()
            => MostSpecificMembers(SupertypeNodes.SelectMany(r => r.ActualEquations).Distinct()).ToFixedList());
        implicitlyDeclaredEquations = new(()
            =>
        {
            if (IsAbstract) return FixedList.Empty<SynthesizedAttributeEquationModel>();
            var actualEquationsNames = ComputeActualSynthesizedEquations(DeclaredEquations).Select(eq => eq.Name).ToFixedSet();
            return ActualAttributes.OfType<SynthesizedAttributeModel>()
                                   .Where(a => a.DefaultExpression is null && !actualEquationsNames.Contains(a.Name))
                                   .Select(ImplicitlyDeclaredEquation).ToFixedList();
        });
        actualEquations = new(() => ComputeActualSynthesizedEquations(AllDeclaredEquations).ToFixedList());
        declaredInheritedAttributeEquationGroups = new(()
            => DeclaredEquations.OfType<InheritedAttributeEquationModel>()
                                .GroupBy(e => e.Name, (_, eqs) => new InheritedAttributeEquationGroupModel(this, eqs))
                                .ToFixedSet());
    }

    /// <summary>
    /// The distinct attributes with the same name that are inherited from supertypes.
    /// </summary>
    public IEnumerable<AttributeModel> InheritedAttributesNamedSameAs(AttributeModel attribute)
        => InheritedAttributesNamed(attribute.Name);
    private IEnumerable<AttributeModel> InheritedAttributesNamed(string name)
        => InheritedAttributes.Where(p => p.Name == name).Distinct();

    public IEnumerable<AttributeModel> AttributesNamed(string name)
        => ActualAttributes.Where(p => p.Name == name);

    private static IEnumerable<T> MostSpecificMembers<T>(IEnumerable<T> attributes)
        where T : IMemberModel
        => attributes.GroupBy(p => p.Name).SelectMany(MostSpecificMembers);

    private static IEnumerable<T> MostSpecificMembers<T>(IGrouping<string, T> attributes)
        where T : IMemberModel
    {
        var mostSpecific = new List<T>();
        foreach (var attribute in attributes)
        {
            for (var i = mostSpecific.Count - 1; i >= 0; i--)
            {
                var mostSpecificAttribute = mostSpecific[i];
                if (IsMoreSpecific(mostSpecificAttribute, attribute))
                    goto nextProperty;
                if (IsMoreSpecific(attribute, mostSpecificAttribute))
                    mostSpecific.RemoveAt(i);
            }
            mostSpecific.Add(attribute);

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

    private static bool IsMoreSpecific<T>(T property, T other)
        where T : IMemberModel
        => property.Node.AncestorNodes.Contains(other.Node);

    private IEnumerable<SynthesizedAttributeEquationModel> ComputeActualSynthesizedEquations(
        IEnumerable<EquationModel> declaredEquations)
    {
        var declaredSynthesizedEquations = declaredEquations.OfType<SynthesizedAttributeEquationModel>().ToList();
        var actualSynthesizedEquations = declaredSynthesizedEquations.Concat(
            InheritedEquations.AllExcept(declaredSynthesizedEquations, SynthesizedAttributeEquationModel.NameComparer));
        return actualSynthesizedEquations;
    }

    private SynthesizedAttributeEquationModel ImplicitlyDeclaredEquation(SynthesizedAttributeModel attribute)
        => new(this, attribute);
}
