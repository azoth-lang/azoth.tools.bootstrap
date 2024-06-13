using Azoth.Tools.Bootstrap.Compiler.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Core.Attributes;
using Azoth.Tools.Bootstrap.Compiler.CST;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Antetypes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.LexicalScopes;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types;
using Azoth.Tools.Bootstrap.Compiler.Semantics.Types.Flow;
using Azoth.Tools.Bootstrap.Compiler.Types;

namespace Azoth.Tools.Bootstrap.Compiler.Semantics.Tree;

internal sealed class ResultStatementNode : StatementNode, IResultStatementNode
{
    public override IResultStatementSyntax Syntax { get; }
    private Child<IAmbiguousExpressionNode> expression;
    public IAmbiguousExpressionNode Expression => expression.Value;
    public IExpressionNode FinalExpression => (IExpressionNode)expression.FinalValue;
    private ValueAttribute<IMaybeAntetype> antetype;
    public IMaybeAntetype Antetype
        => antetype.TryGetValue(out var value) ? value
            : antetype.GetValue(this, ExpressionAntetypesAspect.ResultStatement_Antetype);
    public override IMaybeAntetype ResultAntetype => Antetype;
    private ValueAttribute<DataType> type;
    public DataType Type
        => type.TryGetValue(out var value) ? value
            : type.GetValue(this, ExpressionTypesAspect.ResultStatement_Type);
    public override FlowState FlowStateAfter => ((IExpressionNode)Expression).FlowStateAfter;
    public ValueId ValueId => FinalExpression.ValueId;

    public ResultStatementNode(IResultStatementSyntax syntax, IAmbiguousExpressionNode expression)
    {
        Syntax = syntax;
        this.expression = Child.Create(this, expression);
    }

    public override LexicalScope GetLexicalScope() => InheritedContainingLexicalScope();
}
