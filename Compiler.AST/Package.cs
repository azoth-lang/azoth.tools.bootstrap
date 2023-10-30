using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.AST;

public class Package : IHasSymbolTree
{
    public FixedSet<IDeclaration> Declarations { get; }
    public FixedSet<IDeclaration> TestingDeclarations { get; }
    public FixedSet<INonMemberDeclaration> NonMemberDeclarations { get; }
    public FixedSet<INonMemberDeclaration> TestingNonMemberDeclarations { get; }
    public PackageSymbol Symbol { get; }
    public FixedSymbolTree SymbolTree { get; }
    public FixedSymbolTree TestingSymbolTree { get; }
    public FixedList<Diagnostic> Diagnostics { get; }
    public FixedSet<Package> References { get; }
    public IFunctionDeclaration? EntryPoint { get; }

    public Package(
        FixedSet<INonMemberDeclaration> nonMemberDeclarations,
        FixedSet<INonMemberDeclaration> testingNonMemberDeclarations,
        FixedSymbolTree symbolTree,
        FixedSymbolTree testingSymbolTree,
        FixedList<Diagnostic> diagnostics,
        FixedSet<Package> references,
        IFunctionDeclaration? entryPoint)
    {
        Declarations = GetAllDeclarations(nonMemberDeclarations).ToFixedSet();
        TestingDeclarations = GetAllDeclarations(testingNonMemberDeclarations).ToFixedSet();
        NonMemberDeclarations = nonMemberDeclarations;
        TestingNonMemberDeclarations = testingNonMemberDeclarations;
        Symbol = symbolTree.Package;
        SymbolTree = symbolTree;
        TestingSymbolTree = testingSymbolTree;
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
            if (declaration is ITypeDeclaration syn)
                declarations.EnqueueRange(syn.Members);
        }
    }

    public override string ToString() => Symbol.ToString();
}
