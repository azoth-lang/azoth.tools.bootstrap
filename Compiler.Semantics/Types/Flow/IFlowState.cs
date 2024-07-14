using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

/// <summary>
/// Wraps up all the state that changes with the flow of the code to make it easy to attach to each
/// node in the semantic tree.
/// </summary>
public interface IFlowState : IEquatable<IFlowState>
{
    public static IFlowState Empty => LegacyFlowState.Empty;

    bool IsEmpty { get; }

    /// <summary>
    /// Declare the given parameter as part of the flow state including any independent parameters.
    /// </summary>
    IFlowState Declare(INamedParameterNode parameter);

    /// <summary>
    /// Declare the given parameter as part of the flow state including any independent parameters.
    /// </summary>
    IFlowState Declare(ISelfParameterNode parameter);

    // TODO should be a type that doesn't include INamedParameterNode
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
