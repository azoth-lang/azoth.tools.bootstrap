using System.Collections.Generic;
using System.Threading.Tasks;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.MemoryLayout;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types.Bare;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Interpreter.Intrinsics;

internal delegate ValueTask<Value> IntrinsicFunction(InterpreterProcess interpreter, FunctionSymbol function, IReadOnlyList<Value> arguments);

internal delegate ValueTask<Value> IntrinsicInitializer(BareType selfBareType, InitializerSymbol initializer, IReadOnlyList<Value> arguments);

internal delegate ValueTask<Value> IntrinsicMethod(MethodSymbol method, Value self, IReadOnlyList<Value> arguments);
