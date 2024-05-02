using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class UsingDirectiveNode : CodeNode, IUsingDirective
{
    public override IUsingDirectiveSyntax Syntax { get; }
    public NamespaceName Name => Syntax.Name; // TODO is this right?

    public UsingDirectiveNode(IUsingDirectiveSyntax syntax)
    {
        Syntax = syntax;
    }
}
