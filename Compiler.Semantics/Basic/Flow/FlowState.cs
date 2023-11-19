using System;
using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Errors;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

/// <summary>
/// Wraps up all the state that changes with the flow of the code to make it easy to pass through
/// the analysis.
/// </summary>
/// <remarks>Flow is based of of <see cref="BindingSymbol"/> because you can do things like change
/// the type of <s>self</s> if it started out isolated.</remarks>
public sealed class FlowState
{
    private readonly Diagnostics diagnostics;
    private readonly CodeFile file;
    private readonly Dictionary<BindingSymbol, FlowCapability> capabilities;
    private readonly ResultVariableFactory resultVariableFactory;
    private readonly ImplicitLendFactory implicitLendFactory;

    /// <summary>
    /// All the distinct subsets of variables
    /// </summary>
    private readonly HashSet<SharingSet> sets;

    /// <summary>
    /// This is a lookup of what set each variable is contained in.
    /// </summary>
    private readonly Dictionary<ISharingVariable, SharingSet> subsetFor;

    public FlowState(
        Diagnostics diagnostics,
        CodeFile file,
        ParameterSharingRelation parameterSharing)
        : this(diagnostics, file, new(), parameterSharing.SharingSets.Select(s => s.MutableCopy()), new(), new())
    {
        foreach (var symbol in parameterSharing.Symbols)
            CapabilityDeclare(symbol);
    }

    public FlowState(Diagnostics diagnostics, CodeFile file)
        : this(diagnostics, file, new(), Enumerable.Empty<SharingSet>(), new(), new())
    {
    }

    private FlowState(
        Diagnostics diagnostics,
        CodeFile file,
        Dictionary<BindingSymbol, FlowCapability> capabilities,
        IEnumerable<SharingSet> sharing,
        ResultVariableFactory resultVariableFactory,
        ImplicitLendFactory implicitLendFactory)
    {
        this.diagnostics = diagnostics;
        this.file = file;
        this.capabilities = new(capabilities);
        sets = new(sharing);
        subsetFor = new();
        foreach (var set in sets)
            foreach (var variable in set)
                subsetFor.Add(variable, set);
        this.resultVariableFactory = resultVariableFactory;
        this.implicitLendFactory = implicitLendFactory;
    }

    public FlowState Fork()
        => new(diagnostics, file, capabilities, sets.Select(s => new SharingSet(s)), resultVariableFactory, implicitLendFactory);

    /// <summary>
    /// Declare the given symbol and combine it with the result variable.
    /// </summary>
    /// <remarks>The result variable is dropped as part of this.</remarks>
    public void Declare(BindingSymbol symbol, ResultVariable? variable)
    {
        CapabilityDeclare(symbol);
        var bindingVariable = SharingDeclare(symbol);
        if (bindingVariable is not null && variable is not null)
            SharingUnion(bindingVariable, variable, null);
        Drop(variable);
    }

    public void Drop(BindingSymbol symbol) => LiftRemovedRestrictions(SharingDrop(symbol));

    public void Drop(ResultVariable? variable)
    {
        if (variable is not null)
            LiftRemovedRestrictions(SharingDrop(variable));
    }

    /// <summary>
    /// Drop all local variables and parameters in preparation for a return from the function or
    /// method.
    /// </summary>
    /// <remarks>External references will ensure that parameters are incorrectly treated as isolated.</remarks>
    public void DropBindingsForReturn()
    {
        SharingDropAllLocalVariablesAndParameters();
        // So many sets can be modified, just go through all of them and remove restrictions
        LiftRemovedRestrictions(sets);
    }

    public void Move(BindingSymbol symbol)
    {
        CapabilityMove(symbol);
        // Identity aren't tracked, this symbol is `id` now
        Drop(symbol);
    }

    public void Freeze(BindingSymbol symbol)
        // Because the symbol could reference `lent const` data, it still needs to be tracked
        // and can't be dropped.
        => CapabilityFreeze(symbol);

    public ResultVariable? Alias(BindingSymbol symbol)
    {
        var capability = CapabilityAlias(symbol);
        if (symbol.SharingIsTracked(capability))
            return SharingNewResult(symbol, resultVariableFactory);
        return null;
    }

    /// <summary>
    /// Gives the current flow type of the symbol.
    /// </summary>
    /// <remarks>This is named for it to be used as <c>flow.Type(symbol)</c></remarks>
    public DataType Type(BindingSymbol? symbol) => CurrentType(symbol);

