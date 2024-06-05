using System;

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
            and not IMethodGroupNameNode)
        {

            if (ValidateAntetypes)
            {
                var expectedAntetype = expression.Syntax.DataType.Result?.ToAntetype();
                var antetype = expression.Antetype;
                if (!antetype.Equals(expectedAntetype))
                    throw new InvalidOperationException($"Expected antetype {expectedAntetype}, but got {antetype}");
            }
            if (ValidateTypes)
            {
                _ = expression.ValueId;
                var expectedExpType = expression.Syntax.DataType.Result;
                var expType = expression.Type;
                if (expType != expectedExpType)
                    throw new InvalidOperationException($"Expected type {expectedExpType}, but got {expType}");
            }
        }

        foreach (var child in node.Children())
            Validate(child);
    }
}
