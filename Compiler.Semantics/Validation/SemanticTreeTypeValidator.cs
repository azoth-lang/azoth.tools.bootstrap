using System;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Validation;

internal class SemanticTreeTypeValidator
{
    private static readonly bool ValidateTypes = false;

    public static void Validate(ISemanticNode node)
    {
        // Validate children first since their antetypes are generally used to compute the parent's
        // antetype.
        foreach (var child in node.Children())
            Validate(child);

        if (node is not IExpressionNode expression)
        {
            var nodeSyntax = node.Syntax;
            if (node is IAmbiguousExpressionNode)
                throw new InvalidOperationException($"Ambiguous expression node {node.GetType().GetFriendlyName()}: `{nodeSyntax}` found in semantic tree");
            return;
        }

        if (expression.ShouldNotBeExpression())
            // These nodes should not be expressions and should not have types
            return;

        ValidateAntetype(expression);

        ValidateType(node, expression);
    }

    private static void ValidateAntetype(IExpressionNode expression)
    {
        if (expression is IImplicitConversionExpressionNode)
            // Don't validate implicit conversion expressions since they don't match up with the
            // syntax tree exactly (conversions on syntax tree won't be properly separated).
            return;

        var expectedAntetype = expression.Syntax.DataType.Result?.ToAntetype();
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

    private static void ValidateType(ISemanticNode node, IExpressionNode expression)
    {
        if (!ValidateTypes) return;

        if (expression is IImplicitConversionExpressionNode)
            // Don't validate implicit conversion expressions since they don't match up with the
            // syntax tree exactly (conversions on syntax tree won't be properly separated).
            return;

        // The handling of freeze and move expressions has changed. So skip validating their child
        // expressions.
        if (node is IChildNode { Parent: IFreezeExpressionNode or IMoveExpressionNode })
            return;

        _ = expression.ValueId;
        var isConversion = expression is IFreezeExpressionNode { IsImplicit: true }
            or IMoveExpressionNode { IsImplicit: true } or IImplicitTempMoveExpressionNode;
        var expressionSyntax = expression.Syntax;
        var expectedType = isConversion ? expressionSyntax.ConvertedDataType : expressionSyntax.DataType.Result;
        // Sometimes the new analysis can come up with types when the old one couldn't. So
        // if the expected type is UnknownType, we don't care what the actual type is.
        if (expectedType is not UnknownType)
        {
            var type = expression.Type;
            if (type != expectedType)
                throw new InvalidOperationException(
                    $"Expected type `{expectedType?.ToILString()}`, but got `{type.ToILString()}`");
        }
    }
}
