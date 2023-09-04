using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic;

/// <summary>
/// A collection of reference capabilities assigned to symbols used to track flow sensitive
/// reference capabilities.
/// </summary>
public class ReferenceCapabilities
{
    private readonly Dictionary<BindingSymbol, ReferenceCapability> currentCapabilities;

    public ReferenceCapabilities()
    {
        currentCapabilities = new();
    }

    internal ReferenceCapabilities(
        IReadOnlyDictionary<BindingSymbol, ReferenceCapability> currentCapabilities)
    {
        this.currentCapabilities = new(currentCapabilities);
    }

    public void Declare(BindingSymbol symbol)
    {
        if (symbol.DataType is ReferenceType referenceType)
            currentCapabilities.Add(symbol, referenceType.Capability);

        // Other types don't have capabilities and don't need to be tracked
    }

    public ReferenceCapability? For(BindingSymbol? symbol)
    {
        if (symbol?.DataType is ReferenceType)
            return currentCapabilities[symbol];

        // Other types don't have capabilities and don't need to be tracked
        return null;
    }

    public DataType CurrentType(BindingSymbol? symbol)
    {
        if (symbol?.DataType is ReferenceType referenceType)
            return referenceType.To(currentCapabilities[symbol]);

        // Other types don't have capabilities and don't need to be tracked
        return symbol?.DataType ?? DataType.Unknown;
    }

    /// <summary>
    /// Creates an alias of the symbol therefor restricting the capability to no longer be `iso`.
    /// </summary>
    public void Alias(BindingSymbol? symbol)
    {
        if (symbol?.DataType is ReferenceType)
        {
            var capability = currentCapabilities[symbol];
            if (capability == ReferenceCapability.Isolated)
                currentCapabilities[symbol] = ReferenceCapability.Mutable;
        }
        // Other types don't have capabilities and don't need to be tracked
    }

    /// <summary>
    /// Marks that a reference has been moved therefor restricting the capability to `id`.
    /// </summary>
    public void Move(BindingSymbol? symbol)
    {
        if (symbol?.DataType is ReferenceType)
        {
            var capability = currentCapabilities[symbol];
            if (capability != ReferenceCapability.Identity)
                currentCapabilities[symbol] = ReferenceCapability.Identity;
        }
        // Other types don't have capabilities and don't need to be tracked
    }

    public void Freeze(VariableSymbol symbol)
    {
        if (symbol.DataType is ReferenceType)
        {
            var capability = currentCapabilities[symbol];
            if (capability != ReferenceCapability.Identity)
                currentCapabilities[symbol] = ReferenceCapability.Constant;
        }
        // Other types don't have capabilities and don't need to be tracked
    }

    public ReferenceCapabilitiesSnapshot Snapshot() => new(currentCapabilities);
}
