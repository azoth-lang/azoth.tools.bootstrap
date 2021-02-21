using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.AST.Interpreter.MemoryLayout
{
    internal class VTableList
    {
        private readonly List<FixedDictionary<MethodSymbol, IMethodDeclaration>> vtables = new List<FixedDictionary<MethodSymbol, IMethodDeclaration>>();

        public FixedDictionary<MethodSymbol, IMethodDeclaration> this[VTableRef vTableRef]
            => vtables[vTableRef.Index];

        public VTableRef Add(FixedDictionary<MethodSymbol, IMethodDeclaration> table)
        {
            var vTableRef = new VTableRef(vtables.Count);
            vtables.Add(table);
            return vTableRef;
        }
    }
}
