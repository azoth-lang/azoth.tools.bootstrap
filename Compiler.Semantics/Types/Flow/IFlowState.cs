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

    /// <summary>
    /// Make <paramref name="valueId"/> an alias to the <paramref name="binding"/>.
    /// </summary>
    /// <remarks>This does not alias any independent parameters of the binding because only an alias
    /// to the top level object has been created. For example, if <c>iso List[iso Foo]</c> is aliased
    /// the list elements are still isolated. Only the list itself has been aliased and is now
    /// <c>mut</c>.</remarks>
    IFlowState Alias(IBindingNode? binding, ValueId valueId);

    /// <summary>
    /// Gives the current flow type of the symbol.
    /// </summary>
    /// <remarks>This is named for it to be used as <c>flow.Type(symbol)</c></remarks>
    DataType Type(IBindingNode? binding);


    /// <summary>
    /// Gives the type of an alias to the symbol
    /// </summary>
    /// <remarks>This is named for it to be used as <c>flow.AliasType(symbol)</c></remarks>
    DataType AliasType(IBindingNode? binding);

    bool IsIsolated(IBindingNode? binding);
    bool IsIsolatedExceptFor(IBindingNode? binding, ValueId? valueId);
    bool CanFreezeExceptFor(IBindingNode? binding, ValueId? valueId);

    /// <summary>
    /// Combine the non-lent values representing the arguments into one sharing set with the return
    /// value id and drop the values for all arguments.
    /// </summary>
    // TODO should storage of return value be based on whether the return type requires tracking?
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
