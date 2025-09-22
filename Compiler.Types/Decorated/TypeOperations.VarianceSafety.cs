using Azoth.Tools.Bootstrap.Compiler.Core.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public static partial class TypeOperations
{
    /// <param name="readOnlySelf">Whether the self parameter type is readonly.
    /// <see langword="null"/> is used for base types to indicate that it could behave either way.</param>
    public static bool IsOutputSafe(this IMaybeType type, bool readOnlySelf)
        => type.IsVarianceSafe(TypeVariance.Covariant, readOnlySelf);

    /// <summary>
    /// Check if a bare supertype is output safe.
    /// </summary>
    public static bool IsSupertypeOutputSafe(this BareType type, bool? readOnlySelf)
        => type.TypeParameterArguments.IsVarianceSafe(null, TypeVariance.Covariant, readOnlySelf);

    public static bool IsInputSafe(this IMaybeType type, bool readOnlySelf)
        => type.IsVarianceSafe(TypeVariance.Contravariant, readOnlySelf);

    public static bool IsInputAndOutputSafe(this IMaybeType type, bool readOnlySelf)
        => type.IsVarianceSafe(TypeVariance.Invariant, readOnlySelf);

    private static bool IsVarianceSafe(this IMaybeType type, TypeVariance context, bool? readOnlySelf)
        => type switch
        {
            GenericParameterType t => context.CompatibleWith(t.Parameter.Variance, readOnlySelf),
            // TODO does the viewpoint or constraint need to be checked?
            CapabilityType t => t.TypeParameterArguments.IsVarianceSafe(t.Capability, context, readOnlySelf),
            CapabilityViewpointType t => t.Referent.IsVarianceSafe(context, readOnlySelf),
            SelfViewpointType t => t.Referent.IsVarianceSafe(context, readOnlySelf),
            CapabilitySetRestrictedType t => t.Referent.IsVarianceSafe(context, readOnlySelf),
            // TODO is this correct?
            CapabilitySetSelfType t => true,
            VoidType _ => true,
            NeverType _ => true,
            UnknownType _ => true,
            // The referent of an optional type is basically `out T` (covariant)
            OptionalType t => t.Referent.IsVarianceSafe(context, readOnlySelf),
            FunctionType t => t.IsVarianceSafe(context, readOnlySelf),
            _ => throw ExhaustiveMatch.Failed(type),
        };

    private static bool IsVarianceSafe(this FunctionType type, TypeVariance context, bool? readOnlySelf)
    {
        // The parameters are `in` (contravariant)
        foreach (var parameter in type.Parameters)
            if (!parameter.Type.IsVarianceSafe(context.Inverse(), readOnlySelf))
                return false;

        // The return type is `out` (covariant)
        if (!type.Return.IsVarianceSafe(context, readOnlySelf))
            return false;

        return true;
    }

    private static bool IsVarianceSafe(
        this IFixedList<TypeParameterArgument> typeParameterArguments,
        ICapabilityConstraint? typeCapability,
        TypeVariance context,
        bool? readOnlySelf)
    {
        var nonwritableType = !typeCapability?.AnyCapabilityAllowsWrite;
        foreach (var (parameter, argument) in typeParameterArguments)
        {
            var variance = parameter.Variance.ToTypeVariance(nonwritableType);
            switch (variance)
            {
                default:
                    throw ExhaustiveMatch.Failed(variance);
                case TypeVariance.Contravariant: // i.e. `in`
                    if (!argument.IsVarianceSafe(context.Inverse(), readOnlySelf)) return false;
                    break;
                case TypeVariance.Invariant:
                    if (!argument.IsVarianceSafe(TypeVariance.Invariant, readOnlySelf)) return false;
                    break;
                case null: // i.e. `readonly out` at the first level
                    if (argument is GenericParameterType { Parameter.Variance: TypeParameterVariance.Covariant })
                        return false;
                    if (!argument.IsVarianceSafe(context, readOnlySelf)) return false;
                    break;
                case TypeVariance.Covariant: // i.e. `out`
                    if (!argument.IsVarianceSafe(context, readOnlySelf)) return false;
                    break;
            }
        }

        return true;
    }
}
