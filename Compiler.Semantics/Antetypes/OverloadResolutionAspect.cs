using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal static class OverloadResolutionAspect
{
    public static IFixedSet<IFunctionLikeDeclarationNode> FunctionInvocationExpression_CompatibleDeclarations(
        IFunctionInvocationExpressionNode node)
    {
        // For now, overload resolution is based on number of arguments only, not their types. This
        // avoids the issues with flow typing interacting with overload resolution. In the future,
        // we may want to revisit this and implement a more sophisticated overload resolution algorithm.
        var arity = node.Arguments.Count;
        return node.FunctionGroup.ReferencedDeclarations.Where(d => d.Type.Arity == arity).ToFixedSet();
    }

    public static IFunctionLikeDeclarationNode? FunctionInvocationExpression_ReferencedDeclaration(
        IFunctionInvocationExpressionNode node)
        => node.CompatibleDeclarations.TrySingle();

    public static void FunctionInvocationExpression_ContributeDiagnostics(
        IFunctionInvocationExpressionNode node,
        Diagnostics diagnostics)
    {
        if (node.ReferencedDeclaration is not null)
            return;

        switch (node.CompatibleDeclarations.Count)
        {
            case 0:
                diagnostics.Add(NameBindingError.CouldNotBindFunction(node.File, node.Syntax));
                break;
            case 1:
                throw new UnreachableException("ReferencedDeclaration would not be null");
            default:
                // TODO add back when overloading is fixed
                //diagnostics.Add(NameBindingError.AmbiguousFunctionCall(node.File, node.Syntax));
                break;
        }
    }
}
