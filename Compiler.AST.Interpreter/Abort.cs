using System;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter;

internal class Abort : Exception
{
    public Abort(string message) : base(message) { }
}
