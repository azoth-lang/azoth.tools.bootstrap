using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class BindingPatternNode : CodeNode, IBindingPatternNode
{
    public override IBindingPatternSyntax Syntax { get; }
    public bool IsMutableBinding => Syntax.IsMutableBinding;
    public IdentifierName Name => Syntax.Name;

    public BindingPatternNode(IBindingPatternSyntax syntax)
    {
        Syntax = syntax;
    }
}
