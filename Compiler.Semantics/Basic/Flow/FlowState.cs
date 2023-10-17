using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

/// <summary>
/// Wraps up all the state that changes with the flow of the code to make it easy to pass through
/// the analysis.
/// </summary>
/// <remarks>Flow is based of of <see cref="BindingSymbol"/> because you can do things like change
/// the type of <s>self</s> if it started out isolated.</remarks>
public sealed class FlowState
{
    private readonly ReferenceCapabilities capabilities;
    private readonly SharingRelation sharing;
    private readonly ResultVariableFactory resultVariableFactory;
    private readonly ImplicitLendFactory implicitLendFactory;

    public FlowState(ReferenceCapabilitiesSnapshot capabilities, SharingRelationSnapshot sharing)
        : this(capabilities.MutableCopy(), sharing.MutableCopy(), new(), new())
    {
    }

    public FlowState()
        : this(new ReferenceCapabilities(), new SharingRelation(), new(), new())
    {
    }

    private FlowState(
        ReferenceCapabilities capabilities,
        SharingRelation sharing,
        ResultVariableFactory resultVariableFactory,
        ImplicitLendFactory implicitLendFactory)
    {
        this.capabilities = capabilities;
        this.sharing = sharing;
        this.resultVariableFactory = resultVariableFactory;
        this.implicitLendFactory = implicitLendFactory;
    }

    public FlowState Fork() => new(capabilities.Copy(), sharing.Copy(), resultVariableFactory, implicitLendFactory);

    /// <summary>
    /// Declare the given symbol and combine it with the result variable.
    /// </summary>
    /// <remarks>The result variable is dropped as part of this.</remarks>
    public void Declare(BindingSymbol symbol, ResultVariable? variable)
    {
        capabilities.Declare(symbol);
        var bindingVariable = sharing.Declare(symbol);
        if (bindingVariable is not null && variable is not null)
            sharing.Union(bindingVariable, variable);
        Drop(variable);
    }

    public void Drop(BindingSymbol symbol) => RemoveRestrictions(sharing.Drop(symbol));

    public void Drop(ResultVariable? variable)
    {
        if (variable is not null)
            RemoveRestrictions(sharing.Drop(variable));
    }

    /// <summary>
    /// Drop all local variables and parameters in preparation for a return from the function or
    /// method.
    /// </summary>
    /// <remarks>External references will ensure that parameters are incorrectly treated as isolated.</remarks>
    public void DropBindingsForReturn()
    {
        sharing.DropAllLocalVariablesAndParameters();
        // So many sets can be modified, just go through all of them and remove restrictions
        foreach (var set in sharing.SharingSets)
            RemoveRestrictions(set);
    }

    public void Move(BindingSymbol symbol)
    {
        capabilities.Move(symbol);
        // Identity aren't tracked, this symbol is `id` now
        Drop(symbol);
    }

    public void Freeze(BindingSymbol symbol)
        // Because the symbol could reference `lent` `const` data, it still needs to be tracked
        // and can't be dropped.
        => capabilities.Freeze(symbol);

    public ResultVariable? Alias(BindingSymbol symbol)
    {
        var capability = capabilities.Alias(symbol);
        if (symbol.SharingIsTracked(capability))
            return sharing.NewResult(symbol, resultVariableFactory);
        return null;
    }

    /// <summary>
    /// Gives the current flow type of the symbol.
    /// </summary>
    /// <remarks>This is named for it to be used as <c>flow.Type(symbol)</c></remarks>
    public DataType Type(BindingSymbol? symbol) => capabilities.CurrentType(symbol);

    /// <summary>
    /// Combine two <see cref="ResultVariable"/>s into a single sharing set and return a result
    /// variable representative of the new set.
    /// </summary>
    /// <remarks>If two <see cref="ResultVariable"/>s are passed in, one will be dropped.</remarks>
    public ResultVariable? Combine(ResultVariable? leftVariable, ResultVariable? rightVariable)
    {
        if (leftVariable is null) return rightVariable;
        if (rightVariable is null) return leftVariable;
        sharing.Union(leftVariable, rightVariable);
        Drop(leftVariable);
        // TODO would it be better to create a new result variable to return instead of reusing this one?
        return rightVariable;
    }

    public bool IsIsolated(BindingVariable variable) => sharing.IsIsolated(variable);
    public bool IsIsolated(ISharingVariable? variable)
        => variable is null || sharing.IsIsolated(variable);

    public bool IsIsolatedExceptFor(BindingVariable variable, ResultVariable? resultVariable)
        => resultVariable is null ? IsIsolated(variable)
            : sharing.IsIsolatedExceptFor(variable, resultVariable);

    /// <summary>
    /// Mark the result as being lent const.
    /// </summary>
    /// <remarks>Because the current result must be at least temporarily const, all references in
    /// the sharing set must now not allow mutation.</remarks>
    public ResultVariable LendConst(ResultVariable result)
    {
        var newResult = sharing.LendConst(result, resultVariableFactory, implicitLendFactory);
        foreach (var variable in sharing.ReadSharingSet(result).OfType<BindingVariable>())
            capabilities.RestrictWrite(variable.Symbol);
        return newResult;
    }

    /// <summary>
    /// Remove restrictions on symbols in the set if allowed.
    /// </summary>
    private void RemoveRestrictions(IEnumerable<IReadOnlySharingSet> affectedSets)
    {
        foreach (var set in affectedSets)
            RemoveRestrictions(set);
    }

    private void RemoveRestrictions(IReadOnlySharingSet sharingSet)
    {
        if (sharingSet.IsWriteRestricted) return;

        foreach (var variable in sharingSet.OfType<BindingVariable>())
            capabilities.RemoveWriteRestriction(variable.Symbol);
    }

    public void Merge(FlowState other) => sharing.Merge(other.sharing);
}
