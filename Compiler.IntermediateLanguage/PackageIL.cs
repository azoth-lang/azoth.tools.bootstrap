using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage
{
    public class PackageIL
    {
        public PackageSymbol Symbol { get; }
        public FixedSymbolTree SymbolTree { get; }
        public FixedList<Diagnostic> Diagnostics { get; internal set; }
        /// <summary>
        /// Referenced packages
        /// </summary>
        /// <remarks>No longer need aliases. All references are now explicit.</remarks>
        public FixedSet<PackageIL> References { get; }
        public FixedSet<DeclarationIL> Declarations { get; }
        public FunctionIL? EntryPoint { get; internal set; }

        public PackageIL(
            FixedSymbolTree symbolTree,
            FixedList<Diagnostic> diagnostics,
            FixedSet<PackageIL> references,
            IEnumerable<DeclarationIL> declarations,
            FunctionIL? entryPoint)
        {
            Symbol = symbolTree.Package;
            SymbolTree = symbolTree;
            Diagnostics = diagnostics;
            References = references;
            EntryPoint = entryPoint;
            Declarations = declarations.ToFixedSet();
        }
    }
}
