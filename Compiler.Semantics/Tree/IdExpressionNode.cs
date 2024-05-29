using Azoth.Tools.Bootstrap.Compiler.Core;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class IdExpressionNode : ExpressionNode, IIdExpressionNode
{
    public override IIdExpressionSyntax Syntax { get; }
    private Child<IAmbiguousExpressionNode> referent;
    public IAmbiguousExpressionNode Referent => referent.Value;
    private ValueAttribute<DataType> type;
    public override DataType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, ExpressionTypesAspect.IdExpression_Type);

    public IdExpressionNode(IIdExpressionSyntax syntax, IAmbiguousExpressionNode referent)
    {
        Syntax = syntax;
        this.referent = Child.Create(this, referent);
    }

    public override ConditionalLexicalScope GetFlowLexicalScope() => Referent.GetFlowLexicalScope();

    protected override void CollectDiagnostics(Diagnostics diagnostics)
    {
        ExpressionTypesAspect.IdExpression_ContributeDiagnostics(this, diagnostics);
        base.CollectDiagnostics(diagnostics);
    }
}
