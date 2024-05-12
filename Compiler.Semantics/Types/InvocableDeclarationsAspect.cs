using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static class InvocableDeclarationsAspect
{
    public static FunctionType FunctionDeclaration_Type(IFunctionDeclarationNode node)
        => FunctionType(node.Parameters, node.Return);

    private static FunctionType FunctionType(IEnumerable<INamedParameterNode> parameters, ITypeNode? @return)
    {
        var parameterTypes = parameters.Select(p => p.ParameterType).ToFixedList();
        var returnType = @return?.Type ?? DataType.Void;
        return new FunctionType(parameterTypes, new Return(returnType));
    }

    public static Parameter NamedParameter_ParameterType(INamedParameterNode node)
        => new(node.IsLentBinding, node.Type);

    public static DataType NamedParameterNode_Type(INamedParameterNode node)
        => node.TypeNode.Type;

    public static Pseudotype MethodSelfParameter_Type(IMethodSelfParameterNode node)
    {
        var declaredType = node.ContainingDeclaredType;
        var genericParameterTypes = declaredType.GenericParameterTypes;
        var capability = node.Capability;
        return capability switch
        {
            ICapabilityNode n => declaredType.With(n.Capability, genericParameterTypes),
            ICapabilitySetNode n => declaredType.With(n.Constraint, genericParameterTypes),
            _ => throw ExhaustiveMatch.Failed(capability)
        };
    }

    public static SelfParameter MethodSelfParameter_ParameterType(IMethodSelfParameterNode node)
    {
        bool isLent = node.IsLentBinding && node.Type.CanBeLent();
        return new SelfParameter(isLent, node.Type);
    }

    public static void MethodSelfParameter_ContributeDiagnostics(IMethodSelfParameterNode node, Diagnostics diagnostics)
    {
        var isLent = node.IsLentBinding;
        var selfType = node.Type;
        if (isLent && !selfType.CanBeLent())
            diagnostics.Add(TypeError.TypeCannotBeLent(node.File, node.Syntax.Span, selfType));
    }

    public static ReferenceType ConstructorSelfParameter_Type(IConstructorSelfParameterNode node)
    {
        var declaredType = node.ContainingDeclaredType;
        var capability = node.Syntax.Capability.Declared.ToSelfParameterCapability();
        return declaredType.With(capability, declaredType.GenericParameterTypes);
    }

    public static void ConstructorSelfParameter_ContributeDiagnostics(
        IConstructorSelfParameterNode node,
        Diagnostics diagnostics)
        => ContributeSelfParameterDiagnostics(node.Capability.Syntax, node.File, diagnostics);

    internal static DataType FieldParameter_Type(IFieldParameterNode node)
        => node.ReferencedSymbolNode?.Type ?? DataType.Unknown;

    public static Parameter FieldParameter_ParameterType(IFieldParameterNode node)
        => new Parameter(false, node.Type);

    public static ValueType InitializerSelfParameter_Type(InitializerSelfParameterNode node)
    {
        var declaredType = node.ContainingDeclaredType;
        var capability = node.Syntax.Capability.Declared.ToSelfParameterCapability();
        return declaredType.With(capability, declaredType.GenericParameterTypes);
    }

    public static void InitializerSelfParameter_ContributeDiagnostics(
        IInitializerSelfParameterNode node,
        Diagnostics diagnostics)
        => ContributeSelfParameterDiagnostics(node.Capability.Syntax, node.File, diagnostics);

    private static void ContributeSelfParameterDiagnostics(
        ICapabilitySyntax capabilitySyntax,
        CodeFile file,
        Diagnostics diagnostics)
    {
        var declaredCapability = capabilitySyntax.Declared;
        switch (declaredCapability)
        {
            case DeclaredCapability.Read:
            case DeclaredCapability.Mutable:
                break;
            case DeclaredCapability.Isolated:
            case DeclaredCapability.TemporarilyIsolated:
            case DeclaredCapability.Constant:
            case DeclaredCapability.TemporarilyConstant:
            case DeclaredCapability.Identity:

                diagnostics.Add(TypeError.InvalidConstructorSelfParameterCapability(file, capabilitySyntax));
                break;
            default:
                throw ExhaustiveMatch.Failed(declaredCapability);
        }
    }

    public static FunctionType AssociatedFunctionDeclaration_Type(IAssociatedFunctionDeclarationNode node)
        => FunctionType(node.Parameters, node.Return);
}
