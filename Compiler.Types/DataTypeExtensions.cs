using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Types;

public static class DataTypeExtensions
{
    /// <summary>
    /// Tests whether a place of the target type could be assigned a value of the source type.
    /// This does not account for implicit conversions, but does allow for borrowing
    /// and sharing. It also allows for isolated upgrading to mutable.
    /// </summary>
    public static bool IsAssignableFrom(this DataType target, DataType source)
    {
        switch (target, source)
        {
            case (_, _) when target.Equals(source):
            case (UnknownType, _):
            case (_, UnknownType):
            case (BoolType, BoolConstantType):
            case (_, NeverType):
            case (BigIntegerType { IsSigned: true }, IntegerType):
            case (BigIntegerType, IntegerType { IsSigned: false }):
                return true;
            case (AnyType targetReference, ReferenceType sourceReference):
                return targetReference.Capability.IsAssignableFrom(sourceReference.Capability);
            case (ReferenceType, AnyType):
                return false;
            case (ObjectType targetReference, ObjectType sourceReference):
                return targetReference.Capability.IsAssignableFrom(sourceReference.Capability)
                       && targetReference.Name == sourceReference.Name
                       && targetReference.ContainingNamespace == sourceReference.ContainingNamespace;
            case (OptionalType targetOptional, OptionalType sourceOptional):
                return IsAssignableFrom(targetOptional.Referent, sourceOptional.Referent);
            default:
                return false;
        }
    }

    /// <summary>
    /// Validates that a type as been assigned.
    /// </summary>
    [DebuggerHidden]
    public static DataType Assigned([NotNull] this DataType? type)
        => type ?? throw new InvalidOperationException("Type not assigned.");

    [DebuggerHidden]
    public static DataType Known(this DataType? type)
    {
        if (!type.Assigned().IsFullyKnown)
            throw new InvalidOperationException($"Type {type.ToILString()} not fully known.");

        return type!;
    }

    [DebuggerHidden]
    public static DataType Known(this IPromise<DataType> promise)
    {
        var type = promise.Result;
        if (!type.IsFullyKnown)
            throw new InvalidOperationException($"Type {type.ToILString()} not fully known.");

        return type;
    }

    public static string ToILString(this FixedList<DataType> types)
        => string.Join(", ", types.Select(t => t.ToILString()));

    /// <summary>
    /// If this is a reference type or an optional reference type, the underlying reference type.
    /// Otherwise, <see langword="null"/>.
    /// </summary>
    public static ReferenceType? UnderlyingReferenceType(this DataType type)
    {
        return type switch
        {
            ReferenceType referenceType => referenceType,
            OptionalType { Referent: ReferenceType referenceType } => referenceType,
            _ => null
        };
    }
}
