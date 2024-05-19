using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Names;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes.Model;
using Azoth.Tools.Bootstrap.Compiler.Semantics.NameBinding;
using Azoth.Tools.Bootstrap.Framework;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class IdentifierNameExpressionNode : AmbiguousNameExpressionNode, IIdentifierNameExpressionNode
{
    protected override bool MayHaveRewrite => true;
    public override IIdentifierNameExpressionSyntax Syntax { get; }
    public IdentifierName Name => Syntax.Name;
    private ValueAttribute<LexicalScope> containingLexicalScope;
    public LexicalScope ContainingLexicalScope
        => containingLexicalScope.TryGetValue(out var value) ? value
            : containingLexicalScope.GetValue(InheritedContainingLexicalScope);
    private ValueAttribute<IFixedList<IDeclarationNode>> referencedDeclarations;
    public IFixedList<IDeclarationNode> ReferencedDeclarations
        => referencedDeclarations.TryGetValue(out var value) ? value
            : referencedDeclarations.GetValue(this, BindingAmbiguousNamesAspect.StandardNameExpression_ReferencedDeclarations);

    public IdentifierNameExpressionNode(IIdentifierNameExpressionSyntax syntax)
    {
        Syntax = syntax;
    }

    protected override IAmbiguousExpressionNode? Rewrite()
        => BindingAmbiguousNamesAspect.IdentifierName_Rewrite(this);
}
