using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Symbols;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Types;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Syntax.Equations;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

public sealed class AggregateAttributeEquationModel : ContributorEquationModel
{
    public override AggregateAttributeEquationSyntax Syntax { get; }
    public AggregateAttributeFamilyModel AttributeFamily => attributeFamily.Value;
    private readonly Lazy<AggregateAttributeFamilyModel> attributeFamily;
    public override TypeModel Type => AttributeFamily.Type;
    public TypeModel FromType => AttributeFamily.FromType;
    public FixedDictionary<TreeNodeModel, IFixedSet<AggregateAttributeModel>> Attributes => attributes.Value;
    private readonly Lazy<FixedDictionary<TreeNodeModel, IFixedSet<AggregateAttributeModel>>> attributes;

    public AggregateAttributeEquationModel(AspectModel aspect, AggregateAttributeEquationSyntax syntax)
        : base(aspect, Symbol.CreateInternalFromSyntax(aspect.Tree, syntax.Node), syntax.Name, false, null)
    {
        Syntax = syntax;
        attributeFamily = new(ComputeAttributeFamily<AggregateAttributeFamilyModel>);
        attributes = new(ComputeAttributes);
    }

    private FixedDictionary<TreeNodeModel, IFixedSet<AggregateAttributeModel>> ComputeAttributes()
        // Since each aggregate actually emits from the concrete subtypes, each one must be checked
        // to ensure that it contributes to some attributes.
        => Node.ConcreteSubtypes()
               .ToFixedDictionary(Functions.Identity, ComputeAttributes);

    private IFixedSet<AggregateAttributeModel> ComputeAttributes(TreeNodeModel node)
    {
        var nodesVisited = new HashSet<TreeNodeModel>();
        var attributes = new HashSet<AggregateAttributeModel>();
        ComputeAttributes(node, nodesVisited, attributes);
        return attributes.ToFixedSet();
    }
    private void ComputeAttributes(
        TreeNodeModel node,
        HashSet<TreeNodeModel> nodesVisited,
        HashSet<AggregateAttributeModel> attributes)
    {
        if (!nodesVisited.Add(node))
            return; // Node already visited

        var matchingAttributes = node.AttributesNamed(Name).OfType<AggregateAttributeModel>().ToList();
        if (matchingAttributes.Any())
        {
            attributes.AddRange(matchingAttributes);
            return;
        }

        foreach (var parent in node.TreeParentNodes)
            ComputeAttributes(parent, nodesVisited, attributes);
    }

    public override string ToString() => $"= {NodeSymbol}.↑.{Name}";
}
