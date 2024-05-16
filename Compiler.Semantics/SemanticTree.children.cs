using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics;

[GeneratedCode("AzothCompilerCodeGen", null)]
public static class ISemanticNodeExtensions
{
    [DebuggerStepThrough]
    public static IEnumerable<ISemanticNode> Children(this ISemanticNode node)
    {
        switch (node)
        {
            default:
                throw ExhaustiveMatch.Failed(node);
            case IPackageNode n:
                foreach (var child in n.References)
                    yield return child;
                yield return n.MainFacet;
                yield return n.TestingFacet;
                yield break;
            case IPackageReferenceNode n:
                yield break;
            case IPackageFacetNode n:
                foreach (var child in n.CompilationUnits)
                    yield return child;
                yield break;
            case ICompilationUnitNode n:
                foreach (var child in n.UsingDirectives)
                    yield return child;
                foreach (var child in n.Definitions)
                    yield return child;
                yield break;
            case IUsingDirectiveNode n:
                yield break;
            case INamespaceDefinitionNode n:
                foreach (var child in n.UsingDirectives)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IFunctionDefinitionNode n:
                foreach (var child in n.Attributes)
                    yield return child;
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.Return is not null)
                    yield return n.Return;
                yield return n.Body;
                yield break;
            case IClassDefinitionNode n:
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
            case IStructDefinitionNode n:
                foreach (var child in n.Attributes)
                    yield return child;
                foreach (var child in n.GenericParameters)
                    yield return child;
                foreach (var child in n.SupertypeNames)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case ITraitDefinitionNode n:
                foreach (var child in n.Attributes)
                    yield return child;
                foreach (var child in n.GenericParameters)
                    yield return child;
                foreach (var child in n.SupertypeNames)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IGenericParameterNode n:
                yield return n.Constraint;
                yield break;
            case IAbstractMethodDefinitionNode n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.Return is not null)
                    yield return n.Return;
                yield break;
            case IStandardMethodDefinitionNode n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.Return is not null)
                    yield return n.Return;
                yield return n.Body;
                yield break;
            case IGetterMethodDefinitionNode n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                yield return n.Return;
                yield return n.Body;
                yield break;
            case ISetterMethodDefinitionNode n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.Return is not null)
                    yield return n.Return;
                yield return n.Body;
                yield break;
            case IDefaultConstructorDefinitionNode n:
                foreach (var child in n.Parameters)
                    yield return child;
                yield break;
            case ISourceConstructorDefinitionNode n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                yield return n.Body;
                yield break;
            case IInitializerDefinitionNode n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                yield return n.Body;
                yield break;
            case IFieldDefinitionNode n:
                yield return n.TypeNode;
                if (n.Initializer is not null)
                    yield return n.Initializer;
                yield break;
            case IAssociatedFunctionDefinitionNode n:
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.Return is not null)
                    yield return n.Return;
                yield return n.Body;
                yield break;
            case IAttributeNode n:
                yield return n.TypeName;
                yield break;
            case ICapabilitySetNode n:
                yield break;
            case ICapabilityNode n:
                yield break;
            case INamedParameterNode n:
                yield return n.TypeNode;
                yield break;
            case IConstructorSelfParameterNode n:
                yield return n.Capability;
                yield break;
            case IInitializerSelfParameterNode n:
                yield return n.Capability;
                yield break;
            case IMethodSelfParameterNode n:
                yield return n.Capability;
                yield break;
            case IFieldParameterNode n:
                yield break;
            case IBlockBodyNode n:
                foreach (var child in n.Statements)
                    yield return child;
                yield break;
            case IExpressionBodyNode n:
                yield return n.ResultStatement;
                yield break;
            case IIdentifierTypeNameNode n:
                yield break;
            case ISpecialTypeNameNode n:
                yield break;
            case IGenericTypeNameNode n:
                foreach (var child in n.TypeArguments)
                    yield return child;
                yield break;
            case IQualifiedTypeNameNode n:
                yield return n.Context;
                yield return n.QualifiedName;
                yield break;
            case IOptionalTypeNode n:
                yield return n.Referent;
                yield break;
            case ICapabilityTypeNode n:
                yield return n.Capability;
                yield return n.Referent;
                yield break;
            case IFunctionTypeNode n:
                foreach (var child in n.Parameters)
                    yield return child;
                yield return n.Return;
                yield break;
            case IParameterTypeNode n:
                yield return n.Referent;
                yield break;
            case ICapabilityViewpointTypeNode n:
                yield return n.Capability;
                yield return n.Referent;
                yield break;
            case ISelfViewpointTypeNode n:
                yield return n.Referent;
                yield break;
            case IResultStatementNode n:
                yield return n.Expression;
                yield break;
            case IVariableDeclarationStatementNode n:
                if (n.Capability is not null)
                    yield return n.Capability;
                if (n.Type is not null)
                    yield return n.Type;
                if (n.Initializer is not null)
                    yield return n.Initializer;
                yield break;
            case IExpressionStatementNode n:
                yield return n.Expression;
                yield break;
            case IBindingContextPatternNode n:
                yield return n.Pattern;
                if (n.Type is not null)
                    yield return n.Type;
                yield break;
            case IBindingPatternNode n:
                yield break;
            case IOptionalPatternNode n:
                yield return n.Pattern;
                yield break;
            case IBlockExpressionNode n:
                foreach (var child in n.Statements)
                    yield return child;
                yield break;
            case INewObjectExpressionNode n:
                yield return n.Type;
                foreach (var child in n.Arguments)
                    yield return child;
                yield break;
            case IUnsafeExpressionNode n:
                yield return n.Expression;
                yield break;
            case IBoolLiteralExpressionNode n:
                yield break;
            case IIntegerLiteralExpressionNode n:
                yield break;
            case INoneLiteralExpressionNode n:
                yield break;
            case IStringLiteralExpressionNode n:
                yield break;
            case IAssignmentExpressionNode n:
                yield return n.LeftOperand;
                yield return n.RightOperand;
                yield break;
            case IBinaryOperatorExpressionNode n:
                yield return n.LeftOperand;
                yield return n.RightOperand;
                yield break;
            case IUnaryOperatorExpressionNode n:
                yield return n.Operand;
                yield break;
            case IIdExpressionNode n:
                yield return n.Referent;
                yield break;
            case IConversionExpressionNode n:
                yield return n.Referent;
                yield return n.ConvertToType;
                yield break;
            case IPatternMatchExpressionNode n:
                yield return n.Referent;
                yield return n.Pattern;
                yield break;
            case IIfExpressionNode n:
                yield return n.Condition;
                yield return n.ThenBlock;
                if (n.ElseClause is not null)
                    yield return n.ElseClause;
                yield break;
            case ILoopExpressionNode n:
                yield return n.Block;
                yield break;
            case IWhileExpressionNode n:
                yield return n.Condition;
                yield return n.Block;
                yield break;
            case IForeachExpressionNode n:
                yield return n.InExpression;
                if (n.Type is not null)
                    yield return n.Type;
                yield return n.Block;
                yield break;
            case IBreakExpressionNode n:
                if (n.Value is not null)
                    yield return n.Value;
                yield break;
            case INextExpressionNode n:
                yield break;
            case IReturnExpressionNode n:
                if (n.Value is not null)
                    yield return n.Value;
                yield break;
            case IInvocationExpressionNode n:
                yield return n.Expression;
                foreach (var child in n.Arguments)
                    yield return child;
                yield break;
            case IIdentifierNameExpressionNode n:
                yield break;
            case ISpecialTypeNameExpressionNode n:
                yield break;
            case IGenericNameExpressionNode n:
                foreach (var child in n.TypeArguments)
                    yield return child;
                yield break;
            case IMemberAccessExpressionNode n:
                yield return n.Context;
                foreach (var child in n.TypeArguments)
                    yield return child;
                yield break;
            case ISelfExpressionNode n:
                yield break;
            case IMissingNameExpressionNode n:
                yield break;
            case IMoveExpressionNode n:
                yield return n.Referent;
                yield break;
            case IFreezeExpressionNode n:
                yield return n.Referent;
                yield break;
            case IAsyncBlockExpressionNode n:
                yield return n.Block;
                yield break;
            case IAsyncStartExpressionNode n:
                yield return n.Expression;
                yield break;
            case IAwaitExpressionNode n:
                yield return n.Expression;
                yield break;
            case IPackageSymbolNode n:
                yield return n.MainFacet;
                yield return n.TestingFacet;
                yield break;
            case IPackageFacetSymbolNode n:
                yield return n.GlobalNamespace;
                yield break;
            case INamespaceSymbolNode n:
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IFunctionSymbolNode n:
                yield break;
            case IUserTypeSymbolNode n:
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IClassSymbolNode n:
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IStructSymbolNode n:
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case ITraitSymbolNode n:
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IGenericParameterSymbolNode n:
                yield break;
            case IMethodSymbolNode n:
                yield break;
            case IConstructorSymbolNode n:
                yield break;
            case IInitializerSymbolNode n:
                yield break;
            case IFieldSymbolNode n:
                yield break;
            case IAssociatedFunctionSymbolNode n:
                yield break;
        }
    }
}
