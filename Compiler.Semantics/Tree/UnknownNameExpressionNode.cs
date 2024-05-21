using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class UnknownNameExpressionNode : AmbiguousNameExpressionNode, IUnknownNameExpressionNode
{
    public UnknownType Type => (UnknownType)DataType.Unknown;

    private protected UnknownNameExpressionNode() { }
}
