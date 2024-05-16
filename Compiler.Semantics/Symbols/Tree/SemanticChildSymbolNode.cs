using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols.Tree;

internal abstract class SemanticChildSymbolNode : ChildNode, IChildDeclarationNode
{
    IDeclarationNode IChildDeclarationNode.Parent => (IDeclarationNode)base.Parent;
    public override ISyntax? Syntax => null;
    public abstract Symbol Symbol { get; }

    internal override IPackageFacetDeclarationNode InheritedFacet(IChildDeclarationNode caller, IChildDeclarationNode child)
        => Parent.InheritedFacet(this, child);
}
