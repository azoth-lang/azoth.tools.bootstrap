using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Types.Constructors;
using Azoth.Tools.Bootstrap.Compiler.Types.Decorated;
using Azoth.Tools.Bootstrap.Compiler.Types.Flow;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

internal static class FlowStateExtensions
{
    /// <summary>
    /// Declare the given parameter as part of the flow state including any independent parameters.
    /// </summary>
    public static IFlowState Declare(this IFlowState self, INamedParameterNode parameter)
        => self.Declare(parameter.IsLentBinding, parameter);

    /// <summary>
    /// Declare the given parameter as part of the flow state including any independent parameters.
    /// </summary>
    public static IFlowState Declare(this IFlowState self, ISelfParameterNode parameter)
        => self.Declare(parameter.IsLentBinding, parameter);

    private static IFlowState Declare(this IFlowState self, bool isLent, IParameterNode parameter)
        => self.DeclareParameter(isLent, parameter.BindingValueId, parameter.BindingType);

    public static IFlowState Declare(this IFlowState self, IVariableBindingNode binding, ValueId? initializerId)
        => self.DeclareVariable(binding.BindingValueId, binding.BindingType, initializerId);

    /// <summary>
    /// Make <paramref name="aliasId"/> an alias to the <paramref name="binding"/>.
    /// </summary>
    /// <remarks>This does not alias any independent parameters of the binding because only an alias
    /// to the top level object has been created. For example, if <c>iso List[iso Foo]</c> is aliased
    /// the list elements are still isolated. Only the list itself has been aliased and is now
    /// <c>mut</c>.</remarks>
    public static IFlowState Alias(this IFlowState self, IBindingNode? binding, ValueId aliasId)
        => self.Alias(binding?.BindingValueId, aliasId);

    /// <summary>
    /// Gives the current flow type of the binding.
    /// </summary>
    /// <remarks>This is named for it to be used as <c>flow.Type(binding)</c></remarks>
    public static IMaybeType Type(this IFlowState self, IBindingNode? binding)
    {
        if (binding is null) return IMaybeType.Unknown;
        return self.Type(binding.BindingValueId, binding.BindingType);
    }

    /// <summary>
    /// Gives the type of an alias to the binding.
    /// </summary>
    /// <remarks>This is named for it to be used as <c>flow.AliasType(binding)</c></remarks>
    public static IMaybeType AliasType(this IFlowState self, IBindingNode? binding)
    {
        if (binding is null) return IMaybeType.Unknown;
        return self.AliasType(binding.BindingValueId, binding.BindingType);
    }

    public static bool IsIsolated(this IFlowState self, IBindingNode? binding)
        // TODO what about independent parameters?
        => binding is null || self.IsIsolated(binding.BindingValueId);

    public static bool IsIsolatedExceptFor(this IFlowState self, IBindingNode? binding, ValueId exceptForId)
        => binding is null || self.IsIsolatedExceptFor(binding.BindingValueId, exceptForId);

    public static bool CanFreezeExceptFor(this IFlowState self, IBindingNode? binding, ValueId exceptForId)
        => binding is null || self.CanFreezeExceptFor(binding.BindingValueId, exceptForId);

    public static IFlowState AccessField(this IFlowState self, IFieldAccessExpressionNode node)
    {
        var contextId = node.Context.ValueId;
        var contextType = (CapabilityType)node.Context.Type;
        var declaringTypeConstructor = node.ReferencedDeclaration.ContainingDeclaration.TypeFactory as TypeConstructor
                                       ?? throw new InvalidOperationException("Cannot access field of primitive type.");
        var id = node.ValueId;
        var bindingType = node.ReferencedDeclaration.BindingType;
        var memberType = node.Type;

        return self.AccessField(contextId, contextType, declaringTypeConstructor, id, bindingType, memberType);
    }

    public static IFlowState FreezeVariable(this IFlowState self, IBindingNode? binding, ValueId id, ValueId intoValueId)
        => binding is null
            ? self.FreezeValue(id, intoValueId)
            : self.FreezeVariable(binding.BindingValueId, binding.BindingType, id, intoValueId);

    public static IFlowState MoveVariable(this IFlowState self, IBindingNode? binding, ValueId valueId, ValueId intoValueId)
        => binding is null
            ? self.MoveValue(valueId, intoValueId)
            : self.MoveVariable(binding.BindingValueId, binding.BindingType, valueId, intoValueId);

    public static IFlowState DropBindings(this IFlowState self, IEnumerable<INamedBindingNode> bindings)
        => self.DropBindings(bindings.Select(b => b.BindingValueId));
}
