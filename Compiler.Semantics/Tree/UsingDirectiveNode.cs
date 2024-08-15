using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Syntax;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class UsingDirectiveNode : CodeNode, IUsingDirectiveNode
{
    public override IUsingDirectiveSyntax Syntax { get; }
    public NamespaceName Name => Syntax.Name;

    public UsingDirectiveNode(IUsingDirectiveSyntax syntax)
    {
        Syntax = syntax;
    }
}
