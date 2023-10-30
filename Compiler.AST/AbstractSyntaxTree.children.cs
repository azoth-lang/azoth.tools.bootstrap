using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.AST;

[GeneratedCode("AzothCompilerCodeGen", null)]
public static class IAbstractSyntaxExtensions
{
    [DebuggerStepThrough]
    public static IEnumerable<IAbstractSyntax> Children(this IAbstractSyntax node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IClassDeclaration n:
                if (n.BaseClass is not null)
                    yield return n.BaseClass;
                foreach (var child in n.Supertypes)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case ITraitDeclaration n:
                foreach (var child in n.Supertypes)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IFunctionDeclaration n:
                foreach (var child in n.Attributes)
                    yield return child;
                foreach (var child in n.Parameters)
                    yield return child;
                yield return n.Body;
                yield break;
            case IAbstractMethodDeclaration n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                yield break;
            case IConcreteMethodDeclaration n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                yield return n.Body;
                yield break;
            case IConstructorDeclaration n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                yield return n.Body;
                yield break;
            case IFieldDeclaration n:
                yield break;
            case IAssociatedFunctionDeclaration n:
                foreach (var child in n.Parameters)
                    yield return child;
                yield return n.Body;
                yield break;
            case IAttribute n:
                yield break;
            case INamedParameter n:
                if (n.DefaultValue is not null)
                    yield return n.DefaultValue;
                yield break;
            case ISelfParameter n:
                yield break;
            case IFieldParameter n:
                if (n.DefaultValue is not null)
                    yield return n.DefaultValue;
                yield break;
            case IBody n:
                foreach (var child in n.Statements)
                    yield return child;
                yield break;
            case IResultStatement n:
                yield return n.Expression;
                yield break;
            case IVariableDeclarationStatement n:
                if (n.Initializer is not null)
                    yield return n.Initializer;
                yield break;
            case IExpressionStatement n:
                yield return n.Expression;
                yield break;
            case IBindingPattern n:
                yield break;
            case IBindingContextPattern n:
                yield return n.Pattern;
                yield break;
            case IOptionalPattern n:
                yield return n.Pattern;
                yield break;
            case IBlockExpression n:
                foreach (var child in n.Statements)
                    yield return child;
                yield break;
            case INewObjectExpression n:
                foreach (var child in n.Arguments)
                    yield return child;
                yield break;
            case IUnsafeExpression n:
                yield return n.Expression;
                yield break;
            case IBoolLiteralExpression n:
                yield break;
            case IIntegerLiteralExpression n:
                yield break;
            case INoneLiteralExpression n:
                yield break;
            case IStringLiteralExpression n:
                yield break;
            case IAssignmentExpression n:
                yield return n.LeftOperand;
                yield return n.RightOperand;
                yield break;
            case IBinaryOperatorExpression n:
                yield return n.LeftOperand;
                yield return n.RightOperand;
                yield break;
            case IUnaryOperatorExpression n:
                yield return n.Operand;
                yield break;
            case IPatternMatchExpression n:
                yield return n.Referent;
                yield return n.Pattern;
                yield break;
            case IIfExpression n:
                yield return n.Condition;
                yield return n.ThenBlock;
                if (n.ElseClause is not null)
                    yield return n.ElseClause;
                yield break;
            case ILoopExpression n:
                yield return n.Block;
                yield break;
            case IWhileExpression n:
                yield return n.Condition;
                yield return n.Block;
                yield break;
            case IForeachExpression n:
                yield return n.InExpression;
                yield return n.Block;
                yield break;
            case IBreakExpression n:
                if (n.Value is not null)
                    yield return n.Value;
                yield break;
            case INextExpression n:
                yield break;
            case IReturnExpression n:
                if (n.Value is not null)
                    yield return n.Value;
                yield break;
            case IImplicitNumericConversionExpression n:
                yield return n.Expression;
                yield break;
            case IImplicitOptionalConversionExpression n:
                yield return n.Expression;
                yield break;
            case IImplicitLiftedConversionExpression n:
                yield return n.Expression;
                yield break;
            case IExplicitNumericConversionExpression n:
                yield return n.Expression;
                yield break;
            case IFunctionInvocationExpression n:
                foreach (var child in n.Arguments)
                    yield return child;
                yield break;
            case IMethodInvocationExpression n:
                yield return n.Context;
                foreach (var child in n.Arguments)
                    yield return child;
                yield break;
            case INameExpression n:
                yield break;
            case ISelfExpression n:
                yield break;
            case IFieldAccessExpression n:
                yield return n.Context;
                yield break;
            case IMoveExpression n:
                yield return n.Referent;
                yield break;
            case IFreezeExpression n:
                yield return n.Referent;
                yield break;
            case IShareExpression n:
                yield return n.Referent;
                yield break;
            case IIdExpression n:
                yield return n.Referent;
                yield break;
            case IRecoverConstExpression n:
                yield return n.Value;
                yield break;
            case IRecoverIsolationExpression n:
                yield return n.Value;
                yield break;
        }
    }
}
