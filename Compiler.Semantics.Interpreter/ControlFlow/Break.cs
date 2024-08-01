using System;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.ControlFlow;

internal class Break : Exception
{
    public AzothValue Value { get; }

    public Break() { }

    public Break(AzothValue value)
    {
        Value = value;
    }
}
