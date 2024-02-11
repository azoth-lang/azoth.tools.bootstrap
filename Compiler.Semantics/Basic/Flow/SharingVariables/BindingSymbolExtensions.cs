using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Compiler.Types.Capabilities;
using Azoth.Tools.Bootstrap.Compiler.Types.Pseudotypes;

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
        // ObjectTypeConstraints need tracked because they act like read only references
        if (symbol.Type is ObjectTypeConstraint) return true;
        // If it isn't a reference type, then no need to track it (all current value types are `const`)
        if (symbol.Type is not ReferenceType { Capability: var capability }
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
    public static bool SharingIsTracked(this BindingSymbol symbol, FlowCapabilities? flowCapabilities)
    {
        if (flowCapabilities?.Outer.Modified == ReferenceCapability.Identity) return false;
        return symbol.SharingIsTracked();
    }
}
