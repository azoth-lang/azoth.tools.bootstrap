using System.Collections.Generic;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal abstract class FunctionReference
{
    public abstract FunctionType FunctionType { get; }

    public abstract ValueTask<Value> CallAsync(IReadOnlyList<Value> arguments);
}
