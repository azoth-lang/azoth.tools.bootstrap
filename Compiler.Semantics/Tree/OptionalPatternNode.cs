using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class OptionalPatternNode : CodeNode, IOptionalPatternNode
{
    public override IOptionalPatternSyntax Syntax { get; }
    public IOptionalOrBindingPatternNode Pattern { get; }

    public OptionalPatternNode(IOptionalPatternSyntax syntax, IOptionalOrBindingPatternNode pattern)
    {
        Syntax = syntax;
        Pattern = Child.Attach(this, pattern);
    }
}
