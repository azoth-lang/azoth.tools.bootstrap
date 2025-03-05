using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Syntax;

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
            case IAssignmentExpressionSyntax n:
                yield return n.LeftOperand;
                yield return n.RightOperand;
                yield break;
            case IAssociatedFunctionDefinitionSyntax n:
                foreach (var child in n.Attributes)
                    yield return child;
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.Return is not null)
                    yield return n.Return;
                if (n.Body is not null)
                    yield return n.Body;
                yield break;
            case IAssociatedTypeDefinitionSyntax n:
                foreach (var child in n.Attributes)
                    yield return child;
                if (n.Initializer is not null)
                    yield return n.Initializer;
                yield break;
            case IAsyncBlockExpressionSyntax n:
                yield return n.Block;
                yield break;
            case IAsyncStartExpressionSyntax n:
                yield return n.Expression;
                yield break;
            case IAttributeSyntax n:
                yield return n.TypeName;
                yield break;
            case IAwaitExpressionSyntax n:
                yield return n.Expression;
                yield break;
            case IBinaryOperatorExpressionSyntax n:
                yield return n.LeftOperand;
                yield return n.RightOperand;
                yield break;
            case IBindingContextPatternSyntax n:
                yield return n.Pattern;
                if (n.Type is not null)
                    yield return n.Type;
                yield break;
            case IBindingPatternSyntax n:
                yield break;
            case IBlockBodySyntax n:
                foreach (var child in n.Statements)
                    yield return child;
                yield break;
            case IBlockExpressionSyntax n:
                foreach (var child in n.Statements)
                    yield return child;
                yield break;
            case IBoolLiteralExpressionSyntax n:
                yield break;
            case IBreakExpressionSyntax n:
                if (n.Value is not null)
                    yield return n.Value;
                yield break;
            case IBuiltInTypeNameSyntax n:
                yield break;
            case ICapabilitySetSyntax n:
                yield break;
            case ICapabilitySetTypeSyntax n:
                yield return n.CapabilitySet;
                yield return n.Referent;
                yield break;
            case ICapabilitySyntax n:
                yield break;
            case ICapabilityTypeSyntax n:
                yield return n.Capability;
                yield return n.Referent;
                yield break;
            case ICapabilityViewpointTypeSyntax n:
                yield return n.Capability;
                yield return n.Referent;
                yield break;
            case IClassDefinitionSyntax n:
                foreach (var child in n.Attributes)
                    yield return child;
                foreach (var child in n.GenericParameters)
                    yield return child;
                if (n.BaseTypeName is not null)
                    yield return n.BaseTypeName;
                foreach (var child in n.SupertypeNames)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case ICompilationUnitSyntax n:
                foreach (var child in n.ImportDirectives)
                    yield return child;
                foreach (var child in n.Definitions)
                    yield return child;
                yield break;
            case IConversionExpressionSyntax n:
                yield return n.Referent;
                yield return n.ConvertToType;
                yield break;
            case IExpressionBodySyntax n:
                yield return n.ResultStatement;
                yield break;
            case IExpressionStatementSyntax n:
                yield return n.Expression;
                yield break;
            case IFieldDefinitionSyntax n:
                foreach (var child in n.Attributes)
                    yield return child;
                yield return n.Type;
                if (n.Initializer is not null)
                    yield return n.Initializer;
                yield break;
            case IFieldParameterSyntax n:
                if (n.DefaultValue is not null)
                    yield return n.DefaultValue;
                yield break;
            case IForeachExpressionSyntax n:
                yield return n.InExpression;
                if (n.Type is not null)
                    yield return n.Type;
                yield return n.Block;
                yield break;
            case IFreezeExpressionSyntax n:
                yield return n.Referent;
                yield break;
            case IFunctionDefinitionSyntax n:
                foreach (var child in n.Attributes)
                    yield return child;
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.Return is not null)
                    yield return n.Return;
                yield return n.Body;
                yield break;
            case IFunctionTypeSyntax n:
                foreach (var child in n.Parameters)
                    yield return child;
                yield return n.Return;
                yield break;
            case IGenericNameSyntax n:
                foreach (var child in n.GenericArguments)
                    yield return child;
                yield break;
            case IGenericParameterSyntax n:
                yield return n.Constraint;
                yield break;
            case IGetterMethodDefinitionSyntax n:
                foreach (var child in n.Attributes)
                    yield return child;
                yield return n.SelfParameter;
                yield return n.Return;
                if (n.Body is not null)
                    yield return n.Body;
                yield break;
            case IIdentifierNameSyntax n:
                yield break;
            case IIfExpressionSyntax n:
                yield return n.Condition;
                yield return n.ThenBlock;
                if (n.ElseClause is not null)
                    yield return n.ElseClause;
                yield break;
            case IImportDirectiveSyntax n:
                yield break;
            case IInitializerDefinitionSyntax n:
                foreach (var child in n.Attributes)
                    yield return child;
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                yield return n.Body;
                yield break;
            case IInitializerSelfParameterSyntax n:
                yield return n.Constraint;
                yield break;
            case IIntegerLiteralExpressionSyntax n:
                yield break;
            case IInvocationExpressionSyntax n:
                yield return n.Expression;
                foreach (var child in n.Arguments)
                    yield return child;
                yield break;
            case ILoopExpressionSyntax n:
                yield return n.Block;
                yield break;
            case IMethodSelfParameterSyntax n:
                yield return n.Constraint;
                yield break;
            case IMissingNameExpressionSyntax n:
                yield break;
            case IMoveExpressionSyntax n:
                yield return n.Referent;
                yield break;
            case INamedParameterSyntax n:
                yield return n.Type;
                if (n.DefaultValue is not null)
                    yield return n.DefaultValue;
                yield break;
            case INamespaceBlockDefinitionSyntax n:
                foreach (var child in n.ImportDirectives)
                    yield return child;
                foreach (var child in n.Definitions)
                    yield return child;
                yield break;
            case INextExpressionSyntax n:
                yield break;
            case INoneLiteralExpressionSyntax n:
                yield break;
            case IOptionalPatternSyntax n:
                yield return n.Pattern;
                yield break;
            case IOptionalTypeSyntax n:
                yield return n.Referent;
                yield break;
            case IOrdinaryMethodDefinitionSyntax n:
                foreach (var child in n.Attributes)
                    yield return child;
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.Return is not null)
                    yield return n.Return;
                if (n.Body is not null)
                    yield return n.Body;
                yield break;
            case IPackageReferenceSyntax n:
                yield break;
            case IPackageSyntax n:
                foreach (var child in n.CompilationUnits)
                    yield return child;
                foreach (var child in n.TestingCompilationUnits)
                    yield return child;
                foreach (var child in n.References)
                    yield return child;
                yield break;
            case IParameterTypeSyntax n:
                yield return n.Referent;
                yield break;
            case IPatternMatchExpressionSyntax n:
                yield return n.Referent;
                yield return n.Pattern;
                yield break;
            case IQualifiedNameSyntax n:
                yield return n.Context;
                foreach (var child in n.GenericArguments)
                    yield return child;
                yield break;
            case IMemberAccessExpressionSyntax n:
                yield return n.Context;
                foreach (var child in n.GenericArguments)
                    yield return child;
                yield break;
            case IRefExpressionSyntax n:
                yield return n.Referent;
                yield break;
            case IRefTypeSyntax n:
                yield return n.Referent;
                yield break;
            case IResultStatementSyntax n:
                yield return n.Expression;
                yield break;
            case IReturnExpressionSyntax n:
                if (n.Value is not null)
                    yield return n.Value;
                yield break;
            case IReturnSyntax n:
                yield return n.Type;
                yield break;
            case ISelfExpressionSyntax n:
                yield break;
            case ISelfViewpointTypeSyntax n:
                yield return n.Referent;
                yield break;
            case ISetterMethodDefinitionSyntax n:
                foreach (var child in n.Attributes)
                    yield return child;
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.Body is not null)
                    yield return n.Body;
                yield break;
            case IStringLiteralExpressionSyntax n:
                yield break;
            case IStructDefinitionSyntax n:
                foreach (var child in n.Attributes)
                    yield return child;
                foreach (var child in n.GenericParameters)
                    yield return child;
                foreach (var child in n.SupertypeNames)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case ITraitDefinitionSyntax n:
                foreach (var child in n.Attributes)
                    yield return child;
                foreach (var child in n.GenericParameters)
                    yield return child;
                foreach (var child in n.SupertypeNames)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case ITypePatternSyntax n:
                yield return n.Type;
                yield break;
            case IUnaryOperatorExpressionSyntax n:
                yield return n.Operand;
                yield break;
            case IUnsafeExpressionSyntax n:
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
            case IWhileExpressionSyntax n:
                yield return n.Condition;
                yield return n.Block;
                yield break;
        }
    }
}
