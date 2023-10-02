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

    public FlowState(ReferenceCapabilitiesSnapshot capabilities, SharingRelationSnapshot sharing)
        : this(capabilities.MutableCopy(), sharing.MutableCopy())
    {
    }

    public FlowState() : this(new ReferenceCapabilities(), new SharingRelation())
    {
    }

    private FlowState(ReferenceCapabilities capabilities, SharingRelation sharing)
    {
        this.capabilities = capabilities;
        this.sharing = sharing;
    }

    public ResultVariable CurrentResult => sharing.CurrentResult;

    public FlowState Copy() => new(capabilities.Copy(), sharing.Copy());

    public void Declare(BindingSymbol symbol)
    {
        capabilities.Declare(symbol);
        sharing.Declare(symbol);
    }

    public SharingVariable NewResult() => sharing.NewResult();

    public void Drop(BindingSymbol symbol) => RemoveRestrictions(sharing.Drop(symbol));

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

    public void Move(BindingSymbol symbol) => capabilities.Move(symbol);

    public void Freeze(BindingSymbol symbol)
    {
        capabilities.Freeze(symbol);
        // Constants aren't tracked, this symbol is `const` now
        Drop(symbol);
    }

    // TODO maybe this happens as part of Union?
    public void Alias(BindingSymbol symbol) => capabilities.Alias(symbol);

    /// <summary>
    /// Gives the current flow type of the symbol.
    /// </summary>
    /// <remarks>This is named for it to be used as <c>flow.Type(symbol)</c></remarks>
    public DataType Type(BindingSymbol? symbol) => capabilities.CurrentType(symbol);

    public void UnionWithCurrentResult(SharingVariable var) => sharing.UnionWithCurrentResult(var);

    public void UnionWithCurrentResultAndDrop(ResultVariable variable)
        => sharing.UnionWithCurrentResultAndDrop(variable);

    public void DropCurrentResult() => RemoveRestrictions(sharing.Drop(sharing.CurrentResult));

    public void SplitCurrentResult() => sharing.SplitCurrentResult();

    public bool CurrentResultIsIsolated() => sharing.IsIsolated(sharing.CurrentResult);
    public bool IsIsolated(SharingVariable variable) => sharing.IsIsolated(variable);

    public bool IsIsolatedExceptCurrentResult(SharingVariable variable)
        => sharing.IsIsolatedExceptCurrentResult(variable);

    /// <summary>
    /// Mark the current result as being lent const.
    /// </summary>
    /// <remarks>Because the current result must be at least temporarily const, all references in
    /// the sharing set must now not allow mutation.</remarks>
    public bool LendCurrentResultConst()
    {
        var currentResult = sharing.CurrentResult;
        var lentResult = sharing.NewResult(lent: true);
        foreach (var affectedSymbol in sharing.RestrictWrite(currentResult, lentResult))
            capabilities.RestrictWrite(affectedSymbol);
        return true;
    }

    /// <summary>
    /// Remove restrictions on symbols in the set if allowed.
    /// </summary>
    private void RemoveRestrictions(IReadOnlySharingSet? sharingSet)
    {
        if (sharingSet is null) return;
        if (sharingSet.IsWriteRestricted) return;

        foreach (var variable in sharingSet)
            if (variable.Symbol is not null)
                capabilities.RemoveWriteRestriction(variable.Symbol);
    }
}
