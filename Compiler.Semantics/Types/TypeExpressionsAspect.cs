using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;

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

    public static DataType OptionalType_Type(IOptionalTypeNode node)
        => node.Referent.Type.ToOptional();

    public static DataType FunctionType_Type(IFunctionTypeNode node)
        => new FunctionType(node.Parameters.Select(p => p.Parameter), new(node.Return.Type));

    public static Parameter ParameterType_Parameter(IParameterTypeNode node)
        => new(node.IsLent, node.Referent.Type);
}
