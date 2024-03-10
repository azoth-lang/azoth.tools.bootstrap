using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public static partial class TypeOperations
{
    public static bool IsOutputSafe(this Pseudotype type, bool nonwriteableSelf)
        => type.IsVarianceSafe(TypeVariance.Covariant, nonwriteableSelf);

    public static bool IsInputSafe(this Pseudotype type, bool nonwriteableSelf)
        => type.IsVarianceSafe(TypeVariance.Contravariant, nonwriteableSelf);

    public static bool IsInputAndOutputSafe(this Pseudotype type, bool nonwriteableSelf)
        => type.IsVarianceSafe(TypeVariance.Invariant, nonwriteableSelf);

    private static bool IsVarianceSafe(this Pseudotype type, TypeVariance context, bool nonwritableSelf)
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


    private static bool IsVarianceSafe(this FunctionType type, TypeVariance context, bool nonwritableSelf)
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
        TypeVariance context,
        bool nonwritableSelf)
    {
        var nonwritableType = !typeCapability.AnyCapabilityAllowsWrite;
        foreach (var (parameter, argument) in type.GenericParameterArguments)
            switch (parameter.Variance)
            {
                default:
                    throw ExhaustiveMatch.Failed(parameter.Variance);
                case TypeVariance.Contravariant: // i.e. `in`
                    if (!argument.IsVarianceSafe(context.Inverse(), nonwritableSelf))
                        return false;
                    break;
                case TypeVariance.Invariant:
                    if (!argument.IsVarianceSafe(TypeVariance.Invariant, nonwritableSelf))
                        return false;
                    break;
                case TypeVariance.NonwritableCovariant: // i.e. `nonwriteable out`
                    var newContext = nonwritableType ? TypeVariance.Invariant : context;
                    if (!argument.IsVarianceSafe(newContext, nonwritableSelf))
                        return false;
                    break;
                case TypeVariance.Covariant: // i.e. `out`
                    if (!argument.IsVarianceSafe(context, nonwritableSelf))
                        return false;
                    break;
            }

        return true;
    }
}
