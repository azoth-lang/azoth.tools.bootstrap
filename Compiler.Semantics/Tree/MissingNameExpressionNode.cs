using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class MissingNameExpressionNode : NameExpressionNode, IMissingNameExpressionNode
{
    public override IMissingNameSyntax Syntax { get; }
    public override IMaybeExpressionAntetype Antetype => IAntetype.Unknown;
    public override UnknownType Type => (UnknownType)DataType.Unknown;

    public MissingNameExpressionNode(IMissingNameSyntax syntax)
    {
        Syntax = syntax;
    }
}
