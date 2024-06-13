using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types;

internal static class NameBindingTypesAspect
{
    public static DataType VariableDeclarationStatement_BindingType(IVariableDeclarationStatementNode node)
        // TODO account for node.Capability
        => node.Type?.NamedType ?? node.FinalInitializer?.Type.ToNonConstValueType() ?? DataType.Unknown;

    public static FlowState VariableDeclarationStatement_FlowStateAfter(IVariableDeclarationStatementNode node)
    {
        var flowStateBefore = node.FinalInitializer?.FlowStateAfter ?? node.FlowStateBefore();
        return flowStateBefore.Declare(node);
    }

    public static DataType ForeachExpression_BindingType(IForeachExpressionNode node)
        => node.DeclaredType?.NamedType ?? node.IteratedType;
}
