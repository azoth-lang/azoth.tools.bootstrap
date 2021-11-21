using System;
using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic
{
    /// <summary>
    /// A collection of reference capabilities assigned to symbols used to track flow sensitive
    /// reference capabilities.
    /// </summary>
    public class FlowReferenceCapabilities
    {
        private readonly Dictionary<BindingSymbol, ReferenceCapability> currentCapabilities;

        public FlowReferenceCapabilities()
        {
            currentCapabilities = new();
        }

        public FlowReferenceCapabilities(
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

        public void Restrict(BindingSymbol symbol, ReferenceCapability type)
        {
            throw new NotImplementedException();
        }

        public FlowReferenceCapabilitiesSnapshot Snapshot()
            => new FlowReferenceCapabilitiesSnapshot(currentCapabilities);
    }
}
