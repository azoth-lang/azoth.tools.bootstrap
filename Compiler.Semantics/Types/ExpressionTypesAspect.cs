using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

public static class ExpressionTypesAspect
{
    public static ValueId Expression_ValueId(IExpressionNode node)
    {
        if (node is ITypeNameExpressionNode or IFunctionGroupNameNode or INamespaceNameNode)
            // TODO remove this hack, these names shouldn't have value ids
            return node.ValueIdScope().CreateValueId();

        return (node.Predecessor()?.ValueId.Scope ?? node.ValueIdScope()).CreateValueId();
    }

    public static void NewObjectExpression_ContributeDiagnostics(INewObjectExpressionNode node, Diagnostics diagnostics)
        => CheckConstructingType(node.ConstructingType, diagnostics);

    private static void CheckConstructingType(ITypeNameNode node, Diagnostics diagnostics)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IStandardTypeNameNode n:
                CheckTypeArgumentsAreConstructable(n, diagnostics);
                break;
            case ISpecialTypeNameNode n:
                diagnostics.Add(TypeError.SpecialTypeCannotBeUsedHere(node.File, n.Syntax));
                break;
            case IQualifiedTypeNameNode n:
                diagnostics.Add(TypeError.TypeParameterCannotBeUsedHere(node.File, n.Syntax));
                break;
        }
    }

    public static void CheckTypeArgumentsAreConstructable(
        IStandardTypeNameNode node,
        Diagnostics diagnostics)
    {
        var bareType = node.BareType;
        if (bareType is null) return;

        foreach (GenericParameterArgument arg in bareType.GenericParameterArguments)
            if (!arg.IsConstructable())
                diagnostics.Add(TypeError.CapabilityNotCompatibleWithConstraint(node.File, node.Syntax,
                    arg.Parameter, arg.Argument));
    }
}
