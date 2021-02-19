using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.AST
{
    public class Package : IHasSymbolTree
    {
        public FixedList<IDeclaration> AllDeclarations { get; }
        public FixedList<INonMemberDeclaration> NonMemberDeclarations { get; }
        public PackageSymbol Symbol { get; }
        public FixedSymbolTree SymbolTree { get; }
        public FixedList<Diagnostic> Diagnostics { get; }
        public FixedSet<Package> References { get; }
        public IFunctionDeclaration? EntryPoint { get; internal set; }

        public Package(
            FixedList<INonMemberDeclaration> nonMemberDeclarations,
            FixedSymbolTree symbolTree,
            FixedList<Diagnostic> diagnostics,
            FixedSet<Package> references)
        {
            AllDeclarations = GetAllDeclarations(nonMemberDeclarations).ToFixedList();
            NonMemberDeclarations = nonMemberDeclarations;
            Symbol = symbolTree.Package;
            SymbolTree = symbolTree;
            Diagnostics = diagnostics;
            References = references;
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
