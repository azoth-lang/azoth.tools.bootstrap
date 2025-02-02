using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;

internal sealed class AzothStruct : Dictionary<FieldSymbol, AzothValue>;
