using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal readonly struct AzothRef
{
    private readonly IList<AzothValue> context;
    private readonly int offset;

    public AzothRef(IList<AzothValue> context, int offset)
    {
        this.context = context;
        this.offset = offset;
    }

    public AzothValue Value
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => context[offset];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => context[offset] = value;
    }
}