    /// <summary>
    /// Gives the type of an alias to the symbol
    /// </summary>
    /// <remarks>This is named for it to be used as <c>flow.AliasType(symbol)</c></remarks>
    public DataType AliasType(BindingSymbol? symbol) => CapabilityAliasType(symbol);

    /// <summary>
    /// Combine two <see cref="ResultVariable"/>s into a single sharing set and return a result
    /// variable representative of the new set.
    /// </summary>
    /// <remarks>If two <see cref="ResultVariable"/>s are passed in, one will be dropped.</remarks>
    public ResultVariable? Combine(
        ResultVariable? leftVariable,
        ResultVariable? rightVariable,
        IExpressionSyntax expression)
    {
        if (leftVariable is null) return rightVariable;
        if (rightVariable is null) return leftVariable;
        SharingUnion(leftVariable, rightVariable, () => diagnostics.Add(FlowTypingError.CannotUnion(file, expression.Span)));
        Drop(leftVariable);
        // TODO would it be better to create a new result variable to return instead of reusing this one?
        return rightVariable;
    }

    public bool IsIsolated(BindingVariable variable) => SharingIsIsolated(variable);
    public bool IsIsolated(ISharingVariable? variable)
        => variable is null || SharingIsIsolated(variable);

    public bool IsIsolatedExceptFor(BindingVariable variable, ResultVariable? resultVariable)
        => resultVariable is null ? SharingIsIsolated(variable)
            : SharingIsIsolatedExceptFor(variable, resultVariable);

    /// <summary>
    /// Mark the result as being lent const.
    /// </summary>
    /// <remarks>Because the current result must be at least temporarily const, all references in
    /// the sharing set must now not allow mutation.</remarks>
    public ResultVariable LendConst(ResultVariable result)
    {
        var newResult = SharingLendConst(result, resultVariableFactory, implicitLendFactory);
        // Don't apply read/write restriction because if that is the level, it is already set
        SetRestrictions(ReadSharingSet(result), applyReadWriteRestriction: false);
        return newResult;
    }

    /// <summary>
    /// Mark the result as being lent iso.
    /// </summary>
    /// <remarks>Because the current result must be at least temporarily iso, all references in
    /// the sharing set must now not allow mutation or read.</remarks>
    public ResultVariable LendIso(ResultVariable result)
    {
        var newResult = SharingLendIso(result, resultVariableFactory, implicitLendFactory);
        // Apply read/write restrictions because the level may have increased to that
        SetRestrictions(ReadSharingSet(result), applyReadWriteRestriction: true);
        return newResult;
    }

    /// <summary>
    /// Lift restrictions on symbols in the set if allowed.
    /// </summary>
    private void LiftRemovedRestrictions(IEnumerable<IReadOnlySharingSet> affectedSets)
    {
        foreach (var set in affectedSets)
            LiftRemovedRestrictions(set);
    }

    private void LiftRemovedRestrictions(IReadOnlySharingSet sharingSet)
        // Don't apply read/write restrictions since they have already been applied
        => SetRestrictions(sharingSet, applyReadWriteRestriction: false);

    private void SetRestrictions(IReadOnlySharingSet sharingSet, bool applyReadWriteRestriction)
    {
        var restrictions = sharingSet.Restrictions;
        if (!applyReadWriteRestriction && restrictions == CapabilityRestrictions.ReadWrite)
            return;
        foreach (var variable in sharingSet.OfType<BindingVariable>())
            CapabilitySetRestrictions(variable.Symbol, restrictions);
    }

    public void Merge(FlowState other) => SharingMerge(this);

    public bool IsLent(ResultVariable? variable) => variable is not null && SharingIsLent(variable);

    #region Capability Management
    private void CapabilityDeclare(BindingSymbol symbol)
    {
        if (symbol.SharingIsTracked())
            capabilities.Add(symbol, ((ReferenceType)symbol.DataType).Capability);

        // Other types don't have capabilities and don't need to be tracked
    }

    private ReferenceCapability? CapabilityFor(BindingSymbol? symbol)
    {
        if (symbol?.SharingIsTracked() ?? false)
            return capabilities[symbol].Current;

        // Other types don't have capabilities and don't need to be tracked
        return null;
    }

