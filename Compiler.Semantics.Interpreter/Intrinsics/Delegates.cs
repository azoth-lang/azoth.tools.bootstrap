using System.Collections.Generic;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.Intrinsics;

internal delegate ValueTask<AzothValue> IntrinsicFunction(InterpreterProcess interpreter, FunctionSymbol function, IReadOnlyList<AzothValue> arguments);

internal delegate ValueTask<AzothValue> IntrinsicInitializer(BareType selfBareType, InitializerSymbol initializer, IReadOnlyList<AzothValue> arguments);

internal delegate ValueTask<AzothValue> IntrinsicMethod(MethodSymbol method, AzothValue self, IReadOnlyList<AzothValue> arguments);
