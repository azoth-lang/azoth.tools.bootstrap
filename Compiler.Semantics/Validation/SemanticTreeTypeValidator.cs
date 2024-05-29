using System;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Validation;

internal class SemanticTreeTypeValidator
{
    private static readonly bool ValidateTypes = false;

    public static void Validate(ISemanticNode node)
    {
        if (node is IExpressionNode expression && ValidateTypes)
        {
            _ = expression.ValueId;
            var expectedExpType = expression.Syntax.DataType.Result;
            var expType = expression.Type;
            if (expType != expectedExpType)
                throw new InvalidOperationException($"Expected type {expectedExpType}, but got {expType}");
        }

        foreach (var child in node.Children())
            Validate(child);
    }
}
