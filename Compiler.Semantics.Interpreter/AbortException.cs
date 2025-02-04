using System;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter;

public sealed class AbortException(string message) : Exception(message);
