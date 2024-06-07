using System;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Validation;

internal class SemanticTreeTypeValidator
{
    private static readonly bool ValidateAntetypes = false;
    private static readonly bool ValidateTypes = false;

    public static void Validate(ISemanticNode node)
    {
        if (node is IExpressionNode expression
            and not INamespaceNameNode
            and not IFunctionGroupNameNode
            and not IMethodGroupNameNode
            and not ITypeNameExpressionNode)
        {
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

        foreach (var child in node.Children())
            Validate(child);
    }
}
