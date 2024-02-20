using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public static partial class TypeOperations
{
    /// <summary>
    /// Does this type (of a field) maintain the independence of independent type parameters?
    /// </summary>
    public static bool MaintainsIndependence(this DataType type)
        // Independent types can be used directly as fields, so the top level is an independent context
        => type.MaintainsIndependence(Independence.Allowed);

    private static bool MaintainsIndependence(this DataType type, Independence context)
    {
        return type switch
        {
            GenericParameterType t => !t.Parameter.IsIndependent || context == Independence.Allowed,
            CapabilityType t => t.BareType.MaintainsIndependence(context),
            ViewpointType t => t.Referent.MaintainsIndependence(context),
            EmptyType _ => true,
            UnknownType _ => true,
            ConstValueType _ => true,
            // The referent of an optional type is basically `out T` (covariant)
            OptionalType t => t.Referent.MaintainsIndependence(context.ChildAllows(false)),
            FunctionType t => t.MaintainsIndependence(),
            _ => throw ExhaustiveMatch.Failed(type),
        };
    }

    private static bool MaintainsIndependence(this BareType type, Independence context)
    {
        foreach (var (parameter, argument) in type.GenericParameterArguments)
            switch (parameter.ParameterVariance)
            {
                default:
                    throw ExhaustiveMatch.Failed(parameter.TypeVariance);
                case ParameterVariance.Contravariant: // i.e. `in`
                case ParameterVariance.Invariant:
                case ParameterVariance.Covariant: // i.e. `out`
                    if (!argument.MaintainsIndependence(context.ChildAllows(false)))
                        return false;
                    break;
                case ParameterVariance.Independent: // i.e. `ind`
                    if (!argument.MaintainsIndependence(context.ChildAllows(true)))
                        return false;
                    break;
            }

        return true;
    }

    private static bool MaintainsIndependence(this FunctionType type)
    {
        // Within function types, independent types are completely blocked

        foreach (var parameter in type.Parameters)
            if (!parameter.Type.MaintainsIndependence(Independence.Blocked))
                return false;

        if (!type.Return.Type.MaintainsIndependence(Independence.Blocked))
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
        Allowed,
    }

    private static Independence ChildAllows(this Independence context, bool allowed)
        => context == Independence.Blocked ? Independence.Blocked
            : (allowed ? Independence.Allowed : Independence.Disallowed);
}
