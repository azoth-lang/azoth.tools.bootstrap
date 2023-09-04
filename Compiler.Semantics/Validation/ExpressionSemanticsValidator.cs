using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Walkers;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Validation;

/// <summary>
/// Validates all expressions have been assigned expression semantics
/// </summary>
public class ExpressionSemanticsValidator : SyntaxWalker
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
            case IExpressionSyntax expression:
                WalkChildren(expression);
                expression.Semantics.Assigned();
                return;
            case ITypeSyntax _:
                return;
        }

        WalkChildren(syntax);
    }
}
