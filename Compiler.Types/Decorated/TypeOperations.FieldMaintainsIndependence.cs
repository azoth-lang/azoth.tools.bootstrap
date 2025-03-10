using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Core.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public static partial class TypeOperations
{
    /// <summary>
    /// Does this type (of a field) maintain the independence of independent type parameters?
    /// </summary>
    public static bool FieldMaintainsIndependence(this IMaybeNonVoidType type)
        // Independent types can be used directly as fields, so the top level is an independent context
        => type.FieldMaintainsIndependence(Independence.BothAllowed);

    private static bool FieldMaintainsIndependence(this IMaybeType type, Independence context)
        => type switch
        {
            GenericParameterType t => t.FieldMaintainsIndependence(context),
            CapabilityType t => t.FieldMaintainsIndependence(context),
            CapabilityViewpointType t => t.Referent.FieldMaintainsIndependence(context),
            SelfViewpointType t => t.Referent.FieldMaintainsIndependence(context),
            CapabilitySetRestrictedType t => t.FieldMaintainsIndependence(context),
            CapabilitySetSelfType _ => true,
            VoidType _ => true,
            NeverType _ => true,
            UnknownType _ => true,
            // The referent of an optional type is basically `out T` (covariant)
            OptionalType t => t.Referent.FieldMaintainsIndependence(context.Child(Independence.Disallowed)),
            // TODO not sure RefType is correct
            RefType t => t.Referent.FieldMaintainsIndependence(context.Child(Independence.Disallowed)),
            FunctionType t => t.FieldMaintainsIndependence(),
            _ => throw ExhaustiveMatch.Failed(type),
        };

    private static bool FieldMaintainsIndependence(this GenericParameterType type, Independence context)
        => type.Parameter.Independence switch
        {
            TypeParameterIndependence.None => true,
            TypeParameterIndependence.ShareableIndependent
                => context >= Independence.BothAllowed,
            TypeParameterIndependence.Independent
                => context == Independence.BothAllowed,
            _ => throw ExhaustiveMatch.Failed(type.Parameter.Independence),
        };

    private static bool FieldMaintainsIndependence(this CapabilityType type, Independence context)
    {
        foreach (var (parameter, argument) in type.TypeParameterArguments)
        {
            var parameterIndependenceAllows = parameter.Independence switch
            {
                TypeParameterIndependence.ShareableIndependent => Independence.OnlyShareableAllowed,
                TypeParameterIndependence.Independent => Independence.BothAllowed,
                TypeParameterIndependence.None => Independence.Disallowed,
                _ => throw ExhaustiveMatch.Failed(parameter.Independence),
            };
            var parameterVarianceAllows = parameter.Variance switch
            {
                // TODO does this need to flip the behavior of nested type arguments?
                TypeParameterVariance.Contravariant // i.e. `in`
                    => parameter.Constraint == CapabilitySet.Shareable
                        ? Independence.OnlyShareableAllowed
                        : Independence.Disallowed,
                TypeParameterVariance.Invariant
                    => Independence.Disallowed,
                TypeParameterVariance.NonwritableCovariant
                    => Independence.Disallowed,
                TypeParameterVariance.Covariant // i.e. `out`
                    => Independence.Disallowed, // TODO if field is `let`, then Independence.BothAllowed
                _ => throw ExhaustiveMatch.Failed(parameter.Variance),
            };
            var parameterAllows = Max(parameterIndependenceAllows, parameterVarianceAllows);
            if (!argument.FieldMaintainsIndependence(context.Child(parameterAllows)))
                return false;
        }

        return true;
    }

    private static bool FieldMaintainsIndependence(this CapabilitySetRestrictedType type, Independence context)
    {
        // If the capability set is `shareable` and the parameter is `shareable ind` then the type
        // acts invariantly and is always allowed.
        // TODO handle other capability sets correctly
        if (type.CapabilitySet == CapabilitySet.Shareable
            && type.Referent.Parameter.Independence == TypeParameterIndependence.ShareableIndependent)
            return true;

        return type.Referent.FieldMaintainsIndependence(context);
    }

    private static bool FieldMaintainsIndependence(this FunctionType type)
    {
        // Within function types, independent types are completely blocked

        foreach (var parameter in type.Parameters)
            if (!parameter.Type.FieldMaintainsIndependence(Independence.Blocked))
                return false;

        if (!type.Return.FieldMaintainsIndependence(Independence.Blocked))
            return false;

        return true;
    }

    private enum Independence
    {
        /// <summary>
        /// Independent types are completely blocked in this context and all child contexts.
        /// </summary>
        Blocked = 1,
        Disallowed,
        BothAllowed,
        OnlyShareableAllowed,
    }

    // TODO cannot inline this method due to https://github.com/oleg-st/InlineMethod.Fody/issues/14
    //[Inline]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Independence Max(Independence left, Independence right)
        => (Independence)Math.Max((int)left, (int)right);

    private static Independence Child(this Independence context, Independence independence)
        => context == Independence.Blocked ? Independence.Blocked : independence;
}
