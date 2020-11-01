using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.CST.Walkers;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Validation
{
    /// <summary>
    /// Validates that all types are known.
    /// </summary>
    public class TypeKnownValidator : SyntaxWalker
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
                case IClassDeclarationSyntax classDeclaration:
                    classDeclaration.Symbol.Result.DeclaresDataType.Known();
                    // Don't recur into body, we will see those as separate members
                    return;
                case IConstructorDeclarationSyntax constructorDeclaration:
                    WalkChildren(constructorDeclaration);
                    constructorDeclaration.ImplicitSelfParameter.Symbol.Result.DataType.Known();
                    constructorDeclaration.Symbol.Result.ReturnDataType.Known();
                    return;
                case IMethodDeclarationSyntax methodDeclaration:
                    WalkChildren(methodDeclaration);
                    methodDeclaration.Symbol.Result.ReturnDataType.Known();
                    return;
                case IFunctionDeclarationSyntax functionDeclaration:
                    WalkChildren(functionDeclaration);
                    functionDeclaration.Symbol.Result.ReturnDataType.Known();
                    return;
                case IAssociatedFunctionDeclarationSyntax associatedFunctionDeclaration:
                    WalkChildren(associatedFunctionDeclaration);
                    associatedFunctionDeclaration.Symbol.Result.ReturnDataType.Known();
                    return;
                case IParameterSyntax parameter:
                    WalkChildren(parameter);
                    parameter.DataType.Known();
                    return;
                case IFieldDeclarationSyntax fieldDeclaration:
                    WalkChildren(fieldDeclaration);
                    fieldDeclaration.Symbol.Result.DataType.Known();
                    return;
                case ITypeSyntax type:
                    WalkChildren(type);
                    type.NamedType.Known();
                    return;
                case IVariableDeclarationStatementSyntax variableDeclaration:
                    WalkChildren(variableDeclaration);
                    variableDeclaration.Symbol.Result.DataType.Known();
                    return;
                case IForeachExpressionSyntax foreachExpression:
                    WalkChildren(foreachExpression);
                    foreachExpression.DataType.Known();
                    foreachExpression.Symbol.Result.DataType.Known();
                    return;
                case IExpressionSyntax expression:
                    WalkChildren(expression);
                    expression.DataType.Known();
                    return;
            }

            WalkChildren(syntax);
        }
    }
}
