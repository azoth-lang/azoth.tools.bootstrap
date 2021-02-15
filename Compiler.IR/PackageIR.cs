using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.IR
{
    public class PackageIR
    {
        public PackageSymbol Symbol { get; }
        public FixedSymbolTree SymbolTree { get; }
        public FixedList<Diagnostic> Diagnostics { get; }

        /// <summary>
        /// Referenced packages
        /// </summary>
        /// <remarks>No longer need aliases. All references are now explicit.</remarks>
        public FixedSet<PackageIR> References { get; }
        public PackageIR(
            PackageSymbol symbol,
            FixedSymbolTree symbolTree,
            FixedList<Diagnostic> diagnostics,
            FixedSet<PackageIR> references)
        {
            Symbol = symbol;
            SymbolTree = symbolTree;
            Diagnostics = diagnostics;
            References = references;
        }
    }
}
