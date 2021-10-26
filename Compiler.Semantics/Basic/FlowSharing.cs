using System;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Basic
{
    /// <summary>
    /// Stores the sharing relationship between the variables and the expression result. This
    /// relationship is flow sensitive. Note that the sharing relationship is a partition of
    /// the set of variables into disjoint subsets.
    /// </summary>
    public class FlowSharing
    {
        /// <summary>
        /// Declare a new variable. Newly created variables are not connected to any others.
        /// </summary>
        public void Declare(BindingSymbol symbol)
        {
            throw new NotImplementedException();
        }

        public void Union()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Split the given symbol out from sharing with the other variables it is connected to.
        /// </summary>
        /// <param name="symbol"></param>
        public void Split(BindingSymbol symbol)
        {
            throw new NotImplementedException();
        }

        public bool IsIsolated(BindingSymbol symbol)
        {
            throw new NotImplementedException();
        }
    }
}
