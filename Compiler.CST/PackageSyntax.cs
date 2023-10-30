using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Primitives;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols.Trees;
using Azoth.Tools.Bootstrap.Framework;
using ExhaustiveMatching;

namespace Azoth.Tools.Bootstrap.Compiler.CST;

/// <summary>
/// Represents an entire package worth of syntax
/// </summary>
/// <remarks>Doesn't inherit from <see cref="ISyntax"/> because it is never
/// matched as part of syntax. It is always treated as the singular root.
///
/// Currently, references are stored as ASTs. To avoid referencing the AST
/// project from the CST project, a generic type is used.
/// </remarks>
public class PackageSyntax<TReference>
    where TReference : IHasSymbolTree
{
    public PackageSymbol Symbol { get; }
    public SymbolTreeBuilder SymbolTree { get; }
    public ISymbolTreeBuilder TestingSymbolTree { get; }
    public SymbolForest SymbolTrees { get; }
    public SymbolForest TestingSymbolTrees { get; }

    [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
    public FixedSet<ICompilationUnitSyntax> CompilationUnits { get; }
    public FixedSet<ICompilationUnitSyntax> TestingCompilationUnits { get; }
    public FixedSet<IEntityDeclarationSyntax> EntityDeclarations { get; }
    public FixedSet<IEntityDeclarationSyntax> TestingEntityDeclarations { get; }
    public FixedDictionary<SimpleName, TReference> References { get; }
    public IEnumerable<TReference> ReferencedPackages => References.Values;
    public Diagnostics Diagnostics { get; }

    public PackageSyntax(
        SimpleName name,
        FixedSet<ICompilationUnitSyntax> compilationUnits,
        FixedSet<ICompilationUnitSyntax> testingCompilationUnits,
        FixedDictionary<SimpleName, TReference> references)
    {
        Symbol = new PackageSymbol(name);
        SymbolTree = new SymbolTreeBuilder(Symbol);
        TestingSymbolTree = new TestingSymbolTreeBuilder(SymbolTree);
        CompilationUnits = compilationUnits;
        TestingCompilationUnits = testingCompilationUnits;
        EntityDeclarations = GetEntityDeclarations(CompilationUnits).ToFixedSet();
        TestingEntityDeclarations = GetEntityDeclarations(TestingCompilationUnits).ToFixedSet();
        References = references;
        SymbolTrees = BuiltIn.CreateSymbolForest(SymbolTree, ReferencedPackages.Select(p => p.SymbolTree));
        // TODO use referenced test symbol trees
        TestingSymbolTrees = BuiltIn.CreateSymbolForest(TestingSymbolTree, ReferencedPackages.Select(p => p.SymbolTree));
        Diagnostics = new Diagnostics(CompilationUnits.SelectMany(cu => cu.Diagnostics));
    }

    /// <remarks>
    /// It wouldn't make sense to get all declarations including non-member because
    /// that includes namespace declarations. However, some namespaces come from
    /// the implicit namespace of a compilation unit or are implicitly declared,
    /// so it wouldn't give a full list of the namespaces.
    /// </remarks>
    private static IEnumerable<IEntityDeclarationSyntax> GetEntityDeclarations(
        FixedSet<ICompilationUnitSyntax> compilationUnits)
    {
        var declarations = new Queue<IDeclarationSyntax>();
        declarations.EnqueueRange(compilationUnits.SelectMany(cu => cu.Declarations));
        while (declarations.TryDequeue(out var declaration))
            switch (declaration)
            {
                default:
                    throw ExhaustiveMatch.Failed(declaration);
                case IMemberDeclarationSyntax syn:
                    yield return syn;
                    break;
                case IFunctionDeclarationSyntax syn:
                    yield return syn;
                    break;
                case INamespaceDeclarationSyntax syn:
                    declarations.EnqueueRange(syn.Declarations);
                    break;
                case ITypeDeclarationSyntax syn:
                    yield return syn;
                    declarations.EnqueueRange(syn.Members);
                    break;
            }
    }

    public override string ToString()
        => $"package {Symbol.Name.Text}: {CompilationUnits.Count} Compilation Units";
}
