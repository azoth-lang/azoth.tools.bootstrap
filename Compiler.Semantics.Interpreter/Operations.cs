using System;
using System.Diagnostics;
using System.Numerics;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Plain;
using ExhaustiveMatching;
using InlineMethod;
using Type = Azoth.Tools.Bootstrap.Compiler.Types.Decorated.Type;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter;

internal static class Operations
{
    public static Value Convert(this Value value, Type from, CapabilityType to, bool isOptional)
    {
        var fromPlainType = from.PlainType;
        var toPlainType = to.PlainType;

        if (from is CapabilityType { TypeConstructor: IntegerLiteralTypeConstructor })
        {
            if (ReferenceEquals(toPlainType, PlainType.Int)) return Value.From(value.Int);
            if (ReferenceEquals(toPlainType, PlainType.UInt)) return Value.From(value.Int);
            if (ReferenceEquals(toPlainType, PlainType.Int8)) return Value.FromI8((sbyte)value.Int);
            if (ReferenceEquals(toPlainType, PlainType.Byte)) return Value.FromByte((byte)value.Int);
            if (ReferenceEquals(toPlainType, PlainType.Int16)) return Value.FromI16((short)value.Int);
            if (ReferenceEquals(toPlainType, PlainType.UInt16)) return Value.FromU16((ushort)value.Int);
            if (ReferenceEquals(toPlainType, PlainType.Int32)) return Value.FromI32((int)value.Int);
            if (ReferenceEquals(toPlainType, PlainType.UInt32)) return Value.FromU32((uint)value.Int);
            if (ReferenceEquals(toPlainType, PlainType.Int64)) return Value.FromI64((long)value.Int);
            if (ReferenceEquals(toPlainType, PlainType.UInt64)) return Value.FromU64((ulong)value.Int);
            if (ReferenceEquals(toPlainType, PlainType.Offset)) return Value.FromOffset((nint)(long)value.Int);
            if (ReferenceEquals(toPlainType, PlainType.Size)) return Value.FromSize((nuint)(ulong)value.Int);
            if (ReferenceEquals(toPlainType, PlainType.NInt)) return Value.FromNInt((nint)(long)value.Int);
            if (ReferenceEquals(toPlainType, PlainType.NUInt)) return Value.FromNUInt((nuint)(ulong)value.Int);
        }

        if (from is CapabilityType { TypeConstructor: BoolTypeConstructor or BoolLiteralTypeConstructor })
        {
            if (ReferenceEquals(toPlainType, PlainType.Int8)) return Value.FromI8((sbyte)(value.Bool ? 1 : 0));
            if (ReferenceEquals(toPlainType, PlainType.Byte)) return Value.FromByte((byte)(value.Bool ? 1 : 0));
            if (ReferenceEquals(toPlainType, PlainType.Int16)) return Value.FromI16((short)(value.Bool ? 1 : 0));
            if (ReferenceEquals(toPlainType, PlainType.UInt16)) return Value.FromU16((ushort)(value.Bool ? 1 : 0));
            if (ReferenceEquals(toPlainType, PlainType.Int32)) return Value.FromI32(value.Bool ? 1 : 0);
            if (ReferenceEquals(toPlainType, PlainType.UInt32)) return Value.FromU32((uint)(value.Bool ? 1 : 0));
            if (ReferenceEquals(toPlainType, PlainType.Int64)) return Value.FromI64(value.Bool ? 1 : 0);
            if (ReferenceEquals(toPlainType, PlainType.UInt64)) return Value.FromU64((uint)(value.Bool ? 1 : 0));
            if (ReferenceEquals(toPlainType, PlainType.Offset)) return Value.FromOffset(value.Bool ? 1 : 0);
            if (ReferenceEquals(toPlainType, PlainType.Size)) return Value.FromSize((nuint)(value.Bool ? 1 : 0));
            if (ReferenceEquals(toPlainType, PlainType.NInt)) return Value.FromNInt(value.Bool ? 1 : 0);
            if (ReferenceEquals(toPlainType, PlainType.NUInt)) return Value.FromNUInt((nuint)(value.Bool ? 1 : 0));
            if (ReferenceEquals(toPlainType, PlainType.Int)) return Value.From(value.Bool ? BigInteger.One : BigInteger.Zero);
            if (ReferenceEquals(toPlainType, PlainType.UInt)) return Value.From(value.Bool ? BigInteger.One : BigInteger.Zero);
            if (ReferenceEquals(toPlainType, PlainType.Bool)) return value;
        }

        if (ReferenceEquals(fromPlainType, PlainType.Byte))
        {
            if (ReferenceEquals(toPlainType, PlainType.Int16)) return Value.FromI16(value.Byte);
            if (ReferenceEquals(toPlainType, PlainType.UInt16)) return Value.FromU16(value.Byte);
            if (ReferenceEquals(toPlainType, PlainType.Int32)) return Value.FromI32(value.Byte);
            if (ReferenceEquals(toPlainType, PlainType.UInt32)) return Value.FromU32(value.Byte);
            if (ReferenceEquals(toPlainType, PlainType.Offset)) return Value.FromOffset(value.Byte);
            if (ReferenceEquals(toPlainType, PlainType.Size)) return Value.FromSize(value.Byte);
            if (ReferenceEquals(toPlainType, PlainType.NInt)) return Value.FromNInt(value.Byte);
            if (ReferenceEquals(toPlainType, PlainType.NUInt)) return Value.FromNUInt(value.Byte);
            if (ReferenceEquals(toPlainType, PlainType.Int)) return Value.From(value.Byte);
            if (ReferenceEquals(toPlainType, PlainType.UInt)) return Value.From(value.Byte);
        }

        if (ReferenceEquals(fromPlainType, PlainType.Size))
        {
            if (ReferenceEquals(toPlainType, PlainType.Offset)) return Value.FromOffset((nint)value.Size);
            if (ReferenceEquals(toPlainType, PlainType.Size)) return Value.FromSize(value.Size);
            if (ReferenceEquals(toPlainType, PlainType.NInt)) return Value.FromNInt((nint)value.Size);
            if (ReferenceEquals(toPlainType, PlainType.NUInt)) return Value.FromNUInt(value.Size);
            if (ReferenceEquals(toPlainType, PlainType.Int)) return Value.From(value.Size);
            if (ReferenceEquals(toPlainType, PlainType.UInt)) return Value.From(value.Size);
        }

        if (ReferenceEquals(fromPlainType, PlainType.Offset))
        {
            if (ReferenceEquals(toPlainType, PlainType.NInt)) return Value.FromNInt((nint)value.Size);
            if (ReferenceEquals(toPlainType, PlainType.Int)) return Value.From(value.Offset);
        }

        if (ReferenceEquals(fromPlainType, PlainType.Int)
            || ReferenceEquals(fromPlainType, PlainType.UInt))
        {
            if (ReferenceEquals(toPlainType, PlainType.Int)) return value;
            var fromValue = value.Int;
            if (ReferenceEquals(toPlainType, PlainType.UInt))
            {
                var isSigned = fromValue.Sign < 0;
                if (isSigned)
                {
                    if (isOptional) return Value.None;
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
                    if (isOptional) return Value.None;
                    throw new AbortException($"Cannot convert value {fromValue} to {to.ToILString()}");
                }
            }

            if (ReferenceEquals(toPlainType, PlainType.Int8)) return Value.FromI8((sbyte)fromValue);
            if (ReferenceEquals(toPlainType, PlainType.Byte)) return Value.FromByte((byte)fromValue);
            if (ReferenceEquals(toPlainType, PlainType.Int16)) return Value.FromI16((short)fromValue);
            if (ReferenceEquals(toPlainType, PlainType.UInt16)) return Value.FromU16((ushort)fromValue);
            if (ReferenceEquals(toPlainType, PlainType.Int32)) return Value.FromI32((int)fromValue);
            if (ReferenceEquals(toPlainType, PlainType.UInt32)) return Value.FromU32((uint)fromValue);
            if (ReferenceEquals(toPlainType, PlainType.Int64)) return Value.FromI64((long)fromValue);
            if (ReferenceEquals(toPlainType, PlainType.UInt64)) return Value.FromU64((ulong)fromValue);
            if (ReferenceEquals(toPlainType, PlainType.Offset)) return Value.FromOffset((nint)(long)fromValue);
            if (ReferenceEquals(toPlainType, PlainType.Size)) return Value.FromSize((nuint)(ulong)fromValue);
            if (ReferenceEquals(toPlainType, PlainType.NInt)) return Value.FromNInt((nint)(long)fromValue);
            if (ReferenceEquals(toPlainType, PlainType.NUInt)) return Value.FromNUInt((nuint)(ulong)fromValue);
        }

        throw new NotImplementedException($"Conversion from {from.ToILString()} to {to.ToILString()}");
    }

