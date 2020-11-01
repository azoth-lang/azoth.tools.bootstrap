using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage.CFG;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage
{
    [Closed(
        typeof(FunctionIL),
        typeof(MethodDeclarationIL),
        typeof(ConstructorIL))]
    public interface IInvocableDeclarationIL
    {
        bool IsConstructor { get; }
        FixedList<ParameterIL> Parameters { get; }
        ControlFlowGraph? IL { get; }
    }
}
