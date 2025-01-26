using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.PlainTypes;

internal static partial class TypeExpressionsPlainTypesAspect
{
    public static partial IMaybePlainType ViewpointType_NamedPlainType(IViewpointTypeNode node)
        // Viewpoint has no effect on the plain type
        => node.Referent.NamedPlainType;

    public static partial IMaybePlainType OptionalType_NamedPlainType(IOptionalTypeNode node)
        => node.Referent.NamedPlainType.MakeOptional();

    // TODO report error for `void?`

    public static partial IMaybePlainType CapabilityType_NamedPlainType(ICapabilityTypeNode node)
        // Capability has no effect on the plain type
        => node.Referent.NamedPlainType;

    public static partial IMaybePlainType BuiltInTypeName_NamedPlainType(IBuiltInTypeNameNode node)
        => node.ReferencedDeclaration?.TypeConstructor.TryConstructNullaryPlainType(containingType: null) ?? IMaybePlainType.Unknown;

    public static partial IMaybePlainType FunctionType_NamedPlainType(IFunctionTypeNode node)
    {
        var parameters = node.Parameters.Select(p => p.Referent.NamedPlainType)
                             .OfType<NonVoidPlainType>().ToFixedList();
        if (parameters.Count != node.Parameters.Count)
            // Not all parameters are known and non-void
            return PlainType.Unknown;
        if (node.Return.NamedPlainType is not PlainType returnPlainType)
            return PlainType.Unknown;
        return new FunctionPlainType(parameters, returnPlainType);
    }

    public static partial IMaybePlainType IdentifierTypeName_NamedPlainType(IIdentifierTypeNameNode node)
    {
        // TODO do not use symbols at this stage of the compiler
        var referencedSymbol = node.ReferencedDeclaration?.Symbol;
        return referencedSymbol?.TryGetPlainType()
               ?? referencedSymbol?.TryGetTypeConstructor()?.TryConstructNullaryPlainType(containingType: null)
               ?? IMaybePlainType.Unknown;
    }

    public static partial IMaybePlainType GenericTypeName_NamedPlainType(IGenericTypeNameNode node)
    {
        // TODO do not use symbols at this stage of the compiler
        var referencedSymbol = node.ReferencedDeclaration?.Symbol;
        var declaredPlainType = referencedSymbol?.TryGetTypeConstructor();
        if (declaredPlainType is null)
            return PlainType.Unknown;
        var genericArguments = node.GenericArguments.Select(a => a.NamedPlainType).OfType<PlainType>().ToFixedList();
        if (genericArguments.Count != node.GenericArguments.Count)
            return PlainType.Unknown;
        return declaredPlainType.Construct(containingType: null, genericArguments);
    }
}
