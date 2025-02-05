using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

/// <summary>
/// Attributes for types related to any sort of type expression (e.g. `Foo?`)
/// </summary>
internal static partial class TypeExpressionsAspect
{
    #region Member Definitions
    public static partial IMaybeType? MethodDefinition_Children_Broadcast_MethodSelfType(IMethodDefinitionNode node)
        => node.SelfParameter.BindingType;
    #endregion

    #region Types
    public static partial IMaybeType TypeName_NamedType(ITypeNameNode node)
        => node.NamedBareType?.WithDefaultCapability()
           ?? node.ReferencedDeclaration?.TypeConstructor.TryConstructNullaryType(containingType: null) ?? IMaybeType.Unknown;

    // TODO remove if this remains a duplicate of TypeName_NamedType
    public static partial IMaybeType BuiltInTypeName_NamedType(IBuiltInTypeNameNode node)
        // Special type names don't have bare types
        => node.NamedBareType?.WithDefaultCapability()
           ?? node.ReferencedDeclaration?.TypeConstructor.TryConstructNullaryType(containingType: null) ?? IMaybeType.Unknown;

    public static partial IMaybeType CapabilityType_NamedType(ICapabilityTypeNode node)
        => (node.Referent as ITypeNameNode)?.NamedBareType?.With(node.Capability.DeclaredCapability) ?? node.Referent.NamedType;

    public static partial void CapabilityType_Contribute_Diagnostics(ICapabilityTypeNode node, DiagnosticCollectionBuilder diagnostics)
    {
        if (node.NamedType is CapabilityType { TypeConstructor.IsDeclaredConst: true, Capability.AllowsWrite: true } capabilityType)
            diagnostics.Add(TypeError.CannotApplyCapabilityToConstantType(node.File, node.Syntax, capabilityType.Capability, capabilityType.TypeConstructor));
        if (node.Referent.NamedType is GenericParameterType)
            diagnostics.Add(TypeError.CapabilityAppliedToTypeParameter(node.File, node.Syntax));
        if (node.Referent.NamedType is VoidType or NeverType)
            diagnostics.Add(TypeError.CapabilityAppliedToEmptyType(node.File, node.Syntax));
        // TODO I think there are more errors that can happen
    }

    public static partial IMaybeType OptionalType_NamedType(IOptionalTypeNode node)
        => OptionalType.Create(node.NamedPlainType, node.Referent.NamedType);

    public static partial IMaybeType FunctionType_NamedType(IFunctionTypeNode node)
        => FunctionType.Create(node.Parameters.Select(p => p.Parameter), node.Return.NamedType);

    public static partial IMaybeParameterType ParameterType_Parameter(IParameterTypeNode node)
        // TODO report an error for void type
        => ParameterType.Create(node.IsLent, node.Referent.NamedType.ToNonVoidType());

    public static partial IMaybeType CapabilityViewpointType_NamedType(ICapabilityViewpointTypeNode node)
        // A capability viewpoint type cannot be the default capability, hence not null is safe
        => node.Referent.NamedType.AccessedVia(node.Capability.DeclaredCapability.ToCapability(null)!);

    public static partial void CapabilityViewpointType_Contribute_Diagnostics(
        ICapabilityViewpointTypeNode node,
        DiagnosticCollectionBuilder diagnostics)
    {
        // TODO move this condition into the Types project to remove logic duplication
        if (node.Referent.NamedType is not GenericParameterType)
            diagnostics.Add(TypeError.CapabilityViewpointNotAppliedToTypeParameter(node.File, node.Syntax));
    }

    public static partial IMaybeType SelfViewpointType_NamedType(ISelfViewpointTypeNode node)
    {
        var selfType = node.MethodSelfType;
        var referentType = node.Referent.NamedType;
        if (selfType is CapabilityType { Capability: var capability }
            && referentType is GenericParameterType genericParameterType)
            return CapabilityViewpointType.Create(capability, genericParameterType);

        if (selfType is CapabilitySetSelfType { CapabilitySet: var capabilityConstraint1 })
            return SelfViewpointType.Create(capabilityConstraint1, referentType);

        // TODO this is a hack because we do not yet use `Self`
        if (selfType is SelfViewpointType { CapabilitySet: var capabilityConstraint2 })
            return SelfViewpointType.Create(capabilityConstraint2, referentType);

        // TODO report error if self type is not applicable to referent

        return referentType;
    }

    public static partial void SelfViewpointType_Contribute_Diagnostics(ISelfViewpointTypeNode node, DiagnosticCollectionBuilder diagnostics)
    {
        // TODO properly restrict this once `Self` type is being used
        if (node.MethodSelfType is null)
            diagnostics.Add(TypeError.SelfViewpointNotAvailable(node.File, node.Syntax));

        // TODO move this condition into the Types project to remove logic duplication
        if (node.Referent.NamedType is not (GenericParameterType
            or RefType { IsMutableBinding: true }
            or RefType { Referent: GenericParameterType }))
            diagnostics.Add(TypeError.SelfViewpointAppliedIncorrectly(node.File, node.Syntax));
    }

    public static partial IMaybeType RefType_NamedType(IRefTypeNode node)
        => RefType.Create(node.NamedPlainType, node.Referent.NamedType);
    #endregion

    #region Type Names

    #endregion
}
