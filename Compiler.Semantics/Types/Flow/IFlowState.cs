using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

public interface IFlowState : IEquatable<IFlowState>
{
    public static IFlowState Empty => FlowState.Empty;

    bool IsEmpty { get; }

    IFlowState Declare(INamedParameterNode parameter);
    IFlowState Declare(ISelfParameterNode parameter);
    IFlowState Declare(INamedBindingNode binding, ValueId? initializerValueId);

    IFlowState Alias(IBindingNode? binding, ValueId valueId);
    DataType Type(IBindingNode? binding);
    DataType AliasType(IBindingNode? binding);
    bool IsIsolated(IBindingNode? binding);
    bool IsIsolatedExceptFor(IBindingNode? binding, ValueId? valueId);
    bool CanFreezeExceptFor(IBindingNode? binding, ValueId? valueId);
    IFlowState CombineArguments(IEnumerable<ArgumentValueId> arguments, ValueId returnValueId);
    IFlowState AccessMember(ValueId contextValueId, ValueId valueId, DataType memberType);
    IFlowState Merge(IFlowState? other);
    IFlowState Transform(ValueId? valueId, ValueId intoValueId, DataType withType);
    IFlowState Combine(ValueId left, ValueId? right, ValueId intoValueId);
    IFlowState FreezeVariable(IBindingNode? binding, ValueId valueId, ValueId intoValueId);
    IFlowState FreezeValue(ValueId valueId, ValueId intoValueId);
    IFlowState MoveVariable(IBindingNode? binding, ValueId valueId, ValueId intoValueId);
    IFlowState MoveValue(ValueId valueId, ValueId intoValueId);
    IFlowState TempFreeze(ValueId valueId, ValueId intoValueId);
    IFlowState TempMove(ValueId valueId, ValueId intoValueId);
    IFlowState DropBindings(IEnumerable<INamedBindingNode> bindings);
    IFlowState DropValue(ValueId valueId);
}
