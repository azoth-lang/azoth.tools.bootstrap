using System;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter;

internal sealed class Abort(string message) : Exception(message);
