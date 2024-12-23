using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Flow;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

/// <summary>
/// Wraps up all the state that changes with the flow of the code to make it easy to attach to each
/// node in the semantic tree.
/// </summary>
public interface IFlowState : IEquatable<IFlowState>
{
    public static IFlowState Empty => FlowState.Empty;

    bool IsEmpty { get; }

    /// <summary>
    /// Declare the given parameter as part of the flow state including any independent parameters.
    /// </summary>
    public IFlowState DeclareParameter(bool isLent, ValueId id, IMaybeType type);

    IFlowState DeclareVariable(ValueId id, IMaybeNonVoidType type, ValueId? initializerId);

    IFlowState Constant(ValueId valueId);

    /// <summary>
    /// Make <paramref name="aliasId"/> an alias to the <paramref name="id"/>.
    /// </summary>
    /// <remarks>This does not alias any independent parameters of the binding because only an alias
    /// to the top level object has been created. For example, if <c>iso List[iso Foo]</c> is aliased
    /// the list elements are still isolated. Only the list itself has been aliased and is now
    /// <c>mut</c>.</remarks>
    IFlowState Alias(ValueId? id, ValueId aliasId);

    /// <summary>
    /// The current flow type of the value.
    /// </summary>
    /// <remarks>This is named for it to be used as <c>flow.Type(id, type)</c></remarks>
    IMaybeType Type(ValueId id, IMaybeType declaredType);

    /// <summary>
    /// The current flow type of the value.
    /// </summary>
    /// <remarks>This is named for it to be used as <c>flow.AliasType(id, type)</c></remarks>
    IMaybeType AliasType(ValueId id, IMaybeType declaredType);

    bool IsIsolated(ValueId valueId);
    bool IsIsolatedExceptFor(ValueId id, ValueId exceptForId);
    bool CanFreezeExceptFor(ValueId id, ValueId exceptForId);
    bool CanFreeze(ValueId valueId);
    bool IsLent(ValueId valueId);

    /// <summary>
    /// Combine the non-lent values representing the arguments into one sharing set with the return
    /// value id and drop the values for all arguments.
    /// </summary>
    IFlowState CombineArguments(IEnumerable<ArgumentValueId> arguments, ValueId returnValueId, IMaybeType returnType);

    IEnumerable<ValueId> CombineArgumentsDisallowedDueToLent(IEnumerable<ArgumentValueId> arguments);

    IFlowState AccessField(
        ValueId contextId,
        CapabilityType contextType,
        TypeConstructor declaringTypeConstructor,
        ValueId id,
        IMaybeNonVoidType bindingType,
        IMaybeType memberType);
    IFlowState Merge(IFlowState? other);
    IFlowState Transform(ValueId? valueId, ValueId toValueId, IMaybeType withType);

    // TODO make parameters non-nullable?
    IFlowState Combine(ValueId left, ValueId? right, ValueId intoValueId);

    IEnumerable<ValueId> CombineDisallowedDueToLent(ValueId left, ValueId? right);
    IFlowState FreezeVariable(ValueId bindingId, IMaybeType bindingType, ValueId id, ValueId intoValueId);
    IFlowState FreezeValue(ValueId id, ValueId intoValueId);
    IFlowState MoveVariable(IBindingNode? binding, ValueId valueId, ValueId intoValueId);
    IFlowState MoveValue(ValueId valueId, ValueId intoValueId);
    IFlowState TempFreeze(ValueId valueId, ValueId intoValueId);
    IFlowState TempMove(ValueId valueId, ValueId intoValueId);
    IFlowState DropBindings(IEnumerable<INamedBindingNode> bindings);
    IFlowState DropValue(ValueId valueId);

    /// <summary>
    /// Drop all local variables and parameters in preparation for a return from the function or
    /// method.
    /// </summary>
    /// <remarks>External references will ensure that parameters aren't incorrectly treated as isolated.</remarks>
    IFlowState DropBindingsForReturn();
}
