using System.Collections.Generic;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Structure;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal class PackageFacetNode : ChildNode, IPackageFacetNode
{
    public override IPackageSyntax Syntax { get; }
    public IdentifierName PackageName => Package.Name;
    public PackageSymbol PackageSymbol => Package.Symbol;
    private ValueAttribute<IPackageFacetSymbolNode> symbolNode;
    public IPackageFacetSymbolNode SymbolNode
        => symbolNode.TryGetValue(out var value) ? value
            : symbolNode.GetValue(this, SymbolNodeAttribute.PackageFacet);
    public IFixedSet<ICompilationUnitNode> CompilationUnits { get; }

    private ValueAttribute<IFixedSet<IPackageMemberDeclarationNode>> declarations;
    public IFixedSet<IPackageMemberDeclarationNode> Declarations
        => declarations.TryGetValue(out var value)
            ? value
            : declarations.GetValue(this, DeclarationsAttribute.PackageFacet);

    private ValueAttribute<LexicalScope> lexicalScope;
    public LexicalScope LexicalScope
        => lexicalScope.TryGetValue(out var value) ? value
            : lexicalScope.GetValue(this, _ => Parent.InheritedLexicalScope(this, this));

    public PackageFacetNode(IPackageSyntax syntax, IEnumerable<ICompilationUnitNode> compilationUnits)
    {
        Syntax = syntax;
        CompilationUnits = ChildList.CreateFixedSet(this, compilationUnits);
    }

    internal override ISymbolNode InheritedContainingSymbolNode(IChildNode caller, IChildNode child)
        => SymbolNode;

    internal override LexicalScope InheritedContainingLexicalScope(IChildNode caller, IChildNode child)
        => LexicalScope;
}
