using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.AST;

public class Package : IHasSymbolTree
{
    public FixedSet<IDeclaration> AllDeclarations { get; }
    public FixedSet<INonMemberDeclaration> NonMemberDeclarations { get; }
    public PackageSymbol Symbol { get; }
    public FixedSymbolTree SymbolTree { get; }
    public FixedList<Diagnostic> Diagnostics { get; }
    public FixedSet<Package> References { get; }
    public IFunctionDeclaration? EntryPoint { get; }

    public Package(
        FixedSet<INonMemberDeclaration> nonMemberDeclarations,
        FixedSymbolTree symbolTree,
        FixedList<Diagnostic> diagnostics,
        FixedSet<Package> references,
        IFunctionDeclaration? entryPoint)
    {
        AllDeclarations = GetAllDeclarations(nonMemberDeclarations).ToFixedSet();
        NonMemberDeclarations = nonMemberDeclarations;
        Symbol = symbolTree.Package;
        SymbolTree = symbolTree;
        Diagnostics = diagnostics;
        References = references;
        EntryPoint = entryPoint;
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

    public override string ToString() => Symbol.ToString();
}
