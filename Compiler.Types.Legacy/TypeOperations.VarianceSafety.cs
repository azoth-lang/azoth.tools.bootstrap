using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Legacy.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Legacy;

public static partial class TypeOperations
{
    /// <param name="nonwritableSelf">Whether the self parameter type is nonwriteable.
    /// <see langword="null"/> is used for base types to indicate that it could behave either way.</param>
    public static bool IsOutputSafe(this IMaybePseudotype type, bool nonwritableSelf)
        => type.IsVarianceSafe(TypeVariance.Covariant, nonwritableSelf);

    /// <param name="nonwritableSelf">Whether the self parameter type is nonwriteable.
    /// <see langword="null"/> is used for base types to indicate that it could behave either way.</param>
    public static bool IsOutputSafe(this IMaybeType type, bool nonwritableSelf)
        => type.IsVarianceSafe(TypeVariance.Covariant, nonwritableSelf);

    /// <summary>
    /// Check if a bare supertype is output safe.
    /// </summary>
    public static bool IsSupertypeOutputSafe(this BareType type, bool? nonwritableSelf)
        => type.IsVarianceSafe(null, TypeVariance.Covariant, nonwritableSelf);

    public static bool IsInputSafe(this IMaybePseudotype type, bool nonwriteableSelf)
        => type.IsVarianceSafe(TypeVariance.Contravariant, nonwriteableSelf);

    public static bool IsInputSafe(this IMaybeType type, bool nonwriteableSelf)
        => type.IsVarianceSafe(TypeVariance.Contravariant, nonwriteableSelf);

    public static bool IsInputAndOutputSafe(this IMaybePseudotype type, bool nonwriteableSelf)
        => type.IsVarianceSafe(TypeVariance.Invariant, nonwriteableSelf);

    public static bool IsInputAndOutputSafe(this IMaybeType type, bool nonwriteableSelf)
        => type.IsVarianceSafe(TypeVariance.Invariant, nonwriteableSelf);

    private static bool IsVarianceSafe(this IMaybePseudotype type, TypeVariance context, bool? nonwritableSelf)
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
            // The referent of an optional type is basically `out T` (covariant)
            OptionalType t => t.Referent.IsVarianceSafe(context, nonwritableSelf),
            FunctionType t => t.IsVarianceSafe(context, nonwritableSelf),
            _ => throw ExhaustiveMatch.Failed(type),
        };
    }

    private static bool IsVarianceSafe(this FunctionType type, TypeVariance context, bool? nonwritableSelf)
    {
        // The parameters are `in` (contravariant)
        foreach (var parameter in type.Parameters)
            if (!parameter.Type.IsVarianceSafe(context.Inverse(), nonwritableSelf))
                return false;

        // The return type is `out` (covariant)
        if (!type.Return.IsVarianceSafe(context, nonwritableSelf))
            return false;

        return true;
    }

    private static bool IsVarianceSafe(
        this BareType type,
        ICapabilityConstraint? typeCapability,
        TypeVariance context,
        bool? nonwritableSelf)
    {
        var nonwritableType = !typeCapability?.AnyCapabilityAllowsWrite;
        foreach (var (parameter, argument) in type.GenericParameterArguments)
        {
            var variance = parameter.Variance.ToTypeVariance(nonwritableType);
            switch (variance)
            {
                default:
                    throw ExhaustiveMatch.Failed(variance);
                case TypeVariance.Contravariant: // i.e. `in`
                    if (!argument.IsVarianceSafe(context.Inverse(), nonwritableSelf)) return false;
                    break;
                case TypeVariance.Invariant:
                    if (!argument.IsVarianceSafe(TypeVariance.Invariant, nonwritableSelf)) return false;
                    break;
                case null: // i.e. `nonwriteable out` at the first level
                    if (argument is GenericParameterType { Parameter.Variance: TypeParameterVariance.Covariant })
                        return false;
                    if (!argument.IsVarianceSafe(context, nonwritableSelf)) return false;
                    break;
                case TypeVariance.Covariant: // i.e. `out`
                    if (!argument.IsVarianceSafe(context, nonwritableSelf)) return false;
                    break;
            }
        }

        return true;
    }
}
