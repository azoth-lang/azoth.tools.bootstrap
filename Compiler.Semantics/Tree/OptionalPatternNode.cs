using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class OptionalPatternNode : PatternNode, IOptionalPatternNode
{
    public override IOptionalPatternSyntax Syntax { get; }
    public IOptionalOrBindingPatternNode Pattern { get; }

    public OptionalPatternNode(IOptionalPatternSyntax syntax, IOptionalOrBindingPatternNode pattern)
    {
        Syntax = syntax;
        Pattern = Child.Attach(this, pattern);
    }

    public override ConditionalLexicalScope GetFlowLexicalScope() => Pattern.GetFlowLexicalScope();

    internal override IMaybeAntetype InheritedBindingAntetype(IChildNode child, IChildNode descendant)
    {
        if (descendant == Pattern)
            return NameBindingAntetypesAspect.OptionalPattern_InheritedBindingAntetype_Pattern(this);
        return base.InheritedBindingAntetype(child, descendant);
    }
}
