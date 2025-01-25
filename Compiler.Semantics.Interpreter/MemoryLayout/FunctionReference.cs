using System.Collections.Generic;
using System.Threading.Tasks;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal abstract class FunctionReference
{
    public abstract ValueTask<AzothValue> CallAsync(IReadOnlyList<AzothValue> arguments);
}
