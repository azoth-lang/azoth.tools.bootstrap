using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

/// <summary>
/// Attributes for types related to any sort of type expression (e.g. `Foo?`)
/// </summary>
internal static partial class TypeExpressionsAspect
{
    public static partial IMaybeType TypeName_NamedType(ITypeNameNode node)
        => (node.NamedBareType?.WithRead() ?? node.ReferencedSymbol?.GetDataType()) ?? IMaybeType.Unknown;

    public static partial IMaybeType CapabilityType_NamedType(ICapabilityTypeNode node)
        => (node.Referent as ITypeNameNode)?.NamedBareType?.With(node.Capability.Capability) ?? node.Referent.NamedType;

    public static partial void CapabilityType_Contribute_Diagnostics(ICapabilityTypeNode node, DiagnosticCollectionBuilder diagnostics)
    {
        var capability = node.Capability.Capability;
        if (capability.AllowsWrite && node.NamedType is CapabilityType { IsDeclaredConst: true } capabilityType)
            diagnostics.Add(TypeError.CannotApplyCapabilityToConstantType(node.File, node.Syntax, capability,
                capabilityType.DeclaredType));
        if (node.Referent.NamedType is GenericParameterType)
            diagnostics.Add(TypeError.CapabilityAppliedToTypeParameter(node.File, node.Syntax));
        if (node.Referent.NamedType is EmptyType)
            diagnostics.Add(TypeError.CapabilityAppliedToEmptyType(node.File, node.Syntax));
        // TODO I think there are more errors that can happen
    }

    public static partial IMaybeType OptionalType_NamedType(IOptionalTypeNode node)
        => node.Referent.NamedType.MakeOptional();

    public static partial IMaybeType FunctionType_NamedType(IFunctionTypeNode node)
        => new FunctionType(node.Parameters.Select(p => p.Parameter), node.Return.NamedType);

    public static partial ParameterType ParameterType_Parameter(IParameterTypeNode node)
        => new(node.IsLent, node.Referent.NamedType);

    public static partial IMaybeType CapabilityViewpointType_NamedType(ICapabilityViewpointTypeNode node)
        => CapabilityViewpointType.Create(node.Capability.Capability, node.Referent.NamedType);

    public static partial void CapabilityViewpointType_Contribute_Diagnostics(
        ICapabilityViewpointTypeNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        if (node.Referent.NamedType is not GenericParameterType)
            diagnostics.Add(TypeError.CapabilityViewpointNotAppliedToTypeParameter(node.File, node.Syntax));
    }

    public static partial IMaybePseudotype? ConcreteMethodDefinition_Children_Broadcast_MethodSelfType(IConcreteMethodDefinitionNode node)
        => node.SelfParameter.BindingType;

    public static partial IMaybeType SelfViewpointType_NamedType(ISelfViewpointTypeNode node)
    {
        var selfType = node.MethodSelfType;
        var referentType = node.Referent.NamedType;
        if (selfType is CapabilityType { Capability: var capability }
            && referentType is GenericParameterType genericParameterType)
            return CapabilityViewpointType.Create(capability, genericParameterType);

        if (selfType is CapabilityTypeConstraint { Capability: var capabilityConstraint })
            return SelfViewpointType.Create(capabilityConstraint, referentType);

        // TODO report error if self type is not applicable to referent

        return referentType;
    }

    public static partial void SelfViewpointType_Contribute_Diagnostics(ISelfViewpointTypeNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node.MethodSelfType is not (CapabilityType or CapabilityTypeConstraint))
            diagnostics.Add(TypeError.SelfViewpointNotAvailable(node.File, node.Syntax));

        if (node.Referent.NamedType is not GenericParameterType)
            diagnostics.Add(TypeError.SelfViewpointNotAppliedToTypeParameter(node.File, node.Syntax));
    }
}
