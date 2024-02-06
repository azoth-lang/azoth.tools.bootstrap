using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

public static class PseudotypeExtensions
{
    public static bool IsAssignableFrom(this Pseudotype target, Pseudotype source)
    {
        return (target, source) switch
        {
            (DataType targetType, DataType sourceType) => DataTypeExtensions.IsAssignableFrom(targetType, sourceType),
            (ObjectTypeConstraint targetType, ObjectType sourceType) => targetType.IsAssignableFrom(sourceType),
            (ObjectTypeConstraint targetType, ObjectTypeConstraint sourceType) => targetType.IsAssignableFrom(sourceType),
            _ => false,
        };
    }

    public static bool IsAssignableFrom(this ObjectTypeConstraint target, Pseudotype source)
    {
        return source switch
        {
            ObjectType objectType => target.IsAssignableFrom(objectType),
            ObjectTypeConstraint objectTypeConstraint => target.IsAssignableFrom(objectTypeConstraint),
            DataType _ => false,
            _ => throw ExhaustiveMatch.Failed(source),
        };
    }

    public static bool IsAssignableFrom(this ObjectTypeConstraint target, ObjectType source)
        => target.BareType.IsAssignableFrom(target.Capability.AllowsWrite, source.BareType)
           && target.Capability.IsAssignableFrom(source.Capability);

    public static bool IsAssignableFrom(this ObjectTypeConstraint target, ObjectTypeConstraint source)
        => target.BareType.IsAssignableFrom(target.Capability.AllowsWrite, source.BareType)
           && target.Capability.IsAssignableFrom(source.Capability);

    public static bool IsAssignableFrom(this ObjectType target, Pseudotype source)
    {
        return source switch
        {
            DataType sourceType => target.IsAssignableFrom(sourceType),
            ObjectTypeConstraint constrainedObjectType
                => target.BareType.IsAssignableFrom(target.AllowsWrite, constrainedObjectType.BareType),
            _ => throw ExhaustiveMatch.Failed(source)
        };
    }

    /// <summary>
    /// Validates that a type as been assigned.
    /// </summary>
    [DebuggerHidden]
    public static Pseudotype Assigned([NotNull] this Pseudotype? pseudotype)
        => pseudotype ?? throw new InvalidOperationException("Pseudotype not assigned.");

    [DebuggerHidden]
    public static Pseudotype Known(this Pseudotype? pseudotype)
    {
        if (!pseudotype.Assigned().IsFullyKnown)
            throw new InvalidOperationException($"Type {pseudotype.ToILString()} not fully known.");

        return pseudotype;
    }

    [DebuggerHidden]
    public static Pseudotype Known(this IPromise<Pseudotype> promise)
    {
        var pseudotype = promise.Result;
        if (!pseudotype.IsFullyKnown)
            throw new InvalidOperationException($"Pseudotype {pseudotype.ToILString()} not fully known.");

        return pseudotype;
    }
}
