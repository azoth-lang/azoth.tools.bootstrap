using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class TypeNode : ChildNode, ITypeNode
{
    public abstract override ITypeSyntax Syntax { get; }
}
