using System.Diagnostics;
using InlineMethod;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal sealed class StructMetadata : TypeMetadata
{
    public IStructDefinitionNode Struct { [DebuggerStepThrough] get; }

    public StructMetadata(IStructDefinitionNode @struct) : base(@struct, 1)
    {
        Struct = @struct;
    }

    [Inline(InlineBehavior.Remove)]
    public new Value[] CreateInstanceFields() => base.CreateInstanceFields();
}
