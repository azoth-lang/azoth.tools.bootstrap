using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal sealed class AzothObject : Dictionary<FieldSymbol, AzothValue>
{
    public readonly VTable VTable;

    public AzothObject(VTable vTable)
    {
        VTable = vTable;
    }
}
