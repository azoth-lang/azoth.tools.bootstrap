using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic.Flow;

/// <summary>
/// An immutable snapshot of the flow sensitive reference capabilities at one particular point
/// in the code.
/// </summary>
public class ReferenceCapabilitiesSnapshot
{
    private readonly FixedDictionary<BindingSymbol, ModifiedCapability> currentCapabilities;

    internal ReferenceCapabilitiesSnapshot(
        IDictionary<BindingSymbol, ModifiedCapability> currentCapabilities)
    {
        this.currentCapabilities = currentCapabilities.ToFixedDictionary();
    }

    public ReferenceCapabilities MutableCopy() => new(currentCapabilities);
}
