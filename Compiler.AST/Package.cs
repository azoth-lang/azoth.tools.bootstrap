using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.AST;

public class Package : IHasSymbolTree
{
    public IFixedSet<IDeclaration> Declarations { get; }
    public IFixedSet<IDeclaration> TestingDeclarations { get; }
    public IFixedSet<INonMemberDeclaration> NonMemberDeclarations { get; }
    public IFixedSet<INonMemberDeclaration> TestingNonMemberDeclarations { get; }
    public PackageSymbol Symbol { get; }
    public FixedSymbolTree SymbolTree { get; }
    public FixedSymbolTree TestingSymbolTree { get; }
    public IFixedList<Diagnostic> Diagnostics { get; }
    public IFixedSet<Package> References { get; }
    public IFunctionDeclaration? EntryPoint { get; }

    public Package(
        IFixedSet<INonMemberDeclaration> nonMemberDeclarations,
        IFixedSet<INonMemberDeclaration> testingNonMemberDeclarations,
        FixedSymbolTree symbolTree,
        FixedSymbolTree testingSymbolTree,
        IFixedList<Diagnostic> diagnostics,
        IFixedSet<Package> references,
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
