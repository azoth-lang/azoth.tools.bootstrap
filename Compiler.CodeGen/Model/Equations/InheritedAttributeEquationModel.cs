using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations.Selectors;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class InheritedAttributeEquationModel : ContributorEquationModel
{
    public override InheritedAttributeEquationSyntax Syntax { get; }
    public InheritedAttributeFamilyModel AttributeFamily => attributeFamily.Value;
    private readonly Lazy<InheritedAttributeFamilyModel> attributeFamily;
    public SelectorModel Selector { get; }
    public bool IsAllDescendants => Selector.IsAllDescendants;
    public override TypeModel Type => InheritedToTypes.TrySingle() ?? AttributeFamily.Type;
    public IFixedSet<InheritedAttributeModel> InheritedToAttributes => inheritedToAttributes.Value;
    private readonly Lazy<IFixedSet<InheritedAttributeModel>> inheritedToAttributes;
    /// <summary>
    /// The most specific types that this attribute will need to satisfy.
    /// </summary>
    /// <remarks>This is derived from <see cref="InheritedToAttributes"/>. If two attributes have
    /// types <c>T1</c> and <c>T2</c> where <c>T1 &lt;: T2</c> then only <c>T1</c> will be in the
    /// types that must be inherited to.</remarks>
    public IFixedSet<TypeModel> InheritedToTypes => inheritedToTypes.Value;
    private readonly Lazy<IFixedSet<TypeModel>> inheritedToTypes;

    public InheritedAttributeEquationModel(AspectModel aspect, InheritedAttributeEquationSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name, syntax.IsMethod, syntax.Expression)
    {
        Syntax = syntax;
        Selector = SelectorModel.Create(syntax.Selector);
        attributeFamily = new(ComputeAttributeFamily);
        inheritedToAttributes = new(ComputeInheritedToAttributes);
        inheritedToTypes = new(ComputeInheritedToTypes);
    }

    private InheritedAttributeFamilyModel ComputeAttributeFamily()
        => Aspect.Tree.AllAttributeFamilies.OfType<InheritedAttributeFamilyModel>()
                 .Single(a => a.Name == Name);

    private IFixedSet<InheritedAttributeModel> ComputeInheritedToAttributes()
    {
        if (NodeSymbol.ShortName == "PackageFacetSymbol" && Name == "Facet")
            Debugger.Break();

        var inheritedToNodes = new HashSet<TreeNodeModel>();
        foreach (var childAttribute in Node.ActualAttributes.Where(a => a.IsChild && Selector.MatchesChild(a)))
        {
            var childNode = childAttribute.Type.ReferencedNode()!;

            // The child node can be a direct child, so there would be no equation on an intervening
            // node to provide a value for the inherited attribute.
            inheritedToNodes.AddRange(childNode.ConcreteSubtypes());

            if (Selector.IsBroadcast)
            {
                // Recur down the tree to find all nodes that are descendants of the child node and
                // don't have other equations that would provide a value for the inherited attribute.
                var visitedNodes = new HashSet<TreeNodeModel>();
                ComputeInheritedToBroadcastAttributes(childNode, visitedNodes, inheritedToNodes);
            }
        }

        return MatchingAttributes(inheritedToNodes).ToFixedSet();
    }

    private void ComputeInheritedToBroadcastAttributes(
        TreeNodeModel node,
        HashSet<TreeNodeModel> visitedNodes,
        HashSet<TreeNodeModel> inheritedToNodes)
    {
        // If the node isn't concrete, we can track whether it has been visited already. (Concrete
        // nodes need to be checked inside the loop because if they are added here, they will be
        // skipped in the loop.)
        if (node.IsAbstract && !visitedNodes.Add(node))
            return; // Already visited
        foreach (var concreteNode in node.ConcreteSubtypes())
        {
            if (!visitedNodes.Add(concreteNode))
                continue; // Already visited

            var equationGroup = concreteNode.InheritedAttributeEquationGroups.Where(g => g.Name == Name).TrySingle();

            foreach (var childAttribute in concreteNode.ActualAttributes.Where(a => a.IsChild))
            {
                var equationsCovering = EquationsCovering(equationGroup, childAttribute);
                bool fullyCoverChild = FullyCoverChild(equationsCovering);
                bool fullyCoverBroadcast = FullyCoverBroadcast(equationsCovering);
                var childNode = childAttribute.Type.ReferencedNode()!;
                if (!fullyCoverChild)
                    inheritedToNodes.AddRange(childNode.ConcreteSubtypes());
                if (!fullyCoverChild || !fullyCoverBroadcast)
                    ComputeInheritedToBroadcastAttributes(childNode, visitedNodes, inheritedToNodes);
            }
        }
    }

    private static IFixedList<InheritedAttributeEquationModel> EquationsCovering(
        InheritedAttributeEquationGroupModel? equationGroup, AttributeModel childAttribute)
        => (equationGroup?.Equations.Where(e => e.Selector.MatchesChild(childAttribute)) ?? []).ToFixedList();

    /// <summary>
    /// Whether these equations fully cover the child they select for.
    /// </summary>
    private static bool FullyCoverChild(IEnumerable<InheritedAttributeEquationModel> equations)
        // Most selectors fully cover their child. The only way it could not be fully covered
        // is if there were only index selectors.
        => !equations.All(eq => eq.Selector is ChildAtIndexSelectorModel);

    /// <summary>
    /// Whether these equations fully cover broadcasting to all descendants the child.
    /// </summary>
    private static bool FullyCoverBroadcast(IFixedList<InheritedAttributeEquationModel> equations)
        => equations.Where(eq => eq.Selector is not ChildAtIndexSelectorModel)
                    .Any(eq => eq.Selector.IsBroadcast);

    private IEnumerable<InheritedAttributeModel> MatchingAttributes(IEnumerable<TreeNodeModel> nodes)
        => nodes.SelectMany(n => n.ActualAttributes)
                .OfType<InheritedAttributeModel>()
                .Where(AttributeFamily.Instances.Contains);

    private IFixedSet<TypeModel> ComputeInheritedToTypes()
        => InheritedToAttributes.Select(a => a.Type).MostSpecificTypes().ToFixedSet();

    public override string ToString()
    {
        var parameters = IsMethod ? "()" : "";
        return $"= {NodeSymbol}.{Selector}.{Name}{parameters}";
    }
}
