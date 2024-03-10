using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.ConstValue;
using Azoth.Tools.Bootstrap.Compiler.Types.Declared;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public static partial class TypeOperations
{
    /// <param name="exact">Whether the independence must match exactly. For base classes it must.</param>
    public static bool SupertypeMaintainsIndependence(this DataType type, bool exact)
        => SupertypeMaintainsIndependence(type, exact, ParameterIndependence.None);

    private static bool SupertypeMaintainsIndependence(this DataType type, bool exact, ParameterIndependence context)
        => type switch
        {
            GenericParameterType t => t.SupertypeMaintainsIndependence(exact, context),
            CapabilityType t => t.BareType.SupertypeMaintainsIndependence(exact, context),
            ViewpointType t => t.Referent.SupertypeMaintainsIndependence(exact, context),
            EmptyType _ => true,
            UnknownType _ => true,
            ConstValueType _ => true,
            OptionalType t => t.Referent.SupertypeMaintainsIndependence(exact, context),
            FunctionType t => t.SupertypeMaintainsIndependence(),
            _ => throw ExhaustiveMatch.Failed(type),
        };

    private static bool SupertypeMaintainsIndependence(this GenericParameterType type, bool exact, ParameterIndependence context)
        => context == type.Parameter.Independence || (!exact && context >= type.Parameter.Independence);

    private static bool SupertypeMaintainsIndependence(this BareType type, bool exact, ParameterIndependence context)
    {
        // Once the supertype is a trait exact independence is not required
        exact &= type.DeclaredType is DeclaredReferenceType { IsClass: true } or DeclaredValueType;
        foreach (var (parameter, argument) in type.GenericParameterArguments)
            if (!argument.SupertypeMaintainsIndependence(exact, parameter.Independence))
                return false;

        return true;
    }

    private static bool SupertypeMaintainsIndependence(this FunctionType type)
    {
        // Within function types, independent types are completely blocked

        foreach (var parameter in type.Parameters)
            if (!parameter.Type.SupertypeMaintainsIndependence(false, ParameterIndependence.None))
                return false;

        if (!type.Return.Type.SupertypeMaintainsIndependence(false, ParameterIndependence.None))
            return false;

        return true;
    }
}
