using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Semantics;
using Azoth.Tools.Bootstrap.Compiler.CST.Walkers;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Validation;

public sealed class SemanticsFulfillmentValidator : SyntaxWalker
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
            case IIdentifierNameExpressionSyntax syn:
                ValidateFulfilled(syn, syn.Semantics);
                break;
        }
        WalkChildren(syntax);
    }

    private static void ValidateFulfilled(ISyntax syntax, IPromise<ISyntaxSemantics> semantics)
    {
        if (!semantics.IsFulfilled)
            throw new Exception($"Syntax doesn't have fulfilled semantics '{syntax}'");
    }
}
