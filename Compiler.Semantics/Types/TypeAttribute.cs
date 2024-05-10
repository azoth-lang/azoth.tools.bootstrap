using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static class TypeAttribute
{
    public static DataType IdentifierTypeName(IIdentifierTypeNameNode node)
        => node.BareType?.WithRead() ?? node.ReferencedSymbol?.GetDataType() ?? DataType.Unknown;

    public static DataType GenericTypeName(IGenericTypeNameNode node)
        => node.BareType?.WithRead() ?? node.ReferencedSymbol?.GetDataType() ?? DataType.Unknown;

    public static DataType SpecialTypeName(ISpecialTypeNameNode node)
        => node.BareType.WithRead();

    public static DataType CapabilityType(ICapabilityTypeNode node)
        // TODO better handle of capability applied to wrong type
        => (node.Referent as ITypeNameNode)?.BareType?.With(node.Capability.Capability) ?? DataType.Unknown;

    public static DataType OptionalType(IOptionalTypeNode node)
        => node.Referent.Type.ToOptional();
}
