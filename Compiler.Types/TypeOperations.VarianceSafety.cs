using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public static partial class TypeOperations
{
    public static bool IsOutputSafe(this Pseudotype type)
        => type.IsVarianceSafe(TypeVariance.Covariant);

    public static bool IsInputSafe(this Pseudotype type)
        => type.IsVarianceSafe(TypeVariance.Contravariant);

    public static bool IsInputAndOutputSafe(this Pseudotype type)
        => type.IsVarianceSafe(TypeVariance.Invariant);

    private static bool IsVarianceSafe(this Pseudotype type, TypeVariance variance)
    {
        return type switch
        {
            GenericParameterType t => variance.CompatibleWith(t.Parameter.Variance),
            // TODO does the viewpoint or constraint need to be checked?
            CapabilityType t => t.BareType.IsVarianceSafe(variance),
            ViewpointType t => t.Referent.IsVarianceSafe(variance),
            CapabilityTypeConstraint t => t.BareType.IsVarianceSafe(variance),
            EmptyType _ => true,
            UnknownType _ => true,
            ConstValueType _ => true,
            // The referent of an optional type is basically `out T` (covariant)
            OptionalType t => t.Referent.IsVarianceSafe(variance),
            FunctionType t => t.IsVarianceSafe(variance),
            _ => throw ExhaustiveMatch.Failed(type),
        };
    }

    private static bool IsVarianceSafe(this FunctionType type, TypeVariance variance)
    {
        // The parameters are `in` (contravariant)
        foreach (var parameter in type.Parameters)
            if (!parameter.Type.IsVarianceSafe(variance.Inverse()))
                return false;

        // The return type is `out` (covariant)
        if (!type.Return.Type.IsVarianceSafe(variance))
            return false;

        return true;
    }

    private static bool IsVarianceSafe(this BareType type, TypeVariance variance)
    {
        foreach (var (parameter, argument) in type.GenericParameterArguments)
            switch (parameter.Variance)
            {
                default:
                    throw ExhaustiveMatch.Failed(parameter.Variance);
                case TypeVariance.Contravariant: // i.e. `in`
                    if (!argument.IsVarianceSafe(variance.Inverse()))
                        return false;
                    break;
                case TypeVariance.Invariant:
                    if (!argument.IsVarianceSafe(TypeVariance.Invariant))
                        return false;
                    break;
                case TypeVariance.Covariant: // i.e. `out`
                    if (!argument.IsVarianceSafe(variance))
                        return false;
                    break;
            }

        return true;
    }
}
