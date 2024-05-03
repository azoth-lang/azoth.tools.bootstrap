using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Symbols;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class DeclarationNode : ChildNode, IDeclarationNode
{
    public abstract override IDeclarationSyntax Syntax { get; }

    public Symbol ContainingSymbol => Parent.InheritedContainingSymbol(this, this);
}
