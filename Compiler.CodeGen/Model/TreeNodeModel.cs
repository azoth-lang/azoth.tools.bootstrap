using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Snippets;
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

    /// <summary>
    /// Whether this node type may have rewrite rules.
    /// </summary>
    public bool MayHaveRewrite => mayHaveRewrite.Value;
    private readonly Lazy<bool> mayHaveRewrite;
    public InternalSymbol Defines { get; }
    public SymbolTypeModel DefinesType { get; }
    /// <summary>
    /// The directly declared supertypes of this node.
    /// </summary>
    public IFixedSet<Symbol> DeclaredSupertypes { get; }

    #region Inheritance Relationships
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

    public IFixedSet<TreeNodeModel> CandidateFinalNodes => candidateFinalNodes.Value;
    private readonly Lazy<IFixedSet<TreeNodeModel>> candidateFinalNodes;

    /// <summary>
    /// The node that this node will be transformed into in the final tree.
    /// </summary>
    /// <remarks>If <see cref="IsTemp"/> is false, it will be this node. Otherwise, it will be the
    /// non-temp node type below this.</remarks>
    public TreeNodeModel? FinalNode => finalNodeType.Value;
    private readonly Lazy<TreeNodeModel?> finalNodeType;
    #endregion

    #region Snippets
    public IFixedList<SnippetModel> Snippets => snippets.Value;
    private readonly Lazy<IFixedList<SnippetModel>> snippets;
    #endregion

    #region Attributes
    /// <summary>
    /// The tree attributes declared for the node in the definition file.
    /// </summary>
    public IFixedList<TreeAttributeModel> DeclaredTreeAttributes { get; }

    /// <summary>
    /// Attributes (including properties) declared against this node in both the tree and aspect
    /// definition files.
    /// </summary>
    public IFixedList<AttributeModel> DeclaredAttributes => declaredAttributes.Value;
    private readonly Lazy<IFixedList<AttributeModel>> declaredAttributes;

    /// <summary>
    /// The nodes that are declared as children of this node in the tree. This is NOT inheritance.
    /// This is based on child attributes.
    /// </summary>
    public IFixedSet<TreeNodeModel> TreeChildNodes => treeChildNodes.Value;
    private readonly Lazy<IFixedSet<TreeNodeModel>> treeChildNodes;

    /// <summary>
    /// The nodes that are parents of this node in the tree. This is NOT inheritance. This is based
    /// child attributes.
    /// </summary>
    public IFixedSet<TreeNodeModel> TreeParentNodes => treeParentNodes.Value;
    private readonly Lazy<IFixedSet<TreeNodeModel>> treeParentNodes;

    /// <summary>
    /// Whether this node can be the root of a tree.
    /// </summary>
    public bool IsRoot => isRoot.Value;
    private readonly Lazy<bool> isRoot;

    /// <summary>
    /// Attributes that are implicitly declared on the node.
    /// </summary>
    /// <remarks>This includes attributes declared because multiple supertypes define the
    /// same attribute with the same type. It also includes the implicit parent attribute.</remarks>
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
    #endregion

    #region Equations
    /// <summary>
    /// Equations declared against this specific node in the definition files.
    /// </summary>
    public IFixedList<EquationModel> DeclaredEquations => declaredEquations.Value;
    private readonly Lazy<IFixedList<EquationModel>> declaredEquations;

    public IFixedList<LocalAttributeEquationModel> ImplicitlyDeclaredEquations => implicitlyDeclaredEquations.Value;
    private readonly Lazy<IFixedList<LocalAttributeEquationModel>> implicitlyDeclaredEquations;

    public IFixedList<EquationModel> AllDeclaredEquations => allDeclaredEquations.Value;
    private readonly Lazy<IFixedList<EquationModel>> allDeclaredEquations;

    public IFixedList<SoleEquationModel> EquationsRequiringEmit => equationsRequiringEmit.Value;
    private readonly Lazy<IFixedList<SoleEquationModel>> equationsRequiringEmit;

    public IFixedList<EquationModel> InheritedEquations => inheritedEquations.Value;
    private readonly Lazy<IFixedList<EquationModel>> inheritedEquations;

    public IFixedList<EquationModel> ActualEquations => actualEquations.Value;
    private readonly Lazy<IFixedList<EquationModel>> actualEquations;

    public IFixedSet<InheritedAttributeEquationGroupModel> InheritedAttributeEquationGroups
        => inheritedAttributeEquationGroups.Value;
    private readonly Lazy<IFixedSet<InheritedAttributeEquationGroupModel>> inheritedAttributeEquationGroups;
    #endregion

    #region Rewrite Rules
    public IFixedList<RewriteRuleModel> DeclaredRewriteRules => declaredRewriteRules.Value;
    private readonly Lazy<IFixedList<RewriteRuleModel>> declaredRewriteRules;

    public IFixedList<RewriteRuleModel> InheritedRewriteRules => inheritedRewriteRules.Value;
    private readonly Lazy<IFixedList<RewriteRuleModel>> inheritedRewriteRules;

    public IFixedList<RewriteRuleModel> ActualRewriteRules => actualRewriteRules.Value;
    private readonly Lazy<IFixedList<RewriteRuleModel>> actualRewriteRules;
    #endregion

    public TreeNodeModel(TreeModel tree, TreeNodeSyntax syntax)
    {
        Tree = tree;
        Syntax = syntax;
        Defines = Symbol.CreateInternalFromSyntax(tree, syntax.Defines);
        DefinesType = new SymbolTypeModel(Defines);
        DeclaredSupertypes = syntax.Supertypes.Select(s => Symbol.CreateFromSyntax(tree, s)).ToFixedSet();
        // Add root supertype if no supertypes are declared
        if (Tree.RootSupertype is { } root && root != Defines && !DeclaredSupertypes.Any(s => s is InternalSymbol))
            DeclaredSupertypes = DeclaredSupertypes.Append(root).ToFixedSet();

        isSyncLockRequired = new(() => ActualAttributes.Any(a => a.IsSyncLockRequired)
                                       || ActualEquations.Any(eq => eq.IsSyncLockRequired));

        mayHaveRewrite = new(()
            // Temp nodes ought to have rewrites. If they don't already, they will be added soon.
            => !ActualRewriteRules.IsEmpty || IsTemp || DescendantNodes.Any(n => n.MayHaveRewrite));

        // Inheritance Relationships
        supertypeNodes = new(() => DeclaredSupertypes.OfType<InternalSymbol>().Select(s => s.ReferencedNode)
                                             .EliminateRedundantRules().ToFixedSet());
        ancestorNodes = new(() => SupertypeNodes.Concat(SupertypeNodes.SelectMany(p => p.AncestorNodes)).ToFixedSet());
        childNodes = new(() => Tree.Nodes.Where(r => r.SupertypeNodes.Contains(this)).ToFixedSet());
        descendantNodes = new(() => ChildNodes.Concat(ChildNodes.SelectMany(r => r.DescendantNodes)).ToFixedSet());
        inheritsFromRootSupertype = new(() => Tree.RootSupertype is not null
            && (Defines == Tree.RootSupertype || SupertypeNodes.Any(s => s.InheritsFromRootSupertype)));
        candidateFinalNodes = new(() => DescendantNodes.Where(n => !n.IsTemp)
                                                       .Select(n => n.DefinesType)
                                                       .MostGeneralTypes()
                                                       .Select(t => t.ReferencedNode()!)
                                                       .ToFixedSet());
        finalNodeType = IsTemp ? new(() => CandidateFinalNodes.TrySingle()) : new(this);

        // Snippets
        snippets = new(() => Tree.Aspects.SelectMany(a => a.Snippets).Where(s => s.Node == this).ToFixedList());

        // Attributes
        DeclaredTreeAttributes = syntax.DeclaredAttributes.Select(a => TreeAttributeModel.Create(this, a)).ToFixedList();
        declaredAttributes = new(() => DeclaredTreeAttributes.Concat<AttributeModel>(Tree.Aspects.SelectMany(a => a.DeclaredAttributes).Where(a => a.NodeSymbol == Defines)).ToFixedList());
        treeChildNodes = new(() =>
            // Skip placeholders, the other attributes will give the true children (avoid cyclic dependencies)
            DeclaredAttributes.Where(p => !p.IsPlaceholder && p.IsChild)
                              .Select(a => a.Type.ReferencedNode()!)
                              .ToFixedSet());
        treeParentNodes = new(()
            => Tree.Nodes.Where(n => n.TreeChildNodes
                                      .SelectMany(child => child.DescendantNodes.Append(child))
                                      .Contains(this)).ToFixedSet());
        isRoot = new(() => !Tree.TreeChildNodes.Contains(this)
                           && (!IsAbstract || DescendantNodes.Where(n => !n.IsAbstract).Any(n => n.IsRoot)));
        allInheritedAttributes = new(()
            => SupertypeNodes.SelectMany(r => r.AllAttributes).Distinct().ToFixedList());
        allAttributes = new(() => AllDeclaredAttributes.Concat(AllInheritedAttributes).ToFixedList());
        inheritedAttributes = new(()
            => MostSpecificMembers(SupertypeNodes.SelectMany(r => r.ActualAttributes).Distinct()).ToFixedList());
        implicitlyDeclaredAttributes = new(ComputeImplicitlyDeclaredAttributes);
        attributesRequiringDeclaration = new(()
            => AllDeclaredAttributes.Where(p => p.IsDeclarationRequired).ToFixedList());
        actualAttributes = new(() =>
        {
            var attributeLookup = AttributesRequiringDeclaration
                                 .Concat(InheritedAttributes.AllExcept<AttributeModel>(AttributesRequiringDeclaration, IMemberModel.NameIsPlaceholderComparer))
                                 .ToLookup(p => p.Name);
            // Because of placeholders, there can be duplicate names. When there is both a placeholder
            // and a regular attribute, the regular placeholder order is used.
            var attributeOrder = SupertypeNodes.SelectMany(s => s.ActualAttributes)
                                               .Except(AllDeclaredAttributes, IMemberModel.NameIsPlaceholderComparer)
                                               .Concat(AllDeclaredAttributes)
                                               .DistinctKeepingPlaceholders().ToFixedList();

            return attributeOrder.SelectMany(p => attributeLookup[p.Name]).ToFixedList();
        });

        // Equations
        declaredEquations = new(() => Tree.Aspects.SelectMany(a => a.DeclaredEquations).Where(e => e.NodeSymbol == Defines).ToFixedList());
        implicitlyDeclaredEquations = new(() =>
            {
                if (IsAbstract) return FixedList.Empty<LocalAttributeEquationModel>();
                var actualEquationsNames = ComputeActualEquations(DeclaredEquations)
                                           .OfType<LocalAttributeEquationModel>()
                                           .Select(eq => eq.Name).ToFixedSet();
                return ActualAttributes.OfType<LocalAttributeModel>()
                                       .Where(a => a.DefaultExpression is null && !actualEquationsNames.Contains(a.Name))
                                       .Select(ImplicitlyDeclaredEquation).ToFixedList();
            });
        allDeclaredEquations = new(() => DeclaredEquations.Concat(ImplicitlyDeclaredEquations).ToFixedList());
        equationsRequiringEmit = new(()
            => AllDeclaredEquations.OfType<SoleEquationModel>()
                                   .Where(eq => eq.RequiresEmitOnNode)
                                   .ExceptBy(AttributesRequiringDeclaration.Select(a => a.Name), eq => eq.Name)
                                   .ToFixedList());
        inheritedEquations = new(ComputeInheritedEquations);
        actualEquations = new(() => ComputeActualEquations(AllDeclaredEquations).ToFixedList());
        inheritedAttributeEquationGroups = new(()
            => ActualEquations.OfType<InheritedAttributeEquationModel>()
                              .GroupBy(e => e.Name, (_, eqs) => new InheritedAttributeEquationGroupModel(this, eqs))
                              .ToFixedSet());

        // Rewrite Rules
        declaredRewriteRules = new(() => Tree.Aspects.SelectMany(a => a.RewriteRules).Where(r => r.Node == this).ToFixedList());
        inheritedRewriteRules = new(() => SupertypeNodes.SelectMany(r => r.ActualRewriteRules).Distinct().ToFixedList());
        actualRewriteRules = new(() => DeclaredRewriteRules.Concat(InheritedRewriteRules).ToFixedList());
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

    public IEnumerable<EquationModel> EquationsNamed(string name)
        => ActualEquations.Where(p => p.Name == name);

    public SoleEquationModel? EquationFor(AttributeModel attribute)
        // Compare by attribute rather than name to avoid issues with inherited attribute equations
        => ActualEquations.OfType<SoleEquationModel>().SingleOrDefault(eq => eq.Attribute == attribute);

    private static IEnumerable<T> MostSpecificMembers<T>(IEnumerable<T> members)
        where T : IMemberModel
        => members.GroupBy(a => a.Name).SelectMany(MostSpecificMembers);

    private static IEnumerable<T> MostSpecificMembers<T>(IGrouping<string, T> members)
        where T : IMemberModel
    {
        var mostSpecific = new List<T>(members.Where(m => m.IsPlaceholder));
        foreach (var member in members.Where(m => !m.IsPlaceholder))
        {
            for (var i = mostSpecific.Count - 1; i >= 0; i--)
            {
                var mostSpecificMember = mostSpecific[i];
                if (IsMoreSpecific(mostSpecificMember, member))
                    goto nextProperty;
                if (IsMoreSpecific(member, mostSpecificMember))
                    mostSpecific.RemoveAt(i);
            }
            mostSpecific.Add(member);

        nextProperty:;
        }
        return mostSpecific;
    }

    private IFixedList<AttributeModel> ComputeImplicitlyDeclaredAttributes()
        => ParentAttribute().ToFixedList().Concat(ImplicitlyDeclaredMergedAttributes().ToFixedList()).ToFixedList();

    private IEnumerable<AttributeModel> ParentAttribute()
    {
        if (Tree.SimplifiedTree || IsRoot || Tree.RootSupertype is null)
            yield break;

        yield return new ParentAttributeModel(this, Tree.RootSupertype);
    }

    private IEnumerable<AttributeModel> ImplicitlyDeclaredMergedAttributes()
        => InheritedAttributes.AllExcept<AttributeModel>(DeclaredAttributes, IMemberModel.NameIsPlaceholderComparer)
                              .Where(a => a is not (ParentAttributeModel or PlaceholderModel))
                              .GroupBy(p => p.Name)
                              .Select(ImplicitlyDeclaredMergedAttribute).WhereNotNull()
                              .Assert(p => p.IsDeclarationRequired, p => new($"Implicit attribute {p} no declared."));

    private AttributeModel? ImplicitlyDeclaredMergedAttribute(IGrouping<string, AttributeModel> attributes)
    {
        var name = attributes.Key;
        var (type, count) = attributes.CountBy(p => p.Type).TrySingle();
        return count switch
        {
            0 => null, // Multiple types
            1 => null, // Single property that doesn't need to be redeclared
            _ => ImplicitlyDeclaredMergedAttribute(name, type, attributes.ToFixedList()),
        };
    }

    private AttributeModel? ImplicitlyDeclaredMergedAttribute(string name, TypeModel type, IReadOnlyCollection<AttributeModel> attributes)
    {
        if (TryAllOfType<PropertyModel>(attributes, out _))
            return new PropertyModel(this, name, type);
        if (TryAllOfType<SynthesizedAttributeModel>(attributes, out var synthesized))
            return SynthesizedAttributeModel.TryMerge(this, synthesized);
        if (TryAllOfType<InheritedAttributeModel>(attributes, out var inherited))
            return ContextAttributeModel.TryMerge(this, inherited);
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

    private IFixedList<EquationModel> ComputeInheritedEquations()
    {
        var inheritedEquations = SupertypeNodes.SelectMany(r => r.ActualEquations).Distinct().ToList();
        // Sole equations need to take the mose specific ones. Contributor equations keep all.
        return MostSpecificMembers(inheritedEquations.OfType<SoleEquationModel>())
               .Concat<EquationModel>(inheritedEquations.OfType<ContributorEquationModel>()).ToFixedList();
    }

    private IEnumerable<EquationModel> ComputeActualEquations(IFixedList<EquationModel> declaredEquations)
    {
        // Sole equations are distinct by name and parameters per node.
        var declaredSoleEquations = declaredEquations.OfType<SoleEquationModel>().ToList();
        var inheritedSoleEquations = InheritedEquations
                                     .OfType<SoleEquationModel>()
                                     .AllExcept(declaredSoleEquations, SoleEquationModel.NameAndParametersComparer);
        var actualSoleEquations = declaredSoleEquations.Concat(inheritedSoleEquations);

        // Contributor equations keep all inherited and declared equations.
        // Inherited equations require that the inherited ones come before ones declared on this node.
        var contributorEquations = InheritedEquations.OfType<ContributorEquationModel>()
            .Concat(declaredEquations.OfType<ContributorEquationModel>());

        return actualSoleEquations
               .Concat<EquationModel>(contributorEquations);
    }

    private LocalAttributeEquationModel ImplicitlyDeclaredEquation(LocalAttributeModel attribute)
        => new(this, attribute);

    public override string ToString() => Defines.ToString();
}
