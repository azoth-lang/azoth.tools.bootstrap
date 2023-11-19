using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

/// <summary>
/// A collection of reference capabilities assigned to symbols used to track flow sensitive
/// reference capabilities.
/// </summary>
public sealed class ReferenceCapabilities
{
    private readonly Dictionary<BindingSymbol, FlowCapability> currentCapabilities;

    public ReferenceCapabilities()
    {
        currentCapabilities = new();
    }

    internal ReferenceCapabilities(
        IReadOnlyDictionary<BindingSymbol, FlowCapability> currentCapabilities)
    {
        this.currentCapabilities = new(currentCapabilities);
    }

    public ReferenceCapabilities Copy() => new(currentCapabilities);

    public void Declare(BindingSymbol symbol)
    {
        if (symbol.SharingIsTracked())
            currentCapabilities.Add(symbol, ((ReferenceType)symbol.DataType).Capability);

        // Other types don't have capabilities and don't need to be tracked
    }

    public ReferenceCapability? For(BindingSymbol? symbol)
    {
        if (symbol?.SharingIsTracked() ?? false)
            return currentCapabilities[symbol].Current;

        // Other types don't have capabilities and don't need to be tracked
        return null;
    }

    public DataType CurrentType(BindingSymbol? symbol)
    {
        var current = For(symbol);
        if (current is not null)
            return ((ReferenceType)symbol!.DataType).With(current);

        // Other types don't have capabilities and don't need to be tracked
        return symbol?.DataType ?? DataType.Unknown;
    }

    public DataType AliasType(BindingSymbol? symbol)
    {
        var current = For(symbol);
        if (current is not null)
            return ((ReferenceType)symbol!.DataType).With(current.OfAlias());

        // Other types don't have capabilities and don't need to be tracked
        return symbol?.DataType ?? DataType.Unknown;
    }

    /// <summary>
    /// Creates an alias of the symbol therefor restricting the capability to no longer be `iso`.
    /// </summary>
    public ReferenceCapability? Alias(BindingSymbol symbol)
    {
        if (symbol.SharingIsTracked())
            return (currentCapabilities[symbol] = currentCapabilities[symbol].Alias()).Current;

        // Other types don't have capabilities and don't need to be tracked
        return null;
    }

    /// <summary>
    /// Marks that a reference has been moved therefor restricting the capability to `id`.
    /// </summary>
    public void Move(BindingSymbol? symbol)
    {
        if (symbol?.SharingIsTracked() ?? false)
            currentCapabilities[symbol] = ReferenceCapability.Identity;

        // Other types don't have capabilities and don't need to be tracked
    }

    public void Freeze(BindingSymbol symbol)
    {
        if (symbol.SharingIsTracked())
            currentCapabilities[symbol] = currentCapabilities[symbol].Freeze();

        // Other types don't have capabilities and don't need to be tracked
    }

    public void SetRestrictions(BindingSymbol symbol, CapabilityRestrictions restrictions)
    {
        if (symbol.SharingIsTracked())
            currentCapabilities[symbol] = currentCapabilities[symbol].WithRestrictions(restrictions);

        // Other types don't have capabilities and don't need to be tracked
    }
}
