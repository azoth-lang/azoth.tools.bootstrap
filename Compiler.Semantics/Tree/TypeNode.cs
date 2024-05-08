using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class TypeNode : CodeNode, ITypeNode
{
    public abstract override ITypeSyntax Syntax { get; }
    public abstract DataType Type { get; }
}
