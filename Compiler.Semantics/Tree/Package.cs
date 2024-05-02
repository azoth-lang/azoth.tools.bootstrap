using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class Package : Node
{
    public IPackageSyntax Syntax { get; }

    ISyntax Node.Syntax => Syntax;

    public IdentifierName Name => Syntax.Name;

    private ValueAttribute<PackageSymbol> symbol;
    public PackageSymbol Symbol
        => symbol.TryGetValue(out var value) ? value : symbol.GetValue(this, SymbolDefinitions.Package);

    public IReadOnlyList<PackageReference> PackageReferences { get; }
    public IReadOnlyList<CompilationUnit> CompilationUnits { get; }
    public IReadOnlyList<CompilationUnit> TestingCompilationUnits { get; }

    public Package(
        IPackageSyntax syntax,
        IEnumerable<PackageReference> packageReferences,
        IEnumerable<CompilationUnit> compilationUnits,
        IEnumerable<CompilationUnit> testingCompilationUnits)
    {
        Syntax = syntax;
        PackageReferences = ChildList.Create(packageReferences);
        CompilationUnits = ChildList.Create(compilationUnits);
        TestingCompilationUnits = ChildList.Create(testingCompilationUnits);
    }
}
