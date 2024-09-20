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
                yield return n.IntrinsicsReference;
                foreach (var child in n.PrimitivesDeclarations)
                    yield return child;
                yield break;
            case IStandardPackageReferenceNode n:
                yield return n.SymbolNode;
                yield break;
            case IIntrinsicsPackageReferenceNode n:
                yield return n.SymbolNode;
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
            case INamespaceBlockDefinitionNode n:
                foreach (var child in n.UsingDirectives)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case INamespaceDefinitionNode n:
                foreach (var child in n.MemberNamespaces)
                    yield return child;
                yield break;
            case IFunctionDefinitionNode n:
                foreach (var child in n.Attributes)
                    yield return child;
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.Return is not null)
                    yield return n.Return;
                yield return n.Entry;
                yield return n.Body;
                yield return n.Exit;
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
                foreach (var child in n.SourceMembers)
                    yield return child;
                if (n.DefaultConstructor is not null)
                    yield return n.DefaultConstructor;
                yield break;
            case IStructDefinitionNode n:
                foreach (var child in n.Attributes)
                    yield return child;
                foreach (var child in n.GenericParameters)
                    yield return child;
                foreach (var child in n.SupertypeNames)
                    yield return child;
                foreach (var child in n.SourceMembers)
                    yield return child;
                if (n.DefaultInitializer is not null)
                    yield return n.DefaultInitializer;
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
                yield return n.Entry;
                yield return n.Body;
                yield return n.Exit;
                yield break;
            case IGetterMethodDefinitionNode n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                yield return n.Return;
                yield return n.Entry;
                yield return n.Body;
                yield return n.Exit;
                yield break;
            case ISetterMethodDefinitionNode n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.Return is not null)
                    yield return n.Return;
                yield return n.Entry;
                yield return n.Body;
                yield return n.Exit;
                yield break;
            case IDefaultConstructorDefinitionNode n:
                yield return n.Entry;
                yield return n.Exit;
                yield break;
            case ISourceConstructorDefinitionNode n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                yield return n.Entry;
                yield return n.Body;
                yield return n.Exit;
                yield break;
            case IDefaultInitializerDefinitionNode n:
                yield return n.Entry;
                yield return n.Exit;
                yield break;
            case ISourceInitializerDefinitionNode n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                yield return n.Entry;
                yield return n.Body;
                yield return n.Exit;
                yield break;
            case IFieldDefinitionNode n:
                yield return n.TypeNode;
                yield return n.Entry;
                if (n.TempInitializer is not null)
                    yield return n.TempInitializer;
                yield return n.Exit;
                yield break;
            case IAssociatedFunctionDefinitionNode n:
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.Return is not null)
                    yield return n.Return;
                yield return n.Entry;
                yield return n.Body;
                yield return n.Exit;
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
            case IEntryNode n:
                yield break;
            case IExitNode n:
                yield break;
            case IResultStatementNode n:
                yield return n.TempExpression;
                yield break;
            case IVariableDeclarationStatementNode n:
                if (n.Capability is not null)
                    yield return n.Capability;
                if (n.Type is not null)
                    yield return n.Type;
                if (n.TempInitializer is not null)
                    yield return n.TempInitializer;
                yield break;
            case IExpressionStatementNode n:
                yield return n.TempExpression;
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
                yield return n.ConstructingType;
                foreach (var child in n.TempArguments)
                    yield return child;
                yield break;
            case IUnsafeExpressionNode n:
                yield return n.TempExpression;
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
                yield return n.TempLeftOperand;
                yield return n.TempRightOperand;
                yield break;
            case IBinaryOperatorExpressionNode n:
                yield return n.TempLeftOperand;
                yield return n.TempRightOperand;
                yield break;
            case IUnaryOperatorExpressionNode n:
                yield return n.TempOperand;
                yield break;
            case IIdExpressionNode n:
                yield return n.TempReferent;
                yield break;
            case IConversionExpressionNode n:
                yield return n.TempReferent;
                yield return n.ConvertToType;
                yield break;
            case IImplicitConversionExpressionNode n:
                yield return n.Referent;
                yield break;
            case IPatternMatchExpressionNode n:
                yield return n.TempReferent;
                yield return n.Pattern;
                yield break;
            case IIfExpressionNode n:
                yield return n.TempCondition;
                yield return n.ThenBlock;
                if (n.ElseClause is not null)
                    yield return n.ElseClause;
                yield break;
            case ILoopExpressionNode n:
                yield return n.Block;
                yield break;
            case IWhileExpressionNode n:
                yield return n.TempCondition;
                yield return n.Block;
                yield break;
            case IForeachExpressionNode n:
                yield return n.TempInExpression;
                if (n.DeclaredType is not null)
                    yield return n.DeclaredType;
                yield return n.Block;
                yield break;
            case IBreakExpressionNode n:
                if (n.TempValue is not null)
                    yield return n.TempValue;
                yield break;
            case INextExpressionNode n:
                yield break;
            case IReturnExpressionNode n:
                if (n.TempValue is not null)
                    yield return n.TempValue;
                yield break;
            case IUnknownInvocationExpressionNode n:
                yield return n.TempExpression;
                foreach (var child in n.TempArguments)
                    yield return child;
                yield break;
            case IFunctionInvocationExpressionNode n:
                yield return n.Function;
                foreach (var child in n.TempArguments)
                    yield return child;
                yield break;
            case IMethodInvocationExpressionNode n:
                yield return n.MethodGroup;
                foreach (var child in n.TempArguments)
                    yield return child;
                yield break;
            case IGetterInvocationExpressionNode n:
                yield return n.Context;
                yield break;
            case ISetterInvocationExpressionNode n:
                yield return n.Context;
                yield return n.TempValue;
                yield break;
            case IFunctionReferenceInvocationExpressionNode n:
                yield return n.Expression;
                foreach (var child in n.TempArguments)
                    yield return child;
                yield break;
            case IInitializerInvocationExpressionNode n:
                yield return n.InitializerGroup;
                foreach (var child in n.TempArguments)
                    yield return child;
                yield break;
            case IIdentifierNameExpressionNode n:
                yield break;
            case IGenericNameExpressionNode n:
                foreach (var child in n.TypeArguments)
                    yield return child;
                yield break;
            case IUnresolvedMemberAccessExpressionNode n:
                yield return n.TempContext;
                foreach (var child in n.TypeArguments)
                    yield return child;
                yield break;
            case IUnqualifiedNamespaceNameNode n:
                yield break;
            case IQualifiedNamespaceNameNode n:
                yield return n.Context;
                yield break;
            case IFunctionGroupNameNode n:
                if (n.Context is not null)
                    yield return n.Context;
                foreach (var child in n.TypeArguments)
                    yield return child;
                yield break;
            case IFunctionNameNode n:
                if (n.Context is not null)
                    yield return n.Context;
                foreach (var child in n.TypeArguments)
                    yield return child;
                yield break;
            case IMethodGroupNameNode n:
                yield return n.Context;
                foreach (var child in n.TypeArguments)
                    yield return child;
                yield break;
            case IFieldAccessExpressionNode n:
                yield return n.Context;
                yield break;
            case IVariableNameExpressionNode n:
                yield break;
            case IStandardTypeNameExpressionNode n:
                foreach (var child in n.TypeArguments)
                    yield return child;
                yield break;
            case IQualifiedTypeNameExpressionNode n:
                yield return n.Context;
                foreach (var child in n.TypeArguments)
                    yield return child;
                yield break;
            case IInitializerGroupNameNode n:
                yield return n.Context;
                yield break;
            case ISpecialTypeNameExpressionNode n:
                yield break;
            case ISelfExpressionNode n:
                yield break;
            case IMissingNameExpressionNode n:
                yield break;
            case IUnknownIdentifierNameExpressionNode n:
                yield break;
            case IUnknownGenericNameExpressionNode n:
                foreach (var child in n.TypeArguments)
                    yield return child;
                yield break;
            case IAmbiguousMemberAccessExpressionNode n:
                yield return n.Context;
                foreach (var child in n.TypeArguments)
                    yield return child;
                yield break;
            case IAmbiguousMoveExpressionNode n:
                yield return n.TempReferent;
                yield break;
            case IMoveVariableExpressionNode n:
                yield return n.Referent;
                yield break;
            case IMoveValueExpressionNode n:
                yield return n.Referent;
                yield break;
            case IImplicitTempMoveExpressionNode n:
                yield return n.Referent;
                yield break;
            case IAmbiguousFreezeExpressionNode n:
                yield return n.TempReferent;
                yield break;
            case IFreezeVariableExpressionNode n:
                yield return n.Referent;
                yield break;
            case IFreezeValueExpressionNode n:
                yield return n.Referent;
                yield break;
            case IPrepareToReturnExpressionNode n:
                yield return n.Value;
                yield break;
            case IAsyncBlockExpressionNode n:
                yield return n.Block;
                yield break;
            case IAsyncStartExpressionNode n:
                yield return n.TempExpression;
                yield break;
            case IAwaitExpressionNode n:
                yield return n.TempExpression;
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
            case IEmptyTypeSymbolNode n:
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IPrimitiveTypeSymbolNode n:
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IClassSymbolNode n:
                foreach (var child in n.GenericParameters)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IStructSymbolNode n:
                foreach (var child in n.GenericParameters)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case ITraitSymbolNode n:
                foreach (var child in n.GenericParameters)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IGenericParameterSymbolNode n:
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case IStandardMethodSymbolNode n:
                yield break;
            case IGetterMethodSymbolNode n:
                yield break;
            case ISetterMethodSymbolNode n:
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
