using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.IntermediateLanguage;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST.Tree
{
    internal class Package
    {
        public FixedList<IDeclaration> AllDeclarations { get; }
        public FixedList<INonMemberDeclaration> NonMemberDeclarations { get; }
        public FixedSymbolTree SymbolTree { get; }
        public SymbolForest SymbolTrees { get; }
        public Diagnostics Diagnostics { get; }
        public FixedDictionary<Name, PackageIL> References { get; }
        public IEnumerable<PackageIL> ReferencedPackages => References.Values;

        public Package(
            FixedList<INonMemberDeclaration> nonMemberDeclarations,
            FixedSymbolTree symbolTree,
            Diagnostics diagnostics,
            FixedDictionary<Name, PackageIL> references)
        {
            AllDeclarations = GetAllDeclarations(nonMemberDeclarations).ToFixedList();
            NonMemberDeclarations = nonMemberDeclarations;
            SymbolTree = symbolTree;
            Diagnostics = diagnostics;
            References = references;
            SymbolTrees = new SymbolForest(Primitive.SymbolTree, ReferencedPackages.Select(p => p.SymbolTree).Append(SymbolTree));
        }

        private static IEnumerable<IDeclaration> GetAllDeclarations(
            IEnumerable<INonMemberDeclaration> nonMemberDeclarations)
        {
            var declarations = new Queue<IDeclaration>();
            declarations.EnqueueRange(nonMemberDeclarations);
            while (declarations.TryDequeue(out var declaration))
            {
                yield return declaration;
                if (declaration is IClassDeclaration syn)
                    declarations.EnqueueRange(syn.Members);
            }
        }
    }
}
