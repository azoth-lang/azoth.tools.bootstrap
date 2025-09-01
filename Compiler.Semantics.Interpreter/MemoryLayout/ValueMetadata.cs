using System.Diagnostics;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal sealed class ValueMetadata : TypeMetadata
{
    public IValueDefinitionNode Value { [DebuggerStepThrough] get; }

    public ValueMetadata(IValueDefinitionNode value) : base(value, 1)
    {
        Value = value;
    }

    [Inline(InlineBehavior.Remove)]
    public new Value[] CreateInstanceFields() => base.CreateInstanceFields();
}
