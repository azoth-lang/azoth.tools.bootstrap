using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

public static class ShouldEmit
{
    public static bool Class(TreeNodeModel node)
        => !node.IsAbstract;

    public static bool EquationPartialImplementation(EquationModel equation)
        => equation.Expression is null;

    public static bool CollectContributors(AggregateAttributeFamilyModel family, TreeNodeModel node)
        => node.HasAttribute(family) || OverrideContribute(family, node);

    public static bool CollectContributorsFromChildren(AggregateAttributeFamilyModel family, TreeNodeModel node)
        => !node.HasAttribute(family);

    private static bool HasAttribute(this TreeNodeModel node, AggregateAttributeFamilyModel family)
        => node.ActualAttributes.OfType<AggregateAttributeModel>().Any(attr => attr.AttributeFamily == family);

    public static bool OverrideContribute(AggregateAttributeFamilyModel family, TreeNodeModel node)
        => node.ActualEquations.OfType<AggregateAttributeEquationModel>().Any(eq => eq.AttributeFamily == family);

    public static bool Initial(CircularAttributeModel attribute)
        => attribute.InitialExpression is null;

    public static bool OnClass(AttributeModel attribute)
        => attribute switch
        {
            ContextAttributeModel _ => true,
            AggregateAttributeModel _ => true,
            CollectionAttributeModel _ => true,
            LocalAttributeModel _ => false,
            TreeAttributeModel _ => false,
            ParentAttributeModel _ => false,
            IntertypeMethodAttributeModel _ => false,
            _ => throw ExhaustiveMatch.Failed(attribute)
        };
}
