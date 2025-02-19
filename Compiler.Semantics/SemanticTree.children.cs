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
            case IAmbiguousFreezeExpressionNode n:
                yield return n.TempReferent;
                yield break;
            case IAmbiguousMoveExpressionNode n:
                yield return n.TempReferent;
                yield break;
            case IAssignmentExpressionNode n:
                yield return n.TempLeftOperand;
                yield return n.TempRightOperand;
                yield break;
            case IAssociatedFunctionDefinitionNode n:
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.Return is not null)
                    yield return n.Return;
                yield return n.Entry;
                if (n.Body is not null)
                    yield return n.Body;
                yield return n.Exit;
                yield break;
            case IAssociatedFunctionSymbolNode n:
                yield break;
            case IAssociatedTypeDefinitionNode n:
                if (n.Initializer is not null)
                    yield return n.Initializer;
                yield break;
            case IAssociatedTypeSymbolNode n:
                yield break;
            case IAsyncBlockExpressionNode n:
                yield return n.Block;
                yield break;
            case IAsyncStartExpressionNode n:
                yield return n.TempExpression;
                yield break;
            case IAttributeNode n:
                yield return n.TypeName;
                yield break;
            case IAwaitExpressionNode n:
                yield return n.TempExpression;
                yield break;
            case IBinaryOperatorExpressionNode n:
                yield return n.TempLeftOperand;
                yield return n.TempRightOperand;
                yield break;
            case IBindingContextPatternNode n:
                yield return n.Pattern;
                if (n.Type is not null)
                    yield return n.Type;
                yield break;
            case IBindingPatternNode n:
                yield break;
            case IBlockBodyNode n:
                foreach (var child in n.Statements)
                    yield return child;
                yield break;
            case IBlockExpressionNode n:
                foreach (var child in n.Statements)
                    yield return child;
                yield break;
            case IBoolLiteralExpressionNode n:
                yield break;
            case IBreakExpressionNode n:
                if (n.TempValue is not null)
                    yield return n.TempValue;
                yield break;
            case IBuiltInTypeNameNode n:
                yield break;
            case IBuiltInTypeSymbolNode n:
                yield return n.ImplicitSelf;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case ICapabilityNode n:
                yield break;
            case ICapabilitySetNode n:
                yield break;
            case ICapabilitySetTypeNode n:
                yield return n.CapabilitySet;
                yield return n.Referent;
                yield break;
            case ICapabilityTypeNode n:
                yield return n.Capability;
                yield return n.Referent;
                yield break;
            case ICapabilityViewpointTypeNode n:
                yield return n.Capability;
                yield return n.Referent;
                yield break;
            case IClassDefinitionNode n:
                yield return n.ImplicitSelf;
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
                if (n.DefaultInitializer is not null)
                    yield return n.DefaultInitializer;
                yield break;
            case IClassSymbolNode n:
                yield return n.ImplicitSelf;
                foreach (var child in n.GenericParameters)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case ICompilationUnitNode n:
                foreach (var child in n.ImportDirectives)
                    yield return child;
                foreach (var child in n.Definitions)
                    yield return child;
                yield break;
            case IConversionExpressionNode n:
                yield return n.TempReferent;
                yield return n.ConvertToType;
                yield break;
            case IDefaultInitializerDefinitionNode n:
                yield return n.Entry;
                yield return n.Exit;
                yield break;
            case IEntryNode n:
                yield break;
            case IExitNode n:
                yield break;
            case IExpressionBodyNode n:
                yield return n.ResultStatement;
                yield break;
            case IExpressionStatementNode n:
                yield return n.TempExpression;
                yield break;
            case IFieldAccessExpressionNode n:
                yield return n.Context;
                yield break;
            case IFieldDefinitionNode n:
                yield return n.TypeNode;
                yield return n.Entry;
                if (n.TempInitializer is not null)
                    yield return n.TempInitializer;
                yield return n.Exit;
                yield break;
            case IFieldParameterNode n:
                yield break;
            case IFieldSymbolNode n:
                yield break;
            case IForeachExpressionNode n:
                yield return n.TempInExpression;
                if (n.DeclaredType is not null)
                    yield return n.DeclaredType;
                yield return n.Block;
                yield break;
            case IFreezeValueExpressionNode n:
                yield return n.Referent;
                yield break;
            case IFreezeVariableExpressionNode n:
                yield return n.Referent;
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
            case IFunctionInvocationExpressionNode n:
                yield return n.Function;
                foreach (var child in n.TempArguments)
                    yield return child;
                yield break;
            case IFunctionNameExpressionNode n:
                if (n.Context is not null)
                    yield return n.Context;
                foreach (var child in n.GenericArguments)
                    yield return child;
                yield break;
            case IFunctionReferenceInvocationExpressionNode n:
                yield return n.Expression;
                foreach (var child in n.TempArguments)
                    yield return child;
                yield break;
            case IFunctionSymbolNode n:
                yield break;
            case IFunctionTypeNode n:
                foreach (var child in n.Parameters)
                    yield return child;
                yield return n.Return;
                yield break;
            case IGenericParameterNode n:
                yield return n.Constraint;
                yield break;
            case IGenericParameterSymbolNode n:
                yield break;
            case IGenericTypeNameNode n:
                foreach (var child in n.GenericArguments)
                    yield return child;
                yield break;
            case IGetterInvocationExpressionNode n:
                yield return n.Context;
                yield break;
            case IGetterMethodDefinitionNode n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                yield return n.Return;
                yield return n.Entry;
                if (n.Body is not null)
                    yield return n.Body;
                yield return n.Exit;
                yield break;
            case IGetterMethodSymbolNode n:
                yield break;
            case IIdentifierTypeNameNode n:
                yield break;
            case IIfExpressionNode n:
                yield return n.TempCondition;
                yield return n.ThenBlock;
                if (n.ElseClause is not null)
                    yield return n.ElseClause;
                yield break;
            case IImplicitConversionExpressionNode n:
                yield return n.Referent;
                yield break;
            case IImplicitDerefExpressionNode n:
                yield return n.Referent;
                yield break;
            case IImplicitSelfDefinitionNode n:
                yield break;
            case IImplicitTempMoveExpressionNode n:
                yield return n.Referent;
                yield break;
            case IImportDirectiveNode n:
                yield break;
            case IInitializerInvocationExpressionNode n:
                yield return n.Initializer;
                foreach (var child in n.TempArguments)
                    yield return child;
                yield break;
            case IInitializerNameExpressionNode n:
                yield return n.Context;
                yield break;
            case IInitializerSelfParameterNode n:
                yield return n.Capability;
                yield break;
            case IInitializerSymbolNode n:
                yield break;
            case IIntegerLiteralExpressionNode n:
                yield break;
            case IIntrinsicsPackageReferenceNode n:
                yield return n.SymbolNode;
                yield break;
            case ILoopExpressionNode n:
                yield return n.Block;
                yield break;
            case IMethodAccessExpressionNode n:
                yield return n.Context;
                foreach (var child in n.GenericArguments)
                    yield return child;
                yield break;
            case IMethodInvocationExpressionNode n:
                yield return n.Method;
                foreach (var child in n.TempArguments)
                    yield return child;
                yield break;
            case IMethodSelfParameterNode n:
                yield return n.Constraint;
                yield break;
            case IMissingNameExpressionNode n:
                yield break;
            case IMoveValueExpressionNode n:
                yield return n.Referent;
                yield break;
            case IMoveVariableExpressionNode n:
                yield return n.Referent;
                yield break;
            case INamedParameterNode n:
                yield return n.TypeNode;
                yield break;
            case INamespaceBlockDefinitionNode n:
                foreach (var child in n.ImportDirectives)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case INamespaceDefinitionNode n:
                foreach (var child in n.MemberNamespaces)
                    yield return child;
                yield break;
            case INamespaceSymbolNode n:
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case INeverTypeSymbolNode n:
                yield break;
            case INextExpressionNode n:
                yield break;
            case INoneLiteralExpressionNode n:
                yield break;
            case INonInvocableInvocationExpressionNode n:
                yield return n.TempExpression;
                foreach (var child in n.TempArguments)
                    yield return child;
                yield break;
            case IOptionalPatternNode n:
                yield return n.Pattern;
                yield break;
            case IOptionalTypeNode n:
                yield return n.Referent;
                yield break;
            case IOrdinaryInitializerDefinitionNode n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                yield return n.Entry;
                yield return n.Body;
                yield return n.Exit;
                yield break;
            case IOrdinaryMethodDefinitionNode n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.Return is not null)
                    yield return n.Return;
                yield return n.Entry;
                if (n.Body is not null)
                    yield return n.Body;
                yield return n.Exit;
                yield break;
            case IOrdinaryMethodSymbolNode n:
                yield break;
            case IPackageFacetNode n:
                foreach (var child in n.CompilationUnits)
                    yield return child;
                yield break;
            case IPackageFacetSymbolNode n:
                yield return n.GlobalNamespace;
                yield break;
            case IPackageNode n:
                foreach (var child in n.References)
                    yield return child;
                yield return n.MainFacet;
                yield return n.TestingFacet;
                yield return n.IntrinsicsReference;
                foreach (var child in n.PrimitivesDeclarations)
                    yield return child;
                yield break;
            case IPackageSymbolNode n:
                yield return n.MainFacet;
                yield return n.TestingFacet;
                yield break;
            case IParameterTypeNode n:
                yield return n.Referent;
                yield break;
            case IPatternMatchExpressionNode n:
                yield return n.TempReferent;
                yield return n.Pattern;
                yield break;
            case IPrepareToReturnExpressionNode n:
                yield return n.Value;
                yield break;
            case IQualifiedNamespaceNameNode n:
                yield return n.Context;
                yield break;
            case IQualifiedTypeNameNode n:
                yield return n.Context;
                foreach (var child in n.GenericArguments)
                    yield return child;
                yield break;
            case IRefAssignmentExpressionNode n:
                yield return n.TempLeftOperand;
                yield return n.TempRightOperand;
                yield break;
            case IRefExpressionNode n:
                yield return n.TempReferent;
                yield break;
            case IRefTypeNode n:
                yield return n.Referent;
                yield break;
            case IResultStatementNode n:
                yield return n.TempExpression;
                yield break;
            case IReturnExpressionNode n:
                if (n.TempValue is not null)
                    yield return n.TempValue;
                yield break;
            case ISelfExpressionNode n:
                yield break;
            case ISelfSymbolNode n:
                yield break;
            case ISelfViewpointTypeNode n:
                yield return n.Referent;
                yield break;
            case ISetterInvocationExpressionNode n:
                yield return n.Context;
                yield return n.TempValue;
                yield break;
            case ISetterMethodDefinitionNode n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.Return is not null)
                    yield return n.Return;
                yield return n.Entry;
                if (n.Body is not null)
                    yield return n.Body;
                yield return n.Exit;
                yield break;
            case ISetterMethodSymbolNode n:
                yield break;
            case IStandardPackageReferenceNode n:
                yield return n.SymbolNode;
                yield break;
            case IStringLiteralExpressionNode n:
                yield break;
            case IStructDefinitionNode n:
                yield return n.ImplicitSelf;
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
            case IStructSymbolNode n:
                yield return n.ImplicitSelf;
                foreach (var child in n.GenericParameters)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case ITraitDefinitionNode n:
                yield return n.ImplicitSelf;
                foreach (var child in n.Attributes)
                    yield return child;
                foreach (var child in n.GenericParameters)
                    yield return child;
                foreach (var child in n.SupertypeNames)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case ITraitSymbolNode n:
                yield return n.ImplicitSelf;
                foreach (var child in n.GenericParameters)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case ITypePatternNode n:
                yield return n.Type;
                yield break;
            case IUnaryOperatorExpressionNode n:
                yield return n.TempOperand;
                yield break;
            case IUnqualifiedNamespaceNameNode n:
                yield break;
            case IUnresolvedGenericNameExpressionNode n:
                foreach (var child in n.GenericArguments)
                    yield return child;
                yield break;
            case IUnresolvedGenericNameNode n:
                foreach (var child in n.GenericArguments)
                    yield return child;
                yield break;
            case IUnresolvedIdentifierNameExpressionNode n:
                yield break;
            case IUnresolvedIdentifierNameNode n:
                yield break;
            case IUnresolvedInvocationExpressionNode n:
                yield return n.TempExpression;
                foreach (var child in n.TempArguments)
                    yield return child;
                yield break;
            case IUnresolvedNameExpressionQualifiedNameExpressionNode n:
                yield return n.Context;
                foreach (var child in n.GenericArguments)
                    yield return child;
                yield break;
            case IUnresolvedNameQualifiedNameNode n:
                yield return n.Context;
                foreach (var child in n.GenericArguments)
                    yield return child;
                yield break;
            case IUnresolvedNamespaceQualifiedNameExpressionNode n:
                yield return n.Context;
                foreach (var child in n.GenericArguments)
                    yield return child;
                yield break;
            case IUnresolvedNamespaceQualifiedNameNode n:
                yield return n.Context;
                foreach (var child in n.GenericArguments)
                    yield return child;
                yield break;
            case IUnresolvedTypeQualifiedNameExpressionNode n:
                yield return n.Context;
                foreach (var child in n.GenericArguments)
                    yield return child;
                yield break;
            case IUnresolvedMemberAccessExpressionNode n:
                yield return n.TempContext;
                foreach (var child in n.GenericArguments)
                    yield return child;
                yield break;
            case IUnresolvedTypeQualifiedNameNode n:
                yield return n.Context;
                foreach (var child in n.GenericArguments)
                    yield return child;
                yield break;
            case IUnsafeExpressionNode n:
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
            case IVariableNameExpressionNode n:
                yield break;
            case IVoidTypeSymbolNode n:
                yield break;
            case IWhileExpressionNode n:
                yield return n.TempCondition;
                yield return n.Block;
                yield break;
        }
    }
}
