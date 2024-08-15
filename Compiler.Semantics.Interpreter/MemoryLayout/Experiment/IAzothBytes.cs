using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout.Experiment;

public interface IAzothBytes
{
    public int Length { get; }
}

public static class AzothBytesExtensions
{
    public static Span<byte> AsSpan<TAzothInts>(this ref TAzothInts bytes)
        where TAzothInts : struct, IAzothBytes
        => MemoryMarshal.CreateSpan(ref Unsafe.As<TAzothInts, byte>(ref bytes), bytes.Length);
}
