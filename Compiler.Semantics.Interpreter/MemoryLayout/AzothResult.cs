using System;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal readonly struct AzothResult
{
    public static readonly AzothResult Next = new(AzothValue.None, AzothResultType.Next);

    public readonly AzothValue Value;
    public readonly AzothResultType Type;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ShouldExit() => Type != AzothResultType.Ordinary;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ShouldExit(out AzothValue value)
    {
        value = Value;
        return Type != AzothResultType.Ordinary;
    }

    public bool IsNext => Type == AzothResultType.Next;
    public bool IsReturn => Type == AzothResultType.Return;

    public AzothValue ReturnValue
    {
        get
        {
            if (Type is not (AzothResultType.Return or AzothResultType.Ordinary))
                throw new InvalidOperationException("Result can't be used as a return value.");
            return Value;
        }
    }

    public static implicit operator AzothResult(AzothValue value)
        => new(value, AzothResultType.Ordinary);

    public static AzothResult Return(AzothValue value)
        => new(value, AzothResultType.Return);

    public static AzothResult Return()
        => new(AzothValue.None, AzothResultType.Return);

    public static AzothResult Break()
        => new(AzothValue.None, AzothResultType.Break);

    public static AzothResult Break(AzothValue value)
        => new(value, AzothResultType.Break);

    private AzothResult(AzothValue value, AzothResultType type)
    {
        Value = value;
        Type = type;
    }
}
