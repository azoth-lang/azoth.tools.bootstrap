using System;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Validation;

internal class SemanticTreeTypeValidator
{
    private static readonly bool ValidateNotAmbiguous = false;
    private static readonly bool ValidateAntetypes = false;
    private static readonly bool ValidateTypes = false;

    public static void Validate(ISemanticNode node)
    {
        // Validate children first since their antetypes are generally used to compute the parent's antetype
        foreach (var child in node.Children())
            Validate(child);

        if (node is not IExpressionNode expression)
        {
            if (node is IAmbiguousExpressionNode
                && ValidateNotAmbiguous)
                throw new InvalidOperationException("Ambiguous expression node found in semantic tree");
            return;
        }

        if (expression is INamespaceNameNode
            or IFunctionGroupNameNode
            or IMethodGroupNameNode
            or ITypeNameExpressionNode)
            // These nodes should not be expressions and should not have types
            return;

        var expressionSyntax = expression.Syntax;
        if (ValidateAntetypes)
        {
            var expectedAntetype = expressionSyntax.DataType.Result?.ToAntetype();
            // Sometimes the new analysis can come up with types when the old one couldn't. So
            // if the expected antetype is UnknownAntetype, we don't care what the actual antetype is.
            if (expectedAntetype is not UnknownAntetype)
            {
                var antetype = expression.Antetype;
                if (!antetype.Equals(expectedAntetype))
                    throw new InvalidOperationException(
                        $"Expected antetype {expectedAntetype}, but got {antetype}");
            }
        }
        if (ValidateTypes)
        {
            _ = expression.ValueId;
            var expectedExpType = expressionSyntax.DataType.Result;
            var expType = expression.Type;
            if (expType != expectedExpType)
                throw new InvalidOperationException($"Expected type {expectedExpType}, but got {expType}");
        }
    }
}
