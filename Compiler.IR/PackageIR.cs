using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IR.Declarations;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Compiler.Types;
using Azoth.Tools.Bootstrap.Framework;
using Azoth.Tools.Bootstrap.IL;

namespace Azoth.Tools.Bootstrap.Compiler.IR
{
    public class PackageIR
    {
        public PackageSymbol Symbol { get; }
        public FixedSymbolTree SymbolTree { get; }
        public FixedList<Diagnostic> Diagnostics { get; }
        public Table<ClassIR> Classes { get; }
        public Table<FunctionIR> Functions { get; }
        public Table<DataType> Types { get; }
        /// <summary>
        /// Referenced packages
        /// </summary>
        /// <remarks>No longer need aliases. All references are now explicit.</remarks>
        public FixedSet<PackageIR> References { get; }

        public FunctionIR? EntryPoint { get; }
        public PackageIR(
            FixedSymbolTree symbolTree,
            FixedList<Diagnostic> diagnostics,
            FixedSet<PackageIR> references,
            Table<ClassIR> classes,
            Table<FunctionIR> functions,
            Table<DataType> types)
        {
            Symbol = symbolTree.Package;
            SymbolTree = symbolTree;
            Diagnostics = diagnostics;
            References = references;
            Classes = classes;
            Functions = functions;
            Types = types;
        }

        public PackageIL ToIL()
        {
            var ilBuilder = new ILBuilder();
            return new PackageIL(
                References.Select(r => r.ToReferenceIL()).ToFixedList(),
                FixedList<string>.Empty,
                FixedList<string>.Empty,
                Classes.Select(ilBuilder.Build).ToFixedList(),
                Functions.Select(c => c.ToIL()).ToFixedList(),
                Types.ToFixedList(),
                ilBuilder.Lookup(EntryPoint));
        }

        private PackageReferenceIL ToReferenceIL()
        {
            return new PackageReferenceIL();
        }
    }
}
