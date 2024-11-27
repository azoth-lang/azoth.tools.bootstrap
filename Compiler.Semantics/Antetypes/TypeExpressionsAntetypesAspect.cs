using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal static partial class TypeExpressionsAntetypesAspect
{
    public static partial IMaybeAntetype ViewpointType_NamedAntetype(IViewpointTypeNode node)
        // Viewpoint has no effect on the antetype
        => node.Referent.NamedAntetype;

    public static partial IMaybeAntetype OptionalType_NamedAntetype(IOptionalTypeNode node)
        => node.Referent.NamedAntetype.MakeOptional();

    // TODO report error for `void?`

    public static partial IMaybeAntetype CapabilityType_NamedAntetype(ICapabilityTypeNode node)
        // Capability has not affect on the antetype
        => node.Referent.NamedAntetype;

    public static partial IMaybeAntetype SpecialTypeName_NamedAntetype(ISpecialTypeNameNode node)
    {
        // TODO do not use symbols at this stage of the compiler
        return (IMaybeAntetype?)node.ReferencedSymbol.GetDataType()?.ToAntetype()
               ?? (IMaybeAntetype?)node.ReferencedSymbol.GetDeclaredType()?.ToAntetype() ?? IAntetype.Unknown;
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
        return new FunctionAntetype(parameters, returnAntetype);
    }

    public static partial IMaybeAntetype IdentifierTypeName_NamedAntetype(IIdentifierTypeNameNode node)
    {
        // TODO do not use symbols at this stage of the compiler
        var referencedSymbol = node.ReferencedDeclaration?.Symbol;
        return (IMaybeAntetype?)referencedSymbol?.GetDataType()?.ToAntetype()
               ?? (IMaybeAntetype?)referencedSymbol?.GetDeclaredType()?.ToAntetype()
               ?? IAntetype.Unknown;
    }

    public static partial IMaybeAntetype GenericTypeName_NamedAntetype(IGenericTypeNameNode node)
    {
        // TODO do not use symbols at this stage of the compiler
        var referencedSymbol = node.ReferencedDeclaration?.Symbol;
        var declaredAntetype = referencedSymbol?.GetDeclaredType()?.ToAntetype();
        if (declaredAntetype is null)
            return IAntetype.Unknown;
        var antetypeArguments = node.TypeArguments.Select(a => a.NamedAntetype).OfType<IAntetype>().ToFixedList();
        if (antetypeArguments.Count != node.TypeArguments.Count)
            return IAntetype.Unknown;
        return declaredAntetype.With(antetypeArguments);
    }

    public static partial IMaybeAntetype TypeNameExpression_NamedAntetype(ITypeNameExpressionNode node)
    {
        // TODO do not use symbols at this stage of the compiler
        var referencedSymbol = node.ReferencedDeclaration.Symbol;
        var declaredAntetype = referencedSymbol.GetDeclaredType()?.ToAntetype();
        if (declaredAntetype is null)
            return IAntetype.Unknown;
        var antetypeArguments = node.TypeArguments.Select(a => a.NamedAntetype).OfType<IAntetype>().ToFixedList();
        if (antetypeArguments.Count != node.TypeArguments.Count)
            return IAntetype.Unknown;
        return declaredAntetype.With(antetypeArguments);
    }
}
