using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class PackageNode : SemanticNode, IPackageNode
{
    public override IPackageSyntax Syntax { get; }

    public IdentifierName Name => Syntax.Name;

    private ValueAttribute<PackageSymbol> symbol;
    public PackageSymbol Symbol
        => symbol.TryGetValue(out var value) ? value : symbol.GetValue(this, SymbolAttribute.Package);

    private ValueAttribute<IPackageSymbolNode> symbolNode;
    public IPackageSymbolNode SymbolNode
        => symbolNode.TryGetValue(out var value) ? value
            : symbolNode.GetValue(this, SymbolNodeAttribute.PackageSymbolNode);

    private ValueAttribute<FixedDictionary<IdentifierName, IPackageSymbolNode>> symbolNodes;
    public FixedDictionary<IdentifierName, IPackageSymbolNode> SymbolNodes
        => symbolNodes.TryGetValue(out var value) ? value
            : symbolNodes.GetValue(this, SymbolNodeAttribute.PackageSymbolNodes);

    public IFixedSet<IPackageReferenceNode> References { get; }
    public IFixedSet<ICompilationUnitNode> CompilationUnits { get; }
    public IFixedSet<ICompilationUnitNode> TestingCompilationUnits { get; }

    public PackageNode(
        IPackageSyntax syntax,
        IEnumerable<IPackageReferenceNode> references,
        IEnumerable<ICompilationUnitNode> compilationUnits,
        IEnumerable<ICompilationUnitNode> testingCompilationUnits)
    {
        Syntax = syntax;
        References = FixedSet.Create(references);
        CompilationUnits = FixedSet.Create(compilationUnits);
        TestingCompilationUnits = FixedSet.Create(testingCompilationUnits);
    }
}
