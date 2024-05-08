using Azoth.Tools.Bootstrap.Compiler.Types.Declared;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static class ParameterAttributes
{
    public static GenericParameter GenericParameter(IGenericParameterNode node)
        => new GenericParameter(node.Constraint.Constraint, node.Name, node.Independence, node.Variance);
}
