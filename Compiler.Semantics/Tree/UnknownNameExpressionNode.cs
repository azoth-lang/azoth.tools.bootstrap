using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal abstract class UnknownNameExpressionNode : NameExpressionNode, IUnknownNameExpressionNode
{
    public override IMaybeExpressionAntetype Antetype => IAntetype.Unknown;
    public override UnknownType Type => (UnknownType)DataType.Unknown;

    private protected UnknownNameExpressionNode() { }
}
