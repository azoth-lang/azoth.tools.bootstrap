using System;
using Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.ControlFlow
{
    internal class Break : Exception
    {
        public AzothValue Value { get; }

        public Break() { }

        public Break(AzothValue value)
        {
            Value = value;
        }
    }
}
