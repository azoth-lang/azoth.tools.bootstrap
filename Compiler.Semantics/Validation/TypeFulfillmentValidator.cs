using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Walkers;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Validation;

/// <summary>
/// Validates that all types are fulfilled. That is that everything has an
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
            case ITypeSyntax syn:
                WalkChildren(syn);
                ValidateAssigned(syn, syn.NamedType);
                return;
            case IForeachExpressionSyntax syn:
                WalkChildren(syn);
                ValidateAssigned(syn, syn.ConvertedDataType);
                return;
            case ISelfExpressionSyntax syn:
                ValidateAssigned(syn, syn.DataType);
                ValidateAssigned(syn, syn.Pseudotype);
                return;
            case INameExpressionSyntax exp:
                WalkChildren(exp);
                ValidateFulfilled(exp, exp.DataType);
                return;
            case ITypedExpressionSyntax exp:
                WalkChildren(exp);
                ValidateAssigned(exp, exp.ConvertedDataType);
                return;
        }

        WalkChildren(syntax);
    }

    private static void ValidateFulfilled(ISyntax syntax, IPromise<DataType?> type)
    {
        if (!type.IsFulfilled)
            throw new Exception($"Syntax doesn't have a fulfilled type '{syntax}'");
    }

    private static void ValidateAssigned(ISyntax syntax, DataType? type)
    {
        if (type is null)
            throw new Exception($"Syntax has no type '{syntax}'");
    }

    private static void ValidateAssigned(ISyntax syntax, IPromise<DataType?> type)
    {
        ValidateFulfilled(syntax, type);
        ValidateAssigned(syntax, type.Result);
    }

    private static void ValidateFulfilled(ISyntax syntax, IPromise<Pseudotype?> type)
    {
        if (!type.IsFulfilled)
            throw new Exception($"Syntax doesn't have a fulfilled pseudotype '{syntax}'");
    }

    private static void ValidateAssigned(ISyntax syntax, Pseudotype? type)
    {
        if (type is null)
            throw new Exception($"Syntax has no pseudotype '{syntax}'");
    }

    private static void ValidateAssigned(ISyntax syntax, IPromise<Pseudotype?> type)
    {
        ValidateFulfilled(syntax, type);
        ValidateAssigned(syntax, type.Result);
    }
}
