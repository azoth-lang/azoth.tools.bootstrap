using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core.Promises;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

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
        return (target, source) switch
        {
            (_, _) when target.Equals(source) => true,
            (UnknownType, _)
                or (_, UnknownType)
                or (BoolType, BoolConstantType)
                or (_, NeverType)
                or (BigIntegerType { IsSigned: true }, IntegerType)
                or (BigIntegerType, IntegerType { IsSigned: false })
                => true,
            (ReferenceType targetReference, ReferenceType sourceReference)
                => IsAssignableFrom(targetReference, sourceReference),
            (OptionalType targetOptional, OptionalType sourceOptional)
                => IsAssignableFrom(targetOptional.Referent, sourceOptional.Referent),
            (OptionalType targetOptional, _)
                => IsAssignableFrom(targetOptional.Referent, source),
            _ => false
        };
    }

    public static bool IsAssignableFrom(this ReferenceType target, ReferenceType source)
    {
        if (!target.Capability.IsAssignableFrom(source.Capability)) return false;

        switch (target)
        {
            default:
                throw ExhaustiveMatch.Failed(target);
            case AnyType _:
                return true;
            case ObjectType targetObjectType:
                if (source is not ObjectType sourceObjectType) return false;

                if (IsAssignableFrom(targetObjectType.BareType, sourceObjectType.BareType))
                    return true;

                // TODO remove hack to allow string to exist in both primitives and stdlib
                return target.Name == "String" && source.Name == "String"
                    && target.ContainingNamespace == NamespaceName.Global
                    && source.ContainingNamespace == NamespaceName.Global;
        }
    }

    public static bool IsAssignableFrom(this BareReferenceType target, BareReferenceType source)
        => source.Equals(target) || source.Supertypes.Contains(target);

    /// <summary>
    /// Validates that a type as been assigned.
    /// </summary>
    [DebuggerHidden]
    public static DataType Assigned([NotNull] this DataType? type)
        => type ?? throw new InvalidOperationException("Type not assigned.");

    /// <summary>
    /// Validates that a type as been assigned.
    /// </summary>
    [DebuggerHidden]
    public static ReturnType Assigned([NotNull] this ReturnType? returnType)
        => returnType ?? throw new InvalidOperationException("ReturnType not assigned.");

    [DebuggerHidden]
    public static DataType Known(this DataType? type)
    {
        if (!type.Assigned().IsFullyKnown)
            throw new InvalidOperationException($"Type {type.ToILString()} not fully known.");

        return type!;
    }

    [DebuggerHidden]
    public static ReturnType Known(this ReturnType returnType)
    {
        if (!returnType.Type.IsFullyKnown)
            throw new InvalidOperationException($"Type {returnType.ToILString()} not fully known.");

        return returnType;
    }

    [DebuggerHidden]
    public static ReturnType Known(this ReturnType? returnType) => returnType.Assigned().Known();

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

    [return: NotNullIfNotNull(nameof(type))]
    public static DataType? ToOptional(this DataType? type)
        => type switch
        {
            null => null,
            UnknownType or NeverType or VoidType => type,
            _ => new OptionalType(type),
        };

    /// <summary>
    /// Determine what the common type for two numeric types for a numeric operator is.
    /// </summary>
    /// <returns></returns>
    public static DataType? NumericOperatorCommonType(this DataType leftType, DataType rightType)
        => (leftType, rightType) switch
        {
            ({ IsFullyKnown: false }, _) => DataType.Unknown,
            (_, { IsFullyKnown: false }) => DataType.Unknown,
            (NumericType left, NeverType) => left,
            (NeverType, NumericType right) => right,
            (OptionalType { Referent: var left }, OptionalType { Referent: var right })
                => left.NumericOperatorCommonType(right).ToOptional(),
            (OptionalType { Referent: var left }, _)
                => left.NumericOperatorCommonType(rightType).ToOptional(),
            (_, OptionalType { Referent: var right })
                => leftType.NumericOperatorCommonType(right).ToOptional(),
            (BigIntegerType left, IntegerType right)
                => left.IsSigned || right.IsSigned ? DataType.Int : DataType.UInt,
            (IntegerType left, BigIntegerType right)
                => left.IsSigned || right.IsSigned ? DataType.Int : DataType.UInt,
            (PointerSizedIntegerType left, PointerSizedIntegerType right)
                => left.IsSigned || right.IsSigned ? DataType.Offset : DataType.Size,
            (PointerSizedIntegerType { IsSigned: true }, IntegerConstantType { IsInt16: true })
                or (PointerSizedIntegerType { IsSigned: false }, IntegerConstantType { IsUInt16: true })
                => leftType,
            (PointerSizedIntegerType left, IntegerConstantType right)
                => left.IsSigned || right.IsSigned ? DataType.Int : DataType.UInt,
            (IntegerConstantType { IsInt16: true }, PointerSizedIntegerType { IsSigned: true })
                or (IntegerConstantType { IsUInt16: true }, PointerSizedIntegerType { IsSigned: false })
                => rightType,
            (IntegerConstantType left, PointerSizedIntegerType right)
                => left.IsSigned || right.IsSigned ? DataType.Int : DataType.UInt,
            (FixedSizeIntegerType left, FixedSizeIntegerType right)
                when left.IsSigned == right.IsSigned
                => left.Bits >= right.Bits ? left : right,
            (FixedSizeIntegerType { IsSigned: true } left, FixedSizeIntegerType right)
                when left.Bits > right.Bits
                => left,
            (FixedSizeIntegerType left, FixedSizeIntegerType { IsSigned: true } right)
                when left.Bits < right.Bits
                => right,
            (FixedSizeIntegerType { IsSigned: true } left, IntegerConstantType right)
                when left.IsSigned || right.IsSigned
                => left.NumericOperatorCommonType(right.ToSmallestSignedIntegerType()),
            (FixedSizeIntegerType { IsSigned: false } left, IntegerConstantType { IsSigned: false } right)
                => left.NumericOperatorCommonType(right.ToSmallestUnsignedIntegerType()),
            (IntegerConstantType left, FixedSizeIntegerType right)
                when left.IsSigned || right.IsSigned
                => left.ToSmallestSignedIntegerType().NumericOperatorCommonType(right),
            (IntegerConstantType { IsSigned: false } left, FixedSizeIntegerType { IsSigned: false } right)
                => left.ToSmallestSignedIntegerType().NumericOperatorCommonType(right),
            _ => null
        };
}
