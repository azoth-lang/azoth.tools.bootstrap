using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

/// <summary>
/// A collection of reference capabilities assigned to symbols used to track flow sensitive
/// reference capabilities.
/// </summary>
public sealed class ReferenceCapabilities
{
    private readonly Dictionary<BindingSymbol, ModifiedCapability> currentCapabilities;

    public ReferenceCapabilities()
    {
        currentCapabilities = new();
    }

    internal ReferenceCapabilities(
        IReadOnlyDictionary<BindingSymbol, ModifiedCapability> currentCapabilities)
    {
        this.currentCapabilities = new(currentCapabilities);
    }

    public ReferenceCapabilities Copy() => new(currentCapabilities);

    public void Declare(BindingSymbol symbol)
    {
        if (symbol.DataType is ReferenceType referenceType)
            // Alias all references because when used, an alias will exist
            currentCapabilities.Add(symbol, referenceType.Capability.Alias());

        // Other types don't have capabilities and don't need to be tracked
    }

    public ReferenceCapability? For(BindingSymbol? symbol)
    {
        if (symbol?.DataType is ReferenceType)
            return currentCapabilities[symbol].CurrentCapability;

        // Other types don't have capabilities and don't need to be tracked
        return null;
    }

    public DataType CurrentType(BindingSymbol? symbol)
    {
        if (symbol?.DataType is ReferenceType referenceType)
            return referenceType.With(currentCapabilities[symbol].CurrentCapability);

        // Other types don't have capabilities and don't need to be tracked
        return symbol?.DataType ?? DataType.Unknown;
    }

    /// <summary>
    /// Creates an alias of the symbol therefor restricting the capability to no longer be `iso`.
    /// </summary>
    public void Alias(BindingSymbol? symbol)
    {
        if (symbol?.DataType is ReferenceType)
            currentCapabilities[symbol] = currentCapabilities[symbol].Alias();

        // Other types don't have capabilities and don't need to be tracked
    }

    /// <summary>
    /// Marks that a reference has been moved therefor restricting the capability to `id`.
    /// </summary>
    public void Move(BindingSymbol? symbol)
    {
        if (symbol?.DataType is ReferenceType)
            currentCapabilities[symbol] = ReferenceCapability.Identity;

        // Other types don't have capabilities and don't need to be tracked
    }

    public void Freeze(BindingSymbol symbol)
    {
        if (symbol.DataType is ReferenceType)
            currentCapabilities[symbol] = currentCapabilities[symbol].Freeze();

        // Other types don't have capabilities and don't need to be tracked
    }

    public void RestrictWrite(BindingSymbol symbol)
    {
        if (symbol.DataType is ReferenceType)
            currentCapabilities[symbol] = currentCapabilities[symbol].RestrictWrite();

        // Other types don't have capabilities and don't need to be tracked
    }

    public void RemoveWriteRestriction(BindingSymbol symbol)
    {
        if (symbol.DataType is ReferenceType)
            currentCapabilities[symbol] = currentCapabilities[symbol].RemoveWriteRestriction();

        // Other types don't have capabilities and don't need to be tracked
    }

    public ReferenceCapabilitiesSnapshot Snapshot() => new(currentCapabilities);

    public bool IsTracked(BindingSymbol symbol)
    {
        var capability = For(symbol);
        return capability != ReferenceCapability.Constant && capability != ReferenceCapability.Identity;
    }
}
