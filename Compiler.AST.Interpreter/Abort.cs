using System;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter;

internal class Abort(string message) : Exception(message);
