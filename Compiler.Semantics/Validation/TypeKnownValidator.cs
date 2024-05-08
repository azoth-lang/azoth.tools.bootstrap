using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Walkers;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Validation;

/// <summary>
/// Validates that all types are known.
/// </summary>
public class TypeKnownValidator : SyntaxWalker<bool>
{
    public void Validate(IEnumerable<IEntityDeclarationSyntax> entityDeclarations)
    {
        foreach (var declaration in entityDeclarations)
            WalkNonNull(declaration, true);
    }

    protected override void WalkNonNull(IConcreteSyntax syntax, bool expressionTypeMustBeKnown)
    {
        switch (syntax)
        {
            case IClassDeclarationSyntax syn:
                Walk(syn.BaseTypeName, true);
                // Don't recur into body, we will see those as separate members
                return;
            case IConstructorDeclarationSyntax constructorDeclaration:
                WalkChildren(constructorDeclaration, true);
                constructorDeclaration.SelfParameter.Symbol.Result.Type.Known();
                constructorDeclaration.Symbol.Result.Return.Known();
                return;
            case IMethodDeclarationSyntax methodDeclaration:
                WalkChildren(methodDeclaration, true);
                methodDeclaration.Symbol.Result.Return.Known();
                return;
            case IFunctionDeclarationSyntax functionDeclaration:
                WalkChildren(functionDeclaration, true);
                functionDeclaration.Symbol.Result.Return.Known();
                return;
            case IAssociatedFunctionDeclarationSyntax associatedFunctionDeclaration:
                WalkChildren(associatedFunctionDeclaration, true);
                associatedFunctionDeclaration.Symbol.Result.Return.Known();
                return;
            case IParameterSyntax parameter:
                WalkChildren(parameter, true);
                parameter.DataType.Known();
                return;
            case IFieldDeclarationSyntax fieldDeclaration:
                WalkChildren(fieldDeclaration, true);
                fieldDeclaration.Symbol.Result.Type.Known();
                return;
            case ITypeSyntax type:
                WalkChildren(type, true);
                type.NamedType.Known();
                return;
            case IVariableDeclarationStatementSyntax variableDeclaration:
                WalkChildren(variableDeclaration, true);
                variableDeclaration.Symbol.Result.Type.Known();
                return;
            case IForeachExpressionSyntax foreachExpression:
                WalkChildren(foreachExpression, true);
                foreachExpression.ConvertedDataType.Known();
                foreachExpression.Symbol.Result.Type.Known();
                return;
            case IInvocationExpressionSyntax syn:
                WalkNonNull(syn.Expression, false);
                foreach (var arg in syn.Arguments)
                    WalkNonNull(arg, true);
                syn.ConvertedDataType.Known();
                return;
            case ISelfExpressionSyntax syn:
                syn.ConvertedDataType.Known();
                syn.Pseudotype.Known();
                return;
            case IGenericNameExpressionSyntax syn:
                WalkChildren(syn, true);
                if (expressionTypeMustBeKnown)
                    syn.ConvertedDataType.Known();
                return;
            case IMemberAccessExpressionSyntax syn:
                WalkNonNull(syn.Context, false);
                foreach (var arg in syn.TypeArguments)
                    WalkNonNull(arg, true);
                if (expressionTypeMustBeKnown)
                    syn.ConvertedDataType.Known();
                return;
            case INameExpressionSyntax syn:
                if (expressionTypeMustBeKnown)
                    syn.ConvertedDataType.Known();
                break; // In case a subtype is missed, walk the children
            case ITypedExpressionSyntax syn:
                WalkChildren(syn, true);
                syn.ConvertedDataType.Known();
                return;
        }

        WalkChildren(syntax, true);
    }
}
