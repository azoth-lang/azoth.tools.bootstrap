using System.Diagnostics;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal sealed class StructLayout : TypeLayout
{
    public IStructDefinitionNode Struct { [DebuggerStepThrough] get; }

    public StructLayout(IStructDefinitionNode @struct) : base(@struct, 1)
    {
        Struct = @struct;
    }

    [Inline(InlineBehavior.Remove)]
    public new AzothValue[] CreateInstanceFields() => base.CreateInstanceFields();
}
