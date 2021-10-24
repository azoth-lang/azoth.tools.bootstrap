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
        private readonly Dictionary<BindingSymbol, ReferenceCapability> currentCapabilities = new();

        public void Declare(BindingSymbol symbol)
        {
            //currentTypes = currentTypes.Add(symbol, symbol.DataType);
            throw new NotImplementedException();
        }

        public void Restrict(BindingSymbol symbol, ReferenceCapability type)
        {
            throw new NotImplementedException();
        }

        public FlowReferenceCapabilitiesSnapshot Snapshot()
        {
            return new FlowReferenceCapabilitiesSnapshot(currentCapabilities);
        }
    }
}
