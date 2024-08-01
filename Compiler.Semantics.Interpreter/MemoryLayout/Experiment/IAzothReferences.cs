using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout.Experiment;

public interface IAzothReferences
{
    public int Length { get; }
}

public static class AzothReferencesExtensions
{
    public static Span<object> AsSpan<TAzothReferences>(this ref TAzothReferences references)
        where TAzothReferences : struct, IAzothReferences
        => MemoryMarshal.CreateSpan(ref Unsafe.As<TAzothReferences, object>(ref references), references.Length);
}
