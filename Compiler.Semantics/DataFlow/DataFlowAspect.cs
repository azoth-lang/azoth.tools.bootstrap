using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.DataFlow;

internal static partial class DataFlowAspect
{
    public static partial IFixedSet<IDataFlowNode> DataFlow_DataFlowPrevious(IDataFlowNode node)
        => GetDataFlowPrevious(node);

    public static partial IFixedSet<IDataFlowNode> VariableNameExpression_DataFlowPrevious(IVariableNameExpressionNode node)
        => GetDataFlowPrevious(node);

    private static IFixedSet<IDataFlowNode> GetDataFlowPrevious(this IControlFlowNode node)
    {
        var visited = new HashSet<IControlFlowNode>();
        DataFlowPrevious(node, visited);
        return visited.OfType<IDataFlowNode>().ToFixedSet();
    }

    private static void DataFlowPrevious(IControlFlowNode node, HashSet<IControlFlowNode> visited)
    {
        foreach (var previous in node.ControlFlowPrevious)
            if (visited.Add(previous) && previous is not IDataFlowNode)
                // Any node not visited before that isn't a control flow node needs recursed on.
                DataFlowPrevious(previous, visited);
    }
}
