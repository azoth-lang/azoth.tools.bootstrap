using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public static partial class TypeOperations
{
    public static bool IsOutputSafe(this Pseudotype type)
        => type.IsVarianceSafe(ParameterVariance.Covariant);

    public static bool IsInputSafe(this Pseudotype type)
        => type.IsVarianceSafe(ParameterVariance.Contravariant);

    public static bool IsIndependentSafe(this Pseudotype type)
        => type.IsVarianceSafe(ParameterVariance.Independent);

    private static bool IsVarianceSafe(this Pseudotype type, ParameterVariance variance)
    {
        return type switch
        {
            GenericParameterType t => variance.CompatibleWith(t.Parameter.ParameterVariance),
            // TODO does the viewpoint or constraint need to be checked?
            CapabilityType t => t.BareType.IsVarianceSafe(variance),
            ViewpointType t => t.Referent.IsVarianceSafe(variance),
            ObjectTypeConstraint t => t.BareType.IsVarianceSafe(variance),
            EmptyType _ => true,
            UnknownType _ => true,
            ConstValueType _ => true,
            // The referent of an optional type is basically `out T` (covariant)
            OptionalType t => t.Referent.IsVarianceSafe(variance),
            FunctionType t => t.IsVarianceSafe(variance),
            _ => throw ExhaustiveMatch.Failed(type),
        };
    }

    private static bool IsVarianceSafe(this FunctionType type, ParameterVariance variance)
    {
        // The parameters are `in` (contravariant)
        foreach (var parameter in type.Parameters)
            if (!parameter.Type.IsVarianceSafe(variance.Antivariance()))
                return false;

        // The return type is `out` (covariant)
        if (!type.Return.Type.IsVarianceSafe(variance))
            return false;

        return true;
    }

    private static bool IsVarianceSafe(this BareType type, ParameterVariance variance)
    {
        foreach (var (parameter, argument) in type.GenericParameterArguments)
            switch (parameter.ParameterVariance)
            {
                default:
                    throw ExhaustiveMatch.Failed(parameter.ParameterVariance);
                case ParameterVariance.Contravariant: // i.e. `in`
                    if (!argument.IsVarianceSafe(variance.Antivariance()))
                        return false;
                    break;
                case ParameterVariance.Invariant:
                    if (!argument.IsVarianceSafe(ParameterVariance.Invariant))
                        return false;
                    break;
                case ParameterVariance.Independent: // i.e. `ind`
                    if (!argument.IsVarianceSafe(ParameterVariance.Independent))
                        return false;
                    break;
                case ParameterVariance.Covariant: // i.e. `out`
                    if (!argument.IsVarianceSafe(variance))
                        return false;
                    break;
            }

        return true;
    }
}
