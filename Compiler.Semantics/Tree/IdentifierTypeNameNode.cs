using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class IdentifierTypeNameNode : TypeNameNode, IIdentifierTypeNameNode
{
    public override IIdentifierTypeNameSyntax Syntax { get; }
    public override IdentifierName Name => Syntax.Name;

    public IdentifierTypeNameNode(IIdentifierTypeNameSyntax syntax)
    {
        Syntax = syntax;
    }
}
