using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

using AST = Azoth.Tools.Bootstrap.Compiler.AST;

internal sealed class Package : Node
{
    public PackageSyntax<AST.Package> Syntax { get; }

    public IdentifierName Name => Syntax.Symbol.Name;

    private ValueAttribute<PackageSymbol> symbol;
    public PackageSymbol Symbol
    {
        get
        {
            if (symbol.TryGetValue(out var value))
                return value;

            return symbol.GetValue(this, SymbolDefinitions.Package);
        }
    }

    public IReadOnlyList<CompilationUnit> CompilationUnits { get; }
    public IReadOnlyList<CompilationUnit> TestingCompilationUnits { get; }

    public Package(
        PackageSyntax<AST.Package> syntax,
        IEnumerable<CompilationUnit> compilationUnits,
        IEnumerable<CompilationUnit> testingCompilationUnits)
    {
        Syntax = syntax;
        CompilationUnits = new ChildList<CompilationUnit>(compilationUnits);
        TestingCompilationUnits = new ChildList<CompilationUnit>(testingCompilationUnits);
    }
}
