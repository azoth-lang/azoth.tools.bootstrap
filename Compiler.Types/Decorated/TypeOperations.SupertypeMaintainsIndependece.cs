using Azoth.Tools.Bootstrap.Compiler.Core.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

public static partial class TypeOperations
{
    private static bool SupertypeMaintainsIndependence(this Type type, bool exact, TypeParameterIndependence context)
        => type switch
        {
            GenericParameterType t => t.SupertypeMaintainsIndependence(exact, context),
            CapabilityType t => t.SupertypeMaintainsIndependence(exact),
            CapabilitySetSelfType _ => true,
            CapabilityViewpointType t => t.Referent.SupertypeMaintainsIndependence(exact, context),
            SelfViewpointType t => t.Referent.SupertypeMaintainsIndependence(exact, context),
            VoidType _ => true,
            NeverType _ => true,
            OptionalType t => t.Referent.SupertypeMaintainsIndependence(exact, context),
            // TODO not sure RefType is correct
            RefType t => t.Referent.SupertypeMaintainsIndependence(exact, context),
            FunctionType t => t.SupertypeMaintainsIndependence(),
            _ => throw ExhaustiveMatch.Failed(type),
        };

    private static bool SupertypeMaintainsIndependence(this GenericParameterType type, bool exact, TypeParameterIndependence context)
    {
        var independence = type.Parameter.Independence;
        return context switch
        {
            TypeParameterIndependence.None => independence == TypeParameterIndependence.None,
            TypeParameterIndependence.ShareableIndependent
                => independence == TypeParameterIndependence.ShareableIndependent
                   || (!exact && independence == TypeParameterIndependence.Independent),
            TypeParameterIndependence.Independent => independence == TypeParameterIndependence.Independent,
            _ => throw ExhaustiveMatch.Failed(context),
        };
    }

    /// <param name="exact">Whether the independence must match exactly. For base classes it must.</param>
    public static bool SupertypeMaintainsIndependence(this CapabilityType type, bool exact)
    {
        // Once the supertype is a trait exact independence is not required
        exact &= type.TypeConstructor?.CanHaveFields ?? false;
        foreach (var (parameter, argument) in type.TypeParameterArguments)
            if (!argument.SupertypeMaintainsIndependence(exact, parameter.Independence))
                return false;

        return true;
    }

    /// <param name="exact">Whether the independence must match exactly. For base classes it must.</param>
    public static bool SupertypeMaintainsIndependence(this BareType bareType, bool exact)
    {
        if (bareType is not { TypeConstructor: var typeConstructor }) return true;

        // Once the supertype is a trait exact independence is not required
        exact &= typeConstructor.CanHaveFields;
        foreach (var (parameter, argument) in bareType.TypeParameterArguments)
            if (!argument.SupertypeMaintainsIndependence(exact, parameter.Independence))
                return false;

        return true;
    }

    private static bool SupertypeMaintainsIndependence(this FunctionType type)
    {
        foreach (var parameter in type.Parameters)
            if (!parameter.Type.SupertypeMaintainsIndependence(false, TypeParameterIndependence.None))
                return false;

        if (!type.Return.SupertypeMaintainsIndependence(false, TypeParameterIndependence.None))
            return false;

        return true;
    }
}
