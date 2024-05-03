using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
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
            : symbolNode.GetValue(this, SymbolNodeAttribute.Package);

    private ValueAttribute<FixedDictionary<IdentifierName, IPackageSymbolNode>> symbolNodes;
    public FixedDictionary<IdentifierName, IPackageSymbolNode> SymbolNodes
        => symbolNodes.TryGetValue(out var value) ? value
            : symbolNodes.GetValue(this, SymbolNodeAttribute.PackageSymbolNodes);

    public IFixedSet<IPackageReferenceNode> References { get; }
    public IFixedSet<ICompilationUnitNode> CompilationUnits { get; }
    public IFixedSet<ICompilationUnitNode> TestingCompilationUnits { get; }

    private ValueAttribute<IFixedSet<IPackageMemberDeclarationNode>> declarations;
    public IFixedSet<IPackageMemberDeclarationNode> Declarations
        => declarations.TryGetValue(out var value) ? value
            : declarations.GetValue(this, DeclarationsAttribute.PackageDeclarations);

    private ValueAttribute<IFixedSet<IPackageMemberDeclarationNode>> testingDeclarations;
    public IFixedSet<IPackageMemberDeclarationNode> TestingDeclarations
        => testingDeclarations.TryGetValue(out var value)
            ? value
            : testingDeclarations.GetValue(this, DeclarationsAttribute.PackageDeclarations);

    public PackageNode(
        IPackageSyntax syntax,
        IEnumerable<IPackageReferenceNode> references,
        IEnumerable<ICompilationUnitNode> compilationUnits,
        IEnumerable<ICompilationUnitNode> testingCompilationUnits)
    {
        Syntax = syntax;
        References = ChildList.CreateFixedSet(this, references);
        CompilationUnits = ChildList.CreateFixedSet(this, compilationUnits);
        TestingCompilationUnits = ChildList.CreateFixedSet(this, testingCompilationUnits);
    }

    internal override IPackageNode InheritedPackage(IChildNode caller, IChildNode child) => this;
    internal override ISymbolNode InheritedContainingSymbolNode(IChildNode caller, IChildNode child)
    {
        if (CompilationUnits.Contains(caller))
            return SymbolNode.GlobalNamespace;
        if (TestingCompilationUnits.Contains(caller))
            return SymbolNode.TestingGlobalNamespace;
        return base.InheritedContainingSymbolNode(caller, child);
    }
}
