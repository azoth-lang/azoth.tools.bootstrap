using System.Collections.Generic;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST;

internal class PackageBuilder
{
    public FixedSet<IDeclaration> AllDeclarations { get; }
    public FixedSet<INonMemberDeclaration> NonMemberDeclarations { get; }
    public FixedSymbolTree SymbolTree { get; }
    public SymbolForest SymbolTrees { get; }
    public Diagnostics Diagnostics { get; }
    public FixedDictionary<SimpleName, Package> References { get; }
    public IEnumerable<Package> ReferencedPackages => References.Values;
    public IFunctionDeclaration? EntryPoint { get; set; }

    public PackageBuilder(
        FixedSet<INonMemberDeclaration> nonMemberDeclarations,
        FixedSymbolTree symbolTree,
        Diagnostics diagnostics,
        FixedDictionary<SimpleName, Package> references)
    {
        AllDeclarations = GetAllDeclarations(nonMemberDeclarations).ToFixedSet();
        NonMemberDeclarations = nonMemberDeclarations;
        SymbolTree = symbolTree;
        Diagnostics = diagnostics;
        References = references;
        SymbolTrees = BuiltIn.CreateSymbolForest(
            ReferencedPackages.Select(p => p.SymbolTree).Append(SymbolTree));
    }

    private static IEnumerable<IDeclaration> GetAllDeclarations(
        IEnumerable<INonMemberDeclaration> nonMemberDeclarations)
    {
        var declarations = new Queue<IDeclaration>();
        declarations.EnqueueRange(nonMemberDeclarations);
        while (declarations.TryDequeue(out var declaration))
        {
            yield return declaration;
            if (declaration is IClassDeclaration syn) declarations.EnqueueRange(syn.Members);
        }
    }

    public Package Build()
    {
        return new Package(NonMemberDeclarations, SymbolTree,
            Diagnostics.ToFixedList(), References.Values.ToFixedSet(),
            EntryPoint);
    }
}
