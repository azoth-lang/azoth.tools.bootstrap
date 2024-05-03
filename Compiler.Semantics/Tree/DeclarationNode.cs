using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Symbols;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class DeclarationNode : ChildNode, IDeclarationNode
{
    public abstract StandardName Name { get; }
    public abstract override IDeclarationSyntax Syntax { get; }
    public virtual ISymbolNode ContainingSymbolNode => Parent.InheritedContainingSymbolNode(this, this);
    public virtual Symbol ContainingSymbol => ContainingSymbolNode.Symbol;
}
