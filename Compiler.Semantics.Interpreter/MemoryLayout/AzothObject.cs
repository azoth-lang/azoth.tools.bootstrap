using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal sealed class AzothObject : Dictionary<IdentifierName, AzothValue>
{
    public readonly VTable VTable;

    public AzothObject(VTable vTable)
    {
        VTable = vTable;
    }
}
