using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;

internal static class TypeExpressionsAntetypesAspect
{
    public static IMaybeAntetype ViewpointType_Antetype(IViewpointTypeNode node)
        // Viewpoint has not affect on the antetype
        => node.Referent.Antetype;

    public static IMaybeAntetype OptionalType_Antetype(IOptionalTypeNode node)
        => node.Referent.Antetype.MakeOptional();

    // TODO report error for `void?`

    public static IMaybeAntetype CapabilityType_Antetype(ICapabilityTypeNode node)
        // Capability has not affect on the antetype
        => node.Referent.Antetype;

    public static IMaybeAntetype SpecialTypeName_Antetype(ISpecialTypeNameNode node)
        => (IMaybeAntetype)node.ReferencedSymbol.GetDataType()!.ToAntetype();

    public static IMaybeAntetype FunctionType_Antetype(IFunctionTypeNode node)
    {
        var parameters = node.Parameters.Select(p => p.Referent.Antetype)
                             .OfType<INonVoidAntetype>().ToFixedList();
        if (parameters.Count != node.Parameters.Count)
            // Not all parameters are known and non-void
            return IAntetype.Unknown;
        if (node.Return.Antetype is not IAntetype returnAntetype)
            return IAntetype.Unknown;
        return new FunctionAntetype(parameters, returnAntetype);
    }

    public static IMaybeAntetype IdentifierTypeName_Antetype(IIdentifierTypeNameNode node)
        => (IMaybeAntetype?)node.ReferencedDeclaration?.Symbol.GetDeclaredType()?.ToAntetype()
           ?? IAntetype.Unknown;

    public static IMaybeAntetype GenericTypeName_Antetype(IGenericTypeNameNode node)
        => throw new System.NotImplementedException();
}
