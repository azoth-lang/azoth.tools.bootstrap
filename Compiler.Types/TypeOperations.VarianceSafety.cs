using System;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public static partial class TypeOperations
{
    public static bool IsOutputSafe(this DataType type)
        => type.IsVarianceSafe(Variance.Covariant);

    public static bool IsInputSafe(this DataType type)
        => type.IsVarianceSafe(Variance.Contravariant);

    private static bool IsVarianceSafe(this DataType type, Variance variance)
    {
        return type switch
        {
            GenericParameterType t => variance.CompatibleWith(t.Parameter.Variance),
            CapabilityType t => t.BareType.IsVarianceSafe(variance),
            ViewpointType t => t.Referent.IsVarianceSafe(variance),
            EmptyType _ => true,
            UnknownType _ => true,
            ConstValueType _ => true,
            // The referent of an optional type is basically `out T` (covariant)
            OptionalType t => t.Referent.IsVarianceSafe(variance),
            FunctionType _ => throw new NotImplementedException(),
            _ => throw ExhaustiveMatch.Failed(type),
        };
    }

    private static bool IsVarianceSafe(this BareType type, Variance variance)
    {
        foreach (var (parameter, argument) in type.GenericParameterArguments)
            switch (parameter.Variance)
            {
                default:
                    throw ExhaustiveMatch.Failed(parameter.Variance);
                case Variance.Contravariant: // i.e. `in`
                    if (!argument.IsVarianceSafe(variance.Antivariance()))
                        return false;
                    break;
                case Variance.Invariant:
                    if (!argument.IsVarianceSafe(Variance.Invariant))
                        return false;
                    break;
                case Variance.Independent: // i.e. `ind`
                    if (!argument.IsVarianceSafe(Variance.Independent))
                        return false;
                    break;
                case Variance.Covariant: // i.e. `out`
                    if (!argument.IsVarianceSafe(variance))
                        return false;
                    break;
            }

        return true;
    }
}
