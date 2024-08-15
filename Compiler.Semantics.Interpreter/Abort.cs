using System;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter;

internal class Abort(string message) : Exception(message);
