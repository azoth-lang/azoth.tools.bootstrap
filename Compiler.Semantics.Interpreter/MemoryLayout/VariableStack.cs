using System.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

/// <summary>
/// This type exists primarily to allow other code to clearly determine that an
/// <see cref="AzothValue"/> is a local variable reference.
/// </summary>
internal sealed class VariableStack : List<AzothValue>;
