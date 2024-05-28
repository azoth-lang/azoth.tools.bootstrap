using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class StringLiteralExpressionNode : ExpressionNode, IStringLiteralExpressionNode
{
    public override IStringLiteralExpressionSyntax Syntax { get; }
    public string Value => Syntax.Value;
    private ValueAttribute<DataType> type;
    public override DataType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, TypeExpressionsAspect.StringLiteralExpression_Type);
    private ValueAttribute<LexicalScope> containingLexicalScope;
    public LexicalScope ContainingLexicalScope
        => containingLexicalScope.TryGetValue(out var value) ? value
            : containingLexicalScope.GetValue(InheritedContainingLexicalScope);

    public StringLiteralExpressionNode(IStringLiteralExpressionSyntax syntax)
    {
        Syntax = syntax;
    }

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        TypeExpressionsAspect.StringLiteralExpression_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
