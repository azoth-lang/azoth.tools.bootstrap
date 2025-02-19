using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal sealed class StructLayout : TypeLayout
{
    public IStructDefinitionNode Struct { [DebuggerStepThrough] get; }

    public StructLayout(IStructDefinitionNode @struct) : base(@struct, 1)
    {
        Struct = @struct;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public new AzothValue[] CreateInstanceFields() => base.CreateInstanceFields();
}
