using System;
using Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.ControlFlow
{
    internal class Return : Exception
    {
        public AzothValue Value { get; }

        public Return()
        {
        }

        public Return(AzothValue value)
        {
            Value = value;
        }
    }
}
