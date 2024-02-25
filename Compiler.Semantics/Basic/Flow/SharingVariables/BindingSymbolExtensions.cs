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
        return SharingIsTracked(symbol.Type);
    }

    public static bool SharingIsTracked(this Pseudotype pseudotype)
    {
        // ObjectTypeConstraints need tracked because they act like read only references
        if (pseudotype is ObjectTypeConstraint) return true;
        // If it isn't a reference type, then no need to track it (all current value types are `const`)
        if (pseudotype is not ReferenceType { Capability: var capability })
            return false;

        // Constant and Identity capabilities never need tracked
        return capability != Capability.Constant && capability != Capability.Identity;
    }

    /// <summary>
    /// Whether reference sharing is tracked for this symbol once it has the given capability.
    /// </summary>
    public static bool SharingIsTracked(this BindingSymbol symbol, FlowCapabilities? flowCapabilities)
    {
        if (flowCapabilities?.Outer.Modified == Capability.Identity) return false;
        return symbol.SharingIsTracked();
    }
}
