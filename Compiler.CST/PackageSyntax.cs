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

namespace Azoth.Tools.Bootstrap.Compiler.CST
{
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
        public SymbolForest SymbolTrees { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        public FixedSet<ICompilationUnitSyntax> CompilationUnits { get; }
        public FixedSet<IEntityDeclarationSyntax> AllEntityDeclarations { get; }
        public FixedDictionary<Name, TReference> References { get; }
        public IEnumerable<TReference> ReferencedPackages => References.Values;
        public Diagnostics Diagnostics { get; }

        public PackageSyntax(
            Name name,
            FixedSet<ICompilationUnitSyntax> compilationUnits,
            FixedDictionary<Name, TReference> references)
        {
            Symbol = new PackageSymbol(name);
            SymbolTree = new SymbolTreeBuilder(Symbol);
            CompilationUnits = compilationUnits;
            AllEntityDeclarations = GetEntityDeclarations(CompilationUnits).ToFixedSet();
            References = references;
            SymbolTrees = new SymbolForest(Primitive.SymbolTree, SymbolTree, ReferencedPackages.Select(p => p.SymbolTree));
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
            {
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
                    case IClassDeclarationSyntax syn:
                        yield return syn;
                        declarations.EnqueueRange(syn.Members);
                        break;
                }
            }
        }

        public override string ToString()
        {
            return $"package {Symbol.Name.Text}: {CompilationUnits.Count} Compilation Units";
        }
    }
}
