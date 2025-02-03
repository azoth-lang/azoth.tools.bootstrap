using System;
using System.Runtime.CompilerServices;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal readonly struct AzothResult
{
    public static readonly AzothResult Next = new(AzothValue.None, AzothResultType.Next);
    public static readonly AzothResult ReturnVoid = new(AzothValue.None, AzothResultType.Return);
    public static readonly AzothResult BreakWithoutValue = new(AzothValue.None, AzothResultType.Break);

    public readonly AzothValue Value;
    public readonly AzothResultType Type;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // TODO [Inline(InlineBehavior.Remove)] causes invalid program
    public bool ShouldExit() => Type != AzothResultType.Ordinary;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    // TODO [Inline(InlineBehavior.Remove)] causes invalid program
    public bool ShouldExit(out AzothValue value)
    {
        value = Value;
        return Type != AzothResultType.Ordinary;
    }

    public bool IsReturn
    {
        // TODO [Inline(InlineBehavior.Remove)] causes invalid program
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Type == AzothResultType.Return;
    }

    public AzothValue ReturnValue
    {
        [Inline(InlineBehavior.Remove)]
        get
        {
            if (Type is not (AzothResultType.Return or AzothResultType.Ordinary))
                throw new InvalidOperationException("Result can't be used as a return value.");
            return Value;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static implicit operator AzothResult(AzothValue value)
        => new(value, AzothResultType.Ordinary);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AzothResult Return(AzothValue value)
        => new(value, AzothResultType.Return);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static AzothResult Break(AzothValue value)
        => new(value, AzothResultType.Break);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private AzothResult(AzothValue value, AzothResultType type)
    {
        Value = value;
        Type = type;
    }
}
