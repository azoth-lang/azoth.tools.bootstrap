using System.Diagnostics;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
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
        => type.FieldMaintainsIndependence(Independence.Allowed);

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
        return type.Parameter switch
        {
            { HasIndependence: false } => true,
            { Independence: ParameterIndependence.SharableIndependent }
                => context >= Independence.ShareableAllowed,
            { Independence: ParameterIndependence.Independent }
                => context == Independence.Allowed,
            _ => throw new UnreachableException(),
        };
    }

    private static bool FieldMaintainsIndependence(this BareType type, Independence context)
    {
        foreach (var (parameter, argument) in type.GenericParameterArguments)
            switch (parameter.Independence)
            {
                default:
                    throw ExhaustiveMatch.Failed(parameter.Independence);
                //case ParameterVariance.Contravariant: // i.e. `in`
                //    var childIndependence = parameter.Constraint == CapabilitySet.Shareable
                //        ? Independence.ShareableAllowed
                //        : Independence.Disallowed;
                //    if (!argument.MaintainsIndependence(context.Child(childIndependence)))
                //        return false;
                //    break;
                //case ParameterVariance.Invariant:
                //case ParameterVariance.Covariant: // i.e. `out`
                //    if (!argument.MaintainsIndependence(context.Child(Independence.Disallowed)))
                //        return false;
                //    break;
                case ParameterIndependence.SharableIndependent: // i.e. `shareable ind`
                    if (!argument.FieldMaintainsIndependence(context.Child(Independence.ShareableAllowed)))
                        return false;
                    break;
                case ParameterIndependence.Independent: // i.e. `ind`
                    if (!argument.FieldMaintainsIndependence(context.Child(Independence.Allowed)))
                        return false;
                    break;
                case ParameterIndependence.None:
                    if (!argument.FieldMaintainsIndependence(context.Child(Independence.Blocked)))
                        return false;
                    break;
            }

        return true;
    }

    private static bool FieldMaintainsIndependence(this FunctionType type)
    {
        // Within function types, independent types are completely blocked

        foreach (var parameter in type.Parameters)
            if (!parameter.Type.FieldMaintainsIndependence(Independence.Blocked))
                return false;

        if (!type.Return.Type.FieldMaintainsIndependence(Independence.Blocked))
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
        ShareableAllowed,
        Allowed,
    }

    private static Independence Child(this Independence context, Independence independence)
        => context == Independence.Blocked ? Independence.Blocked : independence;
}
