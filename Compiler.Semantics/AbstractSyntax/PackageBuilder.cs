using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.AST;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.AbstractSyntax;

internal class PackageBuilder
{
    public IFixedSet<IDeclaration> Declarations { get; }
    public IFixedSet<IDeclaration> TestingDeclarations { get; }
    public IFixedSet<INonMemberDeclaration> NonMemberDeclarations { get; }
    public IFixedSet<INonMemberDeclaration> NonMemberTestingDeclarations { get; }
    public FixedSymbolTree SymbolTree { get; }
    public FixedSymbolTree TestingSymbolTree { get; }
    public Diagnostics Diagnostics { get; }
    public IFixedSet<Package> References { get; }
    public IFunctionDeclaration? EntryPoint { get; set; }

    public PackageBuilder(
        IFixedSet<INonMemberDeclaration> nonMemberDeclarations,
        IFixedSet<INonMemberDeclaration> nonMemberTestingDeclarations,
        FixedSymbolTree symbolTree,
        FixedSymbolTree testingSymbolTree,
        Diagnostics diagnostics,
        IFixedSet<Compiler.AST.Package> references)
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
        return new(NonMemberDeclarations, NonMemberTestingDeclarations, SymbolTree, TestingSymbolTree,
            Diagnostics.ToFixedList(), References,
            EntryPoint);
    }
}
