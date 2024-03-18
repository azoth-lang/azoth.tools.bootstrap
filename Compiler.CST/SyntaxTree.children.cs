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
                if (n.BaseTypeName is not null)
                    yield return n.BaseTypeName;
                foreach (var child in n.SupertypeNames)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IStructDeclarationSyntax n:
                foreach (var child in n.GenericParameters)
                    yield return child;
                foreach (var child in n.SupertypeNames)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case ITraitDeclarationSyntax n:
                foreach (var child in n.GenericParameters)
                    yield return child;
                foreach (var child in n.SupertypeNames)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IFunctionDeclarationSyntax n:
                foreach (var child in n.Attributes)
                    yield return child;
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.Return is not null)
                    yield return n.Return;
                yield return n.Body;
                yield break;
            case IGenericParameterSyntax n:
                yield return n.Constraint;
                yield break;
            case ISupertypeNameSyntax n:
                foreach (var child in n.TypeArguments)
                    yield return child;
                yield break;
            case IAbstractMethodDeclarationSyntax n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.Return is not null)
                    yield return n.Return;
                yield break;
            case IStandardMethodDeclarationSyntax n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.Return is not null)
                    yield return n.Return;
                yield return n.Body;
                yield break;
            case IGetterMethodDeclarationSyntax n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                yield return n.Return;
                yield return n.Body;
                yield break;
            case ISetterMethodDeclarationSyntax n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.Return is not null)
                    yield return n.Return;
                yield return n.Body;
                yield break;
            case IConstructorDeclarationSyntax n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                yield return n.Body;
                yield break;
            case IInitializerDeclarationSyntax n:
                yield return n.SelfParameter;
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
                if (n.Return is not null)
                    yield return n.Return;
                yield return n.Body;
                yield break;
            case IAttributeSyntax n:
                yield return n.TypeName;
                yield break;
            case INamedParameterSyntax n:
                yield return n.Type;
                if (n.DefaultValue is not null)
                    yield return n.DefaultValue;
                yield break;
            case IConstructorSelfParameterSyntax n:
                yield return n.Capability;
                yield break;
            case IInitializerSelfParameterSyntax n:
                yield return n.Capability;
                yield break;
            case IMethodSelfParameterSyntax n:
                yield return n.Capability;
                yield break;
            case ICapabilitySetSyntax n:
                yield break;
            case IFieldParameterSyntax n:
                if (n.DefaultValue is not null)
                    yield return n.DefaultValue;
                yield break;
            case IReturnSyntax n:
                yield return n.Type;
                yield break;
            case IBlockBodySyntax n:
                foreach (var child in n.Statements)
                    yield return child;
                yield break;
            case IExpressionBodySyntax n:
                yield return n.ResultStatement;
                foreach (var child in n.Statements)
                    yield return child;
                yield break;
            case ISimpleTypeNameSyntax n:
                yield break;
            case IGenericTypeNameSyntax n:
                foreach (var child in n.TypeArguments)
                    yield return child;
                yield break;
            case IOptionalTypeSyntax n:
                yield return n.Referent;
                yield break;
            case ICapabilityTypeSyntax n:
                yield return n.Capability;
                yield return n.Referent;
                yield break;
            case ICapabilitySyntax n:
                yield break;
            case IFunctionTypeSyntax n:
                foreach (var child in n.Parameters)
                    yield return child;
                yield return n.Return;
                yield break;
            case IParameterTypeSyntax n:
                yield return n.Referent;
                yield break;
            case IReturnTypeSyntax n:
                yield return n.Referent;
                yield break;
            case ICapabilityViewpointTypeSyntax n:
                yield return n.Capability;
                yield return n.Referent;
                yield break;
            case ISelfViewpointTypeSyntax n:
                yield return n.Referent;
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
            case IBindingContextPatternSyntax n:
                yield return n.Pattern;
                if (n.Type is not null)
                    yield return n.Type;
                yield break;
            case IBindingPatternSyntax n:
                yield break;
            case IOptionalPatternSyntax n:
                yield return n.Pattern;
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
            case IPatternMatchExpressionSyntax n:
                yield return n.Referent;
                yield return n.Pattern;
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
            case IMemberAccessExpressionSyntax n:
                yield return n.Context;
                yield return n.Member;
                yield break;
            case IMoveExpressionSyntax n:
                yield return n.Referent;
                yield break;
            case IFreezeExpressionSyntax n:
                yield return n.Referent;
                yield break;
            case IAsyncBlockExpressionSyntax n:
                yield return n.Block;
                yield break;
            case IAsyncStartExpressionSyntax n:
                yield return n.Expression;
                yield break;
            case IAwaitExpressionSyntax n:
                yield return n.Expression;
                yield break;
        }
    }
}
