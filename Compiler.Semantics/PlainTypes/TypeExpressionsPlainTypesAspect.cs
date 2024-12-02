using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.PlainTypes;

internal static partial class TypeExpressionsPlainTypesAspect
{
    public static partial IMaybePlainType ViewpointType_NamedPlainType(IViewpointTypeNode node)
        // Viewpoint has no effect on the plainType
        => node.Referent.NamedPlainType;

    public static partial IMaybePlainType OptionalType_NamedPlainType(IOptionalTypeNode node)
        => node.Referent.NamedPlainType.MakeOptional();

    // TODO report error for `void?`

    public static partial IMaybePlainType CapabilityType_NamedPlainType(ICapabilityTypeNode node)
        // Capability has not affect on the plainType
        => node.Referent.NamedPlainType;

    public static partial IMaybePlainType SpecialTypeName_NamedPlainType(ISpecialTypeNameNode node)
    {
        // TODO do not use symbols at this stage of the compiler
        return node.ReferencedSymbol.TryGetType()?.ToPlainType()
               ?? node.ReferencedSymbol.TryGetDeclaredType()?.TryToPlainType()
               ?? IMaybePlainType.Unknown;
    }

    public static partial IMaybePlainType FunctionType_NamedPlainType(IFunctionTypeNode node)
    {
        var parameters = node.Parameters.Select(p => p.Referent.NamedPlainType)
                             .OfType<INonVoidPlainType>().ToFixedList();
        if (parameters.Count != node.Parameters.Count)
            // Not all parameters are known and non-void
            return IPlainType.Unknown;
        if (node.Return.NamedPlainType is not IPlainType returnAntetype)
            return IPlainType.Unknown;
        return new FunctionPlainType(parameters, returnAntetype);
    }

    public static partial IMaybePlainType IdentifierTypeName_NamedPlainType(IIdentifierTypeNameNode node)
    {
        // TODO do not use symbols at this stage of the compiler
        var referencedSymbol = node.ReferencedDeclaration?.Symbol;
        return referencedSymbol?.TryGetType()?.ToPlainType()
               ?? referencedSymbol?.TryGetDeclaredType()?.TryToPlainType()
               ?? IMaybePlainType.Unknown;
    }

    public static partial IMaybePlainType GenericTypeName_NamedPlainType(IGenericTypeNameNode node)
    {
        // TODO do not use symbols at this stage of the compiler
        var referencedSymbol = node.ReferencedDeclaration?.Symbol;
        var declaredAntetype = referencedSymbol?.TryGetDeclaredType()?.ToTypeConstructor();
        if (declaredAntetype is null)
            return IPlainType.Unknown;
        var antetypeArguments = node.TypeArguments.Select(a => a.NamedPlainType).OfType<IPlainType>().ToFixedList();
        if (antetypeArguments.Count != node.TypeArguments.Count)
            return IPlainType.Unknown;
        return declaredAntetype.Construct(antetypeArguments);
    }

    public static partial IMaybePlainType TypeNameExpression_NamedPlainType(ITypeNameExpressionNode node)
    {
        // TODO do not use symbols at this stage of the compiler
        var referencedSymbol = node.ReferencedDeclaration.Symbol;
        var declaredAntetype = referencedSymbol.TryGetDeclaredType()?.ToTypeConstructor();
        if (declaredAntetype is null)
            return IPlainType.Unknown;
        var antetypeArguments = node.TypeArguments.Select(a => a.NamedPlainType).OfType<IPlainType>().ToFixedList();
        if (antetypeArguments.Count != node.TypeArguments.Count)
            return IPlainType.Unknown;
        return declaredAntetype.Construct(antetypeArguments);
    }
}
