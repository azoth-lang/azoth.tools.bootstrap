using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class PackageNode : SemanticNode, IPackage
{
    public override IPackageSyntax Syntax { get; }

    public IdentifierName Name => Syntax.Name;

    private ValueAttribute<PackageSymbol> symbol;
    public PackageSymbol Symbol
        => symbol.TryGetValue(out var value) ? value : symbol.GetValue(this, SymbolDefinitions.Package);

    public IFixedSet<IPackageReference> References { get; }
    public IFixedSet<ICompilationUnit> CompilationUnits { get; }
    public IFixedSet<ICompilationUnit> TestingCompilationUnits { get; }

    public PackageNode(
        IPackageSyntax syntax,
        IEnumerable<IPackageReference> references,
        IEnumerable<ICompilationUnit> compilationUnits,
        IEnumerable<ICompilationUnit> testingCompilationUnits)
    {
        Syntax = syntax;
        References = FixedSet.Create(references);
        CompilationUnits = FixedSet.Create(compilationUnits);
        TestingCompilationUnits = FixedSet.Create(testingCompilationUnits);
    }
}
