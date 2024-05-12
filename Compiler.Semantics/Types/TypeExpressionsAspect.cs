using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

/// <summary>
/// Attributes for types related to any sort of type expression (e.g. `Foo?`)
/// </summary>
internal static class TypeExpressionsAspect
{
    // TODO combine these into just TypeName
    public static DataType IdentifierTypeName_Type(IIdentifierTypeNameNode node)
        => node.BareType?.WithRead() ?? node.ReferencedSymbol?.GetDataType() ?? DataType.Unknown;

    public static DataType GenericTypeName_Type(IGenericTypeNameNode node)
        => node.BareType?.WithRead() ?? node.ReferencedSymbol?.GetDataType() ?? DataType.Unknown;

    public static DataType SpecialTypeName_Type(ISpecialTypeNameNode node)
        => node.BareType?.WithRead() ?? node.ReferencedSymbol.GetDataType() ?? DataType.Unknown;

    public static DataType CapabilityType_Type(ICapabilityTypeNode node)
        // TODO better handle of capability applied to wrong type
        => (node.Referent as ITypeNameNode)?.BareType?.With(node.Capability.Capability) ?? DataType.Unknown;

    public static void CapabilityType_ContributeDiagnostics(ICapabilityTypeNode node, Diagnostics diagnostics)
    {
        var capability = node.Capability.Capability;
        if (capability.AllowsWrite && node.Type is CapabilityType { IsDeclaredConst: true } referenceType)
            diagnostics.Add(TypeError.CannotApplyCapabilityToConstantType(node.File, node.Syntax, capability,
                referenceType.DeclaredType));
    }

    public static DataType OptionalType_Type(IOptionalTypeNode node)
        => node.Referent.Type.ToOptional();

    public static DataType FunctionType_Type(IFunctionTypeNode node)
        => new FunctionType(node.Parameters.Select(p => p.Parameter), new(node.Return.Type));

    public static Parameter ParameterType_Parameter(IParameterTypeNode node)
        => new(node.IsLent, node.Referent.Type);

    public static DataType CapabilityViewpointType_Type(ICapabilityViewpointTypeNode node)
        // TODO report error if capability is not applicable to referent
        => CapabilityViewpointType.Create(node.Capability.Capability, node.Referent.Type);

    public static Pseudotype ConcreteMethodDeclaration_InheritedSelfType(IConcreteMethodDeclarationNode node)
        => node.SelfParameter.Type;

    public static DataType SelfViewpointType_Type(ISelfViewpointTypeNode node)
    {
        var selfType = node.SelfType;
        var referentType = node.Referent.Type;
        if (selfType is CapabilityType { Capability: var capability }
            && referentType is GenericParameterType genericParameterType)
            return CapabilityViewpointType.Create(capability, genericParameterType);

        if (selfType is CapabilityTypeConstraint { Capability: var capabilityConstraint })
            return SelfViewpointType.Create(capabilityConstraint, referentType);

        // TODO report error if self type is not applicable to referent

        return referentType;
    }
}
