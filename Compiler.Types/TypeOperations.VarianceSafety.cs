using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public static partial class TypeOperations
{
    public static bool IsOutputSafe(this Pseudotype type, bool nonwriteableSelf)
        => type.IsVarianceSafe(Variance.Covariant, nonwriteableSelf);

    public static bool IsInputSafe(this Pseudotype type, bool nonwriteableSelf)
        => type.IsVarianceSafe(Variance.Contravariant, nonwriteableSelf);

    public static bool IsInputAndOutputSafe(this Pseudotype type, bool nonwriteableSelf)
        => type.IsVarianceSafe(Variance.Invariant, nonwriteableSelf);

    private static bool IsVarianceSafe(this Pseudotype type, Variance context, bool nonwritableSelf)
    {
        return type switch
        {
            GenericParameterType t => context.CompatibleWith(t.Parameter.Variance, nonwritableSelf),
            // TODO does the viewpoint or constraint need to be checked?
            CapabilityType t => t.BareType.IsVarianceSafe(t.Capability, context, nonwritableSelf),
            ViewpointType t => t.Referent.IsVarianceSafe(context, nonwritableSelf),
            CapabilityTypeConstraint t => t.BareType.IsVarianceSafe(t.Capability, context, nonwritableSelf),
            EmptyType _ => true,
            UnknownType _ => true,
            ConstValueType _ => true,
            // The referent of an optional type is basically `out T` (covariant)
            OptionalType t => t.Referent.IsVarianceSafe(context, nonwritableSelf),
            FunctionType t => t.IsVarianceSafe(context, nonwritableSelf),
            _ => throw ExhaustiveMatch.Failed(type),
        };
    }


    private static bool IsVarianceSafe(this FunctionType type, Variance context, bool nonwritableSelf)
    {
        // The parameters are `in` (contravariant)
        foreach (var parameter in type.Parameters)
            if (!parameter.Type.IsVarianceSafe(context.Inverse(), nonwritableSelf))
                return false;

        // The return type is `out` (covariant)
        if (!type.Return.Type.IsVarianceSafe(context, nonwritableSelf))
            return false;

        return true;
    }

    private static bool IsVarianceSafe(
        this BareType type,
        ICapabilityConstraint typeCapability,
        Variance context,
        bool nonwritableSelf)
    {
        var nonwritableType = !typeCapability.AnyCapabilityAllowsWrite;
        foreach (var (parameter, argument) in type.GenericParameterArguments)
        {
            var variance = parameter.Variance.ToVariance(nonwritableType);
            switch (variance)
            {
                default:
                    throw ExhaustiveMatch.Failed(variance);
                case Variance.Contravariant: // i.e. `in`
                    if (!argument.IsVarianceSafe(context.Inverse(), nonwritableSelf)) return false;
                    break;
                case Variance.Invariant:
                    if (!argument.IsVarianceSafe(Variance.Invariant, nonwritableSelf)) return false;
                    break;
                case Variance.Covariant: // i.e. `out`
                    if (!argument.IsVarianceSafe(context, nonwritableSelf)) return false;
                    break;
            }
        }

        return true;
    }
}
