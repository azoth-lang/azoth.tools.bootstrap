using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AST;

internal class PackageBuilder
{
    public FixedSet<IDeclaration> Declarations { get; }
    public FixedSet<IDeclaration> TestingDeclarations { get; }
    public FixedSet<INonMemberDeclaration> NonMemberDeclarations { get; }
    public FixedSet<INonMemberDeclaration> NonMemberTestingDeclarations { get; }
    public FixedSymbolTree SymbolTree { get; }
    public FixedSymbolTree TestingSymbolTree { get; }
    public Diagnostics Diagnostics { get; }
    public FixedDictionary<SimpleName, Package> References { get; }
    public IFunctionDeclaration? EntryPoint { get; set; }

    public PackageBuilder(
        FixedSet<INonMemberDeclaration> nonMemberDeclarations,
        FixedSet<INonMemberDeclaration> nonMemberTestingDeclarations,
        FixedSymbolTree symbolTree,
        FixedSymbolTree testingSymbolTree,
        Diagnostics diagnostics,
        FixedDictionary<SimpleName, Package> references)
    {
        Declarations = GetAllDeclarations(nonMemberDeclarations).ToFixedSet();
        TestingDeclarations = GetAllDeclarations(nonMemberTestingDeclarations).ToFixedSet();
        NonMemberDeclarations = nonMemberDeclarations;
        NonMemberTestingDeclarations = nonMemberTestingDeclarations;
        SymbolTree = symbolTree;
        Diagnostics = diagnostics;
        References = references;
        TestingSymbolTree = testingSymbolTree;
    }

    private static IEnumerable<IDeclaration> GetAllDeclarations(
        IEnumerable<INonMemberDeclaration> nonMemberDeclarations)
    {
        var declarations = new Queue<IDeclaration>(nonMemberDeclarations);
        while (declarations.TryDequeue(out var declaration))
        {
            yield return declaration;
            if (declaration is ITypeDeclaration syn)
                declarations.EnqueueRange(syn.Members);
        }
    }

    public Package Build()
    {
        return new Package(NonMemberDeclarations, NonMemberTestingDeclarations, SymbolTree, TestingSymbolTree,
            Diagnostics.ToFixedList(), References.Values.ToFixedSet(),
            EntryPoint);
    }
}
