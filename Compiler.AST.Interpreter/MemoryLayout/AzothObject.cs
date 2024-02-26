using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout;

internal class AzothObject : Dictionary<SimpleName, AzothValue>
{
    public readonly VTable VTable;

    public AzothObject(VTable vTable)
    {
        VTable = vTable;
    }
}
