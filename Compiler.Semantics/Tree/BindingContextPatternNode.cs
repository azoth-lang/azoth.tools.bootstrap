using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class BindingContextPatternNode : CodeNode, IBindingContextPatternNode
{
    public override IBindingContextPatternSyntax Syntax { get; }
    public bool IsMutableBinding => Syntax.IsMutableBinding;
    public IPatternNode Pattern { get; }
    public ITypeNode? Type { get; }

    public BindingContextPatternNode(
        IBindingContextPatternSyntax syntax,
        IPatternNode pattern,
        ITypeNode? type)
    {
        Syntax = syntax;
        Pattern = Child.Attach(this, pattern);
        Type = Child.Attach(this, type);
    }
}
