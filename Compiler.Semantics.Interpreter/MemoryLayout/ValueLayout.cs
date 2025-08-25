using System.Diagnostics;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal sealed class ValueLayout : TypeLayout
{
    public IValueDefinitionNode Value { [DebuggerStepThrough] get; }

    public ValueLayout(IValueDefinitionNode value) : base(value, 1)
    {
        Value = value;
    }

    [Inline(InlineBehavior.Remove)]
    public new AzothValue[] CreateInstanceFields() => base.CreateInstanceFields();
}
