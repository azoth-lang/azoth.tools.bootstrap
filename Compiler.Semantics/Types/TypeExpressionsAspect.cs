using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Parameters;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static class TypeExpressionsAspect
{
    // TODO combine these into just TypeName
    public static DataType IdentifierTypeName(IIdentifierTypeNameNode node)
        => node.BareType?.WithRead() ?? node.ReferencedSymbol?.GetDataType() ?? DataType.Unknown;

    public static DataType GenericTypeName(IGenericTypeNameNode node)
        => node.BareType?.WithRead() ?? node.ReferencedSymbol?.GetDataType() ?? DataType.Unknown;

    public static DataType SpecialTypeName(ISpecialTypeNameNode node)
        => node.BareType?.WithRead() ?? node.ReferencedSymbol.GetDataType() ?? DataType.Unknown;

    public static DataType CapabilityType(ICapabilityTypeNode node)
        // TODO better handle of capability applied to wrong type
        => (node.Referent as ITypeNameNode)?.BareType?.With(node.Capability.Capability) ?? DataType.Unknown;

    public static DataType OptionalType(IOptionalTypeNode node)
        => node.Referent.Type.ToOptional();

    public static DataType FunctionType(IFunctionTypeNode node)
        => new FunctionType(node.Parameters.Select(p => p.Parameter), new(node.Return.Type));

    public static Parameter ParameterType(IParameterTypeNode node)
        => new(node.IsLent, node.Referent.Type);
}
