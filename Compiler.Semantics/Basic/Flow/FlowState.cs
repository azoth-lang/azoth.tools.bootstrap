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
using MoreLinq;

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
    private readonly ResultVariableFactory resultVariableFactory;
    private readonly ImplicitLendFactory implicitLendFactory;

    private readonly Dictionary<ICapabilitySharingVariable, FlowCapability> capabilities;

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
            TrackCapability(symbol);
    }

    public FlowState(Diagnostics diagnostics, CodeFile file)
        : this(diagnostics, file, new(), Enumerable.Empty<SharingSet>(), new(), new())
    {
    }

    private FlowState(
        Diagnostics diagnostics,
        CodeFile file,
        Dictionary<ICapabilitySharingVariable, FlowCapability> capabilities,
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
        // Other types don't participate in sharing
        if (symbol.SharingIsTracked())
        {
            BindingVariable bindingVariable = symbol;
            if (subsetFor.TryGetValue(bindingVariable, out _))
                throw new InvalidOperationException("Symbol already declared.");

            SharingDeclare(bindingVariable, bindingVariable.IsLent);
            TrackCapability(symbol);
            if (variable is not null)
                SharingUnion(bindingVariable, variable, null);
        }

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
        SharingDropAll(subsetFor.Keys.Where(v => v.IsVariableOrParameter)).Consume();
        // So many sets can be modified, just go through all of them and remove restrictions
        LiftRemovedRestrictions(sets);
    }

    public void Move(BindingSymbol symbol)
    {
        if (symbol.SharingIsTracked())
            capabilities[(BindingVariable)symbol] = ReferenceCapability.Identity;

        // Other types don't have capabilities and don't need to be tracked
        // Identity aren't tracked, this symbol is `id` now
        Drop(symbol);
    }

    public void Freeze(BindingSymbol symbol)
    {
        // Because the symbol could reference `lent const` data, it still needs to be tracked
        // and can't be dropped.
        if (symbol.SharingIsTracked())
        {
            BindingVariable variable = symbol;
            capabilities[variable] = capabilities[variable].AfterFreeze();
        }

        // Other types don't have capabilities and don't need to be tracked
    }

    public void Restrict(ResultVariable? variable, DataType type)
    {
        if (variable is null
            || !capabilities.TryGetValue(variable, out var flowCapability)
            || type is not ReferenceType { Capability: var capability })
            return;

        capabilities[variable] = flowCapability.With(capability);
    }

    public ResultVariable? Alias(BindingSymbol symbol)
    {
        if (!symbol.SharingIsTracked())
            return null;

        BindingVariable variable = symbol;
        var capability = (capabilities[variable] = capabilities[variable].WhenAliased()).Current;

        // Other types don't have capabilities and don't need to be tracked
        if (symbol.SharingIsTracked(capability))
        {
            var set = SharingSet(symbol);

            var result = resultVariableFactory.Create();
            set.Declare(result);
            subsetFor.Add(result, set);
            TrackCapability(result, capability.OfAlias());
            return result;
        }

        return null;
    }

    /// <summary>
    /// Gives the current flow type of the symbol.
    /// </summary>
    /// <remarks>This is named for it to be used as <c>flow.Type(symbol)</c></remarks>
    public DataType Type(BindingSymbol? symbol)
        => Type(symbol, Functions.Identity);

    /// <summary>
    /// Gives the type of an alias to the symbol
    /// </summary>
    /// <remarks>This is named for it to be used as <c>flow.AliasType(symbol)</c></remarks>
    public DataType AliasType(BindingSymbol? symbol)
        => Type(symbol, c => c.OfAlias());

    private DataType Type(BindingSymbol? symbol, Func<ReferenceCapability, ReferenceCapability> transform)
    {
        if (symbol is null) return DataType.Unknown;
        if (!symbol.SharingIsTracked())
            // Other types don't have capabilities and don't need to be tracked
            return symbol.DataType;

        var current = capabilities[(BindingVariable)symbol].Current;
        return ((ReferenceType)symbol.DataType).With(transform(current));
    }

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

    public bool IsIsolated(BindingVariable variable) => IsIsolated((ISharingVariable)variable);
    public bool IsIsolated(ISharingVariable? variable)
        => variable is null || subsetFor.TryGetValue(variable, out var set) && set.IsIsolated;
    public bool IsIsolatedExceptFor(BindingVariable variable, ResultVariable? resultVariable)
    {
        return resultVariable is null
            ? IsIsolated((ISharingVariable)variable)
            : subsetFor.TryGetValue(variable, out var set) && set.IsIsolatedExceptFor(resultVariable);
    }

    public bool CanFreeze(BindingVariable variable) => CanFreeze((ISharingVariable)variable);
    public bool CanFreeze(ISharingVariable? variable) => CanFreezeExceptFor(variable, null);
    public bool CanFreezeExceptFor(BindingVariable variable, ResultVariable? resultVariable)
        => CanFreezeExceptFor((ISharingVariable)variable, resultVariable);
    public bool CanFreezeExceptFor(ISharingVariable? variable, ResultVariable? resultVariable)
    {
        if (variable is null) return true;
        if (!subsetFor.TryGetValue(variable, out var set)) return false;
        if (set.IsIsolated) return true;

        foreach (var sharingVariable in set.Except(variable).Except(resultVariable))
        {
            if (sharingVariable is ICapabilitySharingVariable capabilityVariable)
            {
                // The modified capability is what matters because lending can go out of scope later
                var capability = capabilities[capabilityVariable].Modified;
                if (capability.AllowsWrite)
                    return false;
            }
            else
                // All other sharing variable types prevent freezing
                return false;
        }
        return true;
    }

    /// <summary>
    /// Mark the result as being lent const.
    /// </summary>
    /// <remarks>Because the current result must be at least temporarily const, all references in
    /// the sharing set must now not allow mutation.</remarks>
    public ResultVariable LendConst(ResultVariable result)
        => Lend(result, implicitLendFactory.CreateConstLend());

    /// <summary>
    /// Mark the result as being lent iso.
    /// </summary>
    /// <remarks>Because the current result must be at least temporarily iso, all references in
    /// the sharing set must now not allow mutation or read.</remarks>
    public ResultVariable LendIso(ResultVariable result)
        => Lend(result, implicitLendFactory.CreateIsoLend());

    private ResultVariable Lend(ResultVariable result, ImplicitLend lend)
    {
        _ = SharingSet(result);
        var borrowingResult = resultVariableFactory.Create();
        TrackCapability(borrowingResult, lend.LendAs);
        SharingDeclare(borrowingResult, true);
        SharingDeclare(lend.From, false);
        SharingUnion(result, lend.From, null);
        SharingDeclare(lend.To, true);
        SharingUnion(borrowingResult, lend.To, null);
        // Apply read/write restrictions because the level may have increased to that
        SetRestrictions(SharingSet(result), applyReadWriteRestriction: true);
        return borrowingResult;
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
        {
            if (variable.Symbol.SharingIsTracked())
                capabilities[variable] = capabilities[variable].WithRestrictions(restrictions);

            // Other types don't have capabilities and don't need to be tracked
        }
    }

    public void Merge(FlowState other)
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

    public bool IsLent(ResultVariable? variable) => variable is not null && SharingSet(variable).IsLent;

    private void TrackCapability(BindingSymbol symbol)
    {
        if (symbol.SharingIsTracked())
            capabilities.Add((BindingVariable)symbol, ((ReferenceType)symbol.DataType).Capability);

        // Other types don't have capabilities and don't need to be tracked
    }

    private void TrackCapability(ResultVariable result, ReferenceCapability capability)
        => capabilities.Add(result, capability);

    #region Sharing Management
    private SharingSet SharingSet(BindingVariable variable) => SharingSet((ISharingVariable)variable);

    private SharingSet SharingSet(ISharingVariable variable)
    {
        if (!subsetFor.TryGetValue(variable, out var set))
            throw new InvalidOperationException($"Sharing variable {variable} no longer declared.");

        return set;
    }

    private void SharingDeclare(ISharingVariable variable, bool isLent)
    {
        var set = new SharingSet(variable, isLent);
        sets.Add(set);
        subsetFor.Add(variable, set);
    }

    public IEnumerable<IReadOnlySharingSet> SharingDrop(BindingVariable variable)
        => SharingDrop((ISharingVariable)variable);

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
    #endregion

    public override string ToString() => string.Join(", ", sets.Select(s => $"{{{string.Join(", ", s.Distinct())}}}"));
}
