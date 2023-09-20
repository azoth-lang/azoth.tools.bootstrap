using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

[GeneratedCode("AzothCompilerCodeGen", null)]
public static class ISyntaxExtensions
{
    [DebuggerStepThrough]
    public static IEnumerable<ISyntax> Children(this ISyntax node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case ICompilationUnitSyntax n:
                foreach (var child in n.UsingDirectives)
                    yield return child;
                foreach (var child in n.Declarations)
                    yield return child;
                yield break;
            case IUsingDirectiveSyntax n:
                yield break;
            case INamespaceDeclarationSyntax n:
                foreach (var child in n.UsingDirectives)
                    yield return child;
                foreach (var child in n.Declarations)
                    yield return child;
                yield break;
            case IClassDeclarationSyntax n:
                foreach (var child in n.GenericParameters)
                    yield return child;
                if (n.BaseType is not null)
                    yield return n.BaseType;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IFunctionDeclarationSyntax n:
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.ReturnType is not null)
                    yield return n.ReturnType;
                yield return n.Body;
                yield break;
            case IAbstractMethodDeclarationSyntax n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.ReturnType is not null)
                    yield return n.ReturnType;
                yield break;
            case IConcreteMethodDeclarationSyntax n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.ReturnType is not null)
                    yield return n.ReturnType;
                yield return n.Body;
                yield break;
            case IConstructorDeclarationSyntax n:
                yield return n.ImplicitSelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                yield return n.Body;
                yield break;
            case IFieldDeclarationSyntax n:
                yield return n.Type;
                if (n.Initializer is not null)
                    yield return n.Initializer;
                yield break;
            case IAssociatedFunctionDeclarationSyntax n:
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.ReturnType is not null)
                    yield return n.ReturnType;
                yield return n.Body;
                yield break;
            case IGenericParameterSyntax n:
                yield break;
            case INamedParameterSyntax n:
                yield return n.Type;
                if (n.DefaultValue is not null)
                    yield return n.DefaultValue;
                yield break;
            case ISelfParameterSyntax n:
                yield return n.Capability;
                yield break;
            case IFieldParameterSyntax n:
                if (n.DefaultValue is not null)
                    yield return n.DefaultValue;
                yield break;
            case IBodySyntax n:
                foreach (var child in n.Statements)
                    yield return child;
                yield break;
            case ISimpleTypeNameSyntax n:
                yield break;
            case IParameterizedTypeSyntax n:
                foreach (var child in n.TypeArguments)
                    yield return child;
                yield break;
            case IOptionalTypeSyntax n:
                yield return n.Referent;
                yield break;
            case ICapabilityTypeSyntax n:
                yield return n.Capability;
                yield return n.ReferentType;
                yield break;
            case IReferenceCapabilitySyntax n:
                yield break;
            case IResultStatementSyntax n:
                yield return n.Expression;
                yield break;
            case IVariableDeclarationStatementSyntax n:
                if (n.Capability is not null)
                    yield return n.Capability;
                if (n.Type is not null)
                    yield return n.Type;
                if (n.Initializer is not null)
                    yield return n.Initializer;
                yield break;
            case IExpressionStatementSyntax n:
                yield return n.Expression;
                yield break;
            case IBlockExpressionSyntax n:
                foreach (var child in n.Statements)
                    yield return child;
                yield break;
            case INewObjectExpressionSyntax n:
                yield return n.Type;
                foreach (var child in n.Arguments)
                    yield return child;
                yield break;
            case IUnsafeExpressionSyntax n:
                yield return n.Expression;
                yield break;
            case IBoolLiteralExpressionSyntax n:
                yield break;
            case IIntegerLiteralExpressionSyntax n:
                yield break;
            case INoneLiteralExpressionSyntax n:
                yield break;
            case IStringLiteralExpressionSyntax n:
                yield break;
            case IAssignmentExpressionSyntax n:
                yield return n.LeftOperand;
                yield return n.RightOperand;
                yield break;
            case IBinaryOperatorExpressionSyntax n:
                yield return n.LeftOperand;
                yield return n.RightOperand;
                yield break;
            case IUnaryOperatorExpressionSyntax n:
                yield return n.Operand;
                yield break;
            case IIdExpressionSyntax n:
                yield return n.Referent;
                yield break;
            case IConversionExpressionSyntax n:
                yield return n.Referent;
                yield return n.ConvertToType;
                yield break;
            case IIfExpressionSyntax n:
                yield return n.Condition;
                yield return n.ThenBlock;
                if (n.ElseClause is not null)
                    yield return n.ElseClause;
                yield break;
            case ILoopExpressionSyntax n:
                yield return n.Block;
                yield break;
            case IWhileExpressionSyntax n:
                yield return n.Condition;
                yield return n.Block;
                yield break;
            case IForeachExpressionSyntax n:
                yield return n.InExpression;
                if (n.Type is not null)
                    yield return n.Type;
                yield return n.Block;
                yield break;
            case IBreakExpressionSyntax n:
                if (n.Value is not null)
                    yield return n.Value;
                yield break;
            case INextExpressionSyntax n:
                yield break;
            case IReturnExpressionSyntax n:
                if (n.Value is not null)
                    yield return n.Value;
                yield break;
            case IInvocationExpressionSyntax n:
                yield return n.Expression;
                foreach (var child in n.Arguments)
                    yield return child;
                yield break;
            case ISimpleNameExpressionSyntax n:
                yield break;
            case ISelfExpressionSyntax n:
                yield break;
            case IQualifiedNameExpressionSyntax n:
                yield return n.Context;
                yield return n.Member;
                yield break;
            case IMoveExpressionSyntax n:
                yield return n.Referent;
                yield break;
            case IFreezeExpressionSyntax n:
                yield return n.Referent;
                yield break;
        }
    }
}
