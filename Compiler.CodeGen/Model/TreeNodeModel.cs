using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax;
using Azoth.Tools.Bootstrap.Framework;
using MoreLinq;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;

[DebuggerDisplay("{" + nameof(ToString) + "(),nq}")]
public class TreeNodeModel
{
    public TreeModel Tree { get; }
    public TreeNodeSyntax Syntax { get; }

    /// <summary>
    /// Whether this node type is temporary, meaning it should not be part of the final tree.
    /// </summary>
    public bool IsTemp => Syntax.IsTemp;
    public InternalSymbol Defines { get; }
    public SymbolTypeModel DefinesType { get; }
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

    /// <summary>
    /// Whether this node needs a sync lock declared in it for the use by any of its attributes.
    /// </summary>
    public bool IsSyncLockRequired => isSyncLockRequired.Value;
    private readonly Lazy<bool> isSyncLockRequired;

    /// <summary>
    /// All nodes that are descendants of this node in the type hierarchy.
    /// </summary>
    /// <remarks>Does not include the current node.</remarks>
    public IFixedSet<TreeNodeModel> DescendantNodes => descendantNodes.Value;
    private readonly Lazy<IFixedSet<TreeNodeModel>> descendantNodes;

    /// <summary>
    /// Whether this node type directly or indirectly inherits from the root supertype.
    /// </summary>
    public bool InheritsFromRootSupertype => inheritsFromRootSupertype.Value;
    private readonly Lazy<bool> inheritsFromRootSupertype;

    public IFixedSet<TreeNodeModel> CandidateFinalNodes => candidateFinalTypes.Value;
    private readonly Lazy<IFixedSet<TreeNodeModel>> candidateFinalTypes;

    /// <summary>
    /// The node that this node will be transformed into in the final tree.
    /// </summary>
    /// <remarks>If <see cref="IsTemp"/> is false, it will be this node. Otherwise, it will be the
    /// non-temp node type below this.</remarks>
    public TreeNodeModel? FinalNode => finalNodeType.Value;
    private readonly Lazy<TreeNodeModel?> finalNodeType;

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
    /// Attributes that are implicitly declared on the node because multiple supertypes define the
    /// same attribute with the same type.
    /// </summary>
    public IFixedList<AttributeModel> ImplicitlyDeclaredAttributes => implicitlyDeclaredAttributes.Value;
    private readonly Lazy<IFixedList<AttributeModel>> implicitlyDeclaredAttributes;

    /// <summary>
    /// The combination of declared and implicitly declared attributes.
    /// </summary>
    public IEnumerable<AttributeModel> AllDeclaredAttributes
        => DeclaredAttributes.Concat(ImplicitlyDeclaredAttributes);

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

    public IFixedList<AttributeModel> AllInheritedAttributes => allInheritedAttributes.Value;
    private readonly Lazy<IFixedList<AttributeModel>> allInheritedAttributes;

    public IFixedList<AttributeModel> AllAttributes => allAttributes.Value;
    private readonly Lazy<IFixedList<AttributeModel>> allAttributes;

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

    public IFixedList<EquationModel> AllDeclaredEquations => allDeclaredEquations.Value;
    private readonly Lazy<IFixedList<EquationModel>> allDeclaredEquations;

    public IFixedList<SubtreeEquationModel> EquationsRequiringEmit => equationsRequiringEmit.Value;
    private readonly Lazy<IFixedList<SubtreeEquationModel>> equationsRequiringEmit;

    public IFixedList<EquationModel> InheritedEquations => inheritedEquations.Value;
    private readonly Lazy<IFixedList<EquationModel>> inheritedEquations;

    public IFixedList<EquationModel> ActualEquations => actualEquations.Value;
    private readonly Lazy<IFixedList<EquationModel>> actualEquations;

    public IFixedSet<InheritedAttributeEquationGroupModel> InheritedAttributeEquationGroups
        => inheritedAttributeEquationGroups.Value;
    private readonly Lazy<IFixedSet<InheritedAttributeEquationGroupModel>> inheritedAttributeEquationGroups;

