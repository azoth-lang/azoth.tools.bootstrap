using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic
{
    /// <summary>
    /// An immutable snapshot of the flow sensitive reference capabilities at one particular point
    /// in the code.
    /// </summary>
    public class FlowReferenceCapabilitiesSnapshot
    {
        private readonly FixedDictionary<BindingSymbol, ReferenceCapability> currentCapabilities;

        public FlowReferenceCapabilitiesSnapshot(
            IDictionary<BindingSymbol, ReferenceCapability> currentCapabilities)
        {
            this.currentCapabilities = currentCapabilities.ToFixedDictionary();
        }

        public FlowReferenceCapabilities MutableCopy()
            => new FlowReferenceCapabilities(currentCapabilities);
    }
}
