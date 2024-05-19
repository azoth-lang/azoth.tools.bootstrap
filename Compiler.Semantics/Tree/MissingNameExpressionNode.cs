using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class MissingNameExpressionNode : AmbiguousNameExpressionNode, IMissingNameExpressionNode
{
    public override IMissingNameSyntax Syntax { get; }
    public UnknownType Type => (UnknownType)DataType.Unknown;

    public MissingNameExpressionNode(IMissingNameSyntax syntax)
    {
        Syntax = syntax;
    }
}
