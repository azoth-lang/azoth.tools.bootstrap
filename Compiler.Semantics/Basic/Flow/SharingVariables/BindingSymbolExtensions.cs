using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow.SharingVariables;

internal static class BindingSymbolExtensions
{
    /// <summary>
    /// Whether reference sharing is tracked for this symbol.
    /// </summary>
    public static bool SharingIsTracked(this BindingSymbol symbol)
    {
        // Any lent parameter needs tracked
        if (symbol.IsLentBinding) return true;
        // If it isn't a reference type, then no need to track it (all current value types are `const`)
        if (symbol.DataType is not ReferenceType { Capability: var capability }
            // Identity capabilities never need tracked
            || capability == ReferenceCapability.Identity) return false;

        // Even constant locals need tracked because they could alias a lent const
        if (symbol is VariableSymbol { IsLocal: true }) return true;

        // Parameters need tracked unless they are constant (identity has already been checked)
        return capability != ReferenceCapability.Constant;
    }

    /// <summary>
    /// Whether reference sharing is tracked for this symbol once it has the given capability.
    /// </summary
    public static bool SharingIsTracked(this BindingSymbol symbol, ReferenceCapability? currentCapability)
    {
        if (currentCapability == ReferenceCapability.Identity) return false;
        return symbol.SharingIsTracked();
    }
}
