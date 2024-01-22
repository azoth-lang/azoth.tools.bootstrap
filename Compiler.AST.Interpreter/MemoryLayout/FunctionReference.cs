using System.Collections.Generic;
using System.Threading.Tasks;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout;

internal abstract class FunctionReference
{
    public abstract Task<AzothValue> CallAsync(IEnumerable<AzothValue> arguments);
}
