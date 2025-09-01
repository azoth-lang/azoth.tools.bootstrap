using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.PlainTypes;

internal static partial class TypeExpressionsPlainTypesAspect
{
    #region Types
    public static partial IMaybePlainType OptionalType_NamedPlainType(IOptionalTypeNode node)
        => OptionalPlainType.Create(node.Referent.NamedPlainType);

    // TODO report error for `void?`

    public static partial IMaybePlainType CapabilityType_NamedPlainType(ICapabilityTypeNode node)
        // Capability has no effect on the plain type
        => node.Referent.NamedPlainType;

    public static partial IMaybePlainType CapabilitySetType_NamedPlainType(ICapabilitySetTypeNode node)
        // Capability has no effect on the plain type
        => node.Referent.NamedPlainType;

    public static partial IMaybePlainType FunctionType_NamedPlainType(IFunctionTypeNode node)
    {
        var parameters = node.Parameters.Select(p => p.Referent.NamedPlainType).OfType<NonVoidPlainType>()
                             .ToFixedList();
        if (parameters.Count != node.Parameters.Count)
            // Not all parameters are known and non-void
            return PlainType.Unknown;
        if (node.Return.NamedPlainType is not PlainType returnPlainType) return PlainType.Unknown;
        return new FunctionPlainType(parameters, returnPlainType);
    }

    public static partial IMaybePlainType ViewpointType_NamedPlainType(IViewpointTypeNode node)
        // Viewpoint has no effect on the plain type
        => node.Referent.NamedPlainType;
    #endregion

    #region Type Names
    public static partial IMaybePlainType BuiltInTypeName_NamedPlainType(IBuiltInTypeNameNode node)
        => NamedPlainType(containingType: null, node, []);

    public static partial IMaybePlainType IdentifierTypeName_NamedPlainType(IIdentifierTypeNameNode node)
        => NamedPlainType(containingType: null, node, []);

    public static partial IMaybePlainType GenericTypeName_NamedPlainType(IGenericTypeNameNode node)
        => NamedPlainType(containingType: null, node, node.GenericArguments);

    public static partial IMaybePlainType QualifiedTypeName_NamedPlainType(IQualifiedTypeNameNode node)
    {
        // TODO eliminate the `as` here
        var typeNameContext = node.Context as ITypeNameNode;
        return NamedPlainType(typeNameContext?.NamedPlainType as BarePlainType, node, node.GenericArguments);
    }

    private static IMaybePlainType NamedPlainType(
        BarePlainType? containingType,
        ITypeNameNode node,
        IFixedList<ITypeNode> genericArguments)
    {
        var referencedDeclaration = node.ReferencedDeclaration;
        if (referencedDeclaration is null) return PlainType.Unknown;
        var args = genericArguments.IsEmpty ? FixedList.Empty<PlainType>()
            : genericArguments.Select(a => a.NamedPlainType).OfType<PlainType>().ToFixedList();
        if (args.Count != genericArguments.Count) return PlainType.Unknown;
        return referencedDeclaration.TypeConstructor.Construct(containingType, args);
    }
    #endregion
}
