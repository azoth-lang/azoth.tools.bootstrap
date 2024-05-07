using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class TypeNode : CodeNode, ITypeNode
{
    public abstract override ITypeSyntax Syntax { get; }
}
