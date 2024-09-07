using System.Collections.Generic;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;

internal static class ControlFlowSetExtensions
{
    public static ControlFlowSet ToControlFlowSet(this IReadOnlyDictionary<IControlFlowNode, ControlFlowKind> set)
        => ControlFlowSet.Create(set);
}
