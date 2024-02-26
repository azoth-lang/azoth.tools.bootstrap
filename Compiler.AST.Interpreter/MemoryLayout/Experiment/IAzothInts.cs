using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout.Experiment;

public interface IAzothInts
{
    public int Length { get; }
}

public static class AzothIntsExtensions
{
    public static Span<BigInteger> AsSpan<TAzothInts>(this ref TAzothInts ints)
        where TAzothInts : struct, IAzothInts
        => MemoryMarshal.CreateSpan(ref Unsafe.As<TAzothInts, BigInteger>(ref ints), ints.Length);
}