    private DataType CurrentType(BindingSymbol? symbol)
    {
        var current = CapabilityFor(symbol);
        if (current is not null)
            return ((ReferenceType)symbol!.DataType).With(current);

        // Other types don't have capabilities and don't need to be tracked
        return symbol?.DataType ?? DataType.Unknown;
    }

    private DataType CapabilityAliasType(BindingSymbol? symbol)
    {
        var current = CapabilityFor(symbol);
        if (current is not null)
            return ((ReferenceType)symbol!.DataType).With(current.OfAlias());

        // Other types don't have capabilities and don't need to be tracked
        return symbol?.DataType ?? DataType.Unknown;
    }

    /// <summary>
    /// Creates an alias of the symbol therefor restricting the capability to no longer be `iso`.
    /// </summary>
    private ReferenceCapability? CapabilityAlias(BindingSymbol symbol)
    {
        if (symbol.SharingIsTracked())
            return (capabilities[symbol] = capabilities[symbol].Alias()).Current;

        // Other types don't have capabilities and don't need to be tracked
        return null;
    }

    /// <summary>
    /// Marks that a reference has been moved therefor restricting the capability to `id`.
    /// </summary>
    private void CapabilityMove(BindingSymbol? symbol)
    {
        if (symbol?.SharingIsTracked() ?? false)
            capabilities[symbol] = ReferenceCapability.Identity;

        // Other types don't have capabilities and don't need to be tracked
    }

    private void CapabilityFreeze(BindingSymbol symbol)
    {
        if (symbol.SharingIsTracked())
            capabilities[symbol] = capabilities[symbol].Freeze();

        // Other types don't have capabilities and don't need to be tracked
    }

    private void CapabilitySetRestrictions(BindingSymbol symbol, CapabilityRestrictions restrictions)
    {
        if (symbol.SharingIsTracked())
            capabilities[symbol] = capabilities[symbol].WithRestrictions(restrictions);

        // Other types don't have capabilities and don't need to be tracked
    }
    #endregion

    #region Sharing Management
    public IReadOnlySharingSet ReadSharingSet(ResultVariable variable) => SharingSet(variable);
    private SharingSet SharingSet(BindingVariable variable) => SharingSet((ISharingVariable)variable);

    private SharingSet SharingSet(ISharingVariable variable)
    {
        if (!subsetFor.TryGetValue(variable, out var set))
            throw new InvalidOperationException($"Sharing variable {variable} no longer declared.");

        return set;
    }

    public ResultVariable SharingLendConst(
        ResultVariable result,
        ResultVariableFactory resultVariableFactory,
        ImplicitLendFactory implicitLendFactory)
    {
        _ = SharingSet(result);
        var borrowingResult = SharingNewResult(resultVariableFactory, isLent: true);
        var lend = implicitLendFactory.CreateConstLend();
        SharingDeclare(lend.From, false);
        SharingUnion(result, lend.From, null);
        SharingDeclare(lend.To, true);
        SharingUnion(borrowingResult, lend.To, null);
        return borrowingResult;
    }

    public ResultVariable SharingLendIso(
        ResultVariable result,
        ResultVariableFactory resultVariableFactory,
        ImplicitLendFactory implicitLendFactory)
    {
        _ = SharingSet(result);
        var borrowingResult = SharingNewResult(resultVariableFactory, isLent: true);
        var lend = implicitLendFactory.CreateIsoLend();
        SharingDeclare(lend.From, false);
        SharingUnion(result, lend.From, null);
        SharingDeclare(lend.To, true);
        SharingUnion(borrowingResult, lend.To, null);
        return borrowingResult;
    }

    /// <summary>
    /// Declare a new variable. Newly created variables are not connected to any others.
    /// </summary>
    /// <returns>Whether the variable was declared.</returns>
    public BindingVariable? SharingDeclare(BindingSymbol symbol)
    {
        // Other types don't participate in sharing
        if (!symbol.SharingIsTracked()) return null;

        BindingVariable variable = symbol;
        if (subsetFor.TryGetValue(variable, out _))
            throw new InvalidOperationException("Symbol already declared.");

        SharingDeclare(variable, variable.IsLent);
        return variable;
    }

    private void SharingDeclare(ISharingVariable variable, bool isLent)
    {
        var set = new SharingSet(variable, isLent);
        sets.Add(set);
        subsetFor.Add(variable, set);
    }

    private ResultVariable SharingNewResult(ResultVariableFactory resultVariableFactory, bool isLent = false)
    {
        var result = resultVariableFactory.Create();
        SharingDeclare(result, isLent);
        return result;
    }

