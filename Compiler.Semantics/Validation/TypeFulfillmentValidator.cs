using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Walkers;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Validation;

/// <summary>
/// Validates that all types are fulfilled. That is that everything as an
/// assigned type, even if that type is Unknown.
/// </summary>
public class TypeFulfillmentValidator : SyntaxWalker
{
    public void Validate(IEnumerable<IEntityDeclarationSyntax> entityDeclarations)
    {
        foreach (var declaration in entityDeclarations)
            WalkNonNull(declaration);
    }

    protected override void WalkNonNull(ISyntax syntax)
    {
        switch (syntax)
        {
            case IClassDeclarationSyntax _:
                // Don't recur into body, we will see those as separate members
                return;
            case ITypeSyntax type:
                WalkChildren(type);
                type.NamedType.Assigned();
                return;
            case IForeachExpressionSyntax foreachExpression:
                WalkChildren(foreachExpression);
                foreachExpression.ConvertedDataType.Assigned();
                return;
            case IExpressionSyntax expression:
                WalkChildren(expression);
                expression.ConvertedDataType.Assigned();
                return;
        }

        WalkChildren(syntax);
    }
}
