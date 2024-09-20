using System;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public static partial class TypeOperations
{
    /// <summary>
    /// Does this type (of a field) maintain the independence of independent type parameters?
    /// </summary>
    public static bool FieldMaintainsIndependence(this DataType type)
        // Independent types can be used directly as fields, so the top level is an independent context
        => type.FieldMaintainsIndependence(Independence.BothAllowed);

    private static bool FieldMaintainsIndependence(this DataType type, Independence context)
    {
        return type switch
        {
            GenericParameterType t => t.FieldMaintainsIndependence(context),
            CapabilityType t => t.BareType.FieldMaintainsIndependence(context),
            ViewpointType t => t.Referent.FieldMaintainsIndependence(context),
            EmptyType _ => true,
            UnknownType _ => true,
            ConstValueType _ => true,
            // The referent of an optional type is basically `out T` (covariant)
            OptionalType t => t.Referent.FieldMaintainsIndependence(context.Child(Independence.Disallowed)),
            FunctionType t => t.FieldMaintainsIndependence(),
            _ => throw ExhaustiveMatch.Failed(type),
        };
    }

    private static bool FieldMaintainsIndependence(this GenericParameterType type, Independence context)
    {
        return type.Parameter.Independence switch
        {
            TypeParameterIndependence.None => true,
            TypeParameterIndependence.SharableIndependent
                => context >= Independence.BothAllowed,
            TypeParameterIndependence.Independent
                => context == Independence.BothAllowed,
            _ => throw ExhaustiveMatch.Failed(type.Parameter.Independence),
        };
    }

    private static bool FieldMaintainsIndependence(this BareType type, Independence context)
    {
        foreach (var (parameter, argument) in type.GenericParameterArguments)
        {
            var parameterIndependenceAllows = parameter.Independence switch
            {
                TypeParameterIndependence.SharableIndependent => Independence.OnlyShareableAllowed,
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
            var parameterAllows = (Independence)Math.Max((int)parameterIndependenceAllows, (int)parameterVarianceAllows);
            if (!argument.FieldMaintainsIndependence(context.Child(parameterAllows)))
                return false;
        }

        return true;
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

    private static Independence Child(this Independence context, Independence independence)
        => context == Independence.Blocked ? Independence.Blocked : independence;
}