    [Inline(InlineBehavior.Remove)]
    public static Value IncrementInt(this Value value)
        => Value.From(value.Int + 1);

    public static bool IsOfType(this Value value, Type type, BareType? bareSelfType)
    {
        // TODO using isolated here is a hack to take the capability of the type without modification.
        // This is part of the lack of clarity about how capabilities should work when doing type checks.
        type = bareSelfType?.With(Capability.Isolated).TypeReplacements.ApplyTo(type) ?? type;
        return value.IsOfType(type);
    }

    private static bool IsOfType(this Value value, Type type)
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

    private static bool IsOfType(this Value value, BareType bareType, bool otherAllowsWrite)
    {
        if (bareType.TypeConstructor.Semantics != TypeSemantics.Reference)
            // TODO support checking value types, and associated types
            throw new NotImplementedException();

        // It is a reference type
        if (!value.InstanceReference.IsClassReference)
            return false;

        var obj = value.ClassReference;
        return obj.BareType.IsSubtypeOf(bareType, otherAllowsWrite);
    }

    // TODO will this be needed or should it be removed?
    public static Value Increment(this Value value, CapabilityType type)
    {
        var plainType = type.PlainType;
        if (ReferenceEquals(plainType, PlainType.Int) || ReferenceEquals(plainType, PlainType.UInt))
            return Value.From(value.Int + 1);
        if (ReferenceEquals(plainType, PlainType.Int8)) return Value.FromI8((sbyte)(value.I8 + 1));
        if (ReferenceEquals(plainType, PlainType.Byte)) return Value.FromByte((byte)(value.Byte + 1));
        if (ReferenceEquals(plainType, PlainType.Int16)) return Value.FromI16((short)(value.I16 + 1));
        if (ReferenceEquals(plainType, PlainType.UInt16)) return Value.FromU16((ushort)(value.U16 + 1));
        if (ReferenceEquals(plainType, PlainType.Int32)) return Value.FromI32(value.I32 + 1);
        if (ReferenceEquals(plainType, PlainType.UInt32)) return Value.FromU32(value.U32 + 1);
        if (ReferenceEquals(plainType, PlainType.Int64)) return Value.FromI64(value.I64 + 1);
        if (ReferenceEquals(plainType, PlainType.UInt64)) return Value.FromU64(value.U64 + 1);
        if (ReferenceEquals(plainType, PlainType.Size)) return Value.FromSize(value.Size + 1);
        if (ReferenceEquals(plainType, PlainType.Offset)) return Value.FromOffset(value.Offset + 1);

        throw new NotImplementedException($"Increment of {type}");
    }
}
