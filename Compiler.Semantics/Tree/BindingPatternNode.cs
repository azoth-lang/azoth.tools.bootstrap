using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class BindingPatternNode : PatternNode, IBindingPatternNode
{
    public override IBindingPatternSyntax Syntax { get; }
    public bool IsMutableBinding => Syntax.IsMutableBinding;
    public IdentifierName Name => Syntax.Name;
    private ValueAttribute<LexicalScope> containingLexicalScope;
    public LexicalScope ContainingLexicalScope
        => containingLexicalScope.TryGetValue(out var value) ? value
            : containingLexicalScope.GetValue(InheritedContainingLexicalScope);
    private ValueAttribute<ValueId> valueId;
    public ValueId ValueId
        => valueId.TryGetValue(out var value) ? value
            : valueId.GetValue(this, ExpressionTypesAspect.BindingPattern_ValueId);
    private ValueAttribute<IMaybeAntetype> bindingAntetype;
    public IMaybeAntetype BindingAntetype
        => bindingAntetype.TryGetValue(out var value) ? value
            : bindingAntetype.GetValue(this, NameBindingAntetypesAspect.BindingPattern_BindingAntetype);

    public BindingPatternNode(IBindingPatternSyntax syntax)
    {
        Syntax = syntax;
    }

    public override LexicalScope GetContainingLexicalScope() => ContainingLexicalScope;

    public override ConditionalLexicalScope GetFlowLexicalScope()
    {
        var containingLexicalScope = ContainingLexicalScope;
        var variableScope = new DeclarationScope(containingLexicalScope, this);
        return new(variableScope, containingLexicalScope);
    }

    internal override IPreviousValueId PreviousValueId(IChildNode before) => ValueId;
}
