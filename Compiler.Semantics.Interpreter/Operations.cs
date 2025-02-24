using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter;

internal static class Operations
{
    public static AzothValue Convert(this AzothValue value, Type from, CapabilityType to, bool isOptional)
    {
        var fromPlainType = from.PlainType;
        var toPlainType = to.PlainType;

        if (from is CapabilityType { TypeConstructor: IntegerLiteralTypeConstructor })
        {
            if (ReferenceEquals(toPlainType, PlainType.Int)) return AzothValue.Int(value.IntValue);
            if (ReferenceEquals(toPlainType, PlainType.UInt)) return AzothValue.Int(value.IntValue);
            if (ReferenceEquals(toPlainType, PlainType.Int8)) return AzothValue.I8((sbyte)value.IntValue);
            if (ReferenceEquals(toPlainType, PlainType.Byte)) return AzothValue.Byte((byte)value.IntValue);
            if (ReferenceEquals(toPlainType, PlainType.Int16)) return AzothValue.I16((short)value.IntValue);
            if (ReferenceEquals(toPlainType, PlainType.UInt16)) return AzothValue.U16((ushort)value.IntValue);
            if (ReferenceEquals(toPlainType, PlainType.Int32)) return AzothValue.I32((int)value.IntValue);
            if (ReferenceEquals(toPlainType, PlainType.UInt32)) return AzothValue.U32((uint)value.IntValue);
            if (ReferenceEquals(toPlainType, PlainType.Int64)) return AzothValue.I64((long)value.IntValue);
            if (ReferenceEquals(toPlainType, PlainType.UInt64)) return AzothValue.U64((ulong)value.IntValue);
            if (ReferenceEquals(toPlainType, PlainType.Offset)) return AzothValue.Offset((nint)(long)value.IntValue);
            if (ReferenceEquals(toPlainType, PlainType.Size)) return AzothValue.Size((nuint)(ulong)value.IntValue);
            if (ReferenceEquals(toPlainType, PlainType.NInt)) return AzothValue.NInt((nint)(long)value.IntValue);
            if (ReferenceEquals(toPlainType, PlainType.NUInt)) return AzothValue.NUInt((nuint)(ulong)value.IntValue);
        }

        if (from is CapabilityType { TypeConstructor: BoolTypeConstructor or BoolLiteralTypeConstructor })
        {
            if (ReferenceEquals(toPlainType, PlainType.Int8)) return AzothValue.I8((sbyte)(value.BoolValue ? 1 : 0));
            if (ReferenceEquals(toPlainType, PlainType.Byte)) return AzothValue.Byte((byte)(value.BoolValue ? 1 : 0));
            if (ReferenceEquals(toPlainType, PlainType.Int16)) return AzothValue.I16((short)(value.BoolValue ? 1 : 0));
            if (ReferenceEquals(toPlainType, PlainType.UInt16)) return AzothValue.U16((ushort)(value.BoolValue ? 1 : 0));
            if (ReferenceEquals(toPlainType, PlainType.Int32)) return AzothValue.I32(value.BoolValue ? 1 : 0);
            if (ReferenceEquals(toPlainType, PlainType.UInt32)) return AzothValue.U32((uint)(value.BoolValue ? 1 : 0));
            if (ReferenceEquals(toPlainType, PlainType.Int64)) return AzothValue.I64(value.BoolValue ? 1 : 0);
            if (ReferenceEquals(toPlainType, PlainType.UInt64)) return AzothValue.U64((uint)(value.BoolValue ? 1 : 0));
            if (ReferenceEquals(toPlainType, PlainType.Offset)) return AzothValue.Offset(value.BoolValue ? 1 : 0);
            if (ReferenceEquals(toPlainType, PlainType.Size)) return AzothValue.Size((nuint)(value.BoolValue ? 1 : 0));
            if (ReferenceEquals(toPlainType, PlainType.NInt)) return AzothValue.NInt(value.BoolValue ? 1 : 0);
            if (ReferenceEquals(toPlainType, PlainType.NUInt)) return AzothValue.NUInt((nuint)(value.BoolValue ? 1 : 0));
            if (ReferenceEquals(toPlainType, PlainType.Int)) return AzothValue.Int(value.BoolValue ? BigInteger.One : BigInteger.Zero);
            if (ReferenceEquals(toPlainType, PlainType.UInt)) return AzothValue.Int(value.BoolValue ? BigInteger.One : BigInteger.Zero);
            if (ReferenceEquals(toPlainType, PlainType.Bool)) return value;
        }

        if (ReferenceEquals(fromPlainType, PlainType.Byte))
        {
            if (ReferenceEquals(toPlainType, PlainType.Int16)) return AzothValue.I16(value.ByteValue);
            if (ReferenceEquals(toPlainType, PlainType.UInt16)) return AzothValue.U16(value.ByteValue);
            if (ReferenceEquals(toPlainType, PlainType.Int32)) return AzothValue.I32(value.ByteValue);
            if (ReferenceEquals(toPlainType, PlainType.UInt32)) return AzothValue.U32(value.ByteValue);
            if (ReferenceEquals(toPlainType, PlainType.Offset)) return AzothValue.Offset(value.ByteValue);
            if (ReferenceEquals(toPlainType, PlainType.Size)) return AzothValue.Size(value.ByteValue);
            if (ReferenceEquals(toPlainType, PlainType.NInt)) return AzothValue.NInt(value.ByteValue);
            if (ReferenceEquals(toPlainType, PlainType.NUInt)) return AzothValue.NUInt(value.ByteValue);
            if (ReferenceEquals(toPlainType, PlainType.Int)) return AzothValue.Int(value.ByteValue);
            if (ReferenceEquals(toPlainType, PlainType.UInt)) return AzothValue.Int(value.ByteValue);
        }

        if (ReferenceEquals(fromPlainType, PlainType.Size))
        {
            if (ReferenceEquals(toPlainType, PlainType.Offset)) return AzothValue.Offset((nint)value.SizeValue);
            if (ReferenceEquals(toPlainType, PlainType.Size)) return AzothValue.Size(value.SizeValue);
            if (ReferenceEquals(toPlainType, PlainType.NInt)) return AzothValue.NInt((nint)value.SizeValue);
            if (ReferenceEquals(toPlainType, PlainType.NUInt)) return AzothValue.NUInt(value.SizeValue);
            if (ReferenceEquals(toPlainType, PlainType.Int)) return AzothValue.Int(value.SizeValue);
            if (ReferenceEquals(toPlainType, PlainType.UInt)) return AzothValue.Int(value.SizeValue);
        }

        if (ReferenceEquals(fromPlainType, PlainType.Offset))
        {
            if (ReferenceEquals(toPlainType, PlainType.NInt)) return AzothValue.NInt((nint)value.SizeValue);
            if (ReferenceEquals(toPlainType, PlainType.Int)) return AzothValue.Int(value.OffsetValue);
        }

        if (ReferenceEquals(fromPlainType, PlainType.Int)
            || ReferenceEquals(fromPlainType, PlainType.UInt))
        {
            if (ReferenceEquals(toPlainType, PlainType.Int)) return value;
            var fromValue = value.IntValue;
            if (ReferenceEquals(toPlainType, PlainType.UInt))
            {
                var isSigned = fromValue.Sign < 0;
                if (isSigned)
                {
                    if (isOptional) return AzothValue.None;
                    throw new AbortException($"Cannot convert value {fromValue} to {to.ToILString()}");
                }
                return value;
            }

            if (to is { TypeConstructor: FixedSizeIntegerTypeConstructor fixedSizeIntegerType })
            {
                var isSigned = fromValue.Sign < 0;
                if (isSigned && (!fixedSizeIntegerType.IsSigned
                                 || fromValue.GetBitLength() > fixedSizeIntegerType.Bits))
                {
                    if (isOptional) return AzothValue.None;
                    throw new AbortException($"Cannot convert value {fromValue} to {to.ToILString()}");
                }
            }

            if (ReferenceEquals(toPlainType, PlainType.Int8)) return AzothValue.I8((sbyte)fromValue);
            if (ReferenceEquals(toPlainType, PlainType.Byte)) return AzothValue.Byte((byte)fromValue);
            if (ReferenceEquals(toPlainType, PlainType.Int16)) return AzothValue.I16((short)fromValue);
            if (ReferenceEquals(toPlainType, PlainType.UInt16)) return AzothValue.U16((ushort)fromValue);
            if (ReferenceEquals(toPlainType, PlainType.Int32)) return AzothValue.I32((int)fromValue);
            if (ReferenceEquals(toPlainType, PlainType.UInt32)) return AzothValue.U32((uint)fromValue);
            if (ReferenceEquals(toPlainType, PlainType.Int64)) return AzothValue.I64((long)fromValue);
            if (ReferenceEquals(toPlainType, PlainType.UInt64)) return AzothValue.U64((ulong)fromValue);
            if (ReferenceEquals(toPlainType, PlainType.Offset)) return AzothValue.Offset((nint)(long)fromValue);
            if (ReferenceEquals(toPlainType, PlainType.Size)) return AzothValue.Size((nuint)(ulong)fromValue);
            if (ReferenceEquals(toPlainType, PlainType.NInt)) return AzothValue.NInt((nint)(long)fromValue);
            if (ReferenceEquals(toPlainType, PlainType.NUInt)) return AzothValue.NUInt((nuint)(ulong)fromValue);
        }

        throw new NotImplementedException($"Conversion from {from.ToILString()} to {to.ToILString()}");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // TODO [Inline(InlineBehavior.Remove)]
    public static AzothValue IncrementInt(this AzothValue value)
        => AzothValue.Int(value.IntValue + 1);

    public static bool IsOfType(this AzothValue value, Type type, BareType? bareSelfType)
    {
        // TODO using isolated here is a hack to take the capability of the type without modification.
        // This is part of the lack of clarity about how capabilities should work when doing type checks.
        type = bareSelfType?.With(Capability.Isolated).TypeReplacements.ApplyTo(type) ?? type;
        return value.IsOfType(type);
    }

    private static bool IsOfType(this AzothValue value, Type type)
    {
        switch (type)
        {
            default:
                throw ExhaustiveMatch.Failed(type);
            case CapabilityType t:
                return value.IsOfType(t.BareType, t.Capability.AllowsWrite);
            case GenericParameterType _:
                // Should be unreachable since type replacement should have removed all generic parameter types.
                throw new UnreachableException();
            case RefType t:
                // TODO what about `iref` vs `ref` and `var` vs not?
                if (value.AsRef() is not { } refValue)
                    return false;
                return refValue.Value.IsOfType(t.Referent);
            case CapabilitySetSelfType t:
                return value.IsOfType(t.BareType, t.CapabilitySet.AnyCapabilityAllowsWrite);
            case CapabilitySetRestrictedType t:
                return value.IsOfType(t.Referent);
            case CapabilityViewpointType t:
                // TODO what about when the viewpoint removes `var` from `ref var`?
                return value.IsOfType(t.Referent);
            case SelfViewpointType t:
                // TODO what about when the viewpoint removes `var` from `ref var`?
                return value.IsOfType(t.Referent);
            case FunctionType t:
                return value.AsFunctionReference() is { } r && r.FunctionType.IsSubtypeOf(t);
            case OptionalType t:
                if (value.IsNone) return true;
                return value.IsOfType(t.Referent);
            case NeverType _:
                return false;
            case VoidType _:
                return true;
        }
    }

    private static bool IsOfType(this AzothValue value, BareType bareType, bool otherAllowsWrite)
    {
        if (bareType.TypeConstructor.Semantics != TypeSemantics.Reference)
            // TODO support checking value types, and associated types
            throw new NotImplementedException();

        // It is a reference type
        if (!value.InstanceValue.IsObject)
            return false;

        var obj = value.ObjectValue;
        return obj.BareType.IsSubtypeOf(bareType, otherAllowsWrite);
    }

    // TODO will this be needed or should it be removed?
    public static AzothValue Increment(this AzothValue value, CapabilityType type)
    {
        var plainType = type.PlainType;
        if (ReferenceEquals(plainType, PlainType.Int) || ReferenceEquals(plainType, PlainType.UInt))
            return AzothValue.Int(value.IntValue + 1);
        if (ReferenceEquals(plainType, PlainType.Int8)) return AzothValue.I8((sbyte)(value.I8Value + 1));
        if (ReferenceEquals(plainType, PlainType.Byte)) return AzothValue.Byte((byte)(value.ByteValue + 1));
        if (ReferenceEquals(plainType, PlainType.Int16)) return AzothValue.I16((short)(value.I16Value + 1));
        if (ReferenceEquals(plainType, PlainType.UInt16)) return AzothValue.U16((ushort)(value.U16Value + 1));
        if (ReferenceEquals(plainType, PlainType.Int32)) return AzothValue.I32(value.I32Value + 1);
        if (ReferenceEquals(plainType, PlainType.UInt32)) return AzothValue.U32(value.U32Value + 1);
        if (ReferenceEquals(plainType, PlainType.Int64)) return AzothValue.I64(value.I64Value + 1);
        if (ReferenceEquals(plainType, PlainType.UInt64)) return AzothValue.U64(value.U64Value + 1);
        if (ReferenceEquals(plainType, PlainType.Size)) return AzothValue.Size(value.SizeValue + 1);
        if (ReferenceEquals(plainType, PlainType.Offset)) return AzothValue.Offset(value.OffsetValue + 1);

        throw new NotImplementedException($"Increment of {type}");
    }
}
