using System.Collections.Generic;
using System.Collections.ObjectModel;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic
{
    /// <summary>
    /// An immutable snapshot of the flow sensitive reference capabilities at one particular point
    /// in the code.
    /// </summary>
    public class FlowReferenceCapabilitiesSnapshot
    {
        private readonly IReadOnlyDictionary<BindingSymbol, ReferenceCapability> currentCapabilities;

        public FlowReferenceCapabilitiesSnapshot(
            IDictionary<BindingSymbol, ReferenceCapability> currentCapabilities)
        {
            this.currentCapabilities = new ReadOnlyDictionary<BindingSymbol, ReferenceCapability>(currentCapabilities);
        }

        public FlowReferenceCapabilities MutableCopy()
        {
            return new FlowReferenceCapabilities(currentCapabilities);
        }
    }
}
