using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class SupertypeNameNode : CodeNode, ISupertypeNameNode
{
    public override ISupertypeNameSyntax Syntax { get; }
    public TypeName Name => Syntax.Name;

    public SupertypeNameNode(ISupertypeNameSyntax syntax)
    {
        Syntax = syntax;
    }
}
