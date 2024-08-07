using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.ControlFlow;

/// <summary>
/// This aspect establishes a relationship between expressions and statements that follows the
/// control flow of the code.
/// </summary>
/// <remarks>A parent node is considered to come before the control flow of any children. For example,
/// the if statement node comes before the condition of that if expression which feeds into the then
/// and else expressions.</remarks>
internal sealed class ControlFlowAspect
{
    public static FixedDictionary<IControlFlowNode, ControlFlowKind>
        ConcreteInvocableDefinition_InheritedControlFlowFollowing_Entry(IConcreteInvocableDefinitionNode node)
        => ControlFlowSet.CreateNormal(node.Body?.Statements.FirstOrDefault() ?? (IControlFlowNode)node.Exit);

    public static FixedDictionary<IControlFlowNode, ControlFlowKind> Expression_ControlFlowNext(IExpressionNode node)
        => node.ControlFlowFollowing();

    public static FixedDictionary<IControlFlowNode, ControlFlowKind> Statement_ControlFlowNext(IStatementNode node)
        => node.ControlFlowFollowing();

    public static FixedDictionary<IControlFlowNode, ControlFlowKind> Entry_ControlFlowNext(IEntryNode node)
        => node.ControlFlowFollowing();

    public static void ControlFlow_ContributeControlFlowPrevious(
        IControlFlowNode node,
        IControlFlowNode target,
        Dictionary<IControlFlowNode, ControlFlowKind> previous)
    {
        if (node.ControlFlowNext.TryGetValue(target, out var kind))
            previous.Add(node, kind);
    }
}
