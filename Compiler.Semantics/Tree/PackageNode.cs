using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class PackageNode : TreeNode, Package
{
    public override IPackageSyntax Syntax { get; }

    public IdentifierName Name => Syntax.Name;

    private ValueAttribute<PackageSymbol> symbol;
    public PackageSymbol Symbol
        => symbol.TryGetValue(out var value) ? value : symbol.GetValue(this, SymbolDefinitions.Package);

    public IFixedSet<PackageReference> References { get; }
    public IFixedSet<CompilationUnit> CompilationUnits { get; }
    public IFixedSet<CompilationUnit> TestingCompilationUnits { get; }

    public PackageNode(
        IPackageSyntax syntax,
        IEnumerable<PackageReference> references,
        IEnumerable<CompilationUnit> compilationUnits,
        IEnumerable<CompilationUnit> testingCompilationUnits)
    {
        Syntax = syntax;
        References = FixedSet.Create(references);
        CompilationUnits = FixedSet.Create(compilationUnits);
        TestingCompilationUnits = FixedSet.Create(testingCompilationUnits);
    }
}