    public TreeNodeModel(TreeModel tree, TreeNodeSyntax syntax)
    {
        Tree = tree;
        Syntax = syntax;
        Defines = Symbol.CreateInternalFromSyntax(tree, syntax.Defines);
        DefinesType = new SymbolTypeModel(Defines);
        Supertypes = syntax.Supertypes.Select(s => Symbol.CreateFromSyntax(tree, s)).ToFixedSet();
        // Add root supertype if no supertypes are declared
        if (Tree.RootSupertype is { } root && root != Defines && !Supertypes.Any(s => s is InternalSymbol))
            Supertypes = Supertypes.Append(root).ToFixedSet();

        isSyncLockRequired = new(() => ActualAttributes.Any(a => a.IsSyncLockRequired)
                                       || ActualEquations.Any(eq => eq.IsSyncLockRequired));

        // Inheritance relationships
        supertypeNodes = new(() => Supertypes.OfType<InternalSymbol>().Select(s => s.ReferencedNode)
                                             .EliminateRedundantRules().ToFixedSet());
        ancestorNodes = new(() => SupertypeNodes.Concat(SupertypeNodes.SelectMany(p => p.AncestorNodes)).ToFixedSet());
        childNodes = new(() => Tree.Nodes.Where(r => r.SupertypeNodes.Contains(this)).ToFixedSet());
        descendantNodes = new(() => ChildNodes.Concat(ChildNodes.SelectMany(r => r.DescendantNodes)).ToFixedSet());
        inheritsFromRootSupertype = new(() => Tree.RootSupertype is not null
            && (Defines == Tree.RootSupertype || SupertypeNodes.Any(s => s.InheritsFromRootSupertype)));
        candidateFinalTypes = new(() => DescendantNodes.Where(n => !n.IsTemp)
                                                       .Select(n => n.DefinesType)
                                                       .MostGeneralTypes()
                                                       .Select(t => t.ReferencedNode()!)
                                                       .ToFixedSet());
        finalNodeType = IsTemp ? new(() => CandidateFinalNodes.TrySingle()) : new(this);

        // Attributes
        DeclaredProperties = syntax.DeclaredProperties.Select(p => new PropertyModel(this, p)).ToFixedList();
        declaredAttributes = new(() => DeclaredProperties.Concat<AttributeModel>(Tree.Aspects.SelectMany(a => a.Attributes).Where(a => a.NodeSymbol == Defines)).ToFixedList());
        allInheritedAttributes = new(()
            => SupertypeNodes.SelectMany(r => r.AllAttributes).Distinct().ToFixedList());
        allAttributes = new(() => AllDeclaredAttributes.Concat(AllInheritedAttributes).ToFixedList());
        inheritedAttributes = new(()
            => MostSpecificMembers(SupertypeNodes.SelectMany(r => r.ActualAttributes).Distinct()).ToFixedList());
        implicitlyDeclaredAttributes = new(()
            => InheritedAttributes.AllExcept(DeclaredAttributes, AttributeModel.NameComparer)
                                  .GroupBy(p => p.Name)
                                  .Select(ImplicitlyDeclaredAttribute).WhereNotNull()
                                  .Assert(p => p.IsDeclarationRequired, p => new($"Implicit attribute {p} no declared."))
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
        implicitlyDeclaredEquations = new(() =>
            {
                if (IsAbstract) return FixedList.Empty<SynthesizedAttributeEquationModel>();
                var actualEquationsNames = ComputeActualEquations(DeclaredEquations)
                                           .OfType<SynthesizedAttributeEquationModel>()
                                           .Select(eq => eq.Name).ToFixedSet();
                return ActualAttributes.OfType<SynthesizedAttributeModel>()
                                       .Where(a => a.DefaultExpression is null && !actualEquationsNames.Contains(a.Name))
                                       .Select(ImplicitlyDeclaredEquation).ToFixedList();
            });
        allDeclaredEquations = new(() => DeclaredEquations.Concat(ImplicitlyDeclaredEquations).ToFixedList());
        equationsRequiringEmit = new(()
            => AllDeclaredEquations.OfType<SubtreeEquationModel>()
                                   .Where(eq => eq.RequiresEmitOnNode)
                                   .ExceptBy(AttributesRequiringDeclaration.Select(a => a.Name), eq => eq.Name)
                                   .ToFixedList());
        actualEquations = new(() => ComputeActualEquations(AllDeclaredEquations).ToFixedList());
        inheritedEquations = new(()
            => MostSpecificMembers(SupertypeNodes.SelectMany(r => r.ActualEquations).Distinct()).ToFixedList());
        inheritedAttributeEquationGroups = new(()
            => ActualEquations.OfType<InheritedAttributeEquationModel>()
                              .GroupBy(e => e.Name, (_, eqs) => new InheritedAttributeEquationGroupModel(this, eqs))
                              .ToFixedSet());
    }

    /// <summary>
    /// The distinct attributes with the same name that are inherited from supertypes.
    /// </summary>
    public IEnumerable<AttributeModel> InheritedAttributesNamedSameAs(AttributeModel attribute)
        => InheritedAttributesNamed(attribute.Name);
    private IEnumerable<AttributeModel> InheritedAttributesNamed(string name)
        => InheritedAttributes.Where(p => p.Name == name);

    /// <summary>
    /// All the distinct attributes with the same name that are inherited from supertypes at all levels.
    /// </summary>
    public IEnumerable<AttributeModel> AllInheritedAttributesNamedSameAs(AttributeModel attribute)
        => AllInheritedAttributesNamed(attribute.Name);

    private IEnumerable<AttributeModel> AllInheritedAttributesNamed(string name)
        => AllInheritedAttributes.Where(p => p.Name == name).Distinct();

    public IEnumerable<AttributeModel> AttributesNamed(string name)
        => ActualAttributes.Where(p => p.Name == name);

    public EquationModel? EquationFor(AttributeModel attribute)
        => ActualEquations.SingleOrDefault(e => e.Name == attribute.Name);

    private static IEnumerable<T> MostSpecificMembers<T>(IEnumerable<T> attributes)
        where T : IMemberModel
        => attributes.GroupBy(a => a.Name).SelectMany(MostSpecificMembers);

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

    private AttributeModel? ImplicitlyDeclaredAttribute(IGrouping<string, AttributeModel> attributes)
    {
        var name = attributes.Key;
        var (type, count) = attributes.CountBy(p => p.Type).TrySingle();
        return count switch
        {
            0 => null, // Multiple types
            1 => null, // Single property that doesn't need to be redeclared
            _ => ImplicitlyDeclaredAttribute(name, type, attributes.ToFixedList()),
        };
    }

    private AttributeModel? ImplicitlyDeclaredAttribute(string name, TypeModel type, IReadOnlyCollection<AttributeModel> attributes)
    {
        if (TryAllOfType<PropertyModel>(attributes, out _))
            return new PropertyModel(this, name, type);
        if (TryAllOfType<SynthesizedAttributeModel>(attributes, out var synthesized))
            return SynthesizedAttributeModel.TryMerge(this, synthesized);
        if (TryAllOfType<InheritedAttributeModel>(attributes, out var inherited))
            return InheritedAttributeModel.TryMerge(this, inherited);
        return null;
    }

    private static bool TryAllOfType<T>(
        IReadOnlyCollection<AttributeModel> declarations,
        out IFixedList<T> referencedNamespaces)
        where T : AttributeModel
    {
        if (declarations.Count == 0)
        {
            referencedNamespaces = FixedList.Empty<T>();
            return false;
        }

        referencedNamespaces = declarations.OfType<T>().ToFixedList();
        // All of type T when counts match
        return referencedNamespaces.Count == declarations.Count;
    }

    private static bool IsMoreSpecific<T>(T self, T other)
        where T : IMemberModel
        => self.Node.AncestorNodes.Contains(other.Node);

    private IEnumerable<EquationModel> ComputeActualEquations(
        IFixedList<EquationModel> declaredEquations)
    {
        var declaredSynthesizedEquations = declaredEquations.OfType<SynthesizedAttributeEquationModel>().ToList();
        var actualSynthesizedEquations = declaredSynthesizedEquations.Concat(
            InheritedEquations.OfType<SynthesizedAttributeEquationModel>()
                              .AllExcept(declaredSynthesizedEquations, SynthesizedAttributeEquationModel.NameComparer));
        // Inherited equations first because order within the node is important
        var actualInheritedEquations = InheritedEquations.OfType<InheritedAttributeEquationModel>().Concat(
             declaredEquations.OfType<InheritedAttributeEquationModel>());
        return actualSynthesizedEquations.Concat<EquationModel>(actualInheritedEquations);
    }

    private SynthesizedAttributeEquationModel ImplicitlyDeclaredEquation(SynthesizedAttributeModel attribute)
        => new(this, attribute);

    public override string ToString() => Defines.ToString();
}
