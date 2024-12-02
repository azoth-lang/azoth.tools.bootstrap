using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal static partial class TypeExpressionsAntetypesAspect
{
    public static partial IMaybeAntetype ViewpointType_NamedAntetype(IViewpointTypeNode node)
        // Viewpoint has no effect on the plainType
        => node.Referent.NamedAntetype;

    public static partial IMaybeAntetype OptionalType_NamedAntetype(IOptionalTypeNode node)
        => node.Referent.NamedAntetype.MakeOptional();

    // TODO report error for `void?`

    public static partial IMaybeAntetype CapabilityType_NamedAntetype(ICapabilityTypeNode node)
        // Capability has not affect on the plainType
        => node.Referent.NamedAntetype;

    public static partial IMaybeAntetype SpecialTypeName_NamedAntetype(ISpecialTypeNameNode node)
    {
        // TODO do not use symbols at this stage of the compiler
        return node.ReferencedSymbol.TryGetType()?.ToAntetype()
               ?? node.ReferencedSymbol.TryGetDeclaredType()?.TryToAntetype()
               ?? IMaybeAntetype.Unknown;
    }

    public static partial IMaybeAntetype FunctionType_NamedAntetype(IFunctionTypeNode node)
    {
        var parameters = node.Parameters.Select(p => p.Referent.NamedAntetype)
                             .OfType<INonVoidAntetype>().ToFixedList();
        if (parameters.Count != node.Parameters.Count)
            // Not all parameters are known and non-void
            return IAntetype.Unknown;
        if (node.Return.NamedAntetype is not IAntetype returnAntetype)
            return IAntetype.Unknown;
        return new FunctionPlainType(parameters, returnAntetype);
    }

    public static partial IMaybeAntetype IdentifierTypeName_NamedAntetype(IIdentifierTypeNameNode node)
    {
        // TODO do not use symbols at this stage of the compiler
        var referencedSymbol = node.ReferencedDeclaration?.Symbol;
        return referencedSymbol?.TryGetType()?.ToAntetype()
               ?? referencedSymbol?.TryGetDeclaredType()?.TryToAntetype()
               ?? IMaybeAntetype.Unknown;
    }

    public static partial IMaybeAntetype GenericTypeName_NamedAntetype(IGenericTypeNameNode node)
    {
        // TODO do not use symbols at this stage of the compiler
        var referencedSymbol = node.ReferencedDeclaration?.Symbol;
        var declaredAntetype = referencedSymbol?.TryGetDeclaredType()?.ToTypeConstructor();
        if (declaredAntetype is null)
            return IAntetype.Unknown;
        var antetypeArguments = node.TypeArguments.Select(a => a.NamedAntetype).OfType<IAntetype>().ToFixedList();
        if (antetypeArguments.Count != node.TypeArguments.Count)
            return IAntetype.Unknown;
        return declaredAntetype.Construct(antetypeArguments);
    }

    public static partial IMaybeAntetype TypeNameExpression_NamedAntetype(ITypeNameExpressionNode node)
    {
        // TODO do not use symbols at this stage of the compiler
        var referencedSymbol = node.ReferencedDeclaration.Symbol;
        var declaredAntetype = referencedSymbol.TryGetDeclaredType()?.ToTypeConstructor();
        if (declaredAntetype is null)
            return IAntetype.Unknown;
        var antetypeArguments = node.TypeArguments.Select(a => a.NamedAntetype).OfType<IAntetype>().ToFixedList();
        if (antetypeArguments.Count != node.TypeArguments.Count)
            return IAntetype.Unknown;
        return declaredAntetype.Construct(antetypeArguments);
    }
}
