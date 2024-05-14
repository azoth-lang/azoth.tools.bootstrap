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
                foreach (var child in n.Declarations)
                    yield return child;
                yield break;
            case IUsingDirectiveNode n:
                yield break;
            case INamespaceDeclarationNode n:
                foreach (var child in n.UsingDirectives)
                    yield return child;
                foreach (var child in n.Declarations)
                    yield return child;
                yield break;
            case IFunctionDeclarationNode n:
                foreach (var child in n.Attributes)
                    yield return child;
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.Return is not null)
                    yield return n.Return;
                yield break;
            case IClassDeclarationNode n:
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
            case IStructDeclarationNode n:
                foreach (var child in n.Attributes)
                    yield return child;
                foreach (var child in n.GenericParameters)
                    yield return child;
                foreach (var child in n.SupertypeNames)
                    yield return child;
                foreach (var child in n.Members)
                    yield return child;
                yield break;
            case ITraitDeclarationNode n:
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
            case IAbstractMethodDeclarationNode n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.Return is not null)
                    yield return n.Return;
                yield break;
            case IStandardMethodDeclarationNode n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.Return is not null)
                    yield return n.Return;
                yield break;
            case IGetterMethodDeclarationNode n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                yield return n.Return;
                yield break;
            case ISetterMethodDeclarationNode n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.Return is not null)
                    yield return n.Return;
                yield break;
            case IConstructorDeclarationNode n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                yield break;
            case IInitializerDeclarationNode n:
                yield return n.SelfParameter;
                foreach (var child in n.Parameters)
                    yield return child;
                yield break;
            case IFieldDeclarationNode n:
                yield return n.TypeNode;
                yield break;
            case IAssociatedFunctionDeclarationNode n:
                foreach (var child in n.Parameters)
                    yield return child;
                if (n.Return is not null)
                    yield return n.Return;
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
                yield break;
            case IExpressionBodyNode n:
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
        }
    }
}
