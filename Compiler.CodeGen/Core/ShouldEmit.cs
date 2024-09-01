using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.AttributeFamilies;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CodeGen.Model.Equations;

namespace Azoth.Tools.Bootstrap.Compiler.CodeGen.Core;

public static class ShouldEmit
{
    public static bool Class(TreeNodeModel node)
        => !node.IsAbstract;

    public static bool EquationPartialImplementation(EquationModel equation)
        => equation.Expression is null;

    public static bool CollectContributors(AggregateAttributeFamilyModel family, TreeNodeModel node)
    => node.HasAttribute(family) || OverrideContribute(family, node);

    private static bool HasAttribute(this TreeNodeModel node, AggregateAttributeFamilyModel family)
        => node.ActualAttributes.OfType<AggregateAttributeModel>().Any(attr => attr.AttributeFamily == family);

    public static bool OverrideContribute(AggregateAttributeFamilyModel family, TreeNodeModel node)
        => node.ActualEquations.OfType<AggregateAttributeEquationModel>().Any(eq => eq.AttributeFamily == family);
}