    /// <summary>
    /// Create a new result inside the set with the given symbol.
    /// </summary>
    public ResultVariable? SharingNewResult(BindingSymbol symbol, ResultVariableFactory resultVariableFactory)
    {
        if (!symbol.SharingIsTracked()) return null;
        var set = SharingSet(symbol);

        var result = resultVariableFactory.Create();
        set.Declare(result);
        subsetFor.Add(result, set);
        return result;
    }

    public IEnumerable<IReadOnlySharingSet> SharingDrop(BindingVariable variable)
        => SharingDrop((ISharingVariable)variable);

    public IEnumerable<IReadOnlySharingSet> SharingDrop(ResultVariable result)
        => SharingDrop((ISharingVariable)result);

    private IEnumerable<IReadOnlySharingSet> SharingDrop(ISharingVariable variable)
    {
        if (!subsetFor.TryGetValue(variable, out var set))
            return Enumerable.Empty<IReadOnlySharingSet>();

        var affectedSets = new HashSet<IReadOnlySharingSet>();
        set.Remove(variable);
        subsetFor.Remove(variable);
        affectedSets.Add(set);
        if (!set.IsAlive) affectedSets.AddRange(SharingDropAll(set));
        if (set.Count == 0) sets.Remove(set);
        if (variable is ImplicitLendTo lendTo)
            affectedSets.AddRange(SharingDrop(lendTo.From));
        return affectedSets;
    }

    public FixedSet<IReadOnlySharingSet> SharingDropAllLocalVariablesAndParameters()
        => SharingDropAll(subsetFor.Keys.Where(v => v.IsVariableOrParameter)).ToFixedSet();

    private IEnumerable<IReadOnlySharingSet> SharingDropAll(IEnumerable<ISharingVariable> sharingVariables)
        => sharingVariables.ToArray().SelectMany(SharingDrop).Distinct();

    public void SharingUnion(ISharingVariable var1, ISharingVariable var2, Action? addCannotUnionError)
    {
        if (!var1.SharingIsTracked || !var2.SharingIsTracked) return;
        var set1 = SharingSet(var1);
        var set2 = SharingSet(var2);
        if (set1 == set2)
            return;

        var (smallerSet, largerSet) = set1.Count <= set2.Count ? (set1, set2) : (set2, set1);
        if (!largerSet.UnionWith(smallerSet))
        {
            if (addCannotUnionError is null)
                throw new InvalidOperationException(
                    "Cannot union two sharing sets if either is lent unless they are isolated sets.");
            addCannotUnionError();
            return;
        }
        sets.Remove(smallerSet);
        foreach (var symbol in smallerSet)
            subsetFor[symbol] = largerSet;

        // To avoid bugs, clear out the smaller set that shouldn't be used anymore
        smallerSet.Clear();
    }

    public bool SharingIsIsolated(ISharingVariable variable)
        => subsetFor.TryGetValue(variable, out var set) && set.IsIsolated;

    public bool SharingIsIsolatedExceptFor(ISharingVariable variable, ResultVariable result)
        => subsetFor.TryGetValue(variable, out var set) && set.IsIsolatedExceptFor(result);



    public void SharingMerge(FlowState other)
    {
        foreach (var set in (IEnumerable<IReadOnlySharingSet>)other.sets)
        {
            var representative = set.FirstOrDefault(subsetFor.ContainsKey);
            if (representative is null)
            {
                // Whole set is missing, copy and add it
                var newSet = new SharingSet(set);
                sets.Add(newSet);
                foreach (var variable in newSet)
                    subsetFor.Add(variable, newSet);
                continue;
            }

            // At least one item of the set is present. Use that part as a core to union everything
            // in the set together. This is necessary because there could be multiple sets that need
            // unioned.
            foreach (var variable in set)
            {
                // If a variable is completely missing, just declare it. This ensures existing logic
                // can apply to the union.
                if (!subsetFor.ContainsKey(variable))
                    SharingDeclare(variable, set.IsLent);

                SharingUnion(representative, variable, null);
            }
        }
    }

    public bool SharingIsLent(ResultVariable variable) => SharingSet(variable).IsLent;
    #endregion

    public override string ToString() => string.Join(", ", sets.Select(s => $"{{{string.Join(", ", s.Distinct())}}}"));
}
