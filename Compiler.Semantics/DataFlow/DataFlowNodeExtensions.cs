using System;
using System.Linq;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;

internal static class DataFlowNodeExtensions
{
    public static T DataFlowPrevious<T>(this IDataFlowNode node, Func<IDataFlowNode, T> selector, Func<T, T, T> merge)
        => DataFlowFromControlFlowPrevious(node, selector, merge);

    private static T DataFlow<T>(
        IControlFlowNode node,
        Func<IDataFlowNode, T> selector,
        Func<T, T, T> merge)
    {
        if (node is IDataFlowNode dataFlowNode)
            return selector(dataFlowNode);
        return DataFlowFromControlFlowPrevious(node, selector, merge);
    }

    //[Inline]
    private static T DataFlowFromControlFlowPrevious<T>(IControlFlowNode node, Func<IDataFlowNode, T> selector, Func<T, T, T> merge)
        => node.ControlFlowPrevious.Keys
        .Select(n => DataFlow(n, selector, merge))
        .Aggregate(merge);
}
