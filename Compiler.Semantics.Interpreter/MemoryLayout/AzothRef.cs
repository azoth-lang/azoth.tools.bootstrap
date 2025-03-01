using System.Collections.Generic;
using InlineMethod;

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
        [Inline(InlineBehavior.Remove)]
        get => context[offset];
        [Inline(InlineBehavior.Remove)]
        set => context[offset] = value;
    }
}
