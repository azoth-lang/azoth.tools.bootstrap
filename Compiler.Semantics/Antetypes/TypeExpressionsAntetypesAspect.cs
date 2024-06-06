using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Antetypes.ConstValue;
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
        => (IMaybeAntetype?)node.ReferencedSymbol.GetDataType()?.ToAntetype()
           ?? (IMaybeAntetype?)node.ReferencedSymbol.GetDeclaredType()?.ToAntetype()
           ?? IAntetype.Unknown;

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
    {
        var referencedSymbol = node.ReferencedDeclaration?.Symbol;
        return (IMaybeAntetype?)referencedSymbol?.GetDataType()?.ToAntetype()
               ?? (IMaybeAntetype?)referencedSymbol?.GetDeclaredType()?.ToAntetype()
               ?? IAntetype.Unknown;
    }

    public static IMaybeAntetype GenericTypeName_Antetype(IGenericTypeNameNode node)
    {
        var declaredAntetype = node.ReferencedDeclaration?.Symbol.GetDeclaredType()?.ToAntetype();
        if (declaredAntetype is null)
            return IAntetype.Unknown;
        var antetypeArguments = node.TypeArguments.Select(a => a.Antetype).OfType<IAntetype>().ToFixedList();
        if (antetypeArguments.Count != node.TypeArguments.Count)
            return IAntetype.Unknown;
        return declaredAntetype.With(antetypeArguments);
    }

    public static IMaybeExpressionAntetype IntegerLiteralExpression_Antetype(IIntegerLiteralExpressionNode node)
        => new IntegerConstValueAntetype(node.Value);

    public static IMaybeExpressionAntetype BoolLiteralExpression_Antetype(IBoolLiteralExpressionNode node)
        => node.Value ? IExpressionAntetype.True : IExpressionAntetype.False;

    public static IMaybeExpressionAntetype IfExpression_Antetype(IIfExpressionNode node)
    {
        if (node.ElseClause is null)
            return node.ThenBlock.Antetype.MakeOptional();

        // TODO unify with else clause
        return node.ThenBlock.Antetype;
    }
}
