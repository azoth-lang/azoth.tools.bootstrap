using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static class InvocableDeclarationsAspect
{
    public static FunctionType FunctionDeclaration_Type(IFunctionDeclarationNode node)
    {
        var parameterTypes = node.Parameters.Select(p => p.ParameterType).ToFixedList();
        var returnType = node.Return?.Type ?? DataType.Void;
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
        var declaredCapability = node.Syntax.Capability.Declared;
        Capability capability = declaredCapability switch
        {
            DeclaredCapability.Read => Capability.InitReadOnly,
            DeclaredCapability.Mutable => Capability.InitMutable,
            DeclaredCapability.Isolated => Capability.InitMutable,
            DeclaredCapability.TemporarilyIsolated => Capability.InitMutable,
            DeclaredCapability.Constant => Capability.InitReadOnly,
            DeclaredCapability.TemporarilyConstant => Capability.InitReadOnly,
            DeclaredCapability.Identity => Capability.InitReadOnly,
            _ => throw ExhaustiveMatch.Failed(declaredCapability)
        };

        return declaredType.With(capability, declaredType.GenericParameterTypes);
    }

    public static void ConstructorSelfParameter_ContributeDiagnostics(
        IConstructorSelfParameterNode node,
        Diagnostics diagnostics)
    {
        var declaredCapability = node.Syntax.Capability.Declared;
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
                diagnostics.Add(TypeError.InvalidConstructorSelfParameterCapability(node.File, node.Capability.Syntax));
                break;
            default:
                throw ExhaustiveMatch.Failed(declaredCapability);
        }
    }

    internal static DataType FieldParameter_Type(IFieldParameterNode node)
        => node.ReferencedSymbolNode?.Type ?? DataType.Unknown;

    public static Parameter FieldParameter_ParameterType(IFieldParameterNode node)
        => new Parameter(false, node.Type);
}
