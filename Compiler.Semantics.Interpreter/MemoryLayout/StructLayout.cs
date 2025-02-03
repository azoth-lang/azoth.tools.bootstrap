using System.Diagnostics;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal sealed class StructLayout : TypeLayout
{
    public IStructDefinitionNode Struct { [DebuggerStepThrough] get; }

    public StructLayout(IStructDefinitionNode @struct) : base(@struct)
    {
        Struct = @struct;
    }
}
