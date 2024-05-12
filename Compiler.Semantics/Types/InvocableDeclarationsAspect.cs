using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static class InvocableDeclarationsAspect
{
    public static FunctionType FunctionDeclaration_Type(IFunctionDeclarationNode node)
    {
        var parameterTypes = node.Parameters.Select(p => p.Parameter).ToFixedList();
        var returnType = node.Return?.Type ?? DataType.Void;
        return new FunctionType(parameterTypes, new Return(returnType));
    }

    public static Parameter NamedParameter_Parameter(INamedParameterNode node)
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

    public static void MethodSelfParameter_ContributeDiagnostics(IMethodSelfParameterNode node, Diagnostics diagnostics)
    {
        var isLent = node.IsLentBinding;
        var selfType = node.Type;
        // TODO add check back in
        //if (isLent && !selfType.CanBeLent())
        //    diagnostics.Add(TypeError.TypeCannotBeLent(node.File, node.Syntax.Span, selfType));
    }
}
