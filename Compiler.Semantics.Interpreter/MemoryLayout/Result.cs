using System;
using System.Runtime.CompilerServices;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

/// <summary>
/// The result of an expression which could be a value, but could also be that `next`, `break`, or
/// `return` happened.
/// </summary>
internal readonly struct Result
{
    public static readonly Result Next = new(MemoryLayout.Value.None, ResultType.Next);
    public static readonly Result ReturnVoid = new(MemoryLayout.Value.None, ResultType.Return);
    public static readonly Result BreakWithoutValue = new(MemoryLayout.Value.None, ResultType.Break);

    public readonly Value Value;
    public readonly ResultType Type;

    [Inline(InlineBehavior.Remove)]
    public bool ShouldExit() => Type != ResultType.Ordinary;

    [Inline(InlineBehavior.Remove)]
    public bool ShouldExit(out Value value)
    {
        value = Value;
        return Type != ResultType.Ordinary;
    }

    public bool IsReturn
    {
        [Inline(InlineBehavior.Remove)]
        get => Type == ResultType.Return;
    }

    public Value ReturnValue
    {
        [Inline(InlineBehavior.Remove)]
        get
        {
            if (Type is not (ResultType.Return or ResultType.Ordinary))
                throw new InvalidOperationException("Result can't be used as a return value.");
            return Value;
        }
    }

    [Inline(InlineBehavior.Remove)]
    public static implicit operator Result(Value value)
        => new(value, ResultType.Ordinary);

    [Inline(InlineBehavior.Remove)]
    public static Result Return(Value value)
        => new(value, ResultType.Return);

    [Inline(InlineBehavior.Remove)]
    public static Result Break(Value value)
        => new(value, ResultType.Break);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Result(Value value, ResultType type)
    {
        Value = value;
        Type = type;
    }
}
